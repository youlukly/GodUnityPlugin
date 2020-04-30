using UnityEngine;

namespace GodUnityPlugin
{
    // materialized grid array element
    public struct GridCell
    {
        public string id;
        public Vector3 center;
        public Vector3 eulerAngles;
        public float width;
        public float height;
        public int rowIndex, columnIndex;

        public GridCell(string id, Vector3 center, Vector3 eulerAngles, float width, float height, int rowIndex, int columnIndex)
        {
            this.id = id;
            this.center = center;
            this.width = width;
            this.height = height;
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
            this.eulerAngles = eulerAngles;
        }
    }
}
