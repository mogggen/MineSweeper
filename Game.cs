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
        Color hidden = Color.LightGray;

        Land[,] land;

        bool gameOver, first, replay;
        int flags, mines;

        //Initilize
        public Game()
        {
            InitializeComponent();

            NudBombCounter.Maximum = w * h - 9 >= 0 ? w * h - 9 : 0;
            mines = (int)NudBombCounter.Value; // Number of mines on the board.
            LblMineCounter.Text = $"mines flagged:\n{flags} / {mines}";
            flags = 0; // Number of flags used.
            first = true; // Boolean to let the mines be placed once, and asuring that you won't sweep a mine on first sweep.
            replay = false; // Gives the player the oppertunity to replay.
            gameOver = false; // Locking the game state on game over.
            land = new Land[w, h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    land[x, y] = new Land();
                    land[x, y].Btn = new Button
                    {
                        Anchor = AnchorStyles.Top | AnchorStyles.Left,
                        Font = new Font("Microsoft sans serif", s - 19, FontStyle.Regular), // Fontsize: 16, Size: 35
                        BackColor = hidden,
                        Location = new Point(Width / 2 - (w / 2 * s + (w / 2 - 1)) + x * (s + m), Height / 2 - (h / 2 * s + (h / 2 - 1)) + y * (s + m)),
                        Size = new Size(s, s),
                        Tag = new Point(x, y),
                    };

                    Controls.Add(land[x, y].Btn);
                    land[x, y].Btn.MouseDown += Game_MouseDown;
                }
            }
        }

        private void Game_MouseDown(object sender, MouseEventArgs e)
        {
            if (!gameOver)
            {
                b = (Button)sender;
                p = (Point)b.Tag;

                if (MouseButtons == MouseButtons.Left && b.Text != "F")
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

                if (MouseButtons == MouseButtons.Right && !first)
                {
                    if (b.Text != "F" && b.BackColor != Revealed(0))
                    {
                        b.ForeColor = Color.Red;
                        flags++;
                        b.Text = "F";
                    }

                    else if (b.BackColor != Revealed(0))
                    {
                        b.ForeColor = b.BackColor;
                        flags--;
                        b.Text = land[p.X, p.Y].Count.ToString();
                    }
                    LblMineCounter.Text = $"mines flagged:\n{flags} / {mines}";
                }
            }
        }

        //Start new puzzle
        private void GameStatus_Click(object sender, EventArgs e)
        {
            replay = false;
            first = true;
            flags = 0;
            mines = (int)NudBombCounter.Value;
            LblMineCounter.Text = $"mines flagged:\n{flags} / {mines}";
            LblGameStatus.Visible = false;
            GameStatus.Image = Properties.Resources.playing;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (land[x, y].Btn.ForeColor != Color.White)
                        land[x, y].Btn.BackColor =
                        land[x, y].Btn.ForeColor = hidden;
                    //replay puzzle
                    if (!replay)
                        land[x, y].IsMine = false;
                }
            }
            gameOver = false;
            first = true;
        }

        //Replay current puzzle
        private void BtnReplay_Click(object sender, EventArgs e)
        {
            GameStatus_Click(new object(), new EventArgs());
            replay = true;
        }

        //Open the help form
        private void Help_Click(object sender, EventArgs e)
        {
            Help help = new Help();
            help.Show();
        }
        
        //For bigger puzzles
        private void Game_SizeChanged(object sender, EventArgs e)
        {
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    land[x, y].Btn.Location = new Point(Width / 2 - (w / 2 * s + (w / 2 - 1)) + x * (s + m), Height / 2 - (h / 2 * s + (h / 2 - 1)) + y * (s + m));
                }
            }
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

                if (!land[sid, ned].IsMine)
                {
                    avalible = true;
                    for (int i = 0; i < 9; i++)
                    {
                        if (p.X + i % 3 - 1 < w &&
                            p.Y + i / 3 - 1 < h &&
                            p.X + i % 3 - 1 >= 0 &&
                            p.Y + i / 3 - 1 >= 0 &&
                            p.X + i % 3 - 1 == sid && p.Y + i / 3 - 1 == ned)
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
                    land[x, y].Count = 0;

                    for (int i = 0; i < 9; i++)
                    {
                        if (land[x, y].IsMine) break;
                        if (x + i % 3 - 1 < w &&
                            y + i / 3 - 1 < h &&
                            x + i % 3 - 1 >= 0 &&
                            y + i / 3 - 1 >= 0 &&
                            i != 4 &&
                            land[x + i % 3 - 1, y + i / 3 - 1].IsMine) land[x, y].Count++;
                    }
                    land[x, y].Btn.Text = land[x, y].Count.ToString();
                    if (x != p.X || y != p.Y) land[x, y].Btn.ForeColor = b.BackColor;
                    else b.ForeColor = Revealed(land[p.X, p.Y].Count);
                }
            }
            first = false;
        }

        void LeftClick()
        {
            Point p = (Point)b.Tag;

            if (first && !replay)
                First();

            if (land[p.X, p.Y].IsMine)
            {
                Lose(p.X, p.Y);
            }
            else
            {
                Sweep(p.X, p.Y);
            }

            //check for win
            int revealed = 0;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    if (land[x, y].Btn.BackColor == Revealed(0))
                        revealed++;

            if (revealed == w * h - mines && !land[p.X, p.Y].IsMine)
            {
                Win();
            }
        }

        //to skip a few clicks
        void SmartClick(Land l)
        {
            Point p = (Point)l.Btn.Tag;
            int flags = 0;
            for (int i = 0; i < 9; i++)
            {
                if (p.X + i % 3 - 1 < w &&
                    p.Y + i / 3 - 1 < h &&
                    p.X + i % 3 - 1 >= 0 &&
                    p.Y + i / 3 - 1 >= 0 &&
                    land[p.X + i % 3 - 1, p.Y + i / 3 - 1].Btn.Text == "F") flags++;
            }
            if (flags == land[p.X, p.Y].Count)
            {
                for (int i = 0; i < 9; i++)
                    if (i != 4)
                        if (p.X + i % 3 - 1 < w &&
                            p.Y + i / 3 - 1 < h &&
                            p.X + i % 3 - 1 >= 0 &&
                            p.Y + i / 3 - 1 >= 0 &&
                            land[p.X + i % 3 - 1, p.Y + i / 3 - 1].Btn.Text != "F")
                        {
                            if (!land[p.X + i % 3 - 1, p.Y + i / 3 - 1].IsMine)
                                Sweep(p.X + i % 3 - 1, p.Y + i / 3 - 1);
                            else
                                Lose(p.X + i % 3 - 1, p.Y + i / 3 - 1);
                        }
            }

            //check for win
            int revealed = 0;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    if (land[x, y].Btn.BackColor == Revealed(0))
                        revealed++;

            if (revealed == w * h - mines && !land[p.X, p.Y].IsMine)
            {
                Win();
            }
        }

        //Recursivly sweeps all adjacent land.
        void Sweep(int x, int y)
        {
            if (land[x, y].Btn.BackColor == Revealed(0) ||
                land[x, y].Btn.Text == "F" ||
                gameOver) { return; }

            if (land[x, y].Btn.BackColor == hidden)
            {
                land[x, y].Btn.BackColor = Revealed(0);
                land[x, y].Btn.ForeColor = Revealed(int.Parse(land[x, y].Btn.Text));

                if (land[x, y].Btn.Text == "0")
                    for (int i = 0; i < 9; i++)
                        if (x + i % 3 - 1 < w &&
                            y + i / 3 - 1 < h &&
                            x + i % 3 - 1 >= 0 &&
                            y + i / 3 - 1 >= 0)
                        {
                            if (!land[x + i % 3 - 1, y + i / 3 - 1].IsMine)
                                Sweep(x + i % 3 - 1, y + i / 3 - 1);
                            else
                            {
                                Lose(x + i % 3 - 1, y + i / 3 - 1);
                            }
                        }
            }
            return;
        }

        void Win()
        {
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (land[x, y].IsMine && land[x, y].Btn.Text != "F")
                    {
                        land[x, y].Btn.ForeColor = Color.Red;
                        flags++;
                        land[x, y].Btn.Text = "F";

                    }
                }
            }
            LblMineCounter.Text = $"mines flagged:\n{flags} / {mines}";
            LblGameStatus.Visible = true;
            LblGameStatus.ForeColor = Color.ForestGreen;
            LblGameStatus.Text = "You Win!";
            GameStatus.Image = Properties.Resources.win;
            gameOver = true;
        }

        void Lose(int x, int y)
        {
            land[x, y].Btn.BackColor = Color.Red;

            for (int ned = 0; ned < h; ned++)
            {
                for (int sid = 0; sid < w; sid++)
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
            LblGameStatus.Visible = true;
            LblGameStatus.ForeColor = Color.DarkRed;
            LblGameStatus.Text = "Game Over";
            GameStatus.Image = Properties.Resources.lost;
            gameOver = true;
        }

        Color Revealed(int Component)
        {
            int c = Component;
            Color[] palette =
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
            for (int g = 0; g < palette.Length; g++)
                if (c == g) return palette[g];
            return new Color();
        }

        class Land
        {
            public Button Btn { get; set; }
            public int Count { get; set; }
            //public bool IsFlagged { get; set; }
            public bool IsMine { get; set; }
        }
    }
}