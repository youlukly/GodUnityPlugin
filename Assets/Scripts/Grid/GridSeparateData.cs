using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    [CreateAssetMenu(menuName = "GodUnityPlugin/Grid/GridSeparateData")]
    public class GridSeparateData : ScriptableObject
    {
        [SerializeField] private GUPGrid.GroupData[] separatedGroups;

        public GUPGrid.GroupData[] GetGroupDatas()
        {
            return separatedGroups;
        }

        public void SaveGroups(GUPGrid.GroupData[] groupDatas)
        {
            separatedGroups = new GUPGrid.GroupData[groupDatas.Length];

            groupDatas.CopyTo(separatedGroups,0);
        }

    }
}
