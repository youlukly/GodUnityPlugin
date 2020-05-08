using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    public class GridCellComparer : IEqualityComparer<GUPGridCell>
    {
        public bool Equals(GUPGridCell x, GUPGridCell y)
        {
            return x.center == y.center &&
            x.columnIndex == y.columnIndex &&
            x.rowIndex == y.rowIndex &&
            x.height == y.height &&
            x.width == y.width &&
            x.id == y.id &&
            x.normal == y.normal;
        }

        public int GetHashCode(GUPGridCell cell)
        {
            return (int)cell.GetHashCode();    
        }
    }

}