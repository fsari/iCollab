/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Core.Extensions;
using Core.Repository; 
using Model.Settings;

namespace Core.Service
{

    public interface ISettingService
    { 
        Setting GetSettingById(int settingId);
         
        T GetSettingByKey<T>(string key, T defaultValue = default(T), bool loadSharedValueIfNotFound = false);
         
        IList<Setting> GetAllSettings(); 
        bool SettingExists<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector)
            where T : ISettings, new();
         
        T LoadSetting<T>() where T : ISettings, new();
         
        void SetSetting<T>(string key, T value, bool clearCache = true);
         
        void SaveSetting<T>(T settings) where T : ISettings, new();
         
        void SaveSetting<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector,  bool clearCache = true) where T : ISettings, new();
         
        void UpdateSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, bool overrideForStore ) where T : ISettings, new();

        void InsertSetting(Setting setting, bool clearCache = true);

        void UpdateSetting(Setting setting, bool clearCache = true);
         
        void DeleteSetting(Setting setting); 
          
   
    }
    public class SettingService : ISettingService
    {

        private readonly IRepository<Setting> _settingRepository;

        public SettingService(IRepository<Setting> settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public Setting GetSettingById(int settingId)
        {
            if (settingId == 0)
                return null;

            var setting = _settingRepository.Find(settingId);
            return setting;
        }

        public T GetSettingByKey<T>(string key, T defaultValue = default(T), bool loadSharedValueIfNotFound = false)
        {
            throw new NotImplementedException();
        }

        public IList<Setting> GetAllSettings()
        {
            var settings = _settingRepository.CollectionUntracked.ToList();

            return settings;
        }

        public bool SettingExists<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector ) where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        public T LoadSetting<T>() where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        public void SetSetting<T>(string key, T value,  bool clearCache = true)
        {
            throw new NotImplementedException();
        }

        public void SaveSetting<T>(T settings ) where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        public void SaveSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector,  bool clearCache = true) where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        public void UpdateSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, bool overrideForStore ) where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        public void InsertSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Add(setting); 
        }

        public void UpdateSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Update(setting);
        }

        public void DeleteSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Delete(setting);
        }
 
         
    }
}
*/
