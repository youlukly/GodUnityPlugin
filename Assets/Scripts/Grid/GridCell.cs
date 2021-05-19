using UnityEngine;

namespace GodUnityPlugin
{
    // materialized grid array element
    public struct GridCell
    {
        public string id;
        public string groupID;
        public Vector3 center;
        public Vector3 normal;
        public Vector3[] vertices;
        public Quaternion eulerAngle;
        public float width,height;
        public int index;
        public int rowIndex, columnIndex;

        public GridCell(string id, string groupID,Vector3 center,Vector3 normal,Vector3[] vertices,Quaternion euler,float width, float height,int index,int rowIndex, int columnIndex)
        {
            this.id = id;
            this.groupID = groupID;
            this.center = center;
            this.normal = normal;
            this.vertices = vertices;
            this.eulerAngle = euler;
            this.width = width;
            this.height = height;
            this.index = index;
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
        }

        // check if a vector is in cell
        public bool IsInCell(Vector3 point)
        {
            Vector3 a = vertices[0];
            Vector3 b = vertices[1];
            Vector3 c = vertices[2];
            Vector3 d = vertices[3];

            return GUPMath.IsVertexInRectangle(a, b, c, d, point);
        }
    }
}
