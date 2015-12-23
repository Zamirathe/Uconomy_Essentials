using System;
using System.Collections.Generic;

using Rocket.API;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using fr34kyn01535.Uconomy;

namespace Uconomy_Essentials
{
    public class CommandExchange : IRocketCommand
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
                return "exchange";
            }
        }
        public string Help
        {
            get
            {
                return "Exchanges experience for economy currency.";
            }
        }
        public string Syntax
        {
            get
            {
                return "<amount> [money]";
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
        public void Execute(IRocketPlayer caller, string[] amt)
        {
            UnturnedPlayer playerid = (UnturnedPlayer)caller;
            string message;
            if (amt.Length == 0)
            {
                message = Uconomy_Essentials.Instance.Translate("exchange_usage_msg", new object[] { });
                UnturnedChat.Say(playerid, message);
                return;
            }
            if (amt.Length > 2)
            {
                message = Uconomy_Essentials.Instance.Translate("exchange_usage_msg", new object[] { });
                UnturnedChat.Say(playerid, message);
                return;
            }
            if (!Uconomy_Essentials.Instance.Configuration.Instance.ExpExchange && amt.Length == 1)
            {
                message = Uconomy_Essentials.Instance.Translate("experience_exchange_not_available", new object[] { });
                UnturnedChat.Say(playerid, message);
                return;
            }
            if (!Uconomy_Essentials.Instance.Configuration.Instance.MoneyExchange && amt.Length == 2)
            {
                message = Uconomy_Essentials.Instance.Translate("money_exchange_not_available", new object[] { });
                UnturnedChat.Say(playerid, message);
                return;
            }
            // Get expereience balance first
            uint exp = playerid.Experience;
            uint examt = 0;
            UInt32.TryParse(amt[0], out examt);
            if (examt <= 0)
            {
                message = Uconomy_Essentials.Instance.Translate("exchange_zero_amount_error", new object[] { });
                UnturnedChat.Say(playerid, message);
                return;
            }
            if (exp < examt && amt.Length == 1)
            {
                message = Uconomy_Essentials.Instance.Translate("exchange_insufficient_experience", new object[] { examt.ToString() });
                UnturnedChat.Say(playerid, message);
                return;
            }
            // Get balance first
            decimal bal = Uconomy.Instance.Database.GetBalance(playerid.CSteamID.ToString());
            if (amt.Length > 1 && bal < examt)
            {
                message = Uconomy_Essentials.Instance.Translate("exchange_insufficient_money", new object[] { examt.ToString(), Uconomy.Instance.Configuration.Instance.MoneyName });
                UnturnedChat.Say(playerid, message);
                return;
            }
            switch (amt.Length)
            {
                case 1:
                    decimal gain = (decimal)((float)examt * Uconomy_Essentials.Instance.Configuration.Instance.ExpExchangerate);
                    // Just to make sure to avoid any errors
                    gain = Decimal.Round(gain, 2);
                    decimal newbal = Uconomy.Instance.Database.IncreaseBalance(playerid.CSteamID.ToString(), gain);
                    message = Uconomy_Essentials.Instance.Translate("new_balance_msg", new object[] { newbal.ToString(), Uconomy.Instance.Configuration.Instance.MoneyName });
                    UnturnedChat.Say(playerid, message);
                    playerid.Experience -= examt;
                    Uconomy_Essentials.HandleEvent(playerid, gain, "exchange", examt);
                    break;
                case 2:
                    uint gainm = (uint)((float)examt * Uconomy_Essentials.Instance.Configuration.Instance.MoneyExchangerate);
                    // Just to make sure to avoid any errors
                    newbal = Uconomy.Instance.Database.IncreaseBalance(playerid.CSteamID.ToString(), (examt * -1.0m));
                    message = Uconomy_Essentials.Instance.Translate("new_balance_msg", new object[] { newbal.ToString(), Uconomy.Instance.Configuration.Instance.MoneyName });
                    UnturnedChat.Say(playerid, message);
                    playerid.Experience += gainm;
                    Uconomy_Essentials.HandleEvent(playerid, gainm, "exchange", examt, "money");
                    break;
            }
            playerid.Player.SteamChannel.send("tellExperience", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[]
		    {
			    playerid.Experience
		    });
        }
    }
}
