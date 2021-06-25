using System;
using Newtonsoft.Json.Linq;
using Sandbox.Engine.Multiplayer;
using Sandbox.Game.Gui;

namespace TebexSE.Commands
{
    public class TebexBuyModule : TebexCommandModule
    {
        public void TebexBuy(ChatChannel channel, ulong steamUserID)
        {
            MyMultiplayer.Static.SendChatMessage("To support our server, please visit " + TebexSE.Instance.information.domain, channel, Sandbox.Game.Multiplayer.Sync.Players.TryGetIdentityId(steamUserID), "TebexSE");
        }

        public override void HandleResponse(JObject response)
        {
        }

        public override void HandleError(Exception e)
        {
        }

    }
}
