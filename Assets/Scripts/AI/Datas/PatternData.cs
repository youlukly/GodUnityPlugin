using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    [CreateAssetMenu(menuName = "GodUnityPlugin/AI/Data/Patterns/Pattern")]
    public class PatternData : ScriptableObject
    {
        [Header("Type")] [SerializeField] private string id;

        [Header("Conditions")] [Range(0, 999)] [SerializeField]
        private int priority;

        [SerializeField] private float cooldownMin;
        [SerializeField] private float cooldownMax;

        public string ID
        {
            get { return id; }
        }

        public int Priority
        {
            get { return priority; }
        }

        public float CooldownMin
        {
            get { return cooldownMin; }
        }

        public float CooldownMax
        {
            get { return cooldownMax; }
        }
    }
}