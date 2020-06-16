using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    public class StatusEffectController : MonoBehaviour
    {
        public float updateSpeedMult { get; set; } = 1.0f;

        protected List<StatusEffect> effects = new List<StatusEffect>();
        protected Dictionary<StatusEffect, float> remainTimePairs = new Dictionary<StatusEffect, float>();

        public void AddEffect(StatusEffect effect)
        {
            if (effects.Contains(effect))
                return;

            effects.Add(effect);
            effect.OnStartEffect();
        }

        public void AddEffect(StatusEffect effect, float duration)
        {
            if (effects.Contains(effect))
                return;

            AddEffect(effect);

            if (!remainTimePairs.ContainsKey(effect))
                remainTimePairs.Add(effect, duration);
            else
                remainTimePairs[effect] = duration;
        }

        public void SetDuration<T>(float duration) where T : StatusEffect
        {
            if (!IsEffected<T>())
                return;

            if (remainTimePairs.ContainsKey(Get<T>()))
                return;

            remainTimePairs[Get<T>()] = duration;
        }

        public void AddDuration<T>(float duration) where T : StatusEffect
        {
            if (!IsEffected<T>())
                return;

            if (remainTimePairs.ContainsKey(Get<T>()))
                return;

            remainTimePairs[Get<T>()] += duration;
        }

        public void RemoveEffect<T>() where T : StatusEffect
        {
            if (!IsEffected<T>())
                return;

            RemoveEffect(Get<T>());
        }

        public void RemoveAllEffects()
        {
            effects.Clear();
            remainTimePairs.Clear();
        }

        public void RemoveAllEffects<T>() where T : StatusEffect
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (!effects[i].GetType().Equals(typeof(T)))
                    continue;

                RemoveEffect(effects[i]);
            }
        }

        public bool IsEffected<T>() where T : StatusEffect
        {
            foreach (var effect in effects)
            {
                if (!effect.GetType().Equals(typeof(T)))
                    continue;

                return true;
            }

            return false;
        }

        public T Get<T>() where T : StatusEffect
        {
            foreach (var effect in effects)
            {
                if (!effect.GetType().Equals(typeof(T)))
                    continue;

                return effect as T;
            }

            return null;
        }

        public float GetRemain<T>() where T : StatusEffect
        {
            if (!IsEffected<T>() || !remainTimePairs.ContainsKey(Get<T>()))
                return 0f;

            return remainTimePairs[Get<T>()];
        }

        private void Update()
        {
            foreach (var effect in effects)
            {
                effect.OnUpdateEffect();

                if (remainTimePairs.ContainsKey(effect))
                {
                    remainTimePairs[effect] += Time.deltaTime * updateSpeedMult;

                    if(remainTimePairs[effect] <= 0.0f)
                        RemoveEffect(effect);
                }
            }
        }

        private void RemoveEffect(StatusEffect effect)
        {
            if (!effects.Contains(effect))
                return;

            effect.OnFinishEffect();

            if (remainTimePairs.ContainsKey(effect))
                remainTimePairs.Remove(effect);
            effects.Remove(effect);
        }
    }
}