namespace NaftanRailway.UnitTests.SOLID
{
    using System;

    internal class Account
    {
        public int Capital { get; protected set; }

        public virtual void SetCapital(int money)
        {
            if (money < 0)
            {
                throw new Exception("It's not possible to put less than 0 on account");
            }

            this.Capital = money;
        }

        public virtual decimal GetInterest(decimal sum, int month, int rate)
        {
            // Precondition.
            if (sum < 0 || month > 12 || month < 1 || rate < 0)
            {
                throw new Exception("Wrong data");
            }

            decimal result = sum;

            for (int i = 0; i < month; i++)
            {
                result += result * rate / 100;
            }

            // Postcondition.
            if (sum >= 1000)
            {
                result += 100; // add some bonus
            }

            return result;
        }
    }
}