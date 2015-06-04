using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Mappers;
using Core.Service;
using Core.Settings;
using iCollab.Infra;
using Model.Settings;

namespace iCollab.Controllers
{
    public class SettingsController : BaseController
    {

        private readonly ISettingService _service; 
        public SettingsController(IApplicationSettings settings, 
            IUserService userService, 
            ISettingService service
            ) : base(userService, settings)
        {
            _service = service;
        }


        private StoreDependingSettingHelper _storeDependingSettings;

        private StoreDependingSettingHelper StoreDependingSettings
        {
            get
            {
                if (_storeDependingSettings == null)
                    _storeDependingSettings = new StoreDependingSettingHelper(this.ViewData);
                return _storeDependingSettings;
            }
        }

        // GET: Settings
        public ActionResult Index()
        {  
            var appSettings = _service.LoadSetting<AppSetting>();

            return View(appSettings);
        }

        [HttpPost]
        public ActionResult Index(AppSetting setting, FormCollection form)
        { 
            StoreDependingSettings.UpdateSettings(setting, form,_service);

            _service.ClearCache();

            return RedirectToAction("Index");
        }



    }
}