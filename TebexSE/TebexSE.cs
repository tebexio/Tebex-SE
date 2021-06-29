using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TebexSE.Commands;
using TebexSE.Models;
using VRage.Plugins;
using static TebexSE.PatchController;

namespace TebexSE {

    //Notation by Bishbash777#0465
    public class TebexSE : IConfigurablePlugin {
        //Global value for config which when implemented correctly, Can be read anywhere in the plugin assembly
        private PluginConfiguration m_configuration;

        private DateTime lastUpdate = DateTime.Now;
        public int updatePeriod = 10;
        private string baseurl = "https://plugin.tebex.io/";
        public const ushort ConnectionId = 16103;
        private string secret = "";

        public WebstoreInfo information = new WebstoreInfo();

        public static TebexSE Instance { get; private set; }

        public static TebexPurchaseEvent tebexPurchaseEvent = new TebexPurchaseEvent();

        //Init is called once the server has been deemed to be "Ready"
        public void Init(object gameInstance) {

            //GetConfiguration NEEDS to be called at this point in the process or else Developers will experience the
            //behaviour that is exhibited on the description of the GetConfiguration definition below...
            var config = GetConfiguration(VRage.FileSystem.MyFileSystem.UserDataPath);

            //START PATCHING ALL methods marked for patch by controller - only needs to be called once.
            PatchMethods(this);

            Instance = this;

            log("info", "Welcome to Tebex-SE v1.1.1");

            secret = m_configuration.TebexSecret;

            if (secret == "")
            {
                log("warn", "You have not yet defined your secret key. Use !tebex:secret <secret> to define your key");
            } else
            {
                TebexInfoModule tebexInfo = new TebexInfoModule();
                tebexInfo.TebexInfo();
            }
        }

        //Called every gameupdate or 'Tick'
        public void Update() {
            if (lastUpdate.AddSeconds(updatePeriod) < DateTime.Now)
            {
                lastUpdate = DateTime.Now;
                if (secret != "")
                {
                    TebexForcecheckModule forcecheckModule = new TebexForcecheckModule();
                    forcecheckModule.TebexForcecheck();
                }
            }
        }

        public void setSecret(string secret)
        {
            m_configuration.TebexSecret = secret;
            this.secret = secret;
            m_configuration.Save(VRage.FileSystem.MyFileSystem.UserDataPath);
            log("info", "Secret updated");
        }

        public string getBaseUrl()
        {
            return baseurl;
        }

        public string getSecret()
        {
            return secret;
        }

        public NotificationMode getNotificationMode()
        {
            return m_configuration.NotificationMode;
        }

        public static void log(string severity, string message)
        {
            VRage.Utils.MyLog.Default.WriteLineAndConsole("[TebexSE] [" + severity + "] " + message + "");
        }

        //Seems to either be non-functional or more likely called too late in the plugins initialisation stage meaning that
        //if you want to read any configuration values in Update() Or Init(), you will be met with a null ref crash...
        //Maybe consider a mandatory GLOBAL to be defined at the top of the main class which could be read by the DS
        //which will tell it the name of the cfg file therefore cutting out the need for GetConfiguration to be mandatory
        //in each seperate plugin that is ever developed.
        public IPluginConfiguration GetConfiguration(string userDataPath) {
            if (m_configuration == null) {
                string configFile = Path.Combine(userDataPath, "TebexSE.cfg");
                if (File.Exists(configFile)) {
                    XmlSerializer serializer = new XmlSerializer(typeof(PluginConfiguration));
                    using (FileStream stream = File.OpenRead(configFile)) {
                        m_configuration = serializer.Deserialize(stream) as PluginConfiguration;
                    }
                }

                if (m_configuration == null) {
                    m_configuration = new PluginConfiguration();
                }
            }

            return m_configuration;
        }

        //Run when server is in unload/shutdown
        public void Dispose() {
        }

        //Returned to DS to display a friendly name of the plugin to the DS user...
        public string GetPluginTitle() {
            return "TebexSE";
        }
    }
}
