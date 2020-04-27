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
        [SerializeField] private float gridScale = 1f;

        [SerializeField] private int cellCountX = 5;
        [SerializeField] private int cellCountY = 5;

        [SerializeField] private float cellWidth = 5f;
        [SerializeField] private float cellHeight = 5f;

        [SerializeField] private bool drawGizmo = true;
      
        [SerializeField] private Vector3 gridOffset = Vector3.zero;

        public Color gizmoLineColor = new Color(0.4f, 0.4f, 0.3f, 1f);

        public Color gizmoCellColor = new Color(0.0f, 1.0f, 0.3f, 1f);

        public Cell[][] cells;

        public float Width { get { return AbsCellWidth * AbsCellCountX * gridScale; } }

        public float Height { get { return AbsCellHeight * AbsCellCountY * gridScale; } }

        public Vector3 center { get { return transform.position + gridOffset; } }

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

        private float AbsCellWidth { get { return Mathf.Abs(cellWidth); } }

        private float AbsCellHeight { get { return Mathf.Abs(cellHeight); } }

        private float xMin { get { return -(Width / 2.0f); } }

        private float xMax { get { return -xMin; } }

        private float yMin { get { return -(Height / 2.0f); } }

        private float yMax { get { return -yMin; } }

        public void CreateCellMatrix()
        {
            InitializeArray();

            Quaternion rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z);

            Vector3 defaultCenter = center + new Vector3(xMin, yMax);

            Vector3 buffer = new Vector3(AbsCellWidth / 2.0f, -AbsCellHeight / 2.0f) * gridScale;

            defaultCenter = defaultCenter + buffer;

            for (int i = 0; i < AbsCellCountY; i++)
            {
                for (int j = 0; j < AbsCellCountX; j++)
                {
                    int index = i * AbsCellCountX + j;

                    Vector3 center =  defaultCenter + new Vector3(j * AbsCellWidth * gridScale, i * AbsCellHeight * gridScale);

                    center = rotation * center;

                    cells[i][j]  = new Cell("Cell " + index, center, AbsCellWidth, AbsCellHeight, i, j);
                }
            }

            Debug.Log("successfully created cell matrix. cell amount is : " + (AbsCellCountX * AbsCellCountY) );
        }

        // rename + centre the gameobject upon first time dragging the script into the editor. 
        private void Reset()
        {
            if (name == "GameObject")
                name = "Grid";

            transform.position = Vector3.zero;
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

            if (cells != null)
            {
                Gizmos.color = gizmoCellColor;

                float rad = AbsCellCountX * 0.35f;
                if (AbsCellCountY < AbsCellCountX)
                    rad = AbsCellCountY * 0.35f;

                for (int i = 0; i < AbsCellCountY; i++)
                    for (int j = 0; j < AbsCellCountX; j++)
                        Gizmos.DrawSphere(cells[i][j].center, rad);
            }

            Gizmos.matrix = transform.localToWorldMatrix;

            // draw the horizontal lines
            for (int x = 0; x < AbsCellCountY + 1; x++)
            {
                if (x == 0 || x == AbsCellCountY)
                    Gizmos.color = gizmoLineColor;
                else
                    Gizmos.color = Color.Lerp(Color.white, gizmoLineColor, 0.6f);

                float y = yMax - (x * AbsCellHeight * gridScale);

                Vector3 pos1 = new Vector3(xMin, y , 0);
                Vector3 pos2 = new Vector3(xMax, y , 0);

                Gizmos.DrawLine((gridOffset + pos1), (gridOffset + pos2));
            }

            // draw the vertical lines
            for (int y = 0; y < AbsCellCountX + 1; y++)
            {
                if (y == 0 || y == AbsCellCountX)
                    Gizmos.color = gizmoLineColor;
                else
                    Gizmos.color = Color.Lerp(Color.white, gizmoLineColor, 0.6f);

                float x = xMin + (y * AbsCellWidth * gridScale);

                Vector3 pos1 = new Vector3(x, yMax, 0);
                Vector3 pos2 = new Vector3(x, yMin, 0);

                Gizmos.DrawLine((gridOffset + pos1), (gridOffset + pos2));
            }

          
        }
    }
}