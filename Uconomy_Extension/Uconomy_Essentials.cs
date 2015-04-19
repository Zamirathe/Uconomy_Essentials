using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.RocketAPI;
using Rocket.Logging;
using SDG;
using unturned.ROCKS.Uconomy;

namespace Uconomy_Essentials
{
    public class Uconomy_Essentials : RocketPlugin<UconomyEConfiguration>
    {
        public static Uconomy_Essentials Instance;
        public Dictionary<string, decimal> PayGroups = new Dictionary<string,decimal>();

        protected override void Load() {
            Uconomy_Essentials.Instance = this;
            if (Loaded)
            {
                foreach (Group g in this.Configuration.PayGroups)
                {
                    this.PayGroups.Add(g.DisplayName, g.Salary);
                }
            }
        }
        public delegate void PlayerPaidEvent(RocketPlayer player, decimal amount);
        public event PlayerPaidEvent OnPlayerPaid;
        public delegate void PlayerLossEvent(RocketPlayer player, decimal amount);
        public event PlayerLossEvent OnPlayerLoss;
        public delegate void PlayerExchangeEvent(RocketPlayer player, decimal currency, uint experience);
        public event PlayerExchangeEvent OnPlayerExchange;

        public static void HandleEvent(RocketPlayer player, decimal amount, string eventtype, uint exp = 0)
        {
            if (eventtype == "exchange" && exp <= 0)
            {
                Logger.Log("OnPlayerExchange could not be fired as no experience was given.");
                return;
            }
            if (amount == 0.0m || player == null) {
                Logger.Log(eventtype + " was not fired as player or amount not given.");
                return;
            }
            switch (eventtype)
            {
                case "paid":
                    if (Uconomy_Essentials.Instance.OnPlayerPaid != null)
                    Uconomy_Essentials.Instance.OnPlayerPaid(player, amount);
                    break;
                case "loss":
                    if (Uconomy_Essentials.Instance.OnPlayerLoss != null)
                    Uconomy_Essentials.Instance.OnPlayerLoss(player, amount);
                    break;
                case "exchange":
                    if (Uconomy_Essentials.Instance.OnPlayerExchange != null)
                    Uconomy_Essentials.Instance.OnPlayerExchange(player, amount, exp);
                    break;
                default:
                    Logger.Log("Invalid type supplied to throw paid/loss/experience event.");
                    break;
            }
        }
    }
}
