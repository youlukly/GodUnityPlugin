using UnityEngine;

namespace GodUnityPlugin
{
    public class Grid : MonoBehaviour
    {
        [Header("Grid")]
        // universal grid scale
        [SerializeField] private float gridScale = 1f;
        // count of the grid array
        [SerializeField] private int row = 5;
        [SerializeField] private int column = 5;
        [SerializeField] private Vector3 gridOffset = Vector3.zero;

        [Space(10)]
        [Header("GridCell")]
        // size of the cell
        [SerializeField] private float cellWidth = 5f;
        [SerializeField] private float cellHeight = 5f;
        public string cellName = "Cell";

        [Space(10)]
        [Header("Gizmos")]
        [SerializeField] private bool drawGizmos = true;
        public Color gizmoLineColor = new Color(0.25f, 0.1f, 0.25f, 1f);
        public Color gizmoCellColor = new Color(0.2f, 0.6f, 0.45f, 0.8f);

        // array of the grid cells
        public GridCell[][] CellArray { get; private set; }

        // total width of the grid
        public float Width { get { return AbsCellWidth * Row * gridScale; } }

        // total height of the grid
        public float Height { get { return AbsCellHeight * Column * gridScale; } }

        // center of the grid
        public Vector3 center { get { return ( Quaternion.Inverse(quaternionEuler) * transform.position) + gridOffset; } }

        // normal of the grid
        public Vector3 normal { get { return GetNormal(); } }

        public Vector3[] vertices { get { return GetVertices(); } }

        // row value that returns natural number always
        public int Row
        {
            get
            {
                if (row == 0)
                    row = 1;

                return Mathf.Abs(row);
            }
        }

        // column value that returns natural number always
        public int Column
        {
            get
            {
                if (column == 0)
                    column = 1;

                return Mathf.Abs(column);
            }
        }

        // euler quaternion of grid gameObject
        public Quaternion quaternionEuler { get { return Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z); } }

        // absolute number of cell width
        public float AbsCellWidth { get { return Mathf.Abs(cellWidth); } }

        // absolute number of cell height
        public float AbsCellHeight { get { return Mathf.Abs(cellHeight); } }

        // minimal x-axis value of grid
        private float xOffsetMin { get { return -(Width / 2.0f); } }

        // maximum x-axis value of grid
        private float xOffsetMax { get { return -xOffsetMin; } }

        // minimal y-axis value of grid
        private float yOffsetMin { get { return -(Height / 2.0f); } }

        // maximum y-axis value of grid
        private float yOffsetMax { get { return -yOffsetMin; } }
  
        // returns the cell that matches the ID
        public GridCell Get(string id)
        {
            for (int i = 0; i < Column; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    GridCell cell = CellArray[i][j];

                    if (cell.id == id)
                        return cell;
                }
            }

