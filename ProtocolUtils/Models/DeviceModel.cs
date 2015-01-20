using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtocolUtils.Models
{
    public class DeviceModel
    {
        public DeviceModel()
        {
            DeviceNumber = "0";
        }
        public string DeviceNumber { get; set; }
        //区分是显示器还是终端0x01显示器，0x02接收转发数据的客户端
        public string Types { get; set; }
        public string TranDevice { get; set; }
    }
}
