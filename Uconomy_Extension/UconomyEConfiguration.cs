using System;
using System.Collections.Generic;
using Rocket.RocketAPI;

namespace Uconomy_Essentials
{
    public class UconomyEConfiguration : RocketConfiguration
    {
        public bool PayTime;
        public List<Group> PayGroups;
        public ushort PayTimeSeconds;
        public string PayTimeMsg;
        public string UnableToPayGroupMsg;
        public bool PayHit;
        public float PayHitAmt;
        public string ToKillerMsg;
        public bool LoseSuicide;
        public float LoseSuicideAmt;
        public string LoseSuicideMsg;
        public bool ExpExchange;
        public float ExpExchangerate;
        public string NewBalanceMsg;
        public bool LoseMoneyOnDeath;
        public float LoseMoneyOnDeathAmt;
        public string LoseMoneyonDeathMsg;


        public RocketConfiguration DefaultConfiguration
        {
            get
            {
                return new UconomyEConfiguration()
                {
                    PayTime = false,
                    PayGroups = new List<Group>() {
                        new Group{DisplayName = "all", Salary = 1.0m},
                        new Group{DisplayName = "admin", Salary = 5.0m},
                        new Group{DisplayName = "moderator", Salary = 4.0m},
                        new Group{DisplayName = "guest", Salary = 1.0m}
                    },
                    PayTimeSeconds = 900,
                    PayTimeMsg = "You have received {0} {1} in salary for being a {2}.",
                    UnableToPayGroupMsg = "Unable to pay {0} as no {1} group salary set.",
                    PayHit = false,
                    PayHitAmt = 1.0f,
                    ToKillerMsg = "You have received {0} {1} for killing {2}.",
                    LoseSuicide = false,
                    LoseSuicideAmt = 1.0f,
                    LoseSuicideMsg = "You have had {0} {1} deducted from your account for committing suicide.",
                    ExpExchange = false,
                    ExpExchangerate = 0.5f,
                    NewBalanceMsg = "Your new balance is {0} {1}.",
                    LoseMoneyOnDeath = false,
                    LoseMoneyOnDeathAmt = 10.0f,
                    LoseMoneyonDeathMsg = "You have lost {0} {1} for being killed."
                };
            }
        }
    }
}
