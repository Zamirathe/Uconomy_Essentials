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
            if (murderer == null) return;
            SteamPlayer killer = PlayerTool.getSteamPlayer(murderer);
            if (killer == null) return;
            if (this.config.PayHit)
            {
                if (cause == EDeathCause.SUICIDE && this.config.LoseSuicide)
                {
                    // We are going to remove currency for the suicide
                    Uconomy u1 = Uconomy.Instance;
                    decimal bal1 = u1.Database.IncreaseBalance(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, (decimal)this.config.LoseSuicideAmt*-1.0m);
                    RocketChatManager.Say(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, String.Format(this.config.LoseSuicideMsg, this.config.LoseSuicideAmt, Uconomy.Instance.Configuration.MoneyName));
                    if (bal1 != 0m) RocketChatManager.Say(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, String.Format(this.config.NewBalanceMsg, bal1, Uconomy.Instance.Configuration.MoneyName));
                    return;
                }
                else if (cause == EDeathCause.SUICIDE && !this.config.LoseSuicide)
                {
                    // We do nothing if they suicide.
                    return;
                }
                if (Uconomy_Extension.Instance.Configuration.LoseMoneyOnDeath)
                {
                    Uconomy u2 = Uconomy.Instance;
                    decimal lostbal = u2.Database.IncreaseBalance(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, (decimal)this.config.LoseMoneyOnDeathAmt * -1.0m);
                    RocketChatManager.Say(player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID, String.Format(this.config.LoseMoneyonDeathMsg, this.config.LoseMoneyOnDeathAmt.ToString(), u2.Configuration.MoneyName));
                }
                // Pay the other player for the kill
                Uconomy u = Uconomy.Instance;
                decimal bal = u.Database.IncreaseBalance(murderer, (decimal)this.config.PayHitAmt);
                RocketChatManager.Say(murderer, String.Format(this.config.ToKillerMsg, this.config.PayHitAmt, Uconomy.Instance.Configuration.MoneyName, player.SteamChannel.SteamPlayer.SteamPlayerID.CharacterName));
                if (bal != 0m) RocketChatManager.Say(murderer, String.Format(this.config.NewBalanceMsg, bal, Uconomy.Instance.Configuration.MoneyName));
            }
        }
    }
}
