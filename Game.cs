using System;
using System.Drawing;
using System.Windows.Forms;

namespace MineSweeper
{
    public partial class Game : Form
    {
        Random rand = new Random();
        int w = 10, h = 10, s = 35, m = 1;
        Button b;
        Point p;
        Point[] d = { new Point(10, 10), new Point(15, 30) }; //difficulty
        Button[,] box;
        bool[,] isMine;
        bool gameOver = false;
        int revealed = 0;
        int mines = 16;
        bool first = true, replay = false;
        int flags = 0;
        int[,] count;
        int smartClick;

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            replay = false;
            first = true;
            flags = 0;
            revealed = 0;
            mines = (int)numericUpDown1.Value;
            pictureBox1.BackColor = Color.LightGray;
            label1.Text = $"mines flagged:\n{flags} / {mines}";
            label2.Visible = false;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    box[x, y].BackColor = Color.White;
                    box[x, y].ForeColor = Color.White;
                    if (!replay)
                        isMine[x, y] = false;
                    gameOver = false;
                }
            }
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
                    box[x, y].MouseDown += Form1_MouseDown;
                }
            }
        }

        private void BtnReplay_Click(object sender, EventArgs e)
        {
            replay = true;
            first = true;
            flags = 0;
            revealed = 0;
            pictureBox1.BackColor = Color.LightGray;
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

        private void Form1_MouseDown(object sender, MouseEventArgs e)
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

            if (MouseButtons == MouseButtons.Left && first && b.Text != "F")
            {
                if (replay) i = mines;

                while (i < mines)
                {
                    ned = rand.Next(h);
                    sid = rand.Next(w);
                    if (isMine[sid, ned] == false && (p.Y != ned || p.X != sid))
                    {
                        isMine[sid, ned] = true;
                        box[sid, ned].Text = "X";
                        i++;
                    }
                }

                if (b.BackColor != BackColor)
                {
                    b.BackColor = BackColor;

                    for (int j = 0; j < 2; j++)
                    {
                        if (j == 0)
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
                                    box[x, y].Text = "" + count[x, y];
                                }
                            }
                        else
                        {
                            Sweeped(p.X, p.Y);

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
                                pictureBox1.BackColor = Color.Green;
                                gameOver = true;
                            }
                        }
                    }
                }
            }

            else if (MouseButtons == MouseButtons.Left && b.Text != "F" && !gameOver)
            {
                if (b.BackColor == BackColor && int.Parse(b.Text) > 0)
                for (int j = 0; j < 2; j++)
                    for (int y = p.Y - 1; y < p.Y + 2; y++)
                        for (int x = p.X - 1; x < p.X + 2 && !gameOver; x++)
                            try
                            {
                                smartClick = 0;
                                if (j == 0 && box[x, y].Text == "F")
                                {
                                    smartClick++;
                                }
                                else if (j == 1 && smartClick == count[p.X, p.Y])
                                {
                                    if (box[x, y].BackColor != BackColor)
                                    {
                                        box[x, y].BackColor = BackColor;
                                        box[x, y].ForeColor = Revealed(box[x, y].Text);
                                    }
                                    if (isMine[x, y])
                                    {
                                        box[x, y].BackColor = Color.Red;
                                        gameOver = true;
                                    }

                                }
                            }
                            catch
                            {

                            }

                if (b.BackColor != BackColor)
                b.BackColor = BackColor;
            

                if (isMine[p.X, p.Y])
                {
                    b.BackColor = Color.Red;
                    gameOver = true;
                }

                if (gameOver)
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
                    pictureBox1.BackColor = Color.Red;
                }

                else
                {
                    if (b.Text == "0")
                    {
                        
                        Sweeped(p.X, p.Y);
                    }
                    else if (count[p.X, p.Y] == 1) box[p.X, p.Y].ForeColor = Color.LightBlue;
                    else if (count[p.X, p.Y] == 2) box[p.X, p.Y].ForeColor = Color.Green;
                    else if (count[p.X, p.Y] == 3) box[p.X, p.Y].ForeColor = Color.Red;
                    else if (count[p.X, p.Y] >= 4) box[p.X, p.Y].ForeColor = Color.Purple;
                    b.ForeColor = BackColor;
                }

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
                    pictureBox1.BackColor = Color.Green;
                    gameOver = true;
                }
            }
        }

        void Sweeped()
        {
            uint counter = int.MaxValue;
            int stuff = 0;

            if (!first)
                while (counter > 0)
                {
                    counter = 0;
                    for (int y = 0; y < h; y++)
                        for (int x = 0; x < w; x++)
                            if (box[x, y].Text != "F")
                                for (int a = -1; a < 2; a++)
                                    for (int o = -1; o < 2; o++)
                                        try
                                        {
                                            if (
                                                box[x + o, y + a].BackColor == BackColor &&
                                                box[x + o, y + a].Text == "0"
                                                )
                                            {
                                                box[x, y].BackColor = BackColor;
                                                box[x, y].ForeColor = Revealed(box[x, y].Text);
                                                a = 2;
                                                o = 2;
                                                counter++;
                                                stuff++;
                                            }
                                        }
                                        catch { }
                }
            if (true) { }
        }

        void Sweeped(int x, int y)
        {
            if (box[x, y].Text == "F") { return; }

            if (box[x, y].BackColor == Color.White &&
                box[x, y].Text == "0")
            {
                box[x, y].BackColor = BackColor;
                box[x, y].ForeColor = Revealed(box[x, y].Text);
            }
            else return;

            for (int i = 0; i < 10; i++)
            {
                if (i != 5)
                {
                    try
                    {
                        Sweeped(x + i % 3 - 1, y + i / 3 - 1);
                    }
                    catch { }
                }
            }
        }

        Color Revealed(string Component)
        {
            int c;
            Color[] palette =
            {
                BackColor,
                Color.LightBlue,
                Color.Green,
                Color.Red,
                Color.Purple,
            };

            try
            {
                c = int.Parse(Component);
            }
            catch
            {
                return new Color();
            }
            for (int g = 0; g < 4; g++)
                if (c == g) return palette[g];
            return Color.Purple;
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