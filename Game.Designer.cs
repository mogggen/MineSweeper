namespace MineSweeper
{
    partial class Game
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.GameStatus = new System.Windows.Forms.PictureBox();
            this.NudBombCounter = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Help = new System.Windows.Forms.Button();
            this.btnReplay = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.GameStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudBombCounter)).BeginInit();
            this.SuspendLayout();
            // 
            // GameStatus
            // 
            this.GameStatus.BackColor = System.Drawing.Color.LightGray;
            this.GameStatus.Location = new System.Drawing.Point(214, 23);
            this.GameStatus.Name = "GameStatus";
            this.GameStatus.Size = new System.Drawing.Size(100, 84);
            this.GameStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.GameStatus.TabIndex = 0;
            this.GameStatus.TabStop = false;
            this.GameStatus.Click += new System.EventHandler(this.GameStatus_Click);
            // 
            // numericUpDown1
            // 
            this.NudBombCounter.Location = new System.Drawing.Point(170, 87);
            this.NudBombCounter.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.NudBombCounter.Name = "numericUpDown1";
            this.NudBombCounter.Size = new System.Drawing.Size(38, 20);
            this.NudBombCounter.TabIndex = 1;
            this.NudBombCounter.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(330, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(185, 58);
            this.label1.TabIndex = 2;
            this.label1.Text = "Mines flagged:\r\n0 / 16";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(197, 527);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 29);
            this.label2.TabIndex = 3;
            this.label2.Text = "You Win!";
            // 
            // Help
            // 
            this.Help.Location = new System.Drawing.Point(0, 0);
            this.Help.Name = "Help";
            this.Help.Size = new System.Drawing.Size(92, 48);
            this.Help.TabIndex = 4;
            this.Help.Text = "Help";
            this.Help.UseVisualStyleBackColor = true;
            this.Help.Click += new System.EventHandler(this.Help_Click);
            // 
            // btnReplay
            // 
            this.btnReplay.Location = new System.Drawing.Point(434, 532);
            this.btnReplay.Name = "btnReplay";
            this.btnReplay.Size = new System.Drawing.Size(115, 48);
            this.btnReplay.TabIndex = 5;
            this.btnReplay.Text = "Replay this puzzle";
            this.btnReplay.UseVisualStyleBackColor = true;
            this.btnReplay.Click += new System.EventHandler(this.BtnReplay_Click);
            // 
            // Game
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(561, 592);
            this.Controls.Add(this.btnReplay);
            this.Controls.Add(this.Help);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NudBombCounter);
            this.Controls.Add(this.GameStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Game";
            this.Text = "MineSweeper";
            ((System.ComponentModel.ISupportInitialize)(this.GameStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudBombCounter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox GameStatus;
        private System.Windows.Forms.NumericUpDown NudBombCounter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Help;
        private System.Windows.Forms.Button btnReplay;
    }
}

