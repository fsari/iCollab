using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection; 
using Core.Caching;
using Core.Extensions; 
using Core.Repository;
using Fasterflect;
using Model.Settings;
using Newtonsoft.Json;
using SharpRepository.Repository;

namespace Core.Settings
{
    public class SettingService : ISettingService
    {
        #region Constants
        private const string SETTINGS_ALL_KEY = "SmartStore.setting.all";
        #endregion

        #region Fields

        private readonly IRepository<Setting> _settingRepository; 
        private readonly ICacheManager _cacheManager; 

        #endregion

        #region Ctor
         
        public SettingService(Func<string, ICacheManager> cache , 
            IRepository<Setting> settingRepository)
        {
            _cacheManager = cache("static"); 
            _settingRepository = settingRepository;
        }

        #endregion

        #region Nested classes

        [Serializable]
        public class SettingForCaching
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public int StoreId { get; set; }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>Setting collection</returns>
        protected virtual IDictionary<string, IList<SettingForCaching>> GetAllSettingsCached()
        {
            //cache
            string key = string.Format(SETTINGS_ALL_KEY);
            return _cacheManager.Get(key, () =>
            {

                var settings = _settingRepository.AsQueryable().AsNoTracking().OrderBy(x=>x.Name).ToList();
                
                var dictionary = new Dictionary<string, IList<SettingForCaching>>();
                
                foreach (var s in settings)
                {
                    var settingName = s.Name.ToLowerInvariant();
                    var settingForCaching = new SettingForCaching
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Value = s.Value
                    };
                    if (!dictionary.ContainsKey(settingName))
                    {
                        //first setting
                        dictionary.Add(settingName, new List<SettingForCaching>
                        {
                            settingForCaching
                        });
                    }
                    else
                    {
                        //already added 
                        dictionary[settingName].Add(settingForCaching);
                    }
                }
                return dictionary;
            });
        } 

        public virtual void InsertSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Add(setting); 
             
            if (clearCache)
                _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
             
        }

        /// <summary>
        /// Updates a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void UpdateSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Update(setting); 
             
            if (clearCache)
                _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
             
        }

        /// <remarks>codehint: sm-add</remarks>
        private T LoadSettingsJson<T>(int storeId = 0)
        {
            Type t = typeof(T);
            string key = t.Namespace + "." + t.Name;

            T settings = Activator.CreateInstance<T>();

            var rawSetting = GetSettingByKey<string>(key, loadSharedValueIfNotFound: true);
            if (rawSetting.HasValue())
            {
                JsonConvert.PopulateObject(rawSetting, settings);
            }
            return settings;
        }

        private void SaveSettingsJson<T>(T settings)
        {
            Type t = typeof(T);
            string key = t.Namespace + "." + t.Name;
             
            var rawSettings = JsonConvert.SerializeObject(settings);
            SetSetting(key, rawSettings, false);

            // and now clear cache
            ClearCache();
        }

        private void DeleteSettingsJson<T>()
        {
            Type t = typeof(T);
            string key = t.Namespace + "." + t.Name;

            var setting = GetAllSettings().FirstOrDefault(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase));

            if (setting != null)
            {
                DeleteSetting(setting);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a setting by identifier
        /// </summary>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>Setting</returns>
        public virtual Setting GetSettingById(Guid settingId)
        {
            var setting = _settingRepository.AsQueryable().AsNoTracking().FirstOrDefault(x=>x.Id == settingId);

            return setting;
        }
         

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>Setting value</returns>
        public virtual T GetSettingByKey<T>(string key, T defaultValue = default(T), bool loadSharedValueIfNotFound = false)
        {
            if (String.IsNullOrEmpty(key))
                return defaultValue;

            var settings = GetAllSettingsCached();
            key = key.Trim().ToLowerInvariant();
            if (settings.ContainsKey(key))
            {
                var settingsByKey = settings[key];
                var setting = settingsByKey.FirstOrDefault();

                // load shared value?
                if (setting == null && loadSharedValueIfNotFound)
                    setting = settingsByKey.FirstOrDefault(x => x.StoreId == 0);

                if (setting != null)
                    return setting.Value.Convert<T>();
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>Setting collection</returns>
        public virtual IList<Setting> GetAllSettings()
        { 
            var settings = _settingRepository.AsQueryable().AsNoTracking().OrderBy(x => x.Name).ToList();
            return settings;
        }
   
        public virtual bool SettingExists<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector)
            where T : ISettings, new()
        {
            var member = keySelector.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    keySelector));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(
                       "Expression '{0}' refers to a field, not a property.",
                       keySelector));
            }

            string key = typeof(T).Name + "." + propInfo.Name;

            var setting = GetSettingByKey<string>(key);
            return setting != null;
        } 

        public virtual T LoadSetting<T>() where T : ISettings, new()
        {
            if (typeof(T).HasAttribute<JsonPersistAttribute>(true))
            {
                return LoadSettingsJson<T>();
            }

            var settings = Activator.CreateInstance<T>();

            foreach (var prop in typeof(T).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = typeof(T).Name + "." + prop.Name;
                 
                var setting = GetSettingByKey<string>(key,  loadSharedValueIfNotFound: true);

                if (setting == null)
                {
                    if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        // convenience: don't return null for simple list types
                        var listArg = prop.PropertyType.GetGenericArguments()[0];
                        object list = null;

                        if (listArg == typeof(int))
                            list = new List<int>();
                        else if (listArg == typeof(decimal))
                            list = new List<decimal>();
                        else if (listArg == typeof(string))
                            list = new List<string>();

                        if (list != null)
                        {
                            prop.SetValue(settings, list, null);
                        }
                    }

                    continue;
                }

                var converter = CommonHelper.GetTypeConverter(prop.PropertyType);

                if (converter == null || !converter.CanConvertFrom(typeof(string)))
                    continue;

                if (!converter.IsValid(setting))
                    continue;

                object value = converter.ConvertFromInvariantString(setting);

                //set property
                prop.SetValue(settings, value, null);
            }

            return settings;
        }
         
        public virtual void SetSetting<T>(string key, T value,  bool clearCache = true)
        { 
            key = key.Trim().ToLowerInvariant();
            var valueStr = CommonHelper.GetTypeConverter(typeof(T)).ConvertToInvariantString(value);

            var allSettings = GetAllSettingsCached();
            var settingForCaching = allSettings.ContainsKey(key) ?
                allSettings[key].FirstOrDefault() : null;

            if (settingForCaching != null)
            {
                //update
                var setting = GetSettingById(settingForCaching.Id);
                setting.Value = valueStr;
                UpdateSetting(setting, clearCache);
            }
            else
            {
                //insert
                var setting = new Setting
                {
                    Name = key,
                    Value = valueStr
                };
                InsertSetting(setting, clearCache);
            }
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="settings">Setting instance</param>
        /// <param name="storeId">Store identifier</param>
        public virtual void SaveSetting<T>(T settings ) where T : ISettings, new()
        {
            if (typeof(T).HasAttribute<JsonPersistAttribute>(true))
            {
                SaveSettingsJson(settings);
                return;
            }

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            foreach (var prop in typeof(T).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (!CommonHelper.GetTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                string key = typeof(T).Name + "." + prop.Name;
                //Duck typing is not supported in C#. That's why we're using dynamic type
                dynamic value = settings.TryGetPropertyValue(prop.Name);

                SetSetting(key, value ?? "",  false);
            }

            //and now clear cache
            ClearCache();
        }
         
        public virtual void SaveSetting<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector, bool clearCache = true) where T : ISettings, new()
        {
            var member = keySelector.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    keySelector));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(
                       "Expression '{0}' refers to a field, not a property.",
                       keySelector));
            }

            string key = typeof(T).Name + "." + propInfo.Name;
            //Duck typing is not supported in C#. That's why we're using dynamic type
            dynamic value = settings.TryGetPropertyValue(propInfo.Name);

            SetSetting(key, value ?? "", false);
        }

        /// <remarks>codehint: sm-add</remarks>
        public virtual void UpdateSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, bool overrideForStore ) where T : ISettings, new()
        {
            if (overrideForStore)
                SaveSetting(settings, keySelector,  false);
            else 
                DeleteSetting(settings, keySelector);
        }

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        public virtual void DeleteSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Delete(setting);

            //cache
            _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
             
        }

        /// <summary>
        /// Delete all settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        public virtual void DeleteSetting<T>() where T : ISettings, new()
        {
            // codehint: sm-add
            if (typeof(T).HasAttribute<JsonPersistAttribute>(true))
            {
                DeleteSettingsJson<T>();
                return;
            }

            var settingsToDelete = new List<Setting>();
            var allSettings = GetAllSettings();
            foreach (var prop in typeof(T).GetProperties())
            {
                string key = typeof(T).Name + "." + prop.Name;
                settingsToDelete.AddRange(allSettings.Where(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)));
            }

            foreach (var setting in settingsToDelete)
                DeleteSetting(setting);
        }
         
        public virtual void DeleteSetting<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector) where T : ISettings, new()
        {
            var member = keySelector.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    keySelector));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(
                       "Expression '{0}' refers to a field, not a property.",
                       keySelector));
            }

            string key = typeof(T).Name + "." + propInfo.Name;

            DeleteSetting(key);
        }

        public virtual void DeleteSetting(string key)
        {
            if (key.HasValue())
            {
                key = key.Trim().ToLowerInvariant();

                var setting = (
                    from s in _settingRepository.AsQueryable()
                    where  s.Name == key
                    select s).FirstOrDefault();

                if (setting != null)
                    DeleteSetting(setting);
            }
        }

        /// <summary>
        /// Deletes all settings with its key beginning with rootKey.
        /// </summary>
        /// <returns>Number of deleted settings</returns>
        public virtual int DeleteSettings(string rootKey)
        {
            int result = 0;

            if (rootKey.HasValue())
            {
                try
                {
                     
                }
                catch (Exception exc)
                {
                    exc.Dump();
                }
            }

            return result;
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        public virtual void ClearCache()
        {
            _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
        }

        #endregion
    }
}
