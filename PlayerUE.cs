using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Core;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Logging;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Steamworks;
using unturned.ROCKS.Uconomy;

namespace Uconomy_Essentials
{
    public class PlayerUE : RocketPlayerComponent
    {
        private DateTime lastpaid;
        private RocketPlayerEvents rpe;

        protected void Start()
        {
            this.rpe = base.gameObject.transform.GetComponent<RocketPlayerEvents>();
            this.lastpaid = DateTime.Now;
            this.rpe.OnDeath += new RocketPlayerEvents.PlayerDeath(this.rpe_OnPlayerDeath);
            this.rpe.OnUpdateStat += this.rpe_OnUpdateStat;
        }
        private void rpe_OnPlayerDeath(RocketPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            if (player == null) return;
            if (murderer == null) return;
            RocketPlayer killer = RocketPlayer.FromCSteamID(murderer);
            if (killer == null) return;
            if (Uconomy_Essentials.Instance.Configuration.PayHit)
            {
                Uconomy u = Uconomy.Instance;
                decimal bal = u.Database.GetBalance(player.CSteamID);
                if (cause == EDeathCause.SUICIDE && Uconomy_Essentials.Instance.Configuration.LoseSuicide)
                {
                    // We are going to remove currency for the suicide
                    decimal loss = (decimal)Uconomy_Essentials.Instance.Configuration.LoseSuicideAmt * -1.0m;
                    if (bal + loss < 0.0m) loss = bal * -1.0m;
                    decimal bal1 = u.Database.IncreaseBalance(player.CSteamID, loss);
                    Uconomy_Essentials.HandleEvent(player, (loss * -1.0m), "loss");
                    RocketChat.Say(player.CSteamID, Uconomy_Essentials.Instance.Translate("lose_suicide_msg", new object[] {Uconomy_Essentials.Instance.Configuration.LoseSuicideAmt, u.Configuration.MoneyName}));
                    if (bal1 != 0m) RocketChat.Say(player.CSteamID, Uconomy_Essentials.Instance.Translate("new_balance_msg", new object[] {bal1, u.Configuration.MoneyName}));
                    return;
                }
                else if (cause == EDeathCause.SUICIDE && !Uconomy_Essentials.Instance.Configuration.LoseSuicide)
                {
                    // We do nothing if they suicide.
                    return;
                }
                if (Uconomy_Essentials.Instance.Configuration.LoseMoneyOnDeath)
                {
                    decimal loss = (decimal)Uconomy_Essentials.Instance.Configuration.LoseMoneyOnDeathAmt * -1.0m;
                    if (bal + loss < 0.0m) loss = bal * -1.0m;
                    decimal lostbal = u.Database.IncreaseBalance(player.CSteamID, loss);
                    Uconomy_Essentials.HandleEvent(player, (loss * -1.0m), "loss");
                    RocketChat.Say(player.CSteamID, Uconomy_Essentials.Instance.Translate("lose_money_on_death_msg", new object[] {Uconomy_Essentials.Instance.Configuration.LoseMoneyOnDeathAmt.ToString(), u.Configuration.MoneyName}));
                }
                // Pay the other player for the kill
                decimal balk = u.Database.IncreaseBalance(murderer, (decimal)Uconomy_Essentials.Instance.Configuration.PayHitAmt);
                Uconomy_Essentials.HandleEvent(player, (decimal)Uconomy_Essentials.Instance.Configuration.PayHitAmt, "paid");
                if (!Uconomy_Essentials.Instance.Configuration.SendPayHitMsg) return; // No message is to be sent, so job is done.
                RocketChat.Say(murderer, Uconomy_Essentials.Instance.Translate("to_killer_msg", new object[] {Uconomy_Essentials.Instance.Configuration.PayHitAmt.ToString(), u.Configuration.MoneyName, player.CharacterName}));
                if (bal != 0m) RocketChat.Say(murderer, Uconomy_Essentials.Instance.Translate("new_balance_msg", new object[] {balk.ToString(), u.Configuration.MoneyName}));
            }
        }
        private void rpe_OnUpdateStat(RocketPlayer player, EPlayerStat stat)
        {
            if (player == null) return;
            Uconomy u = Uconomy.Instance;
            if (Uconomy_Essentials.Instance.Configuration.PayZombie && stat == EPlayerStat.KILLS_ZOMBIES_NORMAL)
            {
                decimal balzk = u.Database.IncreaseBalance(player.CSteamID, (decimal)Uconomy_Essentials.Instance.Configuration.PayZombieAmt);
                Uconomy_Essentials.HandleEvent(player, (decimal)Uconomy_Essentials.Instance.Configuration.PayZombieAmt, "paid");
                if (!Uconomy_Essentials.Instance.Configuration.SendPayZombieMsg) return; // No message is to be sent, so job is done.
                RocketChat.Say(player.CSteamID, Uconomy_Essentials.Instance.Translate("zombie_kill_paid_msg", new object[] { Uconomy_Essentials.Instance.Configuration.PayZombieAmt.ToString(), u.Configuration.MoneyName, balzk.ToString(), u.Configuration.MoneyName }));
            }
            else if (Uconomy_Essentials.Instance.Configuration.PayMegaZombie && stat == EPlayerStat.KILLS_ZOMBIES_MEGA)
            {
                decimal balzk = u.Database.IncreaseBalance(player.CSteamID, (decimal)Uconomy_Essentials.Instance.Configuration.PayMegaZombieAmt);
                Uconomy_Essentials.HandleEvent(player, (decimal)Uconomy_Essentials.Instance.Configuration.PayMegaZombieAmt, "paid");
                if (!Uconomy_Essentials.Instance.Configuration.SendPayMegaZombieMsg) return; // No message is to be sent, so job is done.
                RocketChat.Say(player.CSteamID, Uconomy_Essentials.Instance.Translate("mega_zombie_kill_paid_msg", new object[] { Uconomy_Essentials.Instance.Configuration.PayMegaZombieAmt.ToString(), u.Configuration.MoneyName, balzk.ToString(), u.Configuration.MoneyName }));
            }
        }
        private void FixedUpdate()
        {
            if (Uconomy_Essentials.Instance.Configuration.PayTime && (DateTime.Now - this.lastpaid).TotalSeconds >= Uconomy_Essentials.Instance.Configuration.PayTimeSeconds)
            {
                this.lastpaid = DateTime.Now;
                decimal pay = 0.0m;
                Uconomy_Essentials.Instance.PayGroups.TryGetValue("all", out pay);
                string paygroup = "Player";
                if (pay == 0.0m)
                {
                    // We are checking for the different groups as All is not set.
                    if (this.Player.IsAdmin && Uconomy_Essentials.Instance.PayGroups.ContainsKey("admin"))
                    {
                        Uconomy_Essentials.Instance.PayGroups.TryGetValue("admin", out pay);
                        paygroup = "admin";
                        if (pay == 0.0m)
                        {
                            Logger.Log(Uconomy_Essentials.Instance.Translate("unable_to_pay_group_msg", new object[] {this.Player.CharacterName, "admin"}));
                            return;
                        }
                    }
                    else
                    {
                        // They aren't admin so we'll just go through like groups like normal.
                        List<Rocket.Core.Permissions.Group> plgroups = this.Player.GetGroups(true);
                        decimal pay2 = 0.0m;
                        foreach (Rocket.Core.Permissions.Group s in plgroups)
                        {
                            Uconomy_Essentials.Instance.PayGroups.TryGetValue(s.Id, out pay2);
                            if (pay2 > pay)
                            {
                                pay = pay2;
                                paygroup = s.Id;
                            }
                        }
                        if (pay == 0.0m)
                        {
                            // We assume they are default group.
                            Uconomy_Essentials.Instance.PayGroups.TryGetValue("default", out pay);
                            if (pay == 0.0m)
                            {
                                // There was an error.  End it.
                                Logger.Log(Uconomy_Essentials.Instance.Translate("unable_to_pay_group_msg", new object[] {this.Player.CharacterName, ""}));
                                return;
                            }
                        }
                    }
                }
                decimal bal = Uconomy.Instance.Database.IncreaseBalance(this.Player.CSteamID, pay);
                Uconomy_Essentials.HandleEvent(this.Player, pay, "paid");
                RocketChat.Say(this.Player.CSteamID, Uconomy_Essentials.Instance.Translate("pay_time_msg", new object[] {pay, Uconomy.Instance.Configuration.MoneyName, paygroup}));
                if (bal >= 0.0m) RocketChat.Say(this.Player.CSteamID, Uconomy_Essentials.Instance.Translate("new_balance_msg", new object[] { bal, Uconomy.Instance.Configuration.MoneyName }));
            }
        }
    }
}
