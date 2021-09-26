using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eltin_Buchard_Keller_Algorithm
{
    public class LevenshteinNode : BKTreeNode
    {
        public LevenshteinNode(string values) : base(values)
        {
        }

        override protected int CalculateDistance(BKTreeNode node)
        {
            return DistanceMetric.CalculateLevenshteinDistance(Data, ((LevenshteinNode)node).Data);
        }
    }
}
