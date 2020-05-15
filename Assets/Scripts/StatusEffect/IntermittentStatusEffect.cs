using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    public abstract class IntermittentStatusEffect : IStatusEffect
    {
        public string id => ID;

        protected abstract string ID { get; }

        private Timer timer;
        private float duration = 0.0f;

        private string key;

        public IntermittentStatusEffect(Timer timer,float duration)
        {
            this.timer = timer;
            this.duration = duration;

            key = id + Time.time;
        }

        public virtual void OnStartEffect()
        {
            timer.StartTimer(key);
        }

        public virtual void OnUpdateEffect()
        {
        }

        public virtual void OnFinishEffect()
        {
            timer.RemoveTimer(key);
        }

        public bool IsRequireFinish()
        {
            return duration <= timer.GetTimer(key);
        }
    }
}