using System;

namespace SevColApp.Helpers
{
    public static class CreditHelper
    {
        public static int DetermineStartingCredit(bool userHasAccount, string wealthLevel)
        {
            if (userHasAccount) return 0;

            return wealthLevel switch
            {
                "Civilian" => 1250,
                "Wealthy" => 1750,
                "Very wealthy" => 28654134,
                _ => 1000,
            };
        }

        public static int DetermineExpectedIncome(bool userHasAccount, string wealthLevel)
        {
            if (userHasAccount) return 0;

            return wealthLevel switch
            {
                "Civilian" => 750,
                "Wealthy" => 1250,
                "Very wealthy" => 1000000,
                _ => 500,
            };
        }

        public static int GenerateRandomNumber(Random generator, int min, int max)
        {
            System.Threading.Thread.Sleep(2);
            var randomNumber = generator.Next(min, max);
            System.Threading.Thread.Sleep(3);
            return randomNumber;
        }
    }
}
