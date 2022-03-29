using UnityEngine;

namespace GodUnityPlugin
{
    // materialized grid array element
    public struct GridCell
    {
        public string groupID;
        public Vector3 center;
        public Vector3 normal;
        public Vector3[] vertices;
        public float width,height;
        public int index;
        public int horizontalIndex, verticalIndex;

        public GridCell(string groupID,Vector3 center,Vector3 normal,Vector3[] vertices,float width, float height,int index,int horizontalIndex, int verticalIndex)
        {
            this.groupID = groupID;
            this.center = center;
            this.normal = normal;
            this.vertices = vertices;
            this.width = width;
            this.height = height;
            this.index = index;
            this.horizontalIndex = horizontalIndex;
            this.verticalIndex = verticalIndex;
        }

        // check if a vector is in cell
        public bool IsInCell(Vector3 point)
        {
            Vector3 ab = vertices[1] - vertices[0];
            Vector3 ac = vertices[2] - vertices[0];
            Vector3 ap = point - vertices[0];

            float dotABAP = Vector3.Dot(ab, ap);
            float abMag = ab.sqrMagnitude;
            float dotACAP = Vector3.Dot(ac, ap);
            float acMag = ac.sqrMagnitude;

            if (dotABAP < 0 || dotABAP > abMag || dotACAP < 0 || dotACAP > acMag)
                return false;

            return true;
        }
    }
}
