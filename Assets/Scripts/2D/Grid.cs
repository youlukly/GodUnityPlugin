using UnityEngine;

namespace GodUnityPlugin
{
    public class Grid : MonoBehaviour
    {
        public struct Cell
        {
            public string id;
            public Vector2 center;
            public float width;
            public float height;
            public int xIndex, yIndex;

            public Cell(string id, Vector2 center, float width, float height, int xIndex, int yIndex)
            {
                this.id = id;
                this.center = center;
                this.width = width;
                this.height = height;
                this.xIndex = xIndex;
                this.yIndex = yIndex;
            }

            public bool Contains(Vector2 point)
            {
                float xDiff = Mathf.Abs((center - point).x);
                float yDiff = Mathf.Abs((center - point).y);

                if (xDiff > (width / 2.0f) || yDiff > (height / 2.0f))
                    return false; 

                return true;
            }
        }

        // universal grid scale
        public float gridScale = 1f;

        public int cellCountX = 5;
        public int cellCountY = 5;

        public float cellWidth = 5f;
        public float cellHeight = 5f;

        public bool drawGizmo = true;

        public Cell[][] cells;

        public int AbsCellCountX
        {
            get
            {
                if (cellCountX == 0)
                    cellCountX = 1;

                return Mathf.Abs(cellCountX);
            }
        }

        public int AbsCellCountY
        {
            get
            {
                if (cellCountY == 0)
                    cellCountY = 1;

                return Mathf.Abs(cellCountY);
            }
        }

        public float xMin
        {
            get
            {
                return -((cellWidth * AbsCellCountX) / 2.0f);
            }
        }

        public float xMax
        {
            get
            {
                return -xMin;
            }
        }

        public float yMin
        {
            get
            {
                return -((cellHeight * AbsCellCountY) / 2.0f);
            }
        }

        public float yMax
        {
            get
            {
                return -yMin;
            }
        }

        public Vector3 gridOffset = Vector3.zero;

        public Color gizmoLineColor = new Color(0.4f, 0.4f, 0.3f, 1f);

        // rename + centre the gameobject upon first time dragging the script into the editor. 
        private void Reset()
        {
            if (name == "GameObject")
                name = "Grid";

            transform.position = Vector3.zero;
        }

        public void CreateCellMatrix()
        {
            InitializeArray();

            Vector2 defaultCenter = new Vector2(xMin, yMax) + new Vector2(-cellWidth / 2.0f, -cellHeight / 2.0f);

            for (int i = 0; i < AbsCellCountY; i++)
            {
                for (int j = 0; j < AbsCellCountX; j++)
                {
                    int index = i * AbsCellCountX + j;

                    Vector2 center = defaultCenter + new Vector2(j * cellWidth, i * cellHeight);

                    cells[i][j] = new Cell("Cell " + index, center, cellWidth, cellHeight, i, j);
                }
            }

            Debug.Log("successfully created cell matrix. cell amount is : " + (AbsCellCountX * AbsCellCountY) );
        }

       
        private void InitializeArray()
        {
            cells = new Cell[AbsCellCountY][];
            for (int i = 0; i < cells.Length; i++)
                cells[i] = new Cell[AbsCellCountX];
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmo)
                return;

            Gizmos.matrix = transform.localToWorldMatrix;

            // draw the horizontal lines
            for (int x = 0; x < AbsCellCountY + 1; x++)
            {
                if (x == 0 || x == AbsCellCountY)
                    Gizmos.color = gizmoLineColor;
                else
                    Gizmos.color = Color.Lerp(Color.white, gizmoLineColor, 0.6f);

                float y = yMax - (x * cellHeight);

                Vector3 pos1 = new Vector3(xMin, y, 0) * gridScale;
                Vector3 pos2 = new Vector3(xMax, y, 0) * gridScale;

                Gizmos.DrawLine((gridOffset + pos1), (gridOffset + pos2));
            }

            // draw the vertical lines
            for (int y = 0; y < AbsCellCountX + 1; y++)
            {
                if (y == 0 || y == AbsCellCountX)
                    Gizmos.color = gizmoLineColor;
                else
                    Gizmos.color = Color.Lerp(Color.white, gizmoLineColor, 0.6f);

                float x = xMin + (y * cellWidth);

                Vector3 pos1 = new Vector3(x, yMax, 0) * gridScale;
                Vector3 pos2 = new Vector3(x, yMin, 0) * gridScale;

                Gizmos.DrawLine((gridOffset + pos1), (gridOffset + pos2));
            }
        }
    }
}