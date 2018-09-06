namespace NaftanRailway.UnitTests.SOLID
{
    using System;

    internal class MicroAccount : Account
    {
        // Preconditions - conditions can't be able to stressed by subclass.
        // Subclasses can't make additional conditional, than in base class.
        public override void SetCapital(int money)
        {
            if (money < 0)
            {
                throw new Exception("Нельзя положить на счет меньше 0");
            }

            // Mistake (additional condition compare to base class.
            if (money > 100)
            {
                throw new Exception("Нельзя положить на счет больше 100");
            }

            this.Capital = money;
        }

        public override decimal GetInterest(decimal sum, int month, int rate)
        {
            if (sum < 0 || month > 12 || month < 1 || rate < 0)
            {
                throw new Exception("Wrong data");
            }

            decimal result = sum;

            for (int i = 0; i < month; i++)
            {
                // Postcondition ( loosen ).
                result += result * rate / 100;
            }

            return result;
        }
    }
}