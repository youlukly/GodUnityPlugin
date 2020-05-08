using UnityEngine;

namespace GodUnityPlugin
{
    // materialized grid array element
    public struct GUPGridCell
    {
        public string id;
        public Vector3 center;
        public Vector3 normal;
        public Vector3[] vertices;
        public Quaternion euler;
        public float width,height;
        public int rowIndex, columnIndex;

        public GUPGridCell(string id, Vector3 center,Vector3 normal,Vector3[] vertices,Quaternion euler,float width, float height, int rowIndex, int columnIndex)
        {
            this.id = id;
            this.center = center;
            this.normal = normal;
            this.vertices = vertices;
            this.euler = euler;
            this.width = width;
            this.height = height;
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
        }
    }
}
