using System;
using System.Collections.Generic;
using Rocket.Components;
using Rocket.RocketAPI.Events;
using Rocket.RocketAPI;
using SDG;

namespace Uconomy_Extension
{
    public class PlayerDeath : RocketPlayerComponent
    {
        private RocketPlayer rp;
        private RocketPlayerEvents rpe;
        private Player p;
        private bool DeathMessages;

        private void Start()
        {
            this.p = base.gameObject.transform.GetComponent<Player>();
            this.rp = base.gameObject.transform.GetComponent<RocketPlayer>();
            this.rpe = base.gameObject.transform.GetComponent<RocketPlayerEvents>();
        }
    }
}
