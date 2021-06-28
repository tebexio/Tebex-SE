using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Newtonsoft.Json.Linq;
using Sandbox.Engine.Multiplayer;
using Sandbox.Game.Gui;
using Sandbox.Game.World;

namespace TebexSE
{
    public class TebexCommandRunner
    {

        public static int deleteAfter = 3;
        
        public static void doOfflineCommands()
        {
            TebexApiClient wc = new TebexApiClient();
            wc.setPlugin(TebexSE.Instance);
            wc.Headers.Add("X-Buycraft-Secret", TebexSE.Instance.getSecret());            
            String url = TebexSE.Instance.getBaseUrl() + "queue/offline-commands";

            wc.DownloadStringCompleted += (sender, e) =>
            {
                JObject json = JObject.Parse(e.Result);
                JArray commands = (JArray) json["commands"];

                int exCount = 0;
                List<int> executedCommands = new List<int>();
                
                foreach (var command in commands.Children())
                {

                    String commandToRun = buildCommand((string) command["command"], (string) command["player"]["name"],
                        (string) command["player"]["uuid"]);
                    
                    TebexSE.log("info", "Run command " + commandToRun);
                    if ((int)command["conditions"]["delay"] > 0)
                    {
                        // Create a timer with a two second interval.
                        var aTimer = new System.Timers.Timer((int)command["conditions"]["delay"] * 1000);
                        aTimer.Elapsed += (Object source, System.Timers.ElapsedEventArgs ev) =>
                        {
                            RunCommand(commandToRun);
                            ((Timer)source).Dispose();
                        };
                        aTimer.AutoReset = false;
                        aTimer.Enabled = true;
                    }
                    else
                    {
                        RunCommand(commandToRun);
                    }
                    executedCommands.Add((int) command["id"]);

                    exCount++;

                    if (exCount % deleteAfter == 0)
                    {
                        try
                        {
                            deleteCommands(executedCommands);
                            executedCommands.Clear();
                        }
                        catch (Exception ex)
                        {
                           TebexSE.log("error", ex.ToString());
                        }
                    }
                    
                }
                
                TebexSE.log("info", exCount.ToString() + " offline commands executed");
                if (exCount % deleteAfter != 0)
                {
                    try
                    {
                        deleteCommands(executedCommands);
                        executedCommands.Clear();
                    }
                    catch (Exception ex)
                    {
                       TebexSE.log("error", ex.ToString());
                    }
                }

                wc.Dispose();
            };

            wc.DownloadStringAsync(new Uri(url));
        }

        public static void doOnlineCommands(int playerPluginId, MyIdentity targetPlayer, string playerId)
        {
            string playerName = (string)targetPlayer.DisplayName;

            TebexSE.log("info", "Running online commands for "+playerName+" (" + playerId + ")");
            
            TebexApiClient wc = new TebexApiClient();
            wc.setPlugin(TebexSE.Instance);
            wc.Headers.Add("X-Buycraft-Secret", TebexSE.Instance.getSecret());
            String url = TebexSE.Instance.getBaseUrl() + "queue/online-commands/" +
                         playerPluginId.ToString();

            wc.DownloadStringCompleted += (sender, e) =>
            {
                JObject json = JObject.Parse(e.Result);
                JArray commands = (JArray) json["commands"];

                int exCount = 0;
                List<int> executedCommands = new List<int>();
                
                foreach (var command in commands.Children())
                {
                    String commandToRun = buildCommand((string) command["command"], playerName, playerId);
                 
                    //if ((int) command["conditions"]["slots"] > 0)
                    //{
                    //    Console.WriteLine("Max Invcentory: " + targetPlayer.Character.InventoryAggregate.MaxItemCount.ToString());
                    //    Console.WriteLine("Currency Invcentory: " + targetPlayer.Character.InventoryAggregate.GetItemsCount().ToString());
                    //    int availableSpace = targetPlayer.Character.InventoryAggregate.MaxItemCount - targetPlayer.Character.InventoryAggregate.GetItemsCount();
                    //    Console.WriteLine("Available Space:" + availableSpace.ToString());

                    //}
                    if ((int)command["conditions"]["delay"] > 0)
                    {
                        // Create a timer with a two second interval.
                        var aTimer = new System.Timers.Timer((int)command["conditions"]["delay"] * 1000);
                        aTimer.Elapsed += (Object source, System.Timers.ElapsedEventArgs ev) =>
                        {
                            RunCommand(commandToRun);
                            ((Timer)source).Dispose();
                        };
                        aTimer.AutoReset = false;
                        aTimer.Enabled = true;
                    }
                    else
                    {
                        RunCommand(commandToRun);
                    }
                    executedCommands.Add((int) command["id"]);

                    exCount++;

                    if (exCount % deleteAfter == 0)
                    {
                        try
                        {
                            deleteCommands(executedCommands);
                            executedCommands.Clear();
                        }
                        catch (Exception ex)
                        {
                           TebexSE.log("error", ex.ToString());
                        }
                    }
                    
                }
                
                TebexSE.log("info", exCount.ToString() + " online commands executed for " + playerName);
                if (exCount % deleteAfter != 0)
                {
                    try
                    {
                        deleteCommands(executedCommands);
                        executedCommands.Clear();
                    }
                    catch (Exception ex)
                    {
                       TebexSE.log("error", ex.ToString());
                    }
                }

                wc.Dispose();
            };

            wc.DownloadStringAsync(new Uri(url));            
        }

        public static void deleteCommands(List<int> commandIds)
        {

            String url = TebexSE.Instance.getBaseUrl() + "queue?";
            String amp = "";

            foreach (int CommandId in commandIds)
            {
                url = url + amp + "ids[]=" + CommandId;
                amp = "&";
            }

            var request = WebRequest.Create(url);
            request.Method = "DELETE";
            request.Headers.Add("X-Buycraft-Secret", TebexSE.Instance.getSecret());
            
            Thread thread = new Thread(() => request.GetResponse());  
            thread.Start();
        }

        public static string buildCommand(string command, string username, string id)
        {
            return command.Replace("{id}", id).Replace("{username}", username);
        }

        private static void RunCommand(string command)
        {
            if (TebexSE.Instance.getNotificationMode() == NotificationMode.ChatCommand)
            {
                TebexSE.log("info", "Run command (chat) " + command);
                //Send Chat
                MyMultiplayer.Static.SendChatMessage(command, ChatChannel.GlobalScripted, 0, "TebexSE");
            } else
            {
                TebexSE.log("info", "Run command (event) " + command);
                //Send Event
                TebexSE.tebexPurchaseEvent.purchaseReceived(command);
            }
        }
    }
}