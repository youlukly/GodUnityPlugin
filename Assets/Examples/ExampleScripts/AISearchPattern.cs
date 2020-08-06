using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin.Examples
{
    public class AISearchPattern : PatternBase
    {
        private PickerAI ai;

        public AISearchPattern(PatternData data,PickerAI ai) : base(data)
        {
            this.ai = ai;
        }

        protected override bool IsAvailable()
        {
            return !ai.currentTarget;
        }

        protected override void OnEnterPattern()
        {
            ai.meshRenderer.material = ai.GetComponent<Renderer>().material;
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
            ai.currentTarget = ai.GetNextPickUp();

            if (!ai.currentTarget)
                return;

            ai.Refresh();
            ai.meshRenderer.material = ai.currentTarget.meshRenderer.material;
        }
    }
}