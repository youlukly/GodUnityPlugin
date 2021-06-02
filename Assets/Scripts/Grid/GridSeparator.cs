using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GodUnityPlugin;
using UnityEditor;

namespace GodUnityPlugin
{
    public class GridSeparator : MonoBehaviour
    {
        public GUPGrid grid;
        public BoxCollider boxCollider;

        public GridSeparateData separateGridData;

        public float selectAreaWidth = 1.0f;
        public float selectAreaHeight = 1.0f;

        [SerializeField] private bool drawGizmos = true;

        private int currentIndex = 0;

        public string GetCurrentGroupName()
        {
            GUPGrid.GroupData[] groupDatas = separateGridData.GetGroupDatas();

            if (groupDatas == null || groupDatas.Length <= currentIndex || currentIndex < 0)
                return null;

            return groupDatas[currentIndex].groupName;
        }

        public void TargetNextGroup()
        {
            currentIndex++;

            GUPGrid.GroupData[] groupDatas = separateGridData.GetGroupDatas();

            if (groupDatas == null || groupDatas.Length <= 0)
            {
                Debug.Log("no group data found");
                return;
            }

            if (groupDatas.Length <= currentIndex || currentIndex < 0)
                currentIndex = 0;

            Debug.Log("editor target group : " + groupDatas[currentIndex].groupName);
        }

        public void TrySeparateCell(string id, int index)
        {
            if (separateGridData == null)
                return;

            grid.SeparateCell(id, index);
            separateGridData.SaveGroups(grid.GetGroups());

#if UNITY_EDITOR
            EditorUtility.SetDirty(separateGridData);
#endif
        }

        public void TryReconnectCell(string id, int index)
        {
            if (separateGridData == null)
                return;

            grid.ReconnectCell(id, index);
            separateGridData.SaveGroups(grid.GetGroups());

#if UNITY_EDITOR
            EditorUtility.SetDirty(separateGridData);
#endif
        }

        public void UpdateData()
        {
            if (separateGridData != null)
            {
                grid.SetGroupDatas(separateGridData.GetGroupDatas());
            }
        }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!drawGizmos)
                return;

            if (separateGridData == null)
                return;

            GUPGrid.GroupData[] groupDatas = separateGridData.GetGroupDatas();

            if (groupDatas == null || groupDatas.Length <= 0)
                return;

            Gizmos.color = groupDatas[currentIndex].editorColor;

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            RaycastHit[] hits = Physics.RaycastAll(ray);

            Vector3 point = Vector3.zero;

            bool allowHit = false;

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != null && hit.collider == boxCollider)
                {
                    point = hit.point;
                    allowHit = true;
                }
            }

            if (!allowHit)
                return;

            float mag = Mathf.Sqrt((grid.cellHeight * grid.cellHeight) + (grid.cellWidth * grid.cellWidth)) * 0.5f;

            Vector3[] vertices = grid.GetVertices();

            Vector3 a = point + (vertices[0] - grid.transform.position).normalized * mag;
            Vector3 b = point + (vertices[1] - grid.transform.position).normalized * mag;
            Vector3 c = point + (vertices[2] - grid.transform.position).normalized * mag;
            Vector3 d = point + (vertices[3] - grid.transform.position).normalized * mag;
            
            Gizmos.DrawLine(a, d);
            Gizmos.DrawLine(b, c);

            UnityEditor.Handles.Label(point + Vector3.up, groupDatas[currentIndex].indices.Count.ToString());
        }
#endif
    }
}