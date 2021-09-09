using Newtonsoft.Json.Linq;
using System;

namespace TebexSE.Models
{
    public class PlayerPurchase
    {
        public string txnId;
        public DateTime date;
        public int quantity;
        public int packageId;
        public string packageName;

        public static PlayerPurchase fromApiResponse(JObject response)
        {
            PlayerPurchase playerPurchase = new PlayerPurchase();

            playerPurchase.txnId = (string)response["txn_id"];
            playerPurchase.date = DateTime.Parse((string)response["date"]);
            playerPurchase.quantity = (int)response["quantity"];
            playerPurchase.packageId = (int)response["package"]["id"];
            playerPurchase.packageName = (string)response["package"]["name"];

            return playerPurchase;
        }
    }
}