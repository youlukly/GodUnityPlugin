using UnityEngine;
using System.Collections.Generic;

namespace GodUnityPlugin
{
    public class GUPGrid : MonoBehaviour
    {
        [System.Serializable]
        public struct GroupData
        {
            public string groupName;
            public Color editorColor;
            public List<int> indices;

            public void SetIndices(List<int> indices)
            {
                this.indices = indices;
            }
        }

        [Header("Grid")]
        // universal grid scale
        [SerializeField] private float scale = 1f;
        // count of the grid array
        [SerializeField] private int row = 5;
        [SerializeField] private int column = 5;
        [SerializeField] private string defaultGroupName;
        [SerializeField] private List<GroupData> groupDatas;

        [Space(10)]
        [Header("GridCell")]
        // size of the cell
        [SerializeField] private float cellWidth = 5f;
        [SerializeField] private float cellHeight = 5f;
        [SerializeField] private string cellName = "Cell";

        [Space(10)]
        [Header("Gizmos")]
        [SerializeField] private bool drawGizmos = true;
        [SerializeField] private bool drawCellName = true;
        [SerializeField] private bool drawCellLines = true;
        [SerializeField] private int cellLineCount = 2;
        [SerializeField] private bool drawNormal = true;
        [SerializeField] private bool drawVertex = true;
        [SerializeField] private Color gizmoLineColor = new Color(0.25f, 0.1f, 0.25f, 1f);

        // array of the grid cells
        public GridCell[][] CellArray 
        {
            get
            {
                if (_cellArray == null)
                    UpdateCellMatrix();

                return _cellArray;
            }
        }

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

        private GridCell[][] _cellArray;

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
      /*  public bool TryGet(int index,out GridCell cell)
        {
            cell = new GridCell();

            if (index > Count || index < 0)
                return false;

            foreach (var seperateGroupData in seperateGroupDatas)
            {
                foreach (var seperateIndex in seperateGroupData.indices)
                {
                    if (seperateIndex == index)
                        return false;
                }
            }

            for (int i = 0; i < Column; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    GridCell gridCell = CellArray[i][j];
                    if (gridCell.index == index)
                    {
                        cell = gridCell;
                        return true;
                    }
                }
            }

            return false;
        }*/

        public bool TryGet(int row, int column,out GridCell cell)
        {
            cell = new GridCell();

            if (row < 0 || column < 0 || column > CellArray.Length-1 || row > CellArray[0].Length-1)
                return false;

            cell = CellArray[column][row];
            return true;
        }

        /* // check if a vector is in grid matrix
         public bool IsInGrid(Vector3 point,out GridCell cell)
         {
             cell = new GridCell();

             for (int i = 0; i < Column; i++)
             {
                 for (int j = 0; j < Row; j++)
                 {
                     GridCell current = CellArray[i][j];

                     foreach (var seperateGroupData in seperateGroupDatas)
                     {
                         foreach (var seperateIndex in seperateGroupData.indices)
                         {
                             if (seperateIndex == current.index)
                                 return false;
                         }
                     }

                     if (current.IsInCell(point))
                     {
                         cell = current;

                         return true;
                     }
                 }
             }

             if (independentGridCells != null )
                 foreach (var independentCell in independentGridCells)
                 {
                     if (independentCell.IsInCell(point))
                     {
                         cell = independentCell.GetCell();
                         return true;
                     }
                 }

             return false;
         }
 */
        public bool IsInAnyGrid(Vector3 point, out GridCell cell)
        {
            cell = new GridCell();

            Vector3 a = vertices[0];
            Vector3 b = vertices[1];
            Vector3 c = vertices[2];

            if (!GUPMath.IsVertexInRectangle(a, b, c, vertices[3], point))
                return false;

            Vector3 projectAB = Vector3.Project(point - a, b - a);
            Vector3 projectAC = Vector3.Project(point - a, c - a);

            float rowMag = projectAB.magnitude;
            float colMag = projectAC.magnitude;

            int row = (int)(rowMag / CellWidth);
            int col = (int)(colMag / CellHeight);

            return TryGet(row, col, out cell);
        }

        // check if a vector is in grid matrix
        public bool IsInGrid(Vector3 point, out GridCell cell)
        {
            return IsInGrid(point, defaultGroupName,out cell);
        }

        public bool IsInGrid(Vector3 point, string groupID, out GridCell cell)
        {
            if (!IsInAnyGrid(point,out cell))
                return false;

            return cell.groupID == groupID;
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

                    CellArray[i][j] = new GridCell(name,defaultGroupName ,cellCenter,normal,GetCellVertices(i,j, CellWidth,CellHeight),quaternionEuler, CellWidth, CellHeight,index , j, i);
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

        public void SetGroupDatas(List<GroupData> groupDatas)
        {
            this.groupDatas = groupDatas;

            foreach (var group in groupDatas)
                foreach (var index in group.indices)
                    SetGroupNameOfCell(index, group.groupName);
        }

        public void AddSeperateIndex(string groupID,int index)
        {
            if(groupDatas != null)
                foreach (var group in groupDatas)
                {
                    if (group.groupName == groupID)
                    {
                        if (group.indices == null)
                            group.SetIndices(new List<int>());
                        else
                        {
                            foreach (var seperateIndex in group.indices)
                            {
                                if (seperateIndex == index)
                                    return;
                            }
                        }
                        group.indices.Add(index);

                        SetGroupNameOfCell(index,group.groupName);

                        return;
                    }
                    else
                    {
                        if (group.indices.Contains(index))
                            group.indices.Remove(index);
                    }
                }
        }

        public void RemoveIgnoreCellIndices(string id, int index)
        {
            if (groupDatas != null)
                foreach (var group in groupDatas)
                {
                    if (group.groupName == id)
                    {
                        if (group.indices == null)
                            return;
                        else
                        {
                            foreach (var seperateIndex in group.indices)
                            {
                                if (seperateIndex == index)
                                {
                                    group.indices.Remove(index);

                                    SetGroupNameOfCell(index, null);

                                    return;
                                }
                            }
                        }
                    }
                }
        }

        public List<GroupData> GetGroups()
        {
            return groupDatas;
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

            UpdateCellMatrix();
        }

        private void OnValidate()
        {
            UpdateCellMatrix();
        }

        // initialize cell array
        private void InitializeArray()
        {
            _cellArray = new GridCell[Column][];
            for (int i = 0; i < _cellArray.Length; i++)
                _cellArray[i] = new GridCell[Row];
        }

        private void SetGroupNameOfCell(int index, string groupName)
        {
            int column = (int)(index / Column);
            int row = index - (column * Column);

            CellArray[column][row].groupID = groupName;
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

            if (drawNormal)
            {
                Gizmos.color = Color.Lerp(Color.black, gizmoLineColor, 0.4f);

                Gizmos.DrawLine(Center, Center + dir);
            }

            if (drawVertex)
                for (int i = 0; i < vertices.Length; i++)
                {
                    Gizmos.DrawSphere(vertices[i], range * 0.1f);
                    UnityEditor.Handles.Label(vertices[i], "vertex " + i);
                }

            if (CellArray == null || CellArray.Length == 0)
                return;

            for (int i = 0; i < CellArray.Length; i++)
                for (int j = 0; j < CellArray[i].Length; j++)
                {
                    GridCell cell = CellArray[j][i];

                    if (string.IsNullOrEmpty(cell.groupID))
                    {
                        Gizmos.color = Color.Lerp(Color.white, gizmoLineColor, 0.2f);
                    }
                    else
                    {
                        if (groupDatas != null)
                            foreach (var group in groupDatas)
                            {
                                if (cell.groupID == group.groupName)
                                    Gizmos.color = Color.Lerp(group.editorColor, gizmoLineColor, 0.2f);
                            }

                    }

                    if (drawCellLines)
                    {
                        Gizmos.DrawLine(cell.vertices[0], cell.vertices[3]);
                        Gizmos.DrawLine(cell.vertices[1], cell.vertices[2]);

                        for (int l = 0; l < cell.vertices.Length; l++)
                        {
                            int next1 = 0;
                            int next2 = 0;

                            switch (l)
                            {
                                case 0:
                                    next1 = 1;
                                    next2 = 3;
                                    break;
                                case 1:
                                    next1 = 3;
                                    next2 = 2;
                                    break;
                                case 2:
                                    next1 = 0;
                                    next2 = 1;
                                    break;
                                case 3:
                                    next1 = 2;
                                    next2 = 0;
                                    break;
                            }

                            Vector3 a = cell.vertices[l];
                            Vector3 b = cell.vertices[next1];
                            Vector3 c = cell.vertices[next2];

                            Vector3 ab = (b - a) / (cellLineCount + 1);
                            Vector3 cb = (b - c) / (cellLineCount + 1);

                            for (int k = 1; k < cellLineCount; k++)
                            {
                                Vector3 from = a + (ab * k);
                                Vector3 to = c + (cb * k);

                                Gizmos.DrawLine(from, to);
                            }
                        }
                    }

                    if (drawNormal)
                        Gizmos.DrawRay(cell.center, cell.normal * scale);

                    if (drawCellName)
                        UnityEditor.Handles.Label(cell.center, cell.id);
                }
        }
#endif

    }
}