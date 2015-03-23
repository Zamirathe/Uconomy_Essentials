using System;
using Rocket.RocketAPI;
using Rocket.Logging;
using unturned.ROCKS.Uconomy;
using SDG;

namespace Uconomy_Extension
{
    class CommandExchange : Command
    {
        public CommandExchange()
        {
            this.commandName = "exchange";
            this.commandHelp = "Exchanges experience for economy currency.";
            this.commandInfo = this.commandName + " - " + this.commandHelp;
        }

        protected override void execute(SteamPlayerID playerid, string amt)
        {
            if (!RocketCommand.IsPlayer(playerid)) return;
            if (!Uconomy_Extension.Instance.Configuration.ExpExchange)
            {
                RocketChatManager.Say(playerid.CSteamID, "I'm sorry, experience exchange is not available.  Ask your admin to enable it.");
                return;
            }
            SteamPlayer player = PlayerTool.getSteamPlayer(playerid.CSteamID);
            // Get expereience balance first
            uint exp = player.Player.Skills.Experience;
            uint examt = 0;
            UInt32.TryParse(amt, out examt);
            if (examt <= 0)
            {
                RocketChatManager.Say(playerid.CSteamID, "You have to enter an amount of experience to exchange.");
                return;
            }
            if (exp < examt)
            {
                RocketChatManager.Say(playerid.CSteamID, "You don't have " + examt.ToString() + " exprience.");
                return;
            }
            // Get experience balance first
            decimal bal = Uconomy.Instance.Database.GetBalance(playerid.CSteamID);
            decimal gain = (decimal)((float)examt * Uconomy_Extension.Instance.Configuration.ExpExchangerate);
            // Just to make sure to avoid any errors
            gain = Decimal.Round(gain, 2);
            decimal newbal = Uconomy.Instance.Database.IncreaseBalance(playerid.CSteamID, gain);
            RocketChatManager.Say(playerid.CSteamID, String.Format(Uconomy_Extension.Instance.Configuration.NewBalanceMsg, newbal.ToString(), Uconomy.Instance.Configuration.MoneyName));
            player.Player.Skills.Experience -= examt;
        }
    }
}
