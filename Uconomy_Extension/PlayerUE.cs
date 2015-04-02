﻿using System;
using System.Collections.Generic;
using Rocket.Components;
using Rocket.RocketAPI.Events;
using Rocket.RocketAPI;
using Rocket.Logging;
using SDG;
using UnityEngine;
using Steamworks;
using unturned.ROCKS.Uconomy;

namespace Uconomy_Essentials
{
    public class PlayerUE : RocketPlayerComponent
    {
        private DateTime lastpaid;
        private RocketPlayerEvents rpe;

        private void Start()
        {
            this.rpe = base.gameObject.transform.GetComponent<RocketPlayerEvents>();
            this.lastpaid = DateTime.Now;
            this.rpe.OnDeath += new RocketPlayerEvents.PlayerDeath(this.rpe_OnPlayerDeath);
        }
        private void rpe_OnPlayerDeath(Player player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            if (player == null) return;
            if (murderer == null) return;
            SteamPlayer killer = PlayerTool.getSteamPlayer(murderer);
            if (killer == null) return;
            if (Uconomy_Essentials.Instance.Configuration.PayHit)
            {
                Uconomy u = Uconomy.Instance;
                decimal bal = u.Database.GetBalance(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID);
                if (cause == EDeathCause.SUICIDE && Uconomy_Essentials.Instance.Configuration.LoseSuicide)
                {
                    // We are going to remove currency for the suicide
                    decimal loss = (decimal)Uconomy_Essentials.Instance.Configuration.LoseSuicideAmt * -1.0m;
                    if (bal + loss < 0.0m) loss = bal * -1.0m;
                    decimal bal1 = u.Database.IncreaseBalance(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, loss);
                    RocketChatManager.Say(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, String.Format(Uconomy_Essentials.Instance.Configuration.LoseSuicideMsg, Uconomy_Essentials.Instance.Configuration.LoseSuicideAmt, u.Configuration.MoneyName));
                    if (bal1 != 0m) RocketChatManager.Say(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, String.Format(Uconomy_Essentials.Instance.Configuration.NewBalanceMsg, bal1, u.Configuration.MoneyName));
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
                    decimal lostbal = u.Database.IncreaseBalance(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, loss);
                    RocketChatManager.Say(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, String.Format(Uconomy_Essentials.Instance.Configuration.LoseMoneyonDeathMsg, Uconomy_Essentials.Instance.Configuration.LoseMoneyOnDeathAmt.ToString(), u.Configuration.MoneyName));
                }
                // Pay the other player for the kill
                decimal balk = u.Database.IncreaseBalance(murderer, (decimal)Uconomy_Essentials.Instance.Configuration.PayHitAmt);
                RocketChatManager.Say(murderer, String.Format(Uconomy_Essentials.Instance.Configuration.ToKillerMsg, Uconomy_Essentials.Instance.Configuration.PayHitAmt.ToString(), u.Configuration.MoneyName, player.SteamChannel.SteamPlayer.SteamPlayerID.CharacterName));
                if (bal != 0m) RocketChatManager.Say(murderer, String.Format(Uconomy_Essentials.Instance.Configuration.NewBalanceMsg, balk.ToString(), u.Configuration.MoneyName));
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
                    if (this.PlayerInstance.SteamChannel.SteamPlayer.IsAdmin && Uconomy_Essentials.Instance.PayGroups.ContainsKey("admin"))
                    {
                        Uconomy_Essentials.Instance.PayGroups.TryGetValue("admin", out pay);
                        paygroup = "admin";
                        if (pay == 0.0m)
                        {
                            Logger.Log(String.Format(Uconomy_Essentials.Instance.Configuration.UnableToPayGroupMsg, this.PlayerInstance.SteamChannel.SteamPlayer.SteamPlayerID.CharacterName, "admin"));
                            return;
                        }
                    }
                    else
                    {
                        // They aren't admin so we'll just go through like groups like normal.
                        List<Rocket.RocketAPI.Group> plgroups = this.PlayerInstance.GetGroups();
                        decimal pay2 = 0.0m;
                        foreach (Rocket.RocketAPI.Group s in plgroups)
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
                            Logger.Log(String.Format(Uconomy_Essentials.Instance.Configuration.UnableToPayGroupMsg, this.PlayerInstance.SteamChannel.SteamPlayer.SteamPlayerID.CharacterName, ""));
                            return;
                        }
                    }
                }
                decimal bal = Uconomy.Instance.Database.IncreaseBalance(this.PlayerInstance.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, pay);
                RocketChatManager.Say(this.PlayerInstance.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, String.Format(Uconomy_Essentials.Instance.Configuration.PayTimeMsg, pay, Uconomy.Instance.Configuration.MoneyName, paygroup));
                if (bal >= 0.0m) RocketChatManager.Say(this.PlayerInstance.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, String.Format(Uconomy_Essentials.Instance.Configuration.NewBalanceMsg, bal, Uconomy.Instance.Configuration.MoneyName));
            }
        }
    }
}
