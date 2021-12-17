using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Chess.Core
{
    public class TryResult
    {
        public PointC Point { get; set; }

        public int VictoryFace { get; set; }

        public PointC[] VictoryLine { get; set; }
    }
}
