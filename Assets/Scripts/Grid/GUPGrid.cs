using UnityEngine;

namespace GodUnityPlugin
{
    public class GUPGrid : MonoBehaviour
    {
        [Header("Grid")]
        // universal grid scale
        [SerializeField] private float scale = 1f;
        // count of the grid array
        [SerializeField] private int row = 5;
        [SerializeField] private int column = 5;

        [Space(10)]
        [Header("GridCell")]
        // size of the cell
        [SerializeField] private float cellWidth = 5f;
        [SerializeField] private float cellHeight = 5f;
        [SerializeField] private string cellName = "Cell";
        [SerializeField] private IndependentGridCell[] independentGridCells;

        [Space(10)]
        [Header("Gizmos")]
        [SerializeField] private bool drawGizmos = true;
        [SerializeField] private Color gizmoLineColor = new Color(0.25f, 0.1f, 0.25f, 1f);

        // array of the grid cells
        public GridCell[][] CellArray { get; private set; }

        public float Scale { get { return Mathf.Abs(scale); } }

        public int Count { get { return Row * Column; } }

        // total width of the grid
        public float Width { get { return CellWidth * Row; } }

        // total height of the grid
        public float Height { get { return CellHeight * Column; } }

        // center of the grid
        public Vector3 Center { get { return transform.position; } }

        // normal of the grid
        public Vector3 normal { get { return GetNormal(); } }

        public Vector3[] vertices { get { return GetVertices(); } }

        // row value that returns natural number always
        public int Row
        {
            get
            {
                return Mathf.Abs(row);
            }
        }

        // column value that returns natural number always
        public int Column
        {
            get
            {
                return Mathf.Abs(column);
            }
        }

        // euler quaternion of grid gameObject
        public Quaternion quaternionEuler { get { return Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z); } }

        // total width of the cell
        public float CellWidth { get { return Mathf.Abs(cellWidth) * Scale; } }

        // total height of the cell
        public float CellHeight { get { return Mathf.Abs(cellHeight) * Scale; } }

        // minimal x-axis value of grid
        private float xOffsetMin { get { return -(Width / 2.0f); } }

        // maximum x-axis value of grid
        private float xOffsetMax { get { return -xOffsetMin; } }

        // minimal y-axis value of grid
        private float yOffsetMin { get { return -(Height / 2.0f); } }

        // maximum y-axis value of grid
        private float yOffsetMax { get { return -yOffsetMin; } }
  
        private Vector3 calibratedCenter { get { return Quaternion.Inverse(quaternionEuler)* Center;  } }

        public IndependentGridCell[] GetIndependentGridCells()
        {
            return independentGridCells;
        }

        // returns the index that matches the Cell
        public bool TryGet(GridCell cell, out int row, out int column)
        {
            row = 0;
            column = 0;

            for (int i = 0; i < Column; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    GridCell gridCell = CellArray[i][j];

                    if (CompareCell(gridCell,cell))
                    {
                        row = j;
                        column = i;
                        return true;
                    }
                }
            }

            return false;
        }

        // returns the cell that matches the ID
        public bool TryGet(string id,out GridCell cell)
        {
            cell = new GridCell();

            for (int i = 0; i < Column; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    GridCell gridCell = CellArray[i][j];

                    if (gridCell.id == id)
                    {
                        cell = gridCell;
                        return true;
                    }
                }
            }

            return false;
        }

        // returns the cell that matches the ID
        public bool TryGet(int index,out GridCell cell)
        {
            cell = new GridCell();

            if (index > Count || index < 0)
                return false;

            int column = GUPMath.Quotient(index, Column);

            int row = index % Row;

            cell = CellArray[column][row];
            return true;
        }

        public bool TryGet(GridCell cell, out int index)
        {
            index = 0;

            for (int i = 0; i < Column; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    GridCell gridCell = CellArray[i][j];

                    if (CompareCell(cell,gridCell))
                        return true;

                    index++;
                }
            }

            return false;
        }

        public bool TryGet(int row, int column,out GridCell cell)
        {
            cell = new GridCell();

            if (row < 0 || column < 0 || column > CellArray.Length-1 || row > CellArray[0].Length-1)
                return false;

            cell = CellArray[column][row];
            return true;
        }

        // check if a vector is in grid matrix
        public bool IsInGrid(Vector3 point,out GridCell cell)
        {
            cell = new GridCell();

            for (int i = 0; i < Column; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    GridCell current = CellArray[i][j];

                    if (current.IsInCell(point))
                    {
                        cell = current;

                        return true;
                    }
                }
            }

            if (independentGridCells == null || independentGridCells.Length == 0)
                return false;

            foreach (var independentCell in independentGridCells)
            {
                if(independentCell.IsInCell(point))
                {
                    cell = independentCell.GetCell();
                    return true;
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
                    Vector3 cellCenter = GetCellCenter(i, j);

                    //name index suffix
                    int index = i * Row + j;

                    string name = cellName + " " + index;

                    CellArray[i][j] = new GridCell(name, cellCenter,normal,GetCellVertices(i,j, CellWidth,CellHeight),quaternionEuler, CellWidth, CellHeight, j, i);
                }
            }
        }

        // compare two cell values
        public bool CompareCell(GridCell x, GridCell y)
        {
            return x.center == y.center &&
            x.columnIndex == y.columnIndex &&
            x.rowIndex == y.rowIndex &&
            x.height == y.height &&
            x.width == y.width &&
            x.id == y.id &&
            x.normal == y.normal;
        }

        private void Awake()
        {
            UpdateCellMatrix();
        }

        // returns vertices of the cell. always returns 4 values with matrix order
        private Vector3[] GetCellVertices(int columnIndex,int rowIndex,float width,float height)
        {
            Vector3 cellCenter = GetCellCenterRaw(columnIndex, rowIndex);

            float xMin = cellCenter.x - (width / 2.0f);
            float xMax = cellCenter.x + (width / 2.0f);
            float yMin = cellCenter.y - (height / 2.0f);
            float yMax = cellCenter.y + (height / 2.0f);

            Vector3 a = quaternionEuler * new Vector3(xMin, yMax, cellCenter.z);
            Vector3 b = quaternionEuler * new Vector3(xMax, yMax, cellCenter.z);
            Vector3 c = quaternionEuler * new Vector3(xMin, yMin, cellCenter.z);
            Vector3 d = quaternionEuler * new Vector3(xMax, yMin, cellCenter.z);

            Vector3[] vertices = new Vector3[] { a,b,c,d };

            return vertices;
        }

        // returns normal of the grid.
        private Vector3 GetNormal()
        {
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
            float xMin = calibratedCenter.x - (Width / 2.0f);
            float xMax = calibratedCenter.x + (Width / 2.0f);
            float yMin = calibratedCenter.y - (Height / 2.0f);
            float yMax = calibratedCenter.y + (Height / 2.0f);

            Vector3 a = quaternionEuler * new Vector3(xMin, yMax, calibratedCenter.z);
            Vector3 b = quaternionEuler * new Vector3(xMax, yMax, calibratedCenter.z);
            Vector3 c = quaternionEuler * new Vector3(xMin, yMin, calibratedCenter.z);
            Vector3 d = quaternionEuler * new Vector3(xMax, yMin, calibratedCenter.z);

            Vector3[] vertices = new Vector3[] { a, b, c, d };

            return vertices;
        }

        // uncalibrated center of the cell 
        private Vector3 GetCellCenterRaw(int columnIndex, int rowIndex)
        {
            Vector3 defaultCenter = calibratedCenter + new Vector3(xOffsetMin, yOffsetMax);

            Vector3 buffer = new Vector3(CellWidth / 2.0f, -CellHeight / 2.0f);

            defaultCenter = defaultCenter + buffer;
                
            Vector3 cellCenter = defaultCenter + new Vector3(rowIndex * CellWidth, columnIndex * -CellHeight);

            return cellCenter;
        }

        // calibrated center of the cell 
        private Vector3 GetCellCenter(int columnIndex, int rowIndex)
        {
            return quaternionEuler * GetCellCenterRaw(columnIndex,rowIndex);
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

                float y = yOffsetMax - (x * CellHeight);

                Vector3 pos1 = calibratedCenter + new Vector3(xOffsetMin, y, 0);
                Vector3 pos2 = calibratedCenter + new Vector3(xOffsetMax, y, 0);

                pos1 = quaternionEuler * pos1;

                pos2 = quaternionEuler * pos2;

                Gizmos.DrawLine(pos1, pos2);
            }

            // draw the vertical lines
            for (int y = 0; y < Row + 1; y++)
            {
                if (y == 0 || y == Row)
                    Gizmos.color = gizmoLineColor;
                else
                    Gizmos.color = Color.Lerp(Color.white, gizmoLineColor, 0.6f);

                float x = xOffsetMin + (y * CellWidth);

                Vector3 pos1 = calibratedCenter + new Vector3(x, yOffsetMax, 0);
                Vector3 pos2 = calibratedCenter + new Vector3(x, yOffsetMin, 0);

                pos1 = quaternionEuler * pos1;

                pos2 = quaternionEuler * pos2;

                Gizmos.DrawLine(pos1,pos2);
            }

            float range = CellWidth + CellHeight;

            range = range / 2.0f;

            Vector3 dir = normal * range;

            Gizmos.color = Color.Lerp(Color.black, gizmoLineColor, 0.4f);

            Gizmos.DrawLine(Center,Center+dir);

            for (int i = 0; i < vertices.Length; i++)
                Gizmos.DrawSphere(vertices[i], range * 0.1f);

            if (CellArray == null || CellArray.Length == 0)
                return;

            for (int i = 0; i < CellArray.Length; i++)
                for (int j = 0; j < CellArray[i].Length; j++)
                {
                    GridCell cell = CellArray[i][j];

                    Gizmos.DrawLine(cell.vertices[0], cell.vertices[3]);
                    Gizmos.DrawLine(cell.vertices[1], cell.vertices[2]);
                    Gizmos.DrawRay(cell.center, cell.normal * scale);
                    UnityEditor.Handles.Label(cell.center,cell.id);
                }
        }
#endif

    }
}