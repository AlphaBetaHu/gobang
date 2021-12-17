using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess.Core
{
    public class PointPath
    {
        public PointC P { get; set; }

        public PointC M { get; set; }

        public PointPath(PointC p, PointC m)
        {
            this.P = p;
            this.M = m;
        }
    }
}
