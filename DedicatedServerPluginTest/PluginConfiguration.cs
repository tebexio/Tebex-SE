using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VRage.Plugins;

namespace TebexSE
{
    public class PluginConfiguration : IPluginConfiguration
    {
        [Display(Name = "Tebex Secret")]
        [Category("General")]
        public string TebexSecret = "";

        [Display(Name = "Purchase Notification Mode")]
        [Category("General")]
        public NotificationMode NotificationMode = NotificationMode.TebexPurchaseEvent;

        public void Save(string userDataPath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PluginConfiguration));
            
            string configFile = Path.Combine(userDataPath, "TebexSE.cfg");
            using(StreamWriter stream = new StreamWriter(configFile, false, Encoding.UTF8))
            {
                serializer.Serialize(stream, this);
            }
        }
    }

    public enum NotificationMode
    {
        ChatCommand,
        TebexPurchaseEvent
    }
}
