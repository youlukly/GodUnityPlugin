using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    [CreateAssetMenu(menuName = "GodUnityPlugin/Grid/GridSeperateData")]
    public class GridSeperateData : ScriptableObject
    {
        [SerializeField] private List<GUPGrid.GroupData> seperatedGroups;

        public List<GUPGrid.GroupData> GetGroupDatas()
        {
            return seperatedGroups;
        }

        public List<int> GetIndices(string id)
        {
            if(seperatedGroups != null)
                foreach (var data in seperatedGroups)
                {
                    if (data.groupName == id)
                        return data.indices;
                }

            return null;
        }

        public void SaveGroups(List<GUPGrid.GroupData> groupDatas)
        {
            this.seperatedGroups = groupDatas;
        }

    /*    public void SaveGroup(string id, List<int> indices)
        {
            if (seperatedGroups != null)
                foreach (var data in seperatedGroups)
                {
                    if (data.groupName == id)
                    {
                        data.SetIndices(indices);
                        return;
                    }
                }

            if (seperatedGroups == null)
                seperatedGroups = new List<GUPGrid.GroupData>();

            seperatedGroups.Add(new GUPGrid.GroupData(id, indices));
        }*/
    }
}
