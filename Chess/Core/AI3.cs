using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Core
{
    public class AI3
    {
        static readonly PointC[] P4 = new PointC[]
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

        //static readonly PointC[] P8 = new PointC[]
        //{
        //    new PointC(0,1),    //  → 
        //    new PointC(1,1),    //  ↘
        //    new PointC(1,0),    //  ↓
        //    new PointC(1,-1),   //  ↙

        //    new PointC(0,-1),   //  ←
        //    new PointC(-1,-1),  //  ↖
        //    new PointC(-1,0),   //  ↑
        //    new PointC(-1,1),   //  ↗
        //};

        private List<PointC[]> PieceLines { get; set; }
        private List<PointC[]> TypedLines { get; set; }

        private int Len { get; set; }

        private Action Debug = null;

        const int Level = 6;

        private int[,] Chess { get; set; }
        private PointC Result { get; set; }

        private static readonly Dictionary<string, float> PieceStd = new Dictionary<string, float>();
        private static readonly Dictionary<string, float> TypedStd = new Dictionary<string, float>();

        static AI3()
        {
            TypedStd["00000"] = 50000;
            TypedStd["+0000+"] = 4320;
            TypedStd["+000++"] = 720;
            TypedStd["++000+"] = 720;
            TypedStd["+00+0+"] = 720;
            TypedStd["+0+00+"] = 720;

            TypedStd["0000+"] = 720;
            TypedStd["+0000"] = 720;
            TypedStd["00+00"] = 720;
            TypedStd["0+000"] = 720;
            TypedStd["000+0"] = 720;

            TypedStd["++00++"] = 120;
            TypedStd["++0+0+"] = 120;
            TypedStd["+0+0++"] = 120;

            TypedStd["+++0++"] = 20;
            TypedStd["++0+++"] = 20;

            PieceStd["00000"] = 800000;
            PieceStd["+0000"] = 15000;
            PieceStd["0000+"] = 15000;
            PieceStd["00+00"] = 12000;
            PieceStd["0+000"] = 13000;
            PieceStd["000+0"] = 13000;

            PieceStd["+000+"] = 15000;
            PieceStd["+000+"] = 15000;

            PieceStd["+0+00"] = 800;
            PieceStd["0+00+"] = 800;
            PieceStd["+00+0"] = 800;
            PieceStd["00+0+"] = 800;

            PieceStd["00+++"] = 35;
            PieceStd["+00++"] = 35;
            PieceStd["++00+"] = 35;
            PieceStd["+++00"] = 35;

            PieceStd["++++0"] = 7;
            PieceStd["+++0+"] = 7;
            PieceStd["++0++"] = 7;
            PieceStd["+0+++"] = 7;
            PieceStd["0++++"] = 7;
        }

        public AI3(int[,] chess, Action debug = null)
        {
            this.Len = chess.GetLength(0);
            this.Chess = chess;
            this.Debug = debug;
            this.PieceLines = new List<PointC[]>();
            this.TypedLines = new List<PointC[]>();

            for (int x = 0; x < this.Len; x++)
            {
                for (int y = 0; y < this.Len; y++)
                {
                    foreach (var position in P4)
                    {
                        PointC[] line5 = new PointC[5];
                        PointC[] line6 = new PointC[6];
                        for (int i = 0; i < 6; i++)
                        {
                            int xi = x + i * position.X;
                            int yi = y + i * position.Y;

                            if (xi >= 0 && xi < this.Len && yi >= 0 && yi < this.Len)
                            {
                                if (i < 5) line5[i] = new PointC(xi, yi);
                                if (i < 6) line6[i] = new PointC(xi, yi);
                            }
                            else
                                break;

                            if (i == 4)
                            {
                                this.PieceLines.Add(line5);
                                this.TypedLines.Add(line5);
                            }
                            if (i == 5)
                            {
                                this.TypedLines.Add(line6);
                            }
                        }
                    }
                }
            }
        }

        public TryResult Go()
        {
            this.Result = null;
            TryResult res = new TryResult();

            foreach (PointC[] optimum in this.PieceLines)
            {
                if (optimum.All(e => this.Chess[e.X, e.Y] == 1))
                {
                    res.VictoryFace = 1;
                    res.VictoryLine = optimum;
                    return res;
                }
            }

            AlphaBeta(Level, -999999999, 999999999);

            res.Point = this.Result;

            if (this.Result != null)
            {
                this.Chess[res.Point.X, res.Point.Y] = 2;

                foreach (PointC[] optimum in this.PieceLines)
                {
                    if (optimum.All(e => this.Chess[e.X, e.Y] == 2))
                    {
                        res.VictoryFace = 2;
                        res.VictoryLine = optimum;
                        return res;
                    }
                }
            }

            return res;
        }

        private List<PointC> Optimums()
        {
            Dictionary<PointC, double> optimums = new Dictionary<PointC, double>();
            foreach (PointC[] optimum in this.PieceLines)
            {
                if (optimum.All(e => this.Chess[e.X, e.Y] == 0))
                    continue;
                if (optimum.All(e => this.Chess[e.X, e.Y] == 0 || this.Chess[e.X, e.Y] == 1))
                {
                    foreach (var pt in optimum)
                        if (this.Chess[pt.X, pt.Y] == 0) //&& Exists(pt)
                        {
                            this.Chess[pt.X, pt.Y] = 1;
                            string key = string.Join(string.Empty, optimum.Select(piece => this.Chess[piece.X, piece.Y] == 0 ? '+' : '0'));
                            this.Chess[pt.X, pt.Y] = 0;
                            if (PieceStd.ContainsKey(key))
                            {
                                if (optimums.ContainsKey(pt))
                                    optimums[pt] += PieceStd[key];
                                else
                                    optimums[pt] = PieceStd[key];

                                if (PieceStd[key] == 800000)
                                {
                                    return new List<PointC>() { pt };
                                }
                            }
                        }
                }
                else if (optimum.All(e => Chess[e.X, e.Y] == 0 || this.Chess[e.X, e.Y] == 2))
                {
                    foreach (var pt in optimum)
                        if (this.Chess[pt.X, pt.Y] == 0)//&& Exists(pt)
                        {
                            this.Chess[pt.X, pt.Y] = 2;
                            string key = string.Join(string.Empty, optimum.Select(piece => this.Chess[piece.X, piece.Y] == 0 ? '+' : '0'));
                            this.Chess[pt.X, pt.Y] = 0;
                            if (PieceStd.ContainsKey(key))
                            {
                                if (optimums.ContainsKey(pt))
                                    optimums[pt] += PieceStd[key];
                                else
                                    optimums[pt] = PieceStd[key];

                                if (PieceStd[key] == 800000)
                                {
                                    return new List<PointC>() { pt };
                                }
                            }
                        }
                }
            }

            int i = 5;
            List<PointC> opts = new List<PointC>();
            foreach (var items in optimums.GroupBy(e => e.Value).OrderByDescending(e => e.Key))
            {
                foreach (var item in items)
                {
                    opts.Add(item.Key);
                }

                if (i-- == 0) break;
            }

            return opts;
        }

        private double AlphaBeta(int depth, double alpha, double beta)
        {
            int pom = (depth % 2) == 0 ? 2 : 1;

            if (depth == 0) return Evaluate();

            foreach (var tryPot in this.Optimums())
            {
                this.Chess[tryPot.X, tryPot.Y] = pom; // 尝试
                double val = -AlphaBeta(depth - 1, -beta, -alpha);
                this.Chess[tryPot.X, tryPot.Y] = 0;// 还原

                if (val > alpha)
                {
                    if (depth == Level)
                        Result = tryPot;
                    if (val >= beta)
                        return beta;
                    alpha = val;
                }
            }

            return alpha;
        }

        private double Evaluate()
        {
            double score = 0;

            foreach (PointC[] optimum in this.TypedLines)
            {
                if (optimum.All(e => this.Chess[e.X, e.Y] == 0))
                    continue;

                else if (optimum.All(e => this.Chess[e.X, e.Y] == 0 || this.Chess[e.X, e.Y] == 2))
                {
                    string key = string.Join(string.Empty, optimum.Select(piece => this.Chess[piece.X, piece.Y] == 0 ? '+' : '0'));
                    if (TypedStd.ContainsKey(key))
                    {
                        score += TypedStd[key];
                        if (TypedStd[key] == 50000)
                        {
                            score += 1000000;
                        }
                    }
                }
                else if (optimum.All(e => this.Chess[e.X, e.Y] == 0 || this.Chess[e.X, e.Y] == 1))
                {
                    string key = string.Join(string.Empty, optimum.Select(piece => this.Chess[piece.X, piece.Y] == 0 ? '+' : '0'));
                    if (TypedStd.ContainsKey(key))
                    {
                        score -= TypedStd[key];
                        if (TypedStd[key] == 50000)
                        {
                            score -= 1000000;
                        }
                    }
                }
            }
            return score;
        }
    }
}