using UnityEngine;

namespace GodUnityPlugin.Examples
{
    public class AIMovePattern : PatternBase
    {
        private PickerAI ai;

        private Rigidbody rigidbody;

        private const float force = 3.0f;

        public AIMovePattern(PatternData data, PickerAI ai) : base(data)
        {
            this.ai = ai;
            rigidbody = ai.GetComponent<Rigidbody>();
        }

        protected override bool IsAvailable()
        {
            return ai.currentTarget;
        }

        protected override void OnEnterPattern()
        {
        }

        protected override void OnExitPattern()
        {
            rigidbody.velocity = Vector3.zero;
        }

        protected override void OnFixedUpdatePattern()
        {
            if (!ai.currentTarget)
            {
                ai.Refresh();
                return;
            }

            Vector3 direction = (ai.currentTarget.transform.position - ai.transform.position).normalized;

            rigidbody.AddForce(direction * force);
        }

        protected override void OnLateUpdatePattern()
        {
        }

        protected override void OnUpdatePattern()
        {
        }
    }
}