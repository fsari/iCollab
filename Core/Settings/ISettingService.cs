using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Model.Settings;

namespace Core.Settings
{
    public interface ISettingService
    {
 
        Setting GetSettingById(Guid settingId);

 
        T GetSettingByKey<T>(string key, T defaultValue = default(T), bool loadSharedValueIfNotFound = false);

 
        IList<Setting> GetAllSettings();

 
        bool SettingExists<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector)
            where T : ISettings, new();

 
        T LoadSetting<T>() where T : ISettings, new();

 
        void SetSetting<T>(string key, T value,bool clearCache = true);
 
        void SaveSetting<T>(T settings) where T : ISettings, new();

 
        void SaveSetting<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector, bool clearCache = true) where T : ISettings, new();
 
        void UpdateSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, bool overrideForStore) where T : ISettings, new();

        void InsertSetting(Setting setting, bool clearCache = true);

        void UpdateSetting(Setting setting, bool clearCache = true);
 
        void DeleteSetting(Setting setting);

 
        void DeleteSetting<T>() where T : ISettings, new();

  
        void DeleteSetting<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector) where T : ISettings, new();
 
        void DeleteSetting(string key);
 
        int DeleteSettings(string rootKey);
 
        void ClearCache();
    }
}
