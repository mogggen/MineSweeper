using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MineSweeper
{
	struct RuleSet
    {
		List<string> actions;
		List<string> counters;
		RuleSet(int len = 5)
        {
			actions = new List<string>();
			counters = new List<string>();
        }
    };
	struct Ruler
    {
		int position, target, durability;
		string action;
    };

	//have one ruleSet per game!
    enum RuleSet1
    {
		dead = 0,
		MoveSE = 1,
		MoveNE = 2,
		goInvsiable = 3,
		assassinate = 4,
    }

	enum RuleSet2
    {
		dead = 0,
		flying = 1,
		swiming = 2,
		revive = 3,
		fullDurability = 4,
    }

	public partial class Game : Form
	{
		RuleSet ruleSet1;
		RuleSet ruleSet2;

		struct Node
		{
			public Button land { get; set; }
			
		}

		Random rand = new Random();
		int w = 16, h = 16, s = 34, m = 0;
		Button b;
		Color hidden = Color.LightGray;
		Color open = Color.GreenYellow;
		Color closed = Color.Red;

		Node[,] nodes;

		//Initilize
		public Game()
		{
			InitializeComponent();
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
						Location = new Point(x * s, y * s),
						Size = new Size(s, s),
						Tag = new Point(x, y),
					};
					Controls.Add(nodes[x, y].land);
                    nodes[x, y].land.MouseDown += Land_MouseDown;
				}
			}
			nodes[0, 0].land.BackColor = open;
			Game_SizeChanged(new object(), new EventArgs());
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
					nodes[x, y].land.Location = new Point(x * s, s * y);
				}
			}
		}

		void LeftClick(Button b)
		{
			Point p = (Point)b.Tag;
            Console.WriteLine(p);
		}
	}
}