using System;
using System.Drawing;
using System.Windows.Forms;

namespace MineSweeper
{
    public partial class Game : Form
    {
        Random rand = new Random(); // used to distribute mines.
        int w = 10, h = 10, s = 35, m = 0; // Dimensions of button: w=width, h=height, s=size, m=margin
        Button b; // The properites of the box that was pressed.
        Point p; // The coordinate of the box that was pressed.
        Button[,] box; // The boxes

        bool[,] isMine;
        bool gameOver = false;
        int revealed = 0;
        int mines = 16; // Number of mines to flag.
        bool first = true, replay = false; // 
        int flags = 0; // Amount of flags.
        int[,] count; // The Number of adjacent mines for each box.

        private void GameStatus_Click(object sender, EventArgs e)
        {
            replay = false;
            first = true;
            flags = 0;
            revealed = 0;
            mines = (int)numericUpDown1.Value;
            GameStatus.BackColor = Color.LightGray;
            label1.Text = $"mines flagged:\n{flags} / {mines}";
            label2.Visible = false;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    box[x, y].BackColor = Color.White;
                    box[x, y].ForeColor = Color.White;
                    //replay puzzle
                    if (!replay)
                        isMine[x, y] = false;
                    
                }
            }
            gameOver = false;
        }

        private void Help_Click(object sender, EventArgs e)
        {
            Help help = new Help();
            help.Show();
        }

        public Game()
        {
            InitializeComponent();
            mines = (int)numericUpDown1.Value;
            box = new Button[w, h];
            count = new int[w, h];
            isMine = new bool[w, h];
            numericUpDown1.Value = mines;
            label1.Text = $"mines flagged:\n{flags} / {mines}";
            label2.Visible = false;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    box[x, y] = new Button()
                    {
                        //FlatStyle = FlatStyle.Flat,
                        Anchor = AnchorStyles.Top | AnchorStyles.Left,
                        Font = new Font("Microsoft sans serif", 16, FontStyle.Regular),
                        BackColor = Color.White,
                        Location = new Point(Width / 2 - (w / 2 * s + (w / 2 - 1)) + x * (s + m), Height / 2 - (h / 2 * s + (h / 2 - 1)) + y * (s + m)),
                        Size = new Size(s, s),
                        Tag = new Point(x, y),
                    };

                    Controls.Add(box[x, y]);
                    box[x, y].MouseDown += Game_MouseDown;
                }
            }
        }

        private void BtnReplay_Click(object sender, EventArgs e)
        {
            replay = true;
            first = true;
            flags = 0;
            revealed = 0;
            GameStatus.BackColor = Color.LightGray;
            label1.Text = $"mines flagged:\n{flags} / {mines}";
            label2.Visible = false;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    box[x, y].BackColor = Color.White;
                    box[x, y].ForeColor = Color.White;
                    if (box[x, y].Text == "X")
                    {
                        box[x, y].Text = "";
                    }
                    else
                        box[x, y].Text = "" + count[x, y];
                    gameOver = false;
                }
            }
        }

        private void Game_MouseDown(object sender, MouseEventArgs e)
        {
            b = (Button)sender;
            p = (Point)b.Tag;
            int ned = 0;
            int sid = 0;
            int i = 0;

            if (MouseButtons == MouseButtons.Right && !gameOver)
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
                    b.Text = "" + count[p.X, p.Y];
                }
                label1.Text = $"mines flagged:\n{flags} / {mines}";
            }

            if (MouseButtons == MouseButtons.Left && b.Text != "F" && !gameOver)
            {
                if (first)
                {
                    if (replay) i = mines;

                    while (i < mines)
                    {
                        ned = rand.Next(h);
                        sid = rand.Next(w);
                        if (!isMine[sid, ned] && (p.Y != ned || p.X != sid))
                        {
                            isMine[sid, ned] = true;
                            box[sid, ned].Text = "X";
                            i++;
                        }
                    }

                    if (b.BackColor != BackColor)
                    {
                        b.BackColor = BackColor;

                        for (int y = 0; y < h; y++)
                        {
                            for (int x = 0; x < w; x++)
                            {
                                count[x, y] = 0;

                                for (int xx = -1; xx < 2; xx++)
                                {
                                    for (int yy = -1; yy < 2; yy++)
                                    {
                                        try
                                        {
                                            if (isMine[x + xx, y + yy] && (xx != 0 || yy != 0)) count[x, y]++;
                                        }
                                        catch { }
                                    }
                                }

                                first = false;
                                box[x, y].ForeColor = box[x, y].BackColor;
                                if (count[p.X, p.Y] == 0)
                                {
                                    box[p.X, p.Y].ForeColor = BackColor;

                                }
                                else if (count[p.X, p.Y] == 1) box[p.X, p.Y].ForeColor = Color.LightBlue;
                                else if (count[p.X, p.Y] == 2) box[p.X, p.Y].ForeColor = Color.Green;
                                else if (count[p.X, p.Y] == 3) box[p.X, p.Y].ForeColor = Color.Red;
                                else if (count[p.X, p.Y] > 3) box[p.X, p.Y].ForeColor = Color.Purple;
                                box[x, y].Text = count[x, y].ToString();
                            }
                        }

                        Sweep(p.X, p.Y);

                        revealed = 0;
                        for (int y = 0; y < h; y++)
                            for (int x = 0; x < w; x++)
                                if (box[x, y].BackColor == BackColor)
                                    revealed++;

                        if (revealed == w * h - mines)
                        {
                            for (int y = 0; y < h; y++)
                            {
                                for (int x = 0; x < w; x++)
                                {
                                    if (isMine[x, y] && box[x, y].Text != "F")
                                    {
                                        box[x, y].ForeColor = Color.Red;
                                        flags++;
                                        box[x, y].Text = "F";

                                    }
                                }
                            }
                            label1.Text = $"mines flagged:\n{flags} / {mines}";
                            label2.Visible = true;
                            label2.Text = "You Win!";
                            GameStatus.BackColor = Color.Green;
                            gameOver = true;
                        }
                    }
                }

                else
                {
                    if (isMine[p.X, p.Y])
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
                                if (isMine[x, y] && box[x, y].Text != "F")
                                {
                                    box[x, y].ForeColor = Color.Black;
                                    box[x, y].Text = "X";
                                }
                                else if (!isMine[x, y] && box[x, y].Text == "F")
                                {
                                    box[x, y].ForeColor = Color.Blue;
                                    box[x, y].Text = ">";
                                }
                            }
                        }
                        label2.Visible = true;
                        label2.Text = "Game Over";
                        GameStatus.BackColor = Color.Red;
                    }

                    //check for win
                    if (!first)
                    {
                        int revealed = 0;
                        for (int y = 0; y < h; y++)
                            for (int x = 0; x < w; x++)
                                if (box[x, y].BackColor == BackColor)
                                    revealed++;

                        if (revealed == w * h - mines && !isMine[p.X, p.Y])
                        {
                            for (int y = 0; y < h; y++)
                            {
                                for (int x = 0; x < w; x++)
                                {
                                    if (isMine[x, y] && box[x, y].Text != "F")
                                    {
                                        box[x, y].ForeColor = Color.Red;
                                        flags++;
                                        box[x, y].Text = "F";
                                        label1.Text = $"mines flagged:\n{flags} / {mines}";
                                    }
                                }
                            }
                            label2.Visible = true;
                            label2.Text = "You Win!";
                            GameStatus.BackColor = Color.Green;
                            gameOver = true;
                        }
                    }
                }
            }
        }

        void Sweep(int x, int y)
        {
            if (box[x, y].Text == "F" ||
                box[x, y].BackColor == BackColor) { return; }

            if (box[x, y].BackColor == Color.White)
            {
                box[x, y].BackColor = BackColor;
                box[x, y].ForeColor = Revealed(int.Parse(box[x, y].Text));

                if (box[x, y].Text == "0")
                    for (int i = 0; i < 9; i++)
                        if (i != 4)
                            try
                            {
                                Sweep(x + i % 3 - 1, y + i / 3 - 1);
                            }
                            catch { }
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