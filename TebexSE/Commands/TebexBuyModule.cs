using System;
using Newtonsoft.Json.Linq;
using Sandbox.Engine.Multiplayer;
using Sandbox.Game.Gui;

namespace TebexSE.Commands
{
    public class TebexBuyModule : TebexCommandModule
    {
        public void TebexBuy(ChatChannel channel, long targetId)
        {
            MyMultiplayer.Static.SendChatMessage("To support our server, please visit " + TebexSE.Instance.information.domain, channel, targetId, "TebexSE");
        }

        public override void HandleResponse(JObject response)
        {
        }

        public override void HandleError(Exception e)
        {
        }

    }
}
