using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TebexSE.Models;

namespace TebexSE.Api
{
    public class ApiCalls
    {
        public static void getPlayerPurchases (string usernameId, Action<List<PlayerPurchase>> callback, int? packageId = null)
        {
            TebexApiClient client = new TebexApiClient();
            client.setPlugin(TebexSE.Instance);
            string endpoint = "player/" + usernameId + "/packages";
            if (packageId != null) {
                endpoint = endpoint + "?package=" + packageId.ToString();
            }
            client.DoGet(endpoint, (string result) =>
            {
                JArray purchaseList = JArray.Parse(result);
                TebexSE.log("info", "Handle result");
                List<PlayerPurchase> purchases = new List<PlayerPurchase>();

                foreach (var purchase in purchaseList)
                {
                    purchases.Add(PlayerPurchase.fromApiResponse((JObject)purchase));
                }

                callback(purchases);

            });
        }
    }
}
