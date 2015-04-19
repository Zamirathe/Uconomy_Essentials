using System;
using Rocket.RocketAPI;
using Rocket.Logging;
using unturned.ROCKS.Uconomy;
using SDG;
using Steamworks;

namespace Uconomy_Essentials
{
    public class CommandExchange : IRocketCommand
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
        public void Execute(RocketPlayer playerid, string amt)
        {
            if (!Uconomy_Essentials.Instance.Configuration.ExpExchange)
            {
                RocketChatManager.Say(playerid, "I'm sorry, experience exchange is not available.  Ask your admin to enable it.");
                return;
            }
            // Get expereience balance first
            uint exp = playerid.Experience;
            uint examt = 0;
            UInt32.TryParse(amt, out examt);
            if (examt <= 0)
            {
                RocketChatManager.Say(playerid, "You have to enter an amount of experience to exchange.");
                return;
            }
            if (exp < examt)
            {
                RocketChatManager.Say(playerid, "You don't have " + examt.ToString() + " exprience.");
                return;
            }
            // Get experience balance first
            decimal bal = Uconomy.Instance.Database.GetBalance(playerid.CSteamID);
            decimal gain = (decimal)((float)examt * Uconomy_Essentials.Instance.Configuration.ExpExchangerate);
            // Just to make sure to avoid any errors
            gain = Decimal.Round(gain, 2);
            decimal newbal = Uconomy.Instance.Database.IncreaseBalance(playerid.CSteamID, gain);
            RocketChatManager.Say(playerid, String.Format(Uconomy_Essentials.Instance.Configuration.NewBalanceMsg, newbal.ToString(), Uconomy.Instance.Configuration.MoneyName));
            playerid.Experience -= examt;
            Uconomy_Essentials.HandleEvent(playerid, gain, "exchange", examt);
        }
    }
}
