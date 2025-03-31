namespace BlazorClassicMines.Client.Code
{
    public class Point
    {

        public bool Uncovered { get; private set; }

        public bool Marked { get; private set; }
        public bool Clickable => !Marked && !Uncovered;

        public bool Shine { get; private set; } = false;

        public string CssClass =>
            (Shine ? " svitit" : "") +
            (Typ == 9 ? " mina" : "") +
            (Typ > 0 ? $" cislo-{Typ}" : "");

        public int X { get; init; }
        public int Y { get; init; }

        int typ = 0;
        public int Typ { get => typ; set { if (value >= 0 && value <= 9) typ = value; } }

        public event EventHandler Uncover;

        public Point(int y, int x, EventHandler uncover)
        {
            (Y, X) = (y, x);
            Uncover += uncover;
        }

        public void Discover(bool triggerEvent)
        {
            if (!Clickable) return;
            Uncovered = true;

            if (triggerEvent)
            {
                if (Typ == 9) Shine = true;
                Uncover?.Invoke(this, EventArgs.Empty);
            }
        }

        public void TagPoint()
        {
            if (Uncovered) return;
            Marked = !Marked;
        }

    }
}