            return new GridCell();
        }

        private void Awake()
        {
            UpdateCellMatrix();
        }

        // check if a vector is in grid matrix
        public bool IsInCell(Vector3 point,out GridCell cell)
        {
            cell = new GridCell();

            for (int i = 0; i < Column; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    GridCell current = CellArray[i][j];

                    if (IsInCell(point,current))
                    {
                        cell = current;

                        return true;
                    }
                }
            }

            return false;
        }

        // recalculate cell matrix data
        public void UpdateCellMatrix()
        {
            InitializeArray();

            for (int i = 0; i < Column; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    //calibrate by rotation
                    Vector3 cellCenter = quaternionEuler * GetCellCenterRaw(i, j);

                    //name index suffix
                    int index = i * Row + j;

                    string name = cellName + " " + index;

                    CellArray[i][j] = new GridCell(name, cellCenter,normal,GetCellVertices(i,j, AbsCellWidth,AbsCellHeight), AbsCellWidth, AbsCellHeight, j, i);
                }
            }
        }

        // compare two cell values
        public bool Equals(GridCell x, GridCell y)
        {
            return x.center == y.center &&
            x.columnIndex == y.columnIndex &&
            x.rowIndex == y.rowIndex &&
            x.height == y.height &&
            x.width == y.width &&
            x.id == y.id &&
            x.normal == y.normal;
        }

        // returns vertices of the cell. always returns 4 values with matrix order
        private Vector3[] GetCellVertices(int columnIndex,int rowIndex,float width,float height)
        {
            Vector3 cellCenter = GetCellCenterRaw(columnIndex, rowIndex);

            float xMin = cellCenter.x - (width / 2.0f);
            float xMax = cellCenter.x + (width / 2.0f);
            float yMin = cellCenter.y - (height / 2.0f);
            float yMax = cellCenter.y + (height / 2.0f);

            Vector3 a = quaternionEuler * new Vector3(xMin, yMax);
            Vector3 b = quaternionEuler * new Vector3(xMax, yMax);
            Vector3 c = quaternionEuler * new Vector3(xMin, yMin);
            Vector3 d = quaternionEuler * new Vector3(xMax, yMin);

            Vector3[] vertices = new Vector3[] { a,b,c,d };

            return vertices;
        }

        // returns normal of the grid.
        private Vector3 GetNormal()
        {
            Vector3 normal = Vector3.zero;

            Vector3[] vert = vertices;

            Vector3 a = vert[0];
            Vector3 b = vert[1];
            Vector3 c = vert[2];

            Vector3 side1 = b - a;
            Vector3 side2 = c - a;

            Vector3 perp = Vector3.Cross(side1,side2);

            return  perp.normalized;
        }

        // returns vertices of the grid. always returns 4 values with matrix order
        private Vector3[] GetVertices()
        {
            Vector3 centerRaw = transform.position + gridOffset;

            float xMin = centerRaw.x - (Width / 2.0f);
            float xMax = centerRaw.x + (Width / 2.0f);
            float yMin = centerRaw.y - (Height / 2.0f);
            float yMax = centerRaw.y + (Height / 2.0f);

            Vector3 a = quaternionEuler * new Vector3(xMin, yMax);
            Vector3 b = quaternionEuler * new Vector3(xMax, yMax);
            Vector3 c = quaternionEuler * new Vector3(xMin, yMin);
            Vector3 d = quaternionEuler * new Vector3(xMax, yMin);

            Vector3[] vertices = new Vector3[] { a, b, c, d };

            return vertices;
        }

        // check if a vector is in cell element
        private bool IsInCell(Vector3 point,GridCell cell)
        {
            Vector3[] vertices = GetCellVertices(cell.columnIndex,cell.rowIndex,cell.width,cell.height);

            Vector3 a = vertices[0];
            Vector3 b = vertices[1];
            Vector3 c = vertices[2];
            Vector3 d = vertices[3];

            return GUPMath.IsVertexInRectangle(a,b,c,d,point);
        }

        // uncalibrated center of the cell 
        private Vector3 GetCellCenterRaw(int columnIndex, int rowIndex)
        {
            Vector3 defaultCenter = center + new Vector3(xOffsetMin, yOffsetMax);

            Vector3 buffer = new Vector3(AbsCellWidth / 2.0f, -AbsCellHeight / 2.0f) * gridScale;

            defaultCenter = defaultCenter + buffer;

            Vector3 cellCenter = defaultCenter + new Vector3(rowIndex * AbsCellWidth * gridScale, columnIndex * -AbsCellHeight * gridScale);

            return cellCenter;
        }

        // rename + centre the gameobject upon first time dragging the script into the editor. 
        private void Reset()
        {
            if (name == "GameObject")
                name = "Grid";

            transform.position = Vector3.zero;
        }

        // initialize cell array
        private void InitializeArray()
        {
            CellArray = new GridCell[Column][];
            for (int i = 0; i < CellArray.Length; i++)
                CellArray[i] = new GridCell[Row];
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!drawGizmos)
                return;

            // draw the horizontal lines
            for (int x = 0; x < Column + 1; x++)
            {
                if (x == 0 || x == Column)
                    Gizmos.color = gizmoLineColor;
                else
                    Gizmos.color = Color.Lerp(Color.white, gizmoLineColor, 0.6f);

                float y = yOffsetMax - (x * AbsCellHeight * gridScale);

                Vector3 pos1 = center + new Vector3(xOffsetMin, y, 0);
                Vector3 pos2 = center + new Vector3(xOffsetMax, y, 0);

                pos1 = quaternionEuler * pos1;

                pos2 = quaternionEuler * pos2;

                Gizmos.DrawLine((gridOffset + pos1), (gridOffset + pos2));
            }

            // draw the vertical lines
            for (int y = 0; y < Row + 1; y++)
            {
                if (y == 0 || y == Row)
                    Gizmos.color = gizmoLineColor;
                else
                    Gizmos.color = Color.Lerp(Color.white, gizmoLineColor, 0.6f);

                float x = xOffsetMin + (y * AbsCellWidth * gridScale);

                Vector3 pos1 = center + new Vector3(x, yOffsetMax, 0);
                Vector3 pos2 = center + new Vector3(x, yOffsetMin, 0);

                pos1 = quaternionEuler * pos1;

                pos2 = quaternionEuler * pos2;

                Gizmos.DrawLine((gridOffset + pos1), (gridOffset + pos2));
            }

            float range = AbsCellWidth + AbsCellHeight;

            range = range / 2.0f;

            Vector3 dir = normal * range;

            Gizmos.color = Color.Lerp(Color.black, gizmoLineColor, 0.4f);

            Gizmos.DrawLine(center,center+dir);
        }
#endif

    }
}