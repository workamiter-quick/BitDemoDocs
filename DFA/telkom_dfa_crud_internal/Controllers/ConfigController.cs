using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;


namespace Crud.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public ConfigController(IConfiguration _conf)
        {
            _configuration = _conf;

        }

        [HttpGet]
        public JsonResult Get()
        {
            ConfigSetting _configSetting = new ConfigSetting();
            try
            {
                string SettingJavaApi_Internal = _configuration["SettingJavaApi_Internal"].ToString();
                string SettingJavaApi_External = _configuration["SettingJavaApi_External"].ToString();
                _configSetting.SettingJavaApi_External = SettingJavaApi_External;
                _configSetting.SettingJavaApi_Internal = SettingJavaApi_Internal;

            }
            catch (Exception ex)
            {
                //throw ex;
                throw new Exception("amit");
            }
            finally { }
            return new JsonResult(_configSetting);



        }


    }

    public class ConfigSetting
    {      

        public string SettingJavaApi_Internal { get; set; }
        public string SettingJavaApi_External { get; set; }
    }
}
