using System.Collections.Generic;
using System.Linq;

namespace Chess.Core
{
    public class AI
    {
        static readonly PointC[] Position = new PointC[]
        {
                new PointC(0,1),    //  →       ↔
                new PointC(1,1),    //  ↘
                new PointC(1,0),    //  ↓
                new PointC(1,-1),   //  ↙

                //new PointC(0,-1),   //  ←
                //new PointC(-1,-1),  //  ↖
                //new PointC(-1,0),   //  ↑
                //new PointC(-1,1),   //  ↗
        };

        public PointC[] Victory1 { get; private set; }
        public PointC[] Victory2 { get; private set; }

        public PointC[] Optimum1 { get; private set; }
        public PointC[] Optimum2 { get; private set; }

        public List<PointC[]> Lines { get; private set; }

        static readonly Dictionary<string, int> Standard = new Dictionary<string, int>();

        public int Len { get; private set; }

        static AI()
        {
            Standard["00000"] = 50000;
            Standard["00000+"] = 50000;
            Standard["+00000"] = 50000;
            Standard["+0000+"] = 4320;
            Standard["+000++"] = 720;
            Standard["++000+"] = 720;
            Standard["+00+0+"] = 720;
            Standard["+0+00+"] = 720;
            Standard["0000+"] = 720;
            Standard["+0000"] = 720;
            Standard["00+00"] = 720;
            Standard["0+000"] = 720;
            Standard["000+0"] = 720;



            Standard["++00++"] = 120;
            Standard["++0+0+"] = 120;
            Standard["+0+0++"] = 120;

            Standard["+++0++"] = 20;
            Standard["++0+++"] = 20;


            /*************************/

            //Standard["00000"] = 50000;

            //Standard["+0000"] = 4321;
            //Standard["0000+"] = 4321;
            //Standard["00+00"] = 4320;
            //Standard["000+0"] = 4320;
            //Standard["0+000"] = 4320;

            //Standard["000++"] = 724;
            //Standard["+000+"] = 725;
            //Standard["++000"] = 725;
            //Standard["0+00+"] = 723;
            //Standard["+0+00"] = 723;
            //Standard["00+0+"] = 723;
            //Standard["+00+0"] = 723;
            //Standard["0+0+0"] = 720;

            //Standard["0+0++"] = 120;
            //Standard["+0+0+"] = 120;
            //Standard["++0+0"] = 120;
            //Standard["0++0+"] = 120;
            //Standard["+0++0"] = 120;
            //Standard["0++0+"] = 120;
            //Standard["+0++0"] = 120;
            //Standard["0+++0"] = 120;
            //Standard["00+++"] = 125;
            //Standard["+00++"] = 125;
            //Standard["++00+"] = 125;
            //Standard["+++00"] = 125;

            //Standard["0++++"] = 20;
            //Standard["+0+++"] = 20;
            //Standard["++0++"] = 20;
            //Standard["+++0+"] = 20;
            //Standard["++++0"] = 20;
            //Standard["+++++"] = 2;
        }

        /// <summary>
        /// 棋盘大小
        /// </summary>
        /// <param name="len"></param>
        public AI(int len)
        {
            this.Len = len;
            this.Lines = new List<PointC[]>();

            for (int x = 0; x < len; x++)
            {
                for (int y = 0; y < len; y++)
                {
                    foreach (var position in Position)
                    {
                        PointC[] line5 = new PointC[5];
                        PointC[] line6 = new PointC[6];
                        for (int i = 0; i < 6; i++)
                        {
                            int xi = x + i * position.X;
                            int yi = y + i * position.Y;

                            if (xi >= 0 && xi < len && yi >= 0 && yi < len)
                            {
                                if (i < 5)
                                    line5[i] = line6[i] = new PointC(xi, yi);
                                line6[i] = new PointC(xi, yi);
                            }
                            else
                                break;

                            if (i == 4) Lines.Add(line5);
                            if (i == 5) Lines.Add(line6);
                        }
                    }
                }
            }
        }

        public PointC Go(int[,] chess)
        {
            this.Victory1 = null;
            this.Victory2 = null;
            this.Optimum1 = null;
            this.Optimum2 = null;

            int s1 = 0;
            int s2 = 0;

            // 找出最优的棋型
            foreach (PointC[] points in Lines)
            {
                string key1 = string.Join(string.Empty, points.Select(e => chess[e.X, e.Y] == 0 ? "+" : chess[e.X, e.Y] == 1 ? "0" : "#"));
                string key2 = string.Join(string.Empty, points.Select(e => chess[e.X, e.Y] == 0 ? "+" : chess[e.X, e.Y] == 2 ? "0" : "#"));

                if (Standard.ContainsKey(key1) && Standard[key1] > s1)
                {
                    this.Optimum1 = points;
                    s1 = Standard[key1];
                }

                if (Standard.ContainsKey(key2) && Standard[key2] > s2)
                {
                    this.Optimum2 = points;
                    s2 = Standard[key2];
                }
            }

            if (s1 == 50000)
            {
                this.Victory1 = this.Optimum1;
                return null;
            }

            TryResult tr2 = Try(chess, this.Optimum2, 2);

            if (tr2.Score == 50000)
            {
                this.Victory2 = tr2.VictoryLine;
                return tr2.Point;
            }

            TryResult tr1 = Try(chess, this.Optimum1, 1);

            return tr2.Score > tr1.Score ? tr2.Point : tr1.Point;

        }

        public TryResult Try(int[,] chess, PointC[] optimum, int pOm)
        {
            TryResult res = new TryResult();
            if (optimum == null)
                return res;

            foreach (PointC tryPot in optimum)
            {
                if (chess[tryPot.X, tryPot.Y] == 0)
                {
                    TryResult tmp = new TryResult();
                    tmp.Point = tryPot;

                    chess[tryPot.X, tryPot.Y] = pOm;

                    foreach (var position in Position)
                    {
                        int x = tryPot.X - position.X * 4;
                        int y = tryPot.Y - position.Y * 4;

                        for (int j = 0; j < 5; j++)
                        {
                            bool ok = true;

                            PointC[] line = new PointC[5];
                            for (int i = 0; i < 5; i++)
                            {

                                int xi = x + (i + j) * position.X;
                                int yi = y + (i + j) * position.Y;

                                if (xi >= 0 && xi < this.Len && yi >= 0 && yi < Len)
                                {
                                    int piece = chess[xi, yi];
                                    if (piece == 0 || piece == pOm)
                                    {
                                        line[i] = new PointC(xi, yi); //piece == 0 ? '+' : '0';
                                        continue;
                                    }
                                }
                                ok = false;
                                break;
                            }

                            if (ok)
                            {
                                string key = string.Join(string.Empty, line.Select(piece => chess[piece.X, piece.Y] == 0 ? '+' : '0'));

                                if (Standard.ContainsKey(key))
                                {
                                    tmp.Score += Standard[key];

                                    if (Standard[key] == 50000 && pOm == 2)
                                    {
                                        chess[tryPot.X, tryPot.Y] = 0;
                                        res.VictoryLine = line;
                                        return res;
                                    }
                                }
                            }
                        }
                    }

                    chess[tryPot.X, tryPot.Y] = 0;

                    if (tmp.Score > res.Score)
                        res = tmp;
                }
            }

            return res;
        }

    }
}
