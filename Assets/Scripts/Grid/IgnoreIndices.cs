using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    [CreateAssetMenu(menuName = "GodUnityPlugin/Grid/IgnoreIndices")]
    public class IgnoreIndices : ScriptableObject
    {
        [SerializeField]private List<int> indices = new List<int>();

        public List<int> GetIndices()
        {
            return indices;
        }

        public void SetIndices(List<int> indices)
        {
            this.indices = indices;
        }
    }
}
