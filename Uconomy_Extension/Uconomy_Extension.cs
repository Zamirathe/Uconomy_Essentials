using System;
using System.Collections.Generic;
using Rocket.RocketAPI;
using SDG;
using unturned.ROCKS.Uconomy;

namespace Uconomy_Extension
{
    public class Uconomy_Extension : RocketPlugin<UconomyEConfiguration>
    {
        private DateTime lastpaid;
        public static Uconomy_Extension Instance;

        protected override void Load() {
            this.lastpaid = DateTime.Now;
            Uconomy_Extension.Instance = this;
        }
        private void FixedUpdate()
        {
            if (Uconomy_Extension.Instance.Configuration.PayTime && (DateTime.Now - this.lastpaid).TotalSeconds >= Uconomy_Extension.Instance.Configuration.PayTimeSeconds)
            {
                foreach (SteamPlayer pl in PlayerTool.getSteamPlayers())
                {
                    decimal bal = Uconomy.Instance.Database.IncreaseBalance(pl.SteamPlayerID.CSteamID, (decimal)Uconomy_Extension.Instance.Configuration.PayTimeAmt);
                    RocketChatManager.Say(pl.SteamPlayerID.CSteamID, String.Format(Uconomy_Extension.Instance.Configuration.PayTimeMsg, Uconomy_Extension.Instance.Configuration.PayTimeAmt, Uconomy.Instance.Configuration.MoneyName));
                    if (bal != null) RocketChatManager.Say(pl.SteamPlayerID.CSteamID, String.Format(Uconomy_Extension.Instance.Configuration.NewBalanceMsg, bal, Uconomy.Instance.Configuration.MoneyName));
                }
                this.lastpaid = DateTime.Now;
            }
        }
    }
}
