using System;

namespace NaftanRailway.UnitTests.Patterns {
    /// <summary>
    /// Когда заранее неизвестно, объекты каких типов необходимо создавать
    /// Когда система должна быть независимой от процесса создания новых объектов и расширяемой: в нее можно легко вводить новые классы, объекты которых система должна создавать.
    /// Когда создание новых объектов необходимо делегировать из базового класса классам наследникам
    /// </summary>
    class FactoryMethod {
        public void FactoryMain() {
            Developer dev = new PanelDeveloper("ООО КирпичСтрой");
            House house2 = dev.Create();

            dev = new WoodDeveloper("Частный застройщик");
            House house = dev.Create();

            Console.ReadLine();
        }
    }

    // абстрактный класс строительной компании
    abstract class Developer {
        public string Name { get; set; }

        public Developer(string n) {
            this.Name = n;
        }
        // фабричный метод
        abstract public House Create();
    }

    // строит панельные дома
    class PanelDeveloper : Developer {
        public PanelDeveloper(string n) : base(n) { }

        public override House Create() {
            return new PanelHouse();
        }
    }

    // строит деревянные дома
    class WoodDeveloper : Developer {
        public WoodDeveloper(string n) : base(n) { }

        public override House Create() {
            return new WoodHouse();
        }
    }

    abstract class House { }

    class PanelHouse : House {
        public PanelHouse() {
            Console.WriteLine("Панельный дом построен");
        }
    }
    class WoodHouse : House {
        public WoodHouse() {
            Console.WriteLine("Деревянный дом построен");
        }
    }
}