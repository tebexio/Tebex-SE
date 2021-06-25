using System;
using Sandbox.Game.World;
using Newtonsoft.Json.Linq;

namespace TebexSE.Commands
{
    public class TebexForcecheckModule : TebexCommandModule
    {
        public void TebexForcecheck()
        {
            TebexSE.log("info", "Checking for commands to be executed...");
            try
            {
                TebexApiClient wc = new TebexApiClient();
                wc.setPlugin(TebexSE.Instance);
                wc.DoGet("queue", this);
                wc.Dispose();
            }
            catch (TimeoutException)
            {
                TebexSE.log("error", "Timeout!");
            }

        }

        public override void HandleResponse(JObject response)
        {
            if ((int) response["meta"]["next_check"] > 0)
            {
                TebexSE.Instance.updatePeriod = (int) response["meta"]["next_check"];
            }
            
            if ((bool) response["meta"]["execute_offline"])
            {
                try
                {
                    TebexCommandRunner.doOfflineCommands();
                }
                catch (Exception e)
                {
                    TebexSE.log("error", e.ToString());
                }
            }
            
            JArray players = (JArray) response["players"];

            var onlinePlayers = MySession.Static.Players.GetOnlinePlayers();
            if (onlinePlayers.Count == 0)
            {
                return;
            }

            foreach (var player in players)
            {
                try
                {
                    ulong steamId = (ulong) player["uuid"];
                    long identityId = MySession.Static.Players.TryGetIdentityId(steamId);
                    MyIdentity targetPlayer = MySession.Static.Players.TryGetIdentity(identityId);

                    bool playerOnline = false;
                    foreach (var onlinePlr in onlinePlayers)
                    {
                        if (onlinePlr.Id.SteamId == steamId)
                        {
                            playerOnline = true;
                        }
                    }                    

                    if (playerOnline)
                    {
                        TebexSE.log("info", "Execute commands for " + (string) targetPlayer.DisplayName + "(ID: "+ steamId.ToString()+")");
                        TebexCommandRunner.doOnlineCommands((int) player["id"], (string)targetPlayer.DisplayName, steamId.ToString());


                    }
                }
                catch (Exception e)
                {
                    TebexSE.log("error", e.Message);
                }
            }
        }

        public override void HandleError(Exception e)
        {
            TebexSE.log("error", "We are unable to fetch your server queue. Please check your secret key.");
            TebexSE.log("error", e.ToString());
        }         
    }
}