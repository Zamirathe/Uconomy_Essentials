using System;
using Rocket.Components;
using Rocket.RocketAPI.Events;
using Rocket.RocketAPI;
using Rocket.Logging;
using SDG;
using UnityEngine;
using Steamworks;
using unturned.ROCKS.Uconomy;

namespace Uconomy_Extension
{
    public class PlayerDeath : RocketPlayerComponent
    {
        private RocketPlayerEvents rpe;
        private UconomyEConfiguration config;

        private void Start()
        {
            this.rpe = base.gameObject.transform.GetComponent<RocketPlayerEvents>();
            this.config = Uconomy_Extension.Instance.Configuration;
            this.rpe.OnDeath += new RocketPlayerEvents.PlayerDeath(this.rpe_OnPlayerDeath);
        }
        private void rpe_OnPlayerDeath(Player player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            if (player == null) return;
            if (murderer == null) return;
            SteamPlayer killer = PlayerTool.getSteamPlayer(murderer);
            if (killer == null) return;
            if (this.config.PayHit)
            {
                Uconomy u = Uconomy.Instance;
                decimal bal = u.Database.GetBalance(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID);
                if (cause == EDeathCause.SUICIDE && this.config.LoseSuicide)
                {
                    // We are going to remove currency for the suicide
                    decimal loss = (decimal)this.config.LoseSuicideAmt * -1.0m;
                    if (bal + loss < 0.0m) loss = bal * -1.0m;
                    decimal bal1 = u.Database.IncreaseBalance(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, loss);
                    RocketChatManager.Say(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, String.Format(this.config.LoseSuicideMsg, this.config.LoseSuicideAmt, u.Configuration.MoneyName));
                    if (bal1 != 0m) RocketChatManager.Say(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, String.Format(this.config.NewBalanceMsg, bal1, u.Configuration.MoneyName));
                    return;
                }
                else if (cause == EDeathCause.SUICIDE && !this.config.LoseSuicide)
                {
                    // We do nothing if they suicide.
                    return;
                }
                if (Uconomy_Extension.Instance.Configuration.LoseMoneyOnDeath)
                {
                    decimal loss = (decimal)this.config.LoseMoneyOnDeathAmt * -1.0m;
                    if (bal + loss < 0.0m) loss = bal * -1.0m;
                    decimal lostbal = u.Database.IncreaseBalance(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, loss);
                    RocketChatManager.Say(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, String.Format(this.config.LoseMoneyonDeathMsg, this.config.LoseMoneyOnDeathAmt.ToString(), u.Configuration.MoneyName));
                }
                // Pay the other player for the kill
                decimal balk = u.Database.IncreaseBalance(murderer, (decimal)this.config.PayHitAmt);
                RocketChatManager.Say(murderer, String.Format(this.config.ToKillerMsg, this.config.PayHitAmt.ToString(), u.Configuration.MoneyName, player.SteamChannel.SteamPlayer.SteamPlayerID.CharacterName));
                if (bal != 0m) RocketChatManager.Say(murderer, String.Format(this.config.NewBalanceMsg, balk.ToString(), u.Configuration.MoneyName));
            }
        }
    }
}
