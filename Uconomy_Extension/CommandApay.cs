using System;
using Rocket.RocketAPI;
using Rocket.Logging;
using unturned.ROCKS.Uconomy;
using SDG;
using Steamworks;

namespace Uconomy_Essentials
{
    public class CommandApay : IRocketCommand
    {
        public bool RunFromConsole
        {
            get
            {
                return false;
            }
        }
        public string Name
        {
            get
            {
                return "apay";
            }
        }
        public string Help
        {
            get
            {
                return "Allows an allowed person to pay someone else not using their own currency.";
            }
        }
        public void Execute(RocketPlayer playerid, string msg)
        {
            string message;
            if (string.IsNullOrEmpty(msg))
            {
                message = "Usage: /apay <player name or id>/<amt>";
                // We are going to print how to use
                RocketChatManager.Say(playerid, message);
                return;
            }
            string[] command = Parser.getComponentsFromSerial(msg, '/');
            if (command.Length != 2)
            {
                message = "Usage: /apay <player name or id>/<amt>";
                // Print how to use
                RocketChatManager.Say(playerid, message);
                return;
            }
            RocketPlayer rp = RocketPlayer.FromName(command[0]);
            if (rp == null)
            {
                ulong id;
                ulong.TryParse(command[0], out id);
                if (!((CSteamID)id).IsValid())
                {
                    message = command[0] + " is not a valid player name or steam id.";
                    RocketChatManager.Say(playerid, message);
                    return;
                }
                else
                {
                    rp = RocketPlayer.FromCSteamID((CSteamID)id);
                }
                
            }
            uint amt;
            uint.TryParse(command[1], out amt);
            if (amt <= 0)
            {
                message = command[1] + " is not a correct amount.";
                RocketChatManager.Say(playerid, message);
                return;
            }
            decimal newbal = Uconomy.Instance.Database.IncreaseBalance(rp.CSteamID, amt);
            RocketChatManager.Say(rp.CSteamID, String.Format(Uconomy_Essentials.Instance.Configuration.APaidMsg, playerid.CharacterName, amt, Uconomy.Instance.Configuration.MoneyName, newbal, Uconomy.Instance.Configuration.MoneyName));
            RocketChatManager.Say(playerid, String.Format(Uconomy_Essentials.Instance.Configuration.APayMsg, rp.CharacterName, amt, Uconomy.Instance.Configuration.MoneyName));
            Uconomy_Essentials.HandleEvent(rp, amt, "paid");
        }
    }
}
