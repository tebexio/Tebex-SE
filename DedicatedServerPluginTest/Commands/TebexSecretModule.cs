using System;
using Newtonsoft.Json.Linq;

namespace TebexSE.Commands
{
    public class TebexSecretModule : TebexCommandModule
    {
        public void TebexSecret(string secret)
        {
            TebexSE.Instance.setSecret(secret);
            try
            {
                TebexApiClient wc = new TebexApiClient();
                wc.setPlugin(TebexSE.Instance);
                wc.DoGet("information", this);
                wc.Dispose();
            }
            catch (TimeoutException)
            {
                TebexSE.log("error", "Timeout!");
            }            
        }

        public override void HandleResponse(JObject response)
        {
            TebexSE.Instance.information.id = (int)response["account"]["id"];
            TebexSE.Instance.information.domain = (string)response["account"]["domain"];
            TebexSE.Instance.information.gameType = (string)response["account"]["game_type"];
            TebexSE.Instance.information.name = (string)response["account"]["name"];
            TebexSE.Instance.information.currency = (string)response["account"]["currency"]["iso_4217"];
            TebexSE.Instance.information.currencySymbol = (string)response["account"]["currency"]["symbol"];
            TebexSE.Instance.information.serverId = (int)response["server"]["id"];
            TebexSE.Instance.information.serverName = (string)response["server"]["name"];

            TebexSE.log("info", "Server Information");
            TebexSE.log("info", "=================");
            TebexSE.log("info", "Server " + TebexSE.Instance.information.serverName + " for webstore " + TebexSE.Instance.information.name + "");
            TebexSE.log("info", "Server prices are in " + TebexSE.Instance.information.currency + "");
            TebexSE.log("info", "Webstore domain: " + TebexSE.Instance.information.domain + "");
        }

        public override void HandleError(Exception e)
        {
            TebexSE.log("error", "We are unable to fetch your server details. Please check your secret key.");
        }

    }
}
