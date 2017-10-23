using System;

namespace NaftanRailway.UnitTests.General {
    internal class Rectangle {
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }

        public int GetArea() {
            return Width * Height;
        }
    }

    internal class Square : Rectangle {
        public override int Width {
            get {
                return base.Width;
            }

            set {
                base.Width = value;
                base.Height = value;
            }
        }
        public override int Height {
            get {
                return base.Height;
            }

            set {
                base.Height = value;
                base.Width = value;
            }
        }
    }

    //main class
    internal class Account {
        public int Capital { get; protected set; }

        public virtual void SetCapital(int money) {
            if (money < 0) {
                throw new Exception("It's not possible to put less than 0 on account");
            }

            this.Capital = money;
        }

        public virtual decimal GetInterest(decimal sum, int month, int rate) {
            //precondition
            if (sum < 0 || month > 12 || month < 1 || rate < 0) {
                throw new Exception("Wrong data");
            }

            decimal result = sum;

            for (int i = 0; i < month; i++)
                result += result * rate / 100;

            //postcondition
            if (sum >= 1000)
                result += 100; // add some bonus

            return result;
        }
    }

    //sub Class
    internal class MicroAccount : Account {
        //preconditions - conditions can't be able to stressed by subclass 
        //Subclasses can't make additional conditional, than in base class
        public override void SetCapital(int money) {
            if (money < 0)
                throw new Exception("Нельзя положить на счет меньше 0");

            //mistake (additional condition compare to base class
            if (money > 100)
                throw new Exception("Нельзя положить на счет больше 100");

            this.Capital = money;
        }

        public override decimal GetInterest(decimal sum, int month, int rate) {
            if (sum < 0 || month > 12 || month < 1 || rate < 0)
                throw new Exception("Wrong data");

            decimal result = sum;

            for (int i = 0; i < month; i++)
                //postcondition ( loosen )
                result += result * rate / 100;

            return result;
        }
    }

    public class Solid {
        public void Liskov() {

            var rect = new Rectangle() {
                Width = 5,
                Height = 10
            };

            if (rect.GetArea() != 50) {
                throw new Exception("Wrong Area!");
            }
        }

        private void PreCondition() {
            Account acc = new MicroAccount();
            InitializeAccount(acc);

            Console.Read();
        }

        private void InitializeAccount(Account account) {
            account.SetCapital(200);

            Console.WriteLine(account.Capital);
        }

        private void CalculateInterest(Account account) {
            decimal sum = account.GetInterest(1000, 1, 10); // 1000 + 1000 * 10 / 100 + 100 (бонус)
            if (sum != 1200) // ожидаем 1200
            {
                throw new Exception("unexpected computing amount");
            }
        }

        private void PostCondition(Account account) {
            Account acc = new MicroAccount();

            //get result without bonus
            CalculateInterest(acc);

            Console.Read();
        }
    }
}