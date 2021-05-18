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

        public IgnoreIndices ignoreIndices;

        private void Awake()
        {
            if (ignoreIndices != null)
                grid.SetIgnoreCellIndices(ignoreIndices.GetIndices());
        }

        private void Reset()
        {
            if (ignoreIndices != null)
                grid.SetIgnoreCellIndices(ignoreIndices.GetIndices());
        }

        private void OnValidate()
        {
            if (ignoreIndices != null)
                grid.SetIgnoreCellIndices(ignoreIndices.GetIndices());
        }

        public void AddIgnoreCellIndices(int index)
        {
            if (ignoreIndices == null)
                return;

            grid.AddIgnoreCellIndices(index);
            ignoreIndices.SetIndices(grid.GetIgnoreCellIndices());
        }

        public void RemoveIgnoreCellIndices(int index)
        {
            if (ignoreIndices == null)
                return;

            grid.RemoveIgnoreCellIndices(index);
            ignoreIndices.SetIndices(grid.GetIgnoreCellIndices());
        }
    }
}