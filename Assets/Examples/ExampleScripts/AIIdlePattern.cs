using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin.Examples
{
    public class AIIdlePattern : PatternBase
    {
        private PickerAI ai;

        private const float idleTime = 1.0f;

        private float currentkTime = 0.0f;

        public AIIdlePattern(PatternData data, PickerAI ai) : base(data)
        {
            this.ai = ai;
        }

        protected override bool IsAvailable()
        {
            return true;
        }

        protected override void OnEnterPattern()
        {
            currentkTime = 0.0f;
        }

        protected override void OnExitPattern()
        {
        }

        protected override void OnFixedUpdatePattern()
        {
        }

        protected override void OnLateUpdatePattern()
        {
        }

        protected override void OnUpdatePattern()
        {
            currentkTime += Time.deltaTime;

            if (currentkTime >= idleTime)
                ai.Refresh();
        }
    }
}