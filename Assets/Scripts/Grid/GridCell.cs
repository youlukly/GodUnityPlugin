using UnityEngine;

namespace GodUnityPlugin
{
    // materialized grid array element
    public struct GridCell
    {
        public string id;
        public Vector3 center;
        public Vector3 normal;
        public Vector3[] vertices;
        public float width,height;
        public int rowIndex, columnIndex;

        public GridCell(string id, Vector3 center,Vector3 normal,Vector3[] vertices,float width, float height, int rowIndex, int columnIndex)
        {
            this.id = id;
            this.center = center;
            this.normal = normal;
            this.vertices = vertices;
            this.width = width;
            this.height = height;
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
        }
    }
}
