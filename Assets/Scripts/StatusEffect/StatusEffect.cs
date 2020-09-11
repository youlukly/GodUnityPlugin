using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    public abstract class StatusEffect
    {
        public abstract string id { get; }

        public void StartEffect()
        {
            OnStartEffect();
        }

        public void UpdateEffect()
        {
            OnUpdateEffect();
        }

        public void FinishEffect()
        {
            OnFinishEffect();
        }

        protected abstract void OnStartEffect();

        protected abstract void OnUpdateEffect();

        protected abstract void OnFinishEffect();
    }
}