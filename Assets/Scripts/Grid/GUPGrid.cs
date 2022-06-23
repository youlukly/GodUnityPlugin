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
        // count of the grid array
        public int horizon = 5;
        public int vertical = 5;
        [SerializeField] private string defaultGroupName;
        [SerializeField] private GroupData[] groupDatas;
        [SerializeField] private GridSeparateData gridSeparateData;

        [Space(10)]
        [Header("GridCell")]
        // size of the cell
        public float cellWidth = 5f;
        public float cellHeight = 5f;

        [Space(10)]
        [Header("Gizmos")]
        [SerializeField] private bool drawGizmos = true;
        [SerializeField] private bool drawCellName = true;
        [SerializeField] private bool drawCellLines = true;
        [SerializeField] private int cellLineCount = 2;
        [SerializeField] private bool drawNormal = true;
        [SerializeField] private bool drawVertex = true;
        [SerializeField] private Color gizmoLineColor = new Color(0.25f, 0.1f, 0.25f, 1f);

        private GridCell[][] cellArray;
        private Vector3 normal;
        private Vector3[] vertices;

        // total width of the grid
        public float GetGridWidth() { return cellWidth * horizon; }

        // total height of the grid
        public float GetGridHeight() { return cellHeight * vertical; }

        // euler quaternion of grid gameObject
        public Quaternion GetQuaternionEuler() {return Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z); }

        public int GetCellCount()
        {
            return horizon * vertical;
        }

        // recalculate cell matrix data
        public void UpdateCellMatrix()
        {
            if (gridSeparateData != null)
                groupDatas = gridSeparateData.GetGroupDatas();

            vertices = CalculateVertices();
            normal = CalculateNormal();

            InitializeArray();

            for (int i = 0; i < vertical; i++)
            {
                for (int j = 0; j < horizon; j++)
                {
                    //calibrate by rotation
                    Vector3 cellCenter = GetCellCenter(i, j);

                    //name index suffix
                    int index = (i * horizon) + j;
                    string groupName = defaultGroupName;

                    foreach (var groupData in groupDatas)
                    {
                        bool isInGroup = false;
                        foreach (var groupIndex in groupData.indices)
                        {
                            if (groupIndex == index)
                            {
                                isInGroup = true;
                                groupName = groupData.groupName;
                                break;
                            }
                        }

                        if (isInGroup)
                            break;
                    }

                    cellArray[i][j] = new GridCell(groupName, cellCenter, GetNormal(), GetCellVertices(i, j, cellWidth, cellHeight), cellWidth, cellHeight, index, j, i);
                }
            }
        }

        // returns vertices of the grid. always returns 4 values with matrix order
        public Vector3[] GetVertices()
        {
            return vertices;
        }

        // returns normal of the grid.
        public Vector3 GetNormal()
        {
            return normal;
        }

        public bool TryGet(int index, out GridCell cell)
        {
            int vertical = (int)(index / this.horizon);
            int horizon = index - (vertical * this.horizon);

            return TryGet(vertical,horizon,out cell);
        }

        public bool TryGet(int vertical, int horizon, out GridCell cell)
        {
            cell = new GridCell();

            if (horizon < 0 || vertical < 0 || vertical > cellArray.Length-1 || horizon > cellArray[0].Length-1)
                return false;

            cell = cellArray[vertical][horizon];
            return true;
        }

        public bool IsInAnyGrid(Vector3 point, out GridCell cell)
        {
            cell = new GridCell();

            Vector3 ab = vertices[1] - vertices[0];
            Vector3 ac = vertices[2] - vertices[0];
            Vector3 ap = point - vertices[0];

            float dotABAP = Vector3.Dot(ab, ap);
            float abMag = ab.sqrMagnitude;
            float dotACAP = Vector3.Dot(ac, ap);
            float acMag = ac.sqrMagnitude;

            if (dotABAP < 0 || dotABAP > abMag || dotACAP < 0 || dotACAP > acMag)
                return false;

            Vector3 projectAB = Vector3.Project(ap, vertices[1] - vertices[0]);
            Vector3 projectAC = Vector3.Project(ap, ac);

            float rowMag = projectAB.magnitude;
            float colMag = projectAC.magnitude;

            int horizon = (int)(rowMag / cellWidth);
            int vertical = (int)(colMag / cellHeight);

            return TryGet(vertical, horizon, out cell);
        }
/*
        public bool IsInAnyGrid(Vector3 point, float height, float width, out GridCell[] cells)
        {
            cells = null;

            GridCell cell;
            if (!IsInAnyGrid(point, out cell))
                return false;

            int maxRowDiff = (int)((width * 0.5f) / cellWidth);
            int maxColDiff = (int)((height * 0.5f) / cellHeight);

            int originRow = cell.rowIndex - maxRowDiff;
            int originCol = cell.columnIndex - maxColDiff;


        }
*/
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

        // compare two cell values
        public bool CompareCell(GridCell x, GridCell y)
        {
            return x.center == y.center &&
            x.verticalIndex == y.verticalIndex &&
            x.horizontalIndex == y.horizontalIndex &&
            x.height == y.height &&
            x.width == y.width &&
            x.normal == y.normal;
        }

        public void SetGroupDatas(GroupData[] groupDatas)
        {
            if (groupDatas == null || groupDatas.Length <= 0)
                return;

            this.groupDatas = groupDatas;

            foreach (var group in groupDatas)
                foreach (var index in group.indices)
                    SetGroupNameOfCell(index, group.groupName);
        }

        public void SeparateCell(string groupID,int index)
        {
            if(groupDatas != null)
                foreach (var group in groupDatas)
                {
                    if (group.groupName == groupID)
                    {
                        if (group.indices == null)
                            group.SetIndices(new List<int>());

                        bool contains = false;

                        foreach (var separateIndex in group.indices)
                        {
                            if (separateIndex == index)
                                contains = true;
                        }

                        if (!contains)
                        {
                            group.indices.Add(index);

                            SetGroupNameOfCell(index, group.groupName);
                        }

                    }
                    else
                    {
                        if (group.indices.Contains(index))
                            group.indices.Remove(index);
                    }
                }
        }

        public void ReconnectCell(string id, int index)
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
                            foreach (var separateIndex in group.indices)
                            {
                                if (separateIndex == index)
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

        public GroupData[] GetGroups()
        {
            return groupDatas;
        }

        // returns vertices of the cell. always returns 4 values with matrix order
        private Vector3[] GetCellVertices(int verticalIndex,int horizontalIndex, float width,float height)
        {
            Vector3 cellCenter = GetCellCenterRaw(verticalIndex, horizontalIndex);

            float xMin = cellCenter.x - (width / 2.0f);
            float xMax = cellCenter.x + (width / 2.0f);
            float yMin = cellCenter.y - (height / 2.0f);
            float yMax = cellCenter.y + (height / 2.0f);

            Quaternion quaternion = GetQuaternionEuler();

            Vector3 a = quaternion * new Vector3(xMin, yMax, cellCenter.z);
            Vector3 b = quaternion * new Vector3(xMax, yMax, cellCenter.z);
            Vector3 c = quaternion * new Vector3(xMin, yMin, cellCenter.z);
            Vector3 d = quaternion * new Vector3(xMax, yMin, cellCenter.z);

            Vector3[] vertices = new Vector3[] { a,b,c,d };

            return vertices;
        }

        // uncalibrated center of the cell 
        private Vector3 GetCellCenterRaw(int verticalIndex, int horizontalIndex)
        {
            Vector3 defaultCenter = GetCalibratedCenter() + new Vector3(GetMinimalOffsetX(), GetMaximumOffsetY());

            Vector3 buffer = new Vector3(cellWidth * 0.5f, -cellHeight * 0.5f);

            defaultCenter = defaultCenter + buffer;
                
            Vector3 cellCenter = defaultCenter + new Vector3(horizontalIndex * cellWidth, verticalIndex * -cellHeight);

            return cellCenter;
        }

        // calibrated center of the cell 
        private Vector3 GetCellCenter(int verticalIndex, int horizontalIndex)
        {
            return GetQuaternionEuler() * GetCellCenterRaw(verticalIndex, horizontalIndex);
        }

   
        // initialize cell array
        private void InitializeArray()
        {
            cellArray = new GridCell[vertical][];
            for (int i = 0; i < cellArray.Length; i++)
                cellArray[i] = new GridCell[horizon];
        }

        private void SetGroupNameOfCell(int index, string groupName)
        {
            GridCell cell;

            if (!TryGet(index, out cell))
            {
                Debug.LogError("No cell found... Try update grid");
                return;
            }

            cell.groupID = groupName;
        }

        // minimal x-axis value of grid
        private float GetMinimalOffsetX()
        {
            return -GetGridWidth() * 0.5f;
        }

        // maximum x-axis value of grid
        private float GetMaximumOffsetX()
        {
            return GetGridWidth() * 0.5f;
        }

        // minimal y-axis value of grid
        private float GetMinimalOffsetY()
        {
            return -GetGridHeight() * 0.5f;
        }

        // maximum y-axis value of grid
        private float GetMaximumOffsetY()
        {
            return GetGridHeight() * 0.5f;
        }

        private Vector3 GetCalibratedCenter() 
        { 
            return Quaternion.Inverse(GetQuaternionEuler()) * transform.position; 
        }

        private Vector3[] CalculateVertices()
        {
            Vector3 calibratedCenter = GetCalibratedCenter();

            float xMin = calibratedCenter.x - (GetGridWidth() / 2.0f);
            float xMax = calibratedCenter.x + (GetGridWidth() / 2.0f);
            float yMin = calibratedCenter.y - (GetGridHeight() / 2.0f);
            float yMax = calibratedCenter.y + (GetGridHeight() / 2.0f);

            Quaternion quaternion = GetQuaternionEuler();

            Vector3 a = quaternion * new Vector3(xMin, yMax, calibratedCenter.z);
            Vector3 b = quaternion * new Vector3(xMax, yMax, calibratedCenter.z);
            Vector3 c = quaternion * new Vector3(xMin, yMin, calibratedCenter.z);
            Vector3 d = quaternion * new Vector3(xMax, yMin, calibratedCenter.z);

            Vector3[] vertices = new Vector3[] { a, b, c, d };

            return vertices;
        }

        private Vector3 CalculateNormal()
        {
            Vector3 a = vertices[0];
            Vector3 b = vertices[1];
            Vector3 c = vertices[2];

            Vector3 side1 = b - a;
            Vector3 side2 = c - a;

            Vector3 perp = Vector3.Cross(side1, side2);

            return perp.normalized;
        }
   

#if UNITY_EDITOR
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

        private void OnDrawGizmos()
        {
            if (!drawGizmos)
                return;

            if (cellArray == null || cellArray.Length == 0)
                UpdateCellMatrix();

            Vector3 calibratedCenter = GetCalibratedCenter();

            Quaternion quaternion = GetQuaternionEuler();

            for (int x = 0; x < vertical + 1; x++)
            {
                if (x == 0 || x == vertical)
                    Gizmos.color = gizmoLineColor;
                else
                    Gizmos.color = Color.Lerp(Color.white, gizmoLineColor, 0.6f);

                float y = GetMaximumOffsetY() - (x * cellHeight);

                Vector3 pos1 = calibratedCenter + new Vector3(GetMinimalOffsetX(), y, 0);
                Vector3 pos2 = calibratedCenter + new Vector3(GetMaximumOffsetX(), y, 0);

                pos1 = quaternion * pos1;

                pos2 = quaternion * pos2;

                Gizmos.DrawLine(pos1, pos2);
            }

            for (int y = 0; y < horizon + 1; y++)
            {
                if (y == 0 || y == horizon)
                    Gizmos.color = gizmoLineColor;
                else
                    Gizmos.color = Color.Lerp(Color.white, gizmoLineColor, 0.6f);

                float x = GetMinimalOffsetX() + (y * cellWidth);

                Vector3 pos1 = calibratedCenter + new Vector3(x, GetMaximumOffsetY(), 0);
                Vector3 pos2 = calibratedCenter + new Vector3(x, GetMinimalOffsetY(), 0);

                pos1 = quaternion * pos1;

                pos2 = quaternion * pos2;

                Gizmos.DrawLine(pos1,pos2);
            }

            float range = cellWidth + cellHeight;

            range *= 0.5f;

            Vector3 dir = GetNormal() * range;

            if (drawNormal)
            {
                Gizmos.color = Color.Lerp(Color.black, gizmoLineColor, 0.4f);

                Gizmos.DrawLine(transform.position, transform.position + dir);
            }

            Vector3[] vertices = GetVertices();

            if (drawVertex)
                for (int i = 0; i < vertices.Length; i++)
                {
                    Gizmos.DrawSphere(vertices[i], range * 0.1f);
                    UnityEditor.Handles.Label(vertices[i], "vertex " + i);
                }

            for (int i = 0; i < cellArray.Length; i++)
                for (int j = 0; j < cellArray[i].Length; j++)
                {
                    GridCell cell = cellArray[i][j];

                    if (string.IsNullOrEmpty(cell.groupID) || cell.groupID == defaultGroupName)
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

                            Vector3 ab = (b - a) / (cellLineCount);
                            Vector3 cb = (b - c) / (cellLineCount);

                            for (int k = 1; k < cellLineCount; k++)
                            {
                                Vector3 from = a + (ab * k);
                                Vector3 to = c + (cb * k);

                                Gizmos.DrawLine(from, to);
                            }
                        }
                    }

                    if (drawNormal)
                        Gizmos.DrawRay(cell.center, cell.normal);

                    if (drawCellName)
                        UnityEditor.Handles.Label(cell.center, cell.index.ToString());
                }
        }
#endif

    }
}