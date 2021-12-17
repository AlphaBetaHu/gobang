
namespace Chess
{
    partial class FrmMain
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
            this.board = new System.Windows.Forms.PictureBox();
            this.init_btn = new System.Windows.Forms.Button();
            this.txt = new System.Windows.Forms.TextBox();
            this.debug = new System.Windows.Forms.PictureBox();
            this.back = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.board)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.debug)).BeginInit();
            this.SuspendLayout();
            // 
            // board
            // 
            this.board.Location = new System.Drawing.Point(30, 30);
            this.board.Name = "board";
            this.board.Size = new System.Drawing.Size(480, 480);
            this.board.TabIndex = 0;
            this.board.TabStop = false;
            this.board.Paint += new System.Windows.Forms.PaintEventHandler(this.BoardPaint);
            // 
            // init_btn
            // 
            this.init_btn.Location = new System.Drawing.Point(30, 536);
            this.init_btn.Name = "init_btn";
            this.init_btn.Size = new System.Drawing.Size(75, 23);
            this.init_btn.TabIndex = 1;
            this.init_btn.Text = "重开";
            this.init_btn.UseVisualStyleBackColor = true;
            this.init_btn.Click += new System.EventHandler(this.init_btn_Click);
            // 
            // txt
            // 
            this.txt.Location = new System.Drawing.Point(534, 30);
            this.txt.Multiline = true;
            this.txt.Name = "txt";
            this.txt.Size = new System.Drawing.Size(192, 480);
            this.txt.TabIndex = 2;
            // 
            // debug
            // 
            this.debug.Location = new System.Drawing.Point(762, 30);
            this.debug.Name = "debug";
            this.debug.Size = new System.Drawing.Size(500, 500);
            this.debug.TabIndex = 3;
            this.debug.TabStop = false;
            // 
            // back
            // 
            this.back.Location = new System.Drawing.Point(111, 536);
            this.back.Name = "back";
            this.back.Size = new System.Drawing.Size(75, 23);
            this.back.TabIndex = 4;
            this.back.Text = "悔一步";
            this.back.UseVisualStyleBackColor = true;
            this.back.Click += new System.EventHandler(this.button1_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 574);
            this.Controls.Add(this.board);
            this.Controls.Add(this.back);
            this.Controls.Add(this.debug);
            this.Controls.Add(this.txt);
            this.Controls.Add(this.init_btn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmMain";
            ((System.ComponentModel.ISupportInitialize)(this.board)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.debug)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox board;
        private System.Windows.Forms.Button init_btn;
        private System.Windows.Forms.TextBox txt;
        private System.Windows.Forms.PictureBox debug;
        private System.Windows.Forms.Button back;
    }
}