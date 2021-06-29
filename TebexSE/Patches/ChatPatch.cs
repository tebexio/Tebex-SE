using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Game;
using Sandbox.Engine;
using Sandbox.Engine.Multiplayer;
using HarmonyLib;
using Sandbox.Game.Gui;
using VRage.Utils;
using static TebexSE.PatchController;
using TebexSE.Commands;

namespace TebexSE.Patches
{
    //Marks the class for further inspection by the controller
    [PatchingClass]
    public class ChatPatch
    {

        public ChatPatch()
        {
            //Constructor
        }


        //return true if you want "RaiseChatMessageRecieved to continue (so server continues processing and distributing to connected clients)"
        //return false to terminate process - Usually in the case after you handled the command.
        //Okay - this is a patch in which i want to be prefixed to the TargetMethod
        [PrefixMethod]
        [TargetMethod(Type = typeof(MyMultiplayerBase), Method = "RaiseChatMessageReceived")]
        public static bool ProcessChat(ulong steamUserID, string messageText, ChatChannel channel, long targetId, string customAuthorName = null)
        {
            //Ensure chat starts with command prefix - you could define this in the config or make it whatever you want
            //Be weary of mod conficts since keen have no standard system for registering/handling commands.
            if (messageText.StartsWith("!tebex:secret"))
            {
                if (!channel.Equals(ChatChannel.GlobalScripted))
                {
                    MyMultiplayer.Static.SendChatMessage("You are not permitted to run this command", channel, Sandbox.Game.Multiplayer.Sync.Players.TryGetIdentityId(steamUserID), "TebexSE");
                    return false;
                }

                var messageParts = messageText.Split(' ');
                TebexSecretModule tebexSecret = new TebexSecretModule();
                tebexSecret.TebexSecret(messageParts[1]);

                return false;
            }

            if (messageText.StartsWith("!tebex:info"))
            {
                if (!channel.Equals(ChatChannel.GlobalScripted))
                {
                    MyMultiplayer.Static.SendChatMessage("You are not permitted to run this command", channel, Sandbox.Game.Multiplayer.Sync.Players.TryGetIdentityId(steamUserID), "TebexSE");
                    return false;
                }

                TebexInfoModule tebexInfoModule = new TebexInfoModule();
                tebexInfoModule.TebexInfo();

                return false;
            }

            if (messageText.StartsWith("!tebex:buy"))
            {
                TebexBuyModule tebexBuy = new TebexBuyModule();
                tebexBuy.TebexBuy(channel, targetId);
            }


            return true;
        }
    }
}