using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess.Core
{
    public class PointScore
    {
        public double Score { get; set; }

        public PointC Point { get; set; }

        public PointScore() { }

        public PointScore(PointC point, double score)
        {
            this.Point = point;
            this.Score = score;
        }
    }
}
