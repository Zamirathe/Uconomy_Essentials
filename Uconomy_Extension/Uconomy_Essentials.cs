using System;
using System.Collections.Generic;
using Rocket.RocketAPI;
using SDG;
using unturned.ROCKS.Uconomy;

namespace Uconomy_Essentials
{
    public class Uconomy_Essentials : RocketPlugin<UconomyEConfiguration>
    {
        private DateTime lastpaid;
        public static Uconomy_Essentials Instance;

        protected override void Load() {
            this.lastpaid = DateTime.Now;
            Uconomy_Essentials.Instance = this;
        }
        private void FixedUpdate()
        {
            if (Uconomy_Essentials.Instance.Configuration.PayTime && (DateTime.Now - this.lastpaid).TotalSeconds >= Uconomy_Essentials.Instance.Configuration.PayTimeSeconds)
            {
                foreach (SteamPlayer pl in PlayerTool.getSteamPlayers())
                {
                    decimal bal = Uconomy.Instance.Database.IncreaseBalance(pl.SteamPlayerID.CSteamID, (decimal)Uconomy_Essentials.Instance.Configuration.PayTimeAmt);
                    RocketChatManager.Say(pl.SteamPlayerID.CSteamID, String.Format(Uconomy_Essentials.Instance.Configuration.PayTimeMsg, Uconomy_Essentials.Instance.Configuration.PayTimeAmt, Uconomy.Instance.Configuration.MoneyName));
                    if (bal != null) RocketChatManager.Say(pl.SteamPlayerID.CSteamID, String.Format(Uconomy_Essentials.Instance.Configuration.NewBalanceMsg, bal, Uconomy.Instance.Configuration.MoneyName));
                }
                this.lastpaid = DateTime.Now;
            }
        }
    }
}
