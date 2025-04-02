namespace BlazorClassicMines.Client.Code
{
    /// <summary>
    /// Enum representing the current status of the game.
    /// </summary>
    public enum StatusGames { Loading, Progress, Win, Loss }

    /// <summary>
    /// Main class representing the player's desktop in the Minesweeper game.
    /// It handles game logic, minefield generation, and game status updates.
    /// </summary>
    public class PlayersDesktop
    {
        /// <summary>
        /// Two-dimensional array representing the minefield.
        /// </summary>
        public Point[,] Minefield { get; private set; }

        /// <summary>
        /// Width of the game window (number of columns).
        /// </summary>
        public int WindowWidth { get; set; }

        /// <summary>
        /// Height of the game window (number of rows).
        /// </summary>
        public int WindowHeight { get; set; }

        /// <summary>
        /// Total number of mines to be placed on the field.
        /// </summary>
        public int NumberMine { get; set; }

        /// <summary>
        /// Current status of the game (Loading, In Progress, Win, or Loss).
        /// </summary>
        public StatusGames Status { get; set; } = StatusGames.Loading;

        /// <summary>
        /// Time when the game started.
        /// </summary>
        public DateTime? StartTime { get; private set; }

        /// <summary>
        /// Gets the total time elapsed since the game started.
        /// </summary>
        public TimeSpan Elapsed => StartTime.HasValue ? DateTime.Now - StartTime.Value : TimeSpan.Zero;

        /// <summary>
        /// Gets the number of flags (marked cells) currently placed by the player.
        /// </summary>
        public int FlagsPlaced => Minefield.Cast<Point>().Count(p => p.Marked);

        /// <summary>
        /// Gets the number of remaining mines that are not yet flagged.
        /// </summary>
        public int MinesLeft => NumberMine - FlagsPlaced;


        /// <summary>
        /// Initializes a new minefield, places mines randomly, and calculates numbers for surrounding cells.
        /// </summary>
        public void CreateField()
        {
            Status = StatusGames.Loading;
            StartTime = DateTime.Now;

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
                    for (int j = Math.Max(x - 1, 0); j <= Math.Min(x + 1, WindowWidth - 1); j++)
                        if (Minefield[i, j].Typ != 9)
                            Minefield[i, j].Typ++;

            }

            Status = StatusGames.Progress;
        }

        Queue<Point> queue = new Queue<Point>();

        /// <summary>
        /// Handles cell uncovering logic based on what type of cell was clicked.
        /// Triggers win/loss conditions accordingly.
        /// </summary>
        /// <param name="sender">The cell that was clicked.</param>
        /// <param name="e">Event arguments (not used).</param>
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

                Status = StatusGames.Loss;
                return;
            }

            //Checking your winnings
            foreach (var b in Minefield)
                if (!b.Uncovered && b.Typ != 9)
                    return;
            Status = StatusGames.Win;
        }

        /// <summary>
        /// Reveals all neighboring cells recursively starting from an empty cell (Typ == 0).
        /// </summary>
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
