using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MineSweeper
{
	public partial class Game : Form
	{
		struct Node
		{
			public Button land { get; set; }
			public int gCost { get; set; }
			public int hCost { get; set; }
			public Button parent { get; set; }
		}

		Random rand = new Random();
		int w = 13, h = 5, s = 34;
		int wMult = 3;
		Button b;
		readonly Color hidden = Color.LightGray;
		readonly Color open = Color.GreenYellow;
		readonly Color closed = Color.Red;

		int gCost = 0;
		Point end;
		Node[,] nodes;
		List<Point> path;

		//Initilize
		public Game()
		{
			InitializeComponent();
			path = new List<Point>();
			end = new Point(w - 1, h - 1);
			nodes = new Node[w, h];

			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					nodes[x, y].land = new Button
					{
						Anchor = AnchorStyles.Top | AnchorStyles.Left,
						Font = new Font("Microsoft sans serif", 12, FontStyle.Regular),
						BackColor = hidden,
						Location = new Point(x * (s * wMult), s * y),
						Size = new Size(s * wMult, s),
						Tag = new Point(x, y),
					};
					nodes[x, y].gCost = 0;
					nodes[x, y].hCost = (int)(Math.Round(Math.Sqrt(Math.Pow(x - end.X, 2) + Math.Pow(y - end.Y, 2)), 2) * 10);
					Controls.Add(nodes[x, y].land);
                    nodes[x, y].land.MouseDown += Land_MouseDown;
				}
			}
			nodes[0, 0].land.BackColor = open;
			//autoDo();
			Game_SizeChanged(new object(), new EventArgs());
		}

		private void autoDo()
        {
			int opened = 0;
			Node best = new Node();
            foreach (Node n in nodes)
            {
				if (n.land.BackColor == open)
					opened++;
					if (n.gCost + n.hCost < best.gCost + best.hCost || best.gCost == 0)
						best = n;
            }
			LeftClick(best.land);
			if ((Point)best.land.Tag != end)
					autoDo();
        }

		private void Land_MouseDown(object sender, MouseEventArgs e)
		{
			b = (Button)sender;

			if (MouseButtons == MouseButtons.Left)
			{
				if (b.BackColor == open)
				{
					b.BackColor = closed;
					LeftClick(b);
				}
			}

			if (MouseButtons == MouseButtons.Right)
			{
				if (b.BackColor == hidden)
				{
					b.BackColor = closed;
				}
			}
		}

		private void Game_SizeChanged(object sender, EventArgs e)
		{
			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					nodes[x, y].land.Location = new Point(x * s * wMult, s * y);
				}
			}
		}

		void LeftClick(Button b)
		{
			Point p = (Point)b.Tag;

			for (int i = 0; i < 9; i++)
			{
				Point n = new Point(p.X - 1 + i % 3, p.Y - 1 + i / 3);
				if (i == 4 || n.X >= 0 && n.X < w && n.Y >= 0 && n.Y < h)
					if (nodes[n.X, n.Y].land.BackColor == closed)
						continue;
					else
					{
						nodes[n.X, n.Y].land.BackColor = open;
						int step = (p.X - n.X + (p.Y - n.Y)) % 2 == 0 ? 14 : 10;
						if (nodes[p.X, p.Y].gCost + step < nodes[n.X, n.Y].gCost || nodes[n.X, n.Y].gCost == 0)
						{
							nodes[n.X, n.Y].gCost = nodes[p.X, p.Y].gCost + step;
							nodes[n.X, n.Y].parent = b;
						}

						//prints the gCost
						nodes[n.X, n.Y].land.Text = $"{nodes[n.X, n.Y].gCost + nodes[n.X, n.Y].hCost}";
					}
			}
			if (p == end)
			{
				Node Current = nodes[p.X, p.Y];
				
				while (!(Current.parent is null))
				{
					path.Add((Point)Current.land.Tag);
					Current.land.BackColor = Color.Yellow;
					Point pp = (Point)Current.parent.Tag;
					Current = nodes[pp.X, pp.Y];
				}
				path.Add((Point)Current.land.Tag);
				Current.land.BackColor = Color.Yellow;

				string makeStr = "";
                foreach (Point str in path)
                {
					makeStr += $"({str.X}, {str.Y}),";
				}
				MessageBox.Show(makeStr);
			}

		}
		void computegCost(Point n, Button parent, bool diag)
        {
			int step = diag ? 14 : 10;
			int g = nodes[n.X, n.Y].gCost;
			
			if (g > gCost + step)
            {
				nodes[n.X, n.Y].gCost = gCost + step;
				nodes[n.X, n.Y].parent = parent;
				gCost += step;
            }
			//Text = $"{parent.Tag}, {gCost}";
		}
	}
}