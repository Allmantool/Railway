using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NaftanRailway.UnitTests.Patterns {
    /// <summary>
    /// Когда процесс создания нового объекта не должен зависеть от того, из каких частей этот объект состоит и как эти части связаны между собой
    /// Когда необходимо обеспечить получение различных вариаций объекта в процессе его создания
    /// </summary>
    [TestClass]
    public class Builder {
        [TestMethod]
        public void BuilderMain() {
            // содаем объект пекаря
            Baker baker = new Baker();

            // создаем билдер для ржаного хлеба
            BreadBuilder builder = new RyeBreadBuilder();

            // выпекаем
            Bread ryeBread = baker.Bake(builder);
            Console.WriteLine(ryeBread.ToString());

            // оздаем билдер для пшеничного хлеба
            builder = new WheatBreadBuilder();

            Bread wheatBread = baker.Bake(builder);
            Console.WriteLine(wheatBread.ToString());

            Console.Read();
        }

        // пекарь
        class Baker {
            public Bread Bake(BreadBuilder breadBuilder) {
                breadBuilder.CreateBread();
                breadBuilder.SetFlour();
                breadBuilder.SetSalt();
                breadBuilder.SetAdditives();

                return breadBuilder.Bread;
            }
        }

        // абстрактный класс строителя
        abstract class BreadBuilder {
            public Bread Bread { get; private set; }
            public void CreateBread() {
                this.Bread = new Bread();
            }
            public abstract void SetFlour();
            public abstract void SetSalt();
            public abstract void SetAdditives();
        }

        // строитель для ржаного хлеба
        class RyeBreadBuilder : BreadBuilder {
            public override void SetFlour() {
                this.Bread.Flour = new Flour { Sort = "Ржаная мука 1 сорт" };
            }

            public override void SetSalt() {
                this.Bread.Salt = new Salt();
            }

            public override void SetAdditives() {
                // не используется
            }
        }
        // строитель для пшеничного хлеба
        class WheatBreadBuilder : BreadBuilder {
            public override void SetFlour() {
                this.Bread.Flour = new Flour { Sort = "Пшеничная мука высший сорт" };
            }

            public override void SetSalt() {
                this.Bread.Salt = new Salt();
            }

            public override void SetAdditives() {
                this.Bread.Additives = new Additives { Name = "улучшитель хлебопекарный" };
            }
        }

        class Bread {
            // мука
            public Flour Flour { get; set; }
            // соль
            public Salt Salt { get; set; }
            // пищевые добавки
            public Additives Additives { get; set; }
            public override string ToString() {
                StringBuilder sb = new StringBuilder();

                if (this.Flour != null)
                    sb.Append(this.Flour.Sort + "\n");
                if (this.Salt != null)
                    sb.Append("Соль \n");
                if (this.Additives != null)
                    sb.Append("Добавки: " + this.Additives.Name + " \n");

                return sb.ToString();
            }
        }

        //мука
        class Flour {
            // какого сорта мука
            public string Sort { get; set; }
        }

        // соль
        class Salt { }

        // пищевые добавки
        class Additives {
            public string Name { get; set; }
        }
    }
}