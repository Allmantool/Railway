namespace NaftanRailway.UnitTests.SOLID
{
    internal class Rectangle
    {
        protected virtual int Width { get; set; }

        protected virtual int Height { get; set; }

        public int GetArea()
        {
            return Width * Height;
        }
    }
}