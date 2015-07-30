using System;
using System.Collections.Generic;
using System.Linq;

using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using Rocket.Unturned.Plugins;
using SDG.Unturned;
using unturned.ROCKS.Uconomy;

namespace Uconomy_Essentials
{
    public class Uconomy_Essentials : RocketPlugin<UconomyEConfiguration>
    {
        public static Uconomy_Essentials Instance;
        public Dictionary<string, decimal> PayGroups = new Dictionary<string,decimal>();

        protected override void Load() {
            Uconomy_Essentials.Instance = this;
            List<Group> nlgroup = this.Configuration.Instance.PayGroups.Distinct( new GroupComparer()).ToList();
            this.Configuration.Instance.PayGroups = nlgroup;
            foreach (Group g in this.Configuration.Instance.PayGroups)
            {
                try
                {
                    this.PayGroups.Add(g.DisplayName, g.Salary);
                }
                catch (Exception e)
                {
                    //Logger.Log(g.DisplayName + " " + g.Salary.ToString());
                    //Logger.Log("There was an exception: " + e);
                }
                
            }
        }
        protected override void Unload()
        {
            Uconomy_Essentials.Instance.PayGroups.Clear();
        }
        public delegate void PlayerPaidEvent(UnturnedPlayer player, decimal amount);
        public event PlayerPaidEvent OnPlayerPaid;
        public delegate void PlayerLossEvent(UnturnedPlayer player, decimal amount);
        public event PlayerLossEvent OnPlayerLoss;
        public delegate void PlayerExchangeEvent(UnturnedPlayer player, decimal currency, uint experience, string type);
        public event PlayerExchangeEvent OnPlayerExchange;
        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList
                {
                    {
                        "pay_time_msg", 
                        "You have received {0} {1} in salary for being a {2}."
                    },
                    {
                        "unable_to_pay_group_msg",
                        "Unable to pay {0} as no {1} group salary set."
                    },
                    {
                        "to_killer_msg",
                        "You have received {0} {1} for killing {2}."
                    },
                    {
                        "lose_suicide_msg",
                        "You have had {0} {1} deducted from your account for committing suicide."
                    },
                    {
                        "new_balance_msg",
                        "Your new balance is {0} {1}."
                    },
                    {
                        "lose_money_on_death_msg",
                        "You have lost {0} {1} for being killed."
                    },
                    {
                        "apay_msg",
                        "You have paid {0} {1} {2}."
                    },
                    {
                        "apaid_msg",
                        "{0} gave you {1} {2}. You now have {3} {4}."
                    },
                    {
                        "experience_exchange_not_available",
                        "I'm sorry, experience exchange is not available.  Ask your admin to enable it."
                    },
                    {
                        "money_exchange_not_available",
                        "I'm sorry, money exchange is not available.  Ask your admin to enable it."
                    },
                    {
                        "exchange_zero_amount_error",
                        "You have to enter an amount of experience/money to exchange."
                    },
                    {
                        "exchange_insufficient_experience",
                        "You don't have {0} exprience."
                    },
                    {
                        "exchange_insufficent_money",
                        "You don't have {0} {1}."
                    },
                    {
                        "apay_usage_msg",
                        "Usage: /apay <player name or id> <amt>"
                    },
                    {
                        "exchange_usage_msg",
                        "Usage: /exchange <amount>[ money] (including money will exchange money to xp otherwise defaults xp to money)"
                    },
                    {
                        "not_valid_player_msg",
                        "{0} is not a valid player name or steam id."
                    },
                    {
                        "not_valid_amount",
                        "{0} is not a correct amount."
                    },
                    {
                        "zombie_kill_paid_msg",
                        "You have been paid {0} {1} for killing a zombie.  Your balance is now {2} {3}."
                    },
                    {
                        "mega_zombie_kill_paid_msg",
                        "You have been paid {0} {1} for killing a mega zombie.  Your balance is now {2} {3}."
                    }
                };
            }
        }

        public static void HandleEvent(UnturnedPlayer player, decimal amount, string eventtype, uint exp = 0, string type = "experience")
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
                    player.Player.gameObject.SendMessage("UEOnPlayerPaid", new object[] { player, amount });
                    break;
                case "loss":
                    if (Uconomy_Essentials.Instance.OnPlayerLoss != null)
                    Uconomy_Essentials.Instance.OnPlayerLoss(player, amount);
                    player.Player.gameObject.SendMessage("UEOnPlayerLoss", new object[] { player, amount });
                    break;
                case "exchange":
                    if (Uconomy_Essentials.Instance.OnPlayerExchange != null)
                    Uconomy_Essentials.Instance.OnPlayerExchange(player, amount, exp, type);
                    player.Player.gameObject.SendMessage("UEOnPlayerExchange", new object[] { player, amount, exp, type });
                    break;
                default:
                    Logger.Log("Invalid type supplied to throw paid/loss/experience event.");
                    break;
            }
        }
    }
}
