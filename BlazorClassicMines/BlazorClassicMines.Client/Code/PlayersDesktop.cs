namespace BlazorClassicMines.Client.Code
{
    public enum StatusGames { Loading, Progress, Win, Loss }
    public class PlayersDesktop
    {
        public Point[,] Minefield { get; private set; }

        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public int NumberMine { get; set; }
        public StatusGames Stav { get; set; } = StatusGames.Loading;

        public void VytvorPole()
        {
            Stav = StatusGames.Loading;

            // Creating and placing cells
            Minefield = new Point[WindowHeight, WindowWidth];
            for (int i = 0; i < WindowHeight; i++)
                for (int j = 0; j < WindowWidth; j++)
                    Minefield[i, j] = new Point(i, j, UncoverCells);

            // Mine placement
            for (int k = 0; k < NumberMine; k++)
            {
                (int y, int x) = (Random.Shared.Next(WindowHeight), Random.Shared.Next(WindowWidth));
                while (Minefield[y, x].Typ == 9)
                    (y, x) = (Random.Shared.Next(WindowHeight), Random.Shared.Next(WindowWidth));

                Minefield[y, x].Typ = 9;

                // Numbering of items adjacent to mines
                for (int i = Math.Max(y - 1, 0); i <= Math.Min(y + 1, WindowHeight - 1); i++)
                    for (int j = Math.Max(x - 1, 0); i <= Math.Min(x + 1, WindowWidth - 1); i++)
                        if (Minefield[i, j].Typ != 9)
                            Minefield[i, j].Typ++;

            }

            Stav = StatusGames.Progress;
        }

        Queue<Point> queue = new Queue<Point>();
        private void UncoverCells(object? sender, EventArgs e)
        {
            var cell = (Point)sender;

            // Clinking to 0=> expose even the neighbors
            if (cell.Typ == 0)
            {
                queue.Clear();
                queue.Enqueue(cell);
                RevealCellAndNeighbors();
            }
            // when you click on a mine, reveal all and lose
            else if (cell.Typ == 9)
            {
                foreach (var b in Minefield)
                    if (!b.Uncovered && b.Typ == 9)
                        b.Discover(false);

                Stav = StatusGames.Loss;
                return;
            }

            //Checking your winnings
            foreach (var b in Minefield)
                if (!b.Uncovered && b.Typ != 9)
                    return;
            Stav = StatusGames.Win;
        }

        private void RevealCellAndNeighbors()
        {
            while (queue.Count > 0)
            {
                var b = queue.Dequeue();
                if (b.Typ == 0)
                {
                    b.Discover(false);
                    for (int i = Math.Max(b.Y - 1, 0); i <= Math.Min(b.Y + 1, WindowHeight - 1); i++)
                        for (int j = Math.Max(b.X - 1, 0); j <= Math.Min(b.X + 1, WindowWidth - 1); j++)
                            if (Minefield[i, j].Clickable && !queue.Contains(Minefield[i, j]))
                                queue.Enqueue(Minefield[i, j]);
                }
                else
                {
                    b.Discover(false);
                }
            }
        }
    }
}
