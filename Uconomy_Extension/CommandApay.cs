using System;
using Rocket.API;
using Rocket.Unturned;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
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
        public void Execute(RocketPlayer playerid, string[] msg)
        {
            string message;
            if (msg.Length == 0)
            {
                message = Uconomy_Essentials.Instance.Translate("apay_usage_msg", new object[] {});
                // We are going to print how to use
                RocketChat.Say(playerid, message);
                return;
            }
            if (msg.Length != 2)
            {
                message = Uconomy_Essentials.Instance.Translate("apay_usage_msg", new object[] { });
                // Print how to use
                RocketChat.Say(playerid, message);
                return;
            }
            RocketPlayer rp = RocketPlayer.FromName(msg[0]);
            if (rp == null)
            {
                ulong id;
                ulong.TryParse(msg[0], out id);
                if (!((CSteamID)id).IsValid())
                {
                    message = Uconomy_Essentials.Instance.Translate("not_valid_player_msg", new object[] {msg[0]});
                    RocketChat.Say(playerid, message);
                    return;
                }
                else
                {
                    rp = RocketPlayer.FromCSteamID((CSteamID)id);
                }
                
            }
            uint amt;
            uint.TryParse(msg[1], out amt);
            if (amt <= 0)
            {
                message = Uconomy_Essentials.Instance.Translate("not_valid_amount", new object[] { msg[1] });
                RocketChat.Say(playerid, message);
                return;
            }
            decimal newbal = Uconomy.Instance.Database.IncreaseBalance(rp.CSteamID, amt);
            RocketChat.Say(rp.CSteamID, Uconomy_Essentials.Instance.Translate("apaid_msg", new object[] {playerid.CharacterName, amt, Uconomy.Instance.Configuration.MoneyName, newbal, Uconomy.Instance.Configuration.MoneyName}));
            RocketChat.Say(playerid, Uconomy_Essentials.Instance.Translate("apay_msg", new object[] {rp.CharacterName, amt, Uconomy.Instance.Configuration.MoneyName}));
            Uconomy_Essentials.HandleEvent(rp, amt, "paid");
        }
    }
}
