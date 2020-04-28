using UnityEngine;

namespace GodUnityPlugin
{
    public class Grid : MonoBehaviour
    {
        // materialized grid array element
        public struct GridCell
        {
            public string id;
            public Vector3 center;
            public float width;
            public float height;
            public int rowIndex, columnIndex;

            public GridCell(string id, Vector3 center, float width, float height, int rowIndex, int columnIndex)
            {
                  this.id = id;
                this.center = center;
                this.width = width;
                this.height = height;
                this.rowIndex = rowIndex;
                this.columnIndex = columnIndex;
            }
        }

        [Header("Grid")]
        // universal grid scale
        [SerializeField] private float gridScale = 1f;
        // count of grid array
        [SerializeField] private int row = 5;
        [SerializeField] private int column = 5;
        [SerializeField] private Vector3 gridOffset = Vector3.zero;
        // force cell data update
        public bool forceUpdateCell = true;

        [Space(10)]
        [Header("GridCell")]
        // size of cell
        [SerializeField] private float cellWidth = 5f;
        [SerializeField] private float cellHeight = 5f;
        public string cellName = "Cell";

        [Space(10)]
        [Header("Gizmos")]
        [SerializeField] private bool drawGizmos = true;
        public Color gizmoLineColor = new Color(0.4f, 0.4f, 0.3f, 1f);

        // array of grid cells
        public GridCell[][] CellArray
        {
            get
            {
                if (forceUpdateCell)
                    UpdateCellMatrix();

                if (cellArray == null)
                    InitializeArray();

                return cellArray;
            }
        }

        // total width of grid
        public float Width { get { return AbsCellWidth * Row * gridScale; } }

        // total height of grid
        public float Height { get { return AbsCellHeight * Column * gridScale; } }

        // center of grid
        public Vector3 center { get { return ( Quaternion.Inverse(eulerRotation) * transform.position) + gridOffset; } }

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

        // absolute number of cell width
        private float AbsCellWidth { get { return Mathf.Abs(cellWidth); } }

        // absolute number of cell height
        private float AbsCellHeight { get { return Mathf.Abs(cellHeight); } }

        // minimal x-axis value of grid
        private float xOffsetMin { get { return -(Width / 2.0f); } }

        // maximum x-axis value of grid
        private float xOffsetMax { get { return -xOffsetMin; } }

        // minimal y-axis value of grid
        private float yOffsetMin { get { return -(Height / 2.0f); } }

        // maximum y-axis value of grid
        private float yOffsetMax { get { return -yOffsetMin; } }

        private Quaternion eulerRotation { get { return Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z); } }

        private GridCell[][] cellArray;

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
                    Vector3 cellCenter = eulerRotation * GetCellCenterRaw(i, j);

                    //name index suffix
                    int index = i * Row + j;

                    string name = cellName + " " + index;

                    cellArray[i][j] = new GridCell(name, cellCenter, AbsCellWidth, AbsCellHeight, j, i);
                }
            }
        }

        // check if a vector is in cell element
        private bool IsInCell(Vector3 point,GridCell cell)
        {
            Vector3 cellCenter = GetCellCenterRaw(cell.columnIndex,cell.rowIndex);

            float xMin = cellCenter.x - (cell.width / 2.0f);
            float xMax = cellCenter.x + (cell.width / 2.0f);
            float yMin = cellCenter.y - (cell.height / 2.0f);
            float yMax = cellCenter.y + (cell.height / 2.0f);

            Vector3 a = eulerRotation * new Vector3(xMin, yMax);

            Vector3 b = eulerRotation * new Vector3(xMax, yMax);

            Vector3 c = eulerRotation * new Vector3(xMin, yMin);

            Vector3 xVector = a - b;

            Vector3 yVector = a - c;

            Vector3 pointVector = a - point;

            float xMag = Vector3.Project(pointVector, xVector).magnitude;
            float yMag = Vector3.Project(pointVector, yVector).magnitude;

            if (xMag < 0.0f || xMag > cell.width || yMag < 0.0f || yMag > cell.height)
                return false;

            return true;
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
            cellArray = new GridCell[Column][];
            for (int i = 0; i < cellArray.Length; i++)
                cellArray[i] = new GridCell[Row];
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!drawGizmos)
                return;

            // draw cell cube
            Gizmos.color = gizmoLineColor;

            for (int i = 0; i < Column; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    GridCell cell = CellArray[i][j];

                    Gizmos.DrawCube(cell.center, Quaternion.Inverse(eulerRotation) * new Vector3(cell.width, cell.height));
                }
            }

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

                pos1 = eulerRotation * pos1;

                pos2 = eulerRotation * pos2;

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

                pos1 = eulerRotation * pos1;

                pos2 = eulerRotation * pos2;

                Gizmos.DrawLine((gridOffset + pos1), (gridOffset + pos2));
            }
        }
#endif

    }
}