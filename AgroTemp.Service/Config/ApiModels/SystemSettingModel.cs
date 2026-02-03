using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Service.Config.ApiModels
{
    public class SystemSettingModel
    {
        private static SystemSettingModel _instance;

        public static SystemSettingModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SystemSettingModel();
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
    }

    public class CloudinarySetting
    {
        public static CloudinarySetting Instance { get; set; }
        public string CloudinaryUrl { get; set; }
    }
}
