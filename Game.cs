using System;
using System.Drawing;
using System.Windows.Forms;

namespace MineSweeper
{
    public partial class Game : Form
    {
        Random rand = new Random(); // module is used to distribute mines.
        int w = 10, h = 10, s = 35, m = 0; // Dimensions of button: w=width, h=height, s=size, m=margin
        Button b; // The properites of the box that was pressed.
        Point p; // The coordinate of the box that was pressed.

        Land[,] land;

        bool gameOver, first, replay;
        int flags, mines;

        public Game()
        {
            InitializeComponent();

            LblMineCounter.Text = $"mines flagged:\n{flags} / {mines}";
            LblGameStatus.Visible = false;
            mines = (int)NudBombCounter.Value; // Number of mines on the board.
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
                        BackColor = Color.White,
                        Location = new Point(Width / 2 - (w / 2 * s + (w / 2 - 1)) + x * (s + m), Height / 2 - (h / 2 * s + (h / 2 - 1)) + y * (s + m)),
                        Size = new Size(s, s),
                        Tag = new Point(x, y),
                    };

                    Controls.Add(land[x, y].Btn);
                    land[x, y].Btn.MouseDown += Game_MouseDown;
                }
            }
        }

        private void GameStatus_Click(object sender, EventArgs e)
        {
            replay = false;
            first = true;
            flags = 0;
            mines = (int)NudBombCounter.Value;
            GameStatus.BackColor = Color.LightGray;
            LblMineCounter.Text = $"mines flagged:\n{flags} / {mines}";
            LblGameStatus.Visible = false;
            GameStatus.Image = Properties.Resources.playing;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    land[x, y].Btn.BackColor =
                    land[x, y].Btn.ForeColor = Color.White;
                    //replay puzzle
                    if (!replay)
                        land[x, y].IsMine = false;
                }
            }
            gameOver = false;
        }

        private void Help_Click(object sender, EventArgs e)
        {
            Help help = new Help();
            help.Show();
        }

        private void BtnReplay_Click(object sender, EventArgs e)
        {
            replay = true;
            first = true;
            flags = 0;
            GameStatus.BackColor = Color.LightGray;
            GameStatus.Image = Properties.Resources.playing;
            LblMineCounter.Text = $"mines flagged:\n{flags} / {mines}";
            LblGameStatus.Visible = false;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    land[x, y].Btn.BackColor = 
                    land[x, y].Btn.ForeColor = Color.White;
                    land[x, y].Btn.Text = land[x, y].Count.ToString();
                    gameOver = false;
                }
            }
        }

        private void Game_MouseDown(object sender, MouseEventArgs e)
        {
            if (!gameOver)
            {
                b = (Button)sender;

                if (MouseButtons == MouseButtons.Left && b.Text != "F")
                {
                    LeftClick(b);
                }

                if (MouseButtons == MouseButtons.Right)
                {
                    if (b.Text != "F" && b.BackColor != BackColor)
                    {
                        b.ForeColor = Color.Red;
                        flags++;
                        b.Text = "F";
                    }

                    else if (b.BackColor != BackColor)
                    {
                        b.ForeColor = b.BackColor;
                        flags--;
                        b.Text = land[p.X, p.Y].Count.ToString();
                    }
                    LblMineCounter.Text = $"mines flagged:\n{flags} / {mines}";
                }
            }
        }

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

        void First(Button b)
        {
            Point p = (Point)b.Tag;
            int ned, sid, placed = 0;
            
            while (placed < mines)
            {
                ned = rand.Next(h);
                sid = rand.Next(w);
                if (!land[sid, ned].IsMine && (p.Y != ned || p.X != sid))
                {
                    land[sid, ned].IsMine = true;
                    land[sid, ned].Btn.Text = "X";
                    placed++;
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
                        try
                        {
                            if (land[x + i % 3 - 1, y + i / 3 - 1].IsMine && i != 4) land[x, y].Count++;
                        }
                        catch (IndexOutOfRangeException) { }
                    }
                    land[x, y].Btn.Text = land[x, y].Count.ToString();
                    if (x != p.X || y != p.Y) land[x, y].Btn.ForeColor = Color.White;
                    else b.ForeColor = Revealed(land[p.X, p.Y].Count);
                }
            }
            first = false;
        }

        class Land
        {
            public Button Btn { get; set; }
            public int Count { get; set; }
            public bool IsFlagged { get; set; }
            public bool IsMine { get; set; }
        }

        void SmartClick(int x, int y, int flagged, int unsweeped)
        {

        }

        void LeftClick( Button b)
        {
            Point p = (Point)b.Tag;

            if (first && !replay)
                First(b);

            if (land[p.X, p.Y].IsMine)
            {
                b.BackColor = Color.Red;
                gameOver = true;
            }

            if (!gameOver)
            {
                Sweep(p.X, p.Y);
            }

            else
            {
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        if (land[x, y].IsMine && land[x, y].Btn.Text != "F")
                        {
                            land[x, y].Btn.ForeColor = Color.Black;
                            land[x, y].Btn.Text = "X";
                        }
                        else if (!land[x, y].IsMine && land[x, y].Btn.Text == "F")
                        {
                            land[x, y].Btn.ForeColor = Color.Blue;
                            land[x, y].Btn.Text = ">";
                        }
                    }
                }
                LblGameStatus.Visible = true;
                LblGameStatus.Text = "Game Over";
                GameStatus.BackColor = Color.Red;
                GameStatus.Image = Properties.Resources.lost;
            }

            //check for win
            int revealed = 0;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    if (land[x, y].Btn.BackColor == BackColor)
                        revealed++;

            if (revealed == w * h - mines && !land[p.X, p.Y].IsMine)
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
                LblGameStatus.Text = "You Win!";
                GameStatus.BackColor = Color.Green;
                GameStatus.Image = Properties.Resources.win;
                gameOver = true;
            }
        }

        //Recursivly sweeps all adjacent land.
        void Sweep(int x, int y)
        {
            if (land[x, y].Btn.Text == "F" ||
                land[x, y].Btn.BackColor == BackColor) { return; }

            if (land[x, y].Btn.BackColor == Color.White)
            {
                land[x, y].Btn.BackColor = BackColor;
                land[x, y].Btn.ForeColor = Revealed(int.Parse(land[x, y].Btn.Text));

                if (land[x, y].Btn.Text == "0")
                    for (int i = 0; i < 9; i++)
                        if (i != 4)
                            try
                            {
                                Sweep(x + i % 3 - 1, y + i / 3 - 1);
                            }
                            catch (IndexOutOfRangeException) { }
            }
            return;
        }

        Color Revealed(int Component)
        {
            int c = Component;
            Color[] palette =
            {
                BackColor,
                Color.LightBlue,
                Color.Green,
                Color.Red,
                Color.Purple,
            };
            for (int g = 0; g < 4; g++)
                if (c == g) return palette[g];
            return Color.Purple;
        }
    }
}