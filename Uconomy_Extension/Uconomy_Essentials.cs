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
    }
}
