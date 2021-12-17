using Chess.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Chess
{
    public partial class FrmMain : Form
    {
        const int Cell = 30;
        const int Len = 15; // 棋盘大小

        AI3 Ai;// 多少行列

        int[,] ChessBoard = null;

        Dictionary<PointC, Button> Pieces = null;

        Stack<PointPath> Path { get; set; }

        Button MachinePieces = null;
        static Bitmap People = null;
        static Bitmap Victory = null;
        static Bitmap Machine = null;
        static Bitmap MachineCurr = null;
        

        static FrmMain()
        {
            Victory = new Bitmap(50, 50);
            Machine = new Bitmap(50, 50);
           
            People = new Bitmap(50, 50);

            MachineCurr = new Bitmap(50, 50);


            Graphics g = Graphics.FromImage(Machine);
            g.FillEllipse(new SolidBrush(Color.White), 0, 0, 50, 50);

            g = Graphics.FromImage(People);
            g.FillEllipse(new SolidBrush(Color.Black), 0, 0, 50, 50);

            g = Graphics.FromImage(Victory);
            g.FillEllipse(new SolidBrush(Color.Blue), 0, 0, 50, 50);

            g = Graphics.FromImage(MachineCurr);
            g.FillEllipse(new SolidBrush(Color.White), 0, 0, 50, 50);
            g.DrawRectangle(new Pen(Color.Red, 2), 0, 0, 48, 48);
            
            
        }

        public FrmMain()
        {
            InitializeComponent();
            InitializeChessBoard();
        }

        void InitializeChessBoard()
        {
            board.Controls.Clear();

            this.MachinePieces = null;
            this.ChessBoard = new int[Len, Len];
            this.Pieces = new Dictionary<PointC, Button>();
            this.Path = new Stack<PointPath>();
            this.Ai = new AI3(this.ChessBoard, this.Debug);
            this.Text = "难度6星";
            this.txt.Text = string.Empty;

            int width = Cell - 7;

            for (int i = 0; i < Len; i++)
            {
                for (int j = 0; j < Len; j++)
                {
                    PointC point = new PointC(j, i);

                    Button btn = new Button();
                    btn.Width = width;
                    btn.Height = width;
                    btn.Cursor = Cursors.Hand;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.FlatAppearance.MouseOverBackColor = Color.Transparent;//鼠标经过
                    btn.FlatAppearance.MouseDownBackColor = Color.Transparent;//鼠标按下
                    btn.BackColor = Color.Transparent;
                    btn.ForeColor = Color.Transparent;
                    btn.BackgroundImageLayout = ImageLayout.Stretch;
                    btn.Location = new Point(i * Cell + Cell - width / 2, j * Cell + Cell - width / 2);
                    btn.Click += Btn_Click;
                    btn.Tag = point;
                    //btn.Text = $"{i},{j}";
                    board.Controls.Add(btn);

                    this.Pieces.Add(point, btn);
                }
            }

            board.Width = board.Height = Cell * Len + Cell;

            //Go(new PointC(5, 5), 1);
            //Go(new PointC(5, 7), 1);
            //Go(new PointC(4, 6), 1);
            //Go(new PointC(6, 8), 1);
            //Go(new PointC(7, 7), 1);
            //Go(new PointC(5, 9), 1);
            //Go(new PointC(5, 8), 1);
            //Go(new PointC(5, 10), 1);
            //Go(new PointC(4, 7), 1);
            //Go(new PointC(4, 8), 1);
            //Go(new PointC(3, 6), 1);
            //Go(new PointC(4, 5), 1);
            //Go(new PointC(4, 9), 1);


            //Go(new PointC(1, 2), new PointC(5, 7));
            //Go(new PointC(1, 3), new PointC(5, 9));
            //Go(new PointC(1, 4), new PointC(5, 10));

        }

        private void Btn_Click(object sender, EventArgs e)
        {
            Button btn = (sender as Button);
            Go((PointC)btn.Tag);
        }

        /// <summary>
        /// 落子
        /// </summary>
        /// <param name="point">落子点</param>
        /// <param name="pOm">人 或 机  1人,2机</param>
        private void Go(PointC point)
        {
            this.ChessBoard[point.X, point.Y] = 1;
            DateTime current = DateTime.Now;
            TryResult res = this.Ai.Go();

            this.Text = $"电脑思考 {(DateTime.Now - current).TotalSeconds} 秒";

            Go(point, res.Point);

            if (res.VictoryFace == 1)
            {
                End(res.VictoryLine);

                txt.AppendText("你赢了~~~");
                MessageBox.Show("黑胜");
                return;
            }
            else if (res.VictoryFace == 2)
            {
                End(res.VictoryLine);

                txt.AppendText("你输了,继续加油?");
                MessageBox.Show("白胜");
                return;
            }
        }

        private void Go(PointC p, PointC m)
        {
            if (this.MachinePieces != null)
                this.MachinePieces.BackgroundImage = Machine;

            this.Path.Push(new PointPath(p, m));
            this.txt.Text = string.Empty;
            this.back.Enabled = true;

            if (p != null)
            {
                this.ChessBoard[p.X, p.Y] = 1;
                Button btnP = this.Pieces[p];
                btnP.BackgroundImage = People;
                btnP.Enabled = false;
            }

            if (m != null)
            {
                this.ChessBoard[m.X, m.Y] = 2;
                Button btnM = this.Pieces[m];
                btnM.BackgroundImage = MachineCurr;
                btnM.Enabled = false;
                this.MachinePieces = btnM;
            }


            foreach (var item in this.Path)
            {
                this.txt.Text = ($"人 {item?.P}\t 机 {item?.M}\r\n") + this.txt.Text;
            }
        }

        private void BoardPaint(object sender, PaintEventArgs e)
        {
            //int cell = Range / Len;
            int width = Cell * Len;
            //int f4 = (Range - cell * 2) / 4;


            Graphics g = e.Graphics;
            g.Clear(Color.FromArgb(200, 150, 45));
            Font f = new Font("宋体", 12);


            g.FillEllipse(Brushes.Black, (width - Cell) / 2 + Cell - 5, (width - Cell) / 2 + Cell - 5, 10, 10);

            g.FillEllipse(Brushes.Black, Cell * 4 - 5, Cell * 4 - 5, 10, 10);
            g.FillEllipse(Brushes.Black, Cell * 12 - 5, Cell * 4 - 5, 10, 10);
            g.FillEllipse(Brushes.Black, Cell * 12 - 5, Cell * 12 - 5, 10, 10);
            g.FillEllipse(Brushes.Black, Cell * 4 - 5, Cell * 12 - 5, 10, 10);

            //g.FillEllipse(Brushes.Black, f4 + cell, f4 * 3 + cell, 10, 10);

            g.DrawLine(new Pen(Brushes.Black, 3), Cell, Cell, width, Cell);
            g.DrawLine(new Pen(Brushes.Black, 3), Cell, Cell, Cell, width);

            g.DrawLine(new Pen(Brushes.Black, 3), width, Cell, width, width);
            g.DrawLine(new Pen(Brushes.Black, 3), Cell, width, width, width);

            for (int i = 1; i <= Len; i++)
            {
                g.DrawLine(new Pen(Brushes.Black, 1), Cell, i * Cell, width, i * Cell);
                g.DrawLine(new Pen(Brushes.Black, 1), i * Cell, Cell, i * Cell, width);

                string s = (i - 1).ToString();
                int cell = i - 1 < 10 ? 8 : 13;

                g.DrawString(s, f, Brushes.Black, 5, i * Cell - cell);
                g.DrawString(s, f, Brushes.Black, i * Cell - cell, 5);
            }
        }

        private void init_btn_Click(object sender, EventArgs e)
        {
            board.Controls.Clear();
            InitializeChessBoard();

            txt.Text = string.Empty;
        }

        void End(PointC[] victory)
        {
            back.Enabled = false;
            foreach (var item in this.Pieces)
            {
                item.Value.Enabled = false;
            }

            //foreach (var item in victory)
            //{
            //    if (this.ChessBoard[item.X, item.Y] > 0)
            //        this.Pieces[item].BackgroundImage = Victory;
            //}
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

        }

        void Debug()
        {
            //Graphics g = debug.CreateGraphics();
            //g.Clear(Color.Yellow);

            //int cell = Range / Len;

            //Font f = new Font("宋体", 20);

            //for (int i = 1; i < Len; i++)
            //{
            //    g.DrawLine(new Pen(Brushes.Blue, 1), 0, i * cell, Range, i * cell);
            //    g.DrawLine(new Pen(Brushes.Blue, 1), i * cell, 0, i * cell, Range);

            //    string s = (i - 1).ToString();

            //    g.DrawString(s, f, Brushes.Red, 0, i * cell - cell / 2);
            //    g.DrawString(s, f, Brushes.Red, i * cell - cell / 3, 0);
            //}

            //for (int i = 1; i < Len; i++)
            //{
            //    for (int j = 1; j < Len; j++)
            //    {
            //        int pom = this.ChessBoard[j - 1, i - 1];
            //        if (pom == 0) continue;

            //        Color color = pom == 1 ? Color.Black : Color.Red;

            //        Point p = new Point(i * cell - (cell / 2), j * cell - (cell / 2));

            //        g.FillEllipse(new SolidBrush(color), p.X, p.Y, cell, cell);
            //    }
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.Path.Count > 0)
            {
                PointPath path = this.Path.Pop();

                Button btnP = this.Pieces[path.P];
                btnP.BackgroundImage = null;
                btnP.Enabled = true;

                Button btnM = this.Pieces[path.M];
                btnM.BackgroundImage = null;
                btnM.Enabled = true;

                this.ChessBoard[path.P.X, path.P.Y] = 0;
                this.ChessBoard[path.M.X, path.M.Y] = 0;
                this.MachinePieces = null;
            }
        }
    }
}