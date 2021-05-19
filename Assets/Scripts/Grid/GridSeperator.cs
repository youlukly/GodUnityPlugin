using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GodUnityPlugin;

namespace GodUnityPlugin
{
    public class GridSeperator : MonoBehaviour
    {
        public GUPGrid grid;
        public BoxCollider boxCollider;

        public GridSeperateData seperateGridData;

        private int currentIndex = 0;

        private void Awake()
        {
            SetData();
        }

        private void Reset()
        {
            SetData();
        }

        private void OnValidate()
        {
            SetData();
        }

        public string GetCurrentGroupName()
        {
            List<GUPGrid.GroupData> groupDatas = seperateGridData.GetGroupDatas();

            if (groupDatas == null || groupDatas.Count <= currentIndex || currentIndex < 0)
                return null;

            return groupDatas[currentIndex].groupName;
        }

        public void TargetNextGroup()
        {
            currentIndex++;

            List<GUPGrid.GroupData> groupDatas = seperateGridData.GetGroupDatas();

            if (groupDatas == null || groupDatas.Count <= 0)
            {
                Debug.Log("no group data found");
                return;
            }

            if (groupDatas.Count <= currentIndex || currentIndex < 0)
                currentIndex = 0;

            Debug.Log("editor target group : " + groupDatas[currentIndex].groupName);
        }

        public void AddIgnoreCellIndices(string id, int index)
        {
            if (seperateGridData == null)
                return;

            grid.AddSeperateIndex(id, index);
            seperateGridData.SaveGroups(grid.GetGroups());
        }

        public void RemoveIgnoreCellIndices(string id, int index)
        {
            if (seperateGridData == null)
                return;

            grid.RemoveIgnoreCellIndices(id, index);
            seperateGridData.SaveGroups(grid.GetGroups());
        }

        private void SetData()
        {
            if (seperateGridData != null)
            {
                grid.SetGroupDatas(seperateGridData.GetGroupDatas());
            }
        }
    }
}