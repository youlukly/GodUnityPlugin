using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    public class StatusEffectController : MonoBehaviour
    {
        public StatusEffectUnityEvent onStartEffect = new StatusEffectUnityEvent();
        public StatusEffectUnityEvent onUpdateEffect = new StatusEffectUnityEvent();
        public StatusEffectUnityEvent onFinishEffect = new StatusEffectUnityEvent();

        public float updateSpeedMult { get; set; } = 1.0f;
        
        protected List<StatusEffect> effects = new List<StatusEffect>();
        protected Dictionary<StatusEffect, float> remainTimePairs = new Dictionary<StatusEffect, float>();

        public void AddEffect(StatusEffect effect)
        {
            if (effects.Contains(effect))
                return;

            effects.Add(effect);
            effect.StartEffect();
            if (onStartEffect != null)
                onStartEffect.Invoke(effect);
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

        public void SetDuration(StatusEffect effect, float duration)
        {
            if (!IsEffectedBy(effect))
                return;

            if (!remainTimePairs.ContainsKey(effect))
                return;

            remainTimePairs[effect] = duration;
        }

        public void AddDuration(StatusEffect effect, float duration)
        {
            if (!IsEffectedBy(effect))
                return;

            if (remainTimePairs.ContainsKey(effect))
                return;

            remainTimePairs[effect] += duration;
        }

        public void RemoveEffect(StatusEffect effect)
        {
            if (!IsEffectedBy(effect))
                return;

            effect.FinishEffect();
            ClearEffect(effect);
        }

        public void RemoveAllEffects()
        {
            for (int i = 0; i < effects.Count; i++)
            {
                StatusEffect effect = effects[i];
                RemoveEffect(effects[i]);
            }
        }

        public void RemoveAllEffects<T>() where T : StatusEffect
        {
            if (!IsEffectedBy<T>())
                return;

            for (int i = 0; i < effects.Count; i++)
            {
                if (!effects[i].GetType().Equals(typeof(T)))
                    continue;

                RemoveEffect(effects[i]);
            }
        }

        //public void ClearAllEffects()
        //{
        //    effects.Clear();
        //    remainTimePairs.Clear();
        //}

        //public void ClaerAllEffects<T>() where T : StatusEffect
        //{
        //    if (!IsEffectedBy<T>())
        //        return;

        //    for (int i = 0; i < effects.Count; i++)
        //    {
        //        if (!effects[i].GetType().Equals(typeof(T)))
        //            continue;

        //        ClearEffect(effects[i]);
        //    }
        //}

        public bool IsEffectedBy(StatusEffect effect)
        {
            return effects.Contains(effect);
        }

        public bool IsEffectedBy<T>() where T : StatusEffect
        {
            foreach (var effect in effects)
            {
                if (!effect.GetType().Equals(typeof(T)))
                    continue;

                return true;
            }

            return false;
        }

        public List<T> GetActiveEffects<T>() where T : StatusEffect
        {
            List<T> results = new List<T>();

            foreach (var effect in effects)
            {
                if (!effect.GetType().Equals(typeof(T)))
                    continue;

                T result = effect as T;

                results.Add(result);
            }

            return results;
        }

        public float GetRemain(StatusEffect effect)
        {
            if (!IsEffectedBy(effect) || !remainTimePairs.ContainsKey(effect))
                return 0f;

            return remainTimePairs[effect];
        }

        private void Update()
        {
            for (int i = 0; i < effects.Count; i++)
            {
                StatusEffect effect = effects[i];

                effect.UpdateEffect();
                if (onUpdateEffect != null)
                    onUpdateEffect.Invoke(effect);

                if (remainTimePairs.ContainsKey(effect))
                {
                    remainTimePairs[effect] -= Time.deltaTime * updateSpeedMult;

                    if(remainTimePairs[effect] <= 0.0f)
                        RemoveEffect(effect);
                }
            }
        }

        private void ClearEffect(StatusEffect effect)
        {
            if (!IsEffectedBy(effect))
                return;

            if (remainTimePairs.ContainsKey(effect))
                remainTimePairs.Remove(effect);

            effects.Remove(effect);
        }
    }
}