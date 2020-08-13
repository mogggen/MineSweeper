using System;
using System.Drawing;
using System.Windows.Forms;

namespace MineSweeper
{
    public partial class Game : Form
    {
        Random rand = new Random(); // module is used to distribute mines.
        int w = 10, h = 10, s = 34, m = 0; // Dimensions of button: w=width, h=height, s=size, m=margin
        Button b; // The properites of the box that was pressed.
        Point p; // The coordinate of the box that was pressed.
        readonly Color hidden = Color.LightGray;
        readonly Color[] palette =
            {
                Color.DarkGray,
                Color.Blue,
                Color.Green,
                Color.Red,
                Color.Purple,
                Color.Maroon,
                Color.Turquoise,
                Color.Black,
                Color.DimGray,
            };

        Land[,] land;

        bool gameOver, first;
        int flags, mines;

        //Initilize
        public Game()
        {
            InitializeComponent();
            NudBombCounter.Maximum = w * h - 13 >= 0 ? w * h - 13 : 0;
            mines = (int)NudBombCounter.Value; // Number of mines on the board.
            LblMineCounter.Text = $"bombs: {mines - flags}";
            flags = 0; // Number of flags used.
            first = true; // Boolean to let the mines be placed once, and asuring that you won't sweep a mine on first sweep.
            gameOver = false; // Locking the game state on game over.
            land = new Land[w, h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (!IsCorner(x, y))
                    {
                        land[x, y] = new Land();
                        land[x, y].Btn = new Button
                        {
                            Anchor = AnchorStyles.Top | AnchorStyles.Left,
                            Font = new Font("Microsoft sans serif", s - 19, FontStyle.Regular), // Fontsize: 16, Size: 35
                            BackColor = hidden,
                            Size = new Size(s, s),
                            Tag = new Point(x, y),
                        };


                        Controls.Add(land[x, y].Btn);
                        land[x, y].Btn.MouseDown += Game_MouseDown;
                    }
                }
            }

            Game_SizeChanged(new object(), new EventArgs());
        }

        private void Game_MouseDown(object sender, MouseEventArgs e)
        {
            if (!gameOver)
            {
                b = (Button)sender;
                p = (Point)b.Tag;

                if (MouseButtons == MouseButtons.Left)
                {
                    if (b.BackColor == hidden)
                        LeftClick();
                    else if (b.BackColor == Revealed(0))
                        SmartClick(land[((Point)b.Tag).X, ((Point)b.Tag).Y]);
                }

                if (MouseButtons == MouseButtons.Middle)
                {
                    if (b.BackColor == Revealed(0))
                        SmartClick(land[((Point)b.Tag).X, ((Point)b.Tag).Y]);
                }

                if (MouseButtons == MouseButtons.Right)
                {
                    if (b.BackColor != Revealed(0) && !first)
                    {
                        if (b.Text != "F")
                        {
                            b.ForeColor = Color.Red;
                            flags++;
                            b.Text = "F";
                        }
                        else
                        {
                            b.ForeColor = b.BackColor;
                            flags--;
                            b.Text = land[p.X, p.Y].Count.ToString();
                        }
                        //smartFlag();
                    }
                    LblMineCounter.Text = $"bombs: {mines - flags}";
                }
            }
        }

        //Start new puzzle
        private void GameStatus_Click(object sender, EventArgs e)
        {
            first = true;
            flags = 0;
            mines = (int)NudBombCounter.Value;
            LblMineCounter.Text = $"bombs: {mines - flags}";
            LblGameStatus.Visible = false;
            GameStatus.Image = Properties.Resources.playing;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (!IsCorner(x, y) && land[x, y].Btn.ForeColor != Color.White)
                    {
                        land[x, y].Btn.BackColor =
                        land[x, y].Btn.ForeColor = hidden;
                    }
                }
            }
            gameOver = false;
            first = true;
        }

        //For bigger puzzles
        private void Game_SizeChanged(object sender, EventArgs e)
        {
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (!((x == 0 || x == w - 1) && y == 0 ||
                        (x == 0 || x == w - 1) && y == h - 1))
                        land[x, y].Btn.Location = new Point((Width - (w + 1) * s) / 2 + x * (s + m), 128 + y * (s + m));
                }
            }

            NudBombCounter.Location = new Point(Width / 2 - NudBombCounter.Size.Width - GameStatus.Size.Width - 6, 87);
            GameStatus.Location = new Point(NudBombCounter.Location.X + NudBombCounter.Size.Width + 6, 23);
            LblMineCounter.Location = new Point(GameStatus.Location.X + GameStatus.Size.Width + 6, 30);
            LblGameStatus.Location = new Point(GameStatus.Location.X + GameStatus.Size.Width + 6, 77);
        }

        //places mines
        void First()
        {
            Point p = (Point)b.Tag;
            int ned, sid, placed = 0;
            bool avalible = false;

            while (placed < mines)
            {
                ned = rand.Next(h);
                sid = rand.Next(w);

                if (!IsCorner(sid, ned) &&
                    !land[sid, ned].IsMine)
                {
                    avalible = true;
                    for (int i = 0; i < 9; i++)
                    {
                        if (// is within grid
                            IsAvalible(sid, ned, i) &&

                            // safezone
                            p.X + i % 3 - 1 == sid &&
                            p.Y + i / 3 - 1 == ned)
                        {
                            avalible = false;
                        }
                    }
                    if (avalible)
                    {
                        land[sid, ned].IsMine = true;
                        land[sid, ned].Btn.Text = "X";
                        placed++;
                    }
                }
            }

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (IsAvalible(x, y))
                    {
                        land[x, y].Count = 0;

                        for (int i = 0; i < 9; i++)
                        {
                            if (land[x, y].IsMine) break;
                            if (i != 4 &&
                                IsWithinGrid(x, y, i) &&
                                !IsCorner(x, y, i) &&
                                    land[x + i % 3 - 1, y + i / 3 - 1].IsMine) land[x, y].Count++;
                        }
                        land[x, y].Btn.Text = land[x, y].Count.ToString();
                        if (x != p.X || y != p.Y) land[x, y].Btn.ForeColor = b.BackColor;
                        else b.ForeColor = Revealed(land[p.X, p.Y].Count);
                    }
                }
            }
            first = false;
        }

        void LeftClick()
        {
            Point p = (Point)b.Tag;

            if (first)
                First();

            Sweep(p.X, p.Y); // check for Lose(x, y)

            //check for win
            Win();
        }

        //to skip a few clicks
        void SmartClick(Land l)
        {
            Point p = (Point)l.Btn.Tag;
            int flags = 0;
            for (int i = 0; i < 9; i++)
            {
                if (IsAvalible(p.X, p.Y, i) &&
                    land[p.X + i % 3 - 1, p.Y + i / 3 - 1].Btn.Text == "F") flags++;
            }
            if (flags == land[p.X, p.Y].Count)
            {
                for (int i = 0; i < 9; i++)
                    if (i != 4 &&
                        IsAvalible(p.X, p.Y, i) &&
                        land[p.X + i % 3 - 1, p.Y + i / 3 - 1].Btn.Text != "F")
                        {
                            if (!land[p.X + i % 3 - 1, p.Y + i / 3 - 1].IsMine)
                                Sweep(p.X + i % 3 - 1, p.Y + i / 3 - 1);
                            else
                            {
                                Lose(p.X + i % 3 - 1, p.Y + i / 3 - 1);
                                return;
                            }
                        }
            }

            //check for win
            Win();
        }

        //Recursivly sweeps all adjacent land.
        void Sweep(int x, int y)
        {
            if (land[x, y].IsMine && land[x, y].Btn.Text != "F") Lose(x, y);

            if (land[x, y].Btn.BackColor == Revealed(0) ||
                land[x, y].Btn.Text == "F" ||
                gameOver) { return; }
            
            if (land[x, y].Btn.BackColor == hidden)
            {
                land[x, y].Btn.BackColor = Revealed(0);
                land[x, y].Btn.ForeColor = Revealed(int.Parse(land[x, y].Btn.Text));

                if (land[x, y].Btn.Text == "0")
                    for (int i = 0; i < 9; i++)
                        if (IsAvalible(x, y, i))
                        {
                            if (!land[x + i % 3 - 1, y + i / 3 - 1].IsMine)
                                Sweep(x + i % 3 - 1, y + i / 3 - 1);
                            else
                            {
                                Lose(x + i % 3 - 1, y + i / 3 - 1);
                                return;
                            }
                        }
            }
            return;
        }

        /// <summary>
        /// IsWithinGrid && !IsCorner
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        bool IsAvalible (int x, int y)
        {
            // is within grid
            bool a = IsWithinGrid(x, y);

            // is not corner
            bool c = !IsCorner(x, y);

            return a && c;
        }

        bool IsAvalible (int x, int y, int i)
        {
            // is within grid
            bool a = IsWithinGrid(x, y, i);

            // is not corner
            bool c = !IsCorner(x, y, i);

            return a && c;
        }

        bool IsWithinGrid(int x, int y) => x < w && y < h && x >= 0 && y >= 0;
        bool IsWithinGrid(int x, int y, int i) => x + i % 3 - 1 < w && y + i / 3 - 1 < h && x + i % 3 - 1 >= 0 && y + i / 3 - 1 >= 0;

        bool IsCorner(int x, int y) => (x == 0 || x == w - 1) && y == 0 || (x == 0 || x == w - 1) && y == h - 1;
        bool IsCorner(int x, int y, int i)
        {
            x = x + i % 3 - 1;
            y = y + i / 3 - 1;
            return (x == 0 || x == w - 1) && y == 0 || (x == 0 || x == w - 1) && y == h - 1;
        }

        void SmartFlag()
        {
            for (int i = 0; i < 9; i++)
            {
                int px = p.X + i % 3 - 1;
                int py = p.Y + i / 3 - 1;

                if (px < w &&
                    py < h &&
                    px >= 0 &&
                    py >= 0 &&
                    i != 4 &&
                    land[px, py].Btn.Text != "F" &&
                    int.Parse(land[px, py].Btn.Text) >= 0)
                {
                    land[px, py].Count += b.Text == "F" ? -1 : 1;
                    land[px, py].Btn.Text = land[px, py].Count.ToString();
                    land[px, py].Btn.ForeColor = land[px, py].Btn.ForeColor == hidden ? hidden : Revealed(land[px, py].Count);
                }
            }
        }

        //Check for win
        void Win()
        {
            int revealed = 0;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    if (!IsCorner(x, y) && land[x, y].Btn.BackColor == Revealed(0))
                        revealed++;

            if (revealed == w * h - mines - 4)
            {
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        if (!IsCorner(x, y))
                        {
                            if (land[x, y].IsMine &&
                                land[x, y].Btn.Text != "F")
                            {
                                land[x, y].Btn.ForeColor = Color.Red;
                                flags++;
                                land[x, y].Btn.Text = "F";
                            }
                        }
                    }
                }
                LblMineCounter.Text = $"bombs: {mines - flags}";
                GameState(true);
            }
        }

        void Lose(int x, int y)
        {
            land[x, y].Btn.BackColor = Color.Red;

            for (int ned = 0; ned < h; ned++)
            {
                for (int sid = 0; sid < w; sid++)
                {
                    if (!IsCorner(sid, ned))
                    {
                        if (land[sid, ned].IsMine && land[sid, ned].Btn.Text != "F")
                        {
                            land[sid, ned].Btn.ForeColor = Color.Black;
                            land[sid, ned].Btn.Text = "X";
                        }
                        else if (!land[sid, ned].IsMine && land[sid, ned].Btn.Text == "F")
                        {
                            land[sid, ned].Btn.ForeColor = Color.Blue;
                            land[sid, ned].Btn.Text = ">";
                        }
                    }
                }
            }
            GameState(false);
        }

        void GameState(bool win)
        {
            LblGameStatus.Visible = true;
            gameOver = true;
            LblMineCounter.Text = "< - Press me!";

            if (win)
            {
                LblGameStatus.ForeColor = Color.ForestGreen;
                LblGameStatus.Text = "You Win!";
                GameStatus.Image = Properties.Resources.win;
            }
            else
            {
                LblGameStatus.ForeColor = Color.DarkRed;
                LblGameStatus.Text = "Game Over";
                GameStatus.Image = Properties.Resources.lost;
            }
        }

        Color Revealed(int Component)
        {
            int c = Component;
            
            for (int g = 0; g < palette.Length; g++)
                if (c == g) return palette[g];
            return new Color();
        }
    }

    class Land
        {
            public Button Btn { get; set; }
            public int Count { get; set; }
            //public bool IsFlagged { get; set; }
            public bool IsMine { get; set; }
        }
}