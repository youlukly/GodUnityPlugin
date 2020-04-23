using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    [CreateAssetMenu(menuName = "VaporWorld/AI/Data/DynamicSight")]
    public class DynamicSightData : ScriptableObject
    {
        [SerializeField] private DynamicSight.Sight[] sights;
        [SerializeField] private float alertTimeMax;
        [SerializeField] private float alertTimeMin;
        [SerializeField] private float alertExitInterval;

        public DynamicSight.Sight[] Sights
        {
            get { return sights; }
        }

        public float AlertTimeMax
        {
            get { return alertTimeMax; }
        }


        public float AlertTimeMin
        {
            get { return alertTimeMin; }
        }


        public float AlertExitInterval
        {
            get { return alertExitInterval; }
        }
    }
}