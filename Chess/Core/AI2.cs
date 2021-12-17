using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Core
{
    public class AI2
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

        private List<PointC[]> Lines { get; set; }

        private int Len { get; set; }

        private static readonly Dictionary<string, int> Standard = new Dictionary<string, int>();

        private Action Callback = null;

        static AI2()
        {
            Standard["00000"] = 50000;

            Standard["+0000"] = 4321;
            Standard["0000+"] = 4321;
            Standard["00+00"] = 4320;
            Standard["000+0"] = 4320;
            Standard["0+000"] = 4320;

            Standard["000++"] = 724;
            Standard["+000+"] = 725;
            Standard["++000"] = 725;
            Standard["0+00+"] = 723;
            Standard["+0+00"] = 723;
            Standard["00+0+"] = 723;
            Standard["+00+0"] = 723;
            Standard["0+0+0"] = 720;

            Standard["0+0++"] = 120;
            Standard["+0+0+"] = 120;
            Standard["++0+0"] = 120;
            Standard["0++0+"] = 120;
            Standard["+0++0"] = 120;
            Standard["0++0+"] = 120;
            Standard["+0++0"] = 120;
            Standard["0+++0"] = 120;
            Standard["00+++"] = 125;
            Standard["+00++"] = 125;
            Standard["++00+"] = 125;
            Standard["+++00"] = 125;

            Standard["0++++"] = 20;
            Standard["+0+++"] = 20;
            Standard["++0++"] = 20;
            Standard["+++0+"] = 20;
            Standard["++++0"] = 20;
        }

        /// <summary>
        /// 棋盘大小
        /// </summary>
        /// <param name="len"></param>
        public AI2(int len, Action callback = null)
        {
            this.Len = len;
            this.Lines = new List<PointC[]>();
            this.Callback = callback;
            for (int x = 0; x < len; x++)
            {
                for (int y = 0; y < len; y++)
                {
                    foreach (var position in Position)
                    {
                        PointC[] line5 = new PointC[5];
                        for (int i = 0; i < 5; i++)
                        {
                            int xi = x + i * position.X;
                            int yi = y + i * position.Y;

                            if (xi >= 0 && xi < len && yi >= 0 && yi < len)
                            {
                                if (i < 5)
                                    line5[i] = new PointC(xi, yi);
                            }
                            else
                                break;

                            if (i == 4) Lines.Add(line5);
                        }
                    }
                }
            }
        }

        public TryResult Go(int[,] chess)
        {
            TryResult tryM = Go(chess, 2);
            TryResult tryP = Go(chess, 1);

            if (tryP.VictoryLine != null)
            {
                return tryP;
            }
            if (tryM.VictoryLine != null)
            {
                return tryM;
            }

            TryResult res = tryP.Score > tryM.Score ? tryP : tryM;

            for (int i = 0; i < 20; i++)
            {
                chess[res.Point.X, res.Point.Y] = 2;
                tryP = Go(chess, 1);
                chess[res.Point.X, res.Point.Y] = 0;
                res = tryP.Score > res.Score ? tryP : res;

                chess[res.Point.X, res.Point.Y] = 1;
                tryM = Go(chess, 2);
                chess[res.Point.X, res.Point.Y] = 0;
                res = tryM.Score > res.Score ? tryM : res;
            }
            res.VictoryFace = 0;
            return res;
        }

        private TryResult Go(int[,] chess, int pOm)
        {
            List<PointC[]> optimums = new List<PointC[]>();
            int[,] score = new int[Len, Len];
            TryResult res = new TryResult();
            res.VictoryFace = pOm;

            // 找出有效的落子棋型
            foreach (PointC[] optimum in this.Lines)
            {
                if (optimum.All(e => chess[e.X, e.Y] == 0))
                    continue;

                if (optimum.All(e => chess[e.X, e.Y] == pOm))
                {
                    return new TryResult()
                    {
                        VictoryFace = pOm,
                        VictoryLine = optimum
                    };
                }
                else if (optimum.All(e => chess[e.X, e.Y] == 0 || chess[e.X, e.Y] == pOm))
                    optimums.Add(optimum);
            }

            foreach (var optimum in optimums)
            {
                foreach (PointC tryPot in optimum)
                {
                    if (chess[tryPot.X, tryPot.Y] == 0)
                    {
                        chess[tryPot.X, tryPot.Y] = pOm; // 尝试

                        string key = string.Join(string.Empty, optimum.Select(piece => chess[piece.X, piece.Y] == 0 ? '+' : '0'));

                        if (Standard.ContainsKey(key))
                        {
                            score[tryPot.X, tryPot.Y] += Standard[key];

                            if (Standard[key] == 50000 && pOm == 2)
                            {
                                chess[tryPot.X, tryPot.Y] = 2;
                                res.Score = Standard[key];
                                res.VictoryLine = optimum;
                                res.VictoryFace = 2;
                                res.Point = tryPot;
                                return res;
                            }
                        }

                        chess[tryPot.X, tryPot.Y] = 0;// 还原

                        //Callback?.Invoke();
                    }
                }
            }

            for (int i = 0; i < Len; i++)
            {
                for (int j = 0; j < Len; j++)
                {
                    if (score[i, j] >= res.Score)
                    {
                        res.Score = score[i, j];
                        res.Point = new PointC(i, j);
                    }
                }
            }

            return res;
        }
    }
}
