using System;
using System.Collections.Generic;

using Rocket.API;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using fr34kyn01535.Uconomy;
using SDG.Unturned;
using Steamworks;

namespace Uconomy_Essentials
{
    public class CommandApay : IRocketCommand
    {
        public bool AllowFromConsole
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
        public string Syntax
        {
            get
            {
                return "<player name or id> <amt>";
            }
        }
        public List<string> Aliases
        {
            get { return new List<string>(); }
        }
        public List<string> Permissions
        {
            get { return new List<string>(); }
        }
        public void Execute(IRocketPlayer caller, string[] msg)
        {
            UnturnedPlayer playerid = (UnturnedPlayer)caller;
            string message;
            if (msg.Length == 0)
            {
                message = Uconomy_Essentials.Instance.Translate("apay_usage_msg", new object[] {});
                // We are going to print how to use
                UnturnedChat.Say(playerid, message);
                return;
            }
            if (msg.Length != 2)
            {
                message = Uconomy_Essentials.Instance.Translate("apay_usage_msg", new object[] { });
                // Print how to use
                UnturnedChat.Say(playerid, message);
                return;
            }
            UnturnedPlayer rp = UnturnedPlayer.FromName(msg[0]);
            if (rp == null)
            {
                ulong id;
                ulong.TryParse(msg[0], out id);
                if (!((CSteamID)id).IsValid())
                {
                    message = Uconomy_Essentials.Instance.Translate("not_valid_player_msg", new object[] {msg[0]});
                    UnturnedChat.Say(playerid, message);
                    return;
                }
                else
                {
                    rp = UnturnedPlayer.FromCSteamID((CSteamID)id);
                }
                
            }
            uint amt;
            uint.TryParse(msg[1], out amt);
            if (amt <= 0)
            {
                message = Uconomy_Essentials.Instance.Translate("not_valid_amount", new object[] { msg[1] });
                UnturnedChat.Say(playerid, message);
                return;
            }
            decimal newbal = Uconomy.Instance.Database.IncreaseBalance(rp.CSteamID.ToString(), amt);
            UnturnedChat.Say(rp.CSteamID, Uconomy_Essentials.Instance.Translate("apaid_msg", new object[] {playerid.CharacterName, amt, Uconomy.Instance.Configuration.Instance.MoneyName, newbal, Uconomy.Instance.Configuration.Instance.MoneyName}));
            UnturnedChat.Say(playerid, Uconomy_Essentials.Instance.Translate("apay_msg", new object[] {rp.CharacterName, amt, Uconomy.Instance.Configuration.Instance.MoneyName}));
            Uconomy_Essentials.HandleEvent(rp, amt, "paid");
        }
    }
}
