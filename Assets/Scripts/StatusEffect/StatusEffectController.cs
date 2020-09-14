using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    public class StatusEffectController
    {
        public StatusEffectUnityEvent onStartEffect = new StatusEffectUnityEvent();
        public StatusEffectUnityEvent onUpdateEffect = new StatusEffectUnityEvent();
        public StatusEffectUnityEvent onFinishEffect = new StatusEffectUnityEvent();

        public float updateSpeedMult { get; set; } = 1.0f;

        protected StatusEffect[] effects;
        protected List<StatusEffect> currentEffects = new List<StatusEffect>();
        protected Dictionary<string, float> remainTimePairs = new Dictionary<string, float>();

        public StatusEffectController(params StatusEffect[] effects)
        {
            this.effects = effects;
        }

        public void ManualUpdate()
        {
            for (int i = 0; i < currentEffects.Count; i++)
            {
                StatusEffect effect = currentEffects[i];

                effect.UpdateEffect();
                if (onUpdateEffect != null)
                    onUpdateEffect.Invoke(effect);

                if (remainTimePairs.ContainsKey(effect.id))
                {
                    remainTimePairs[effect.id] -= Time.deltaTime * updateSpeedMult;

                    if (remainTimePairs[effect.id] <= 0.0f)
                        RemoveEffect(effect.id);
                }
            }
        }

        public void AddEffect(string id)
        {
            if (!Contains(id))
                return;

            StatusEffect effect = Get(id);

            currentEffects.Add(effect);
            effect.StartEffect();
            if (onStartEffect != null)
                onStartEffect.Invoke(effect);
        }

        public void AddEffect(string id, float duration)
        {
            if (!Contains(id))
                return;

            AddEffect(id);

            StatusEffect effect = Get(id);

            if (!remainTimePairs.ContainsKey(effect.id))
                remainTimePairs.Add(effect.id, duration);
            else
                remainTimePairs[effect.id] = duration;
        }

        public void SetDuration(string id, float duration)
        {
            if (!IsEffectedBy(id))
                return;

            if (!remainTimePairs.ContainsKey(id))
                return;

            remainTimePairs[id] = duration;
        }

        public void AddDuration(string id, float duration)
        {
            if (!IsEffectedBy(id))
                return;

            if (!remainTimePairs.ContainsKey(id))
                return;

            remainTimePairs[id] += duration;
        }

        public void RemoveEffect(string id)
        {
            if (!IsEffectedBy(id))
                return;

            StatusEffect effect = GetCurrent(id);

            effect.FinishEffect();
            if (onFinishEffect != null)
                onFinishEffect.Invoke(effect);
            ClearEffect(id);
        }

        public void RemoveAllEffects()
        {
            for (int i = 0; i < currentEffects.Count; i++)
            {
                StatusEffect effect = currentEffects[i];
                effect.FinishEffect();
                if (onFinishEffect != null)
                    onFinishEffect.Invoke(effect);
            }

            ClearAllEffects();
        }

        public void ClearAllEffects()
        {
            currentEffects.Clear();
            remainTimePairs.Clear();
        }

        public bool IsEffectedBy(string id)
        {
            foreach (var statusEffect in currentEffects)
            {
                if (statusEffect.id == id)
                    return true;
            }

            return false;
        }

        public StatusEffect GetCurrent(string id)
        {
            foreach (var statusEffect in currentEffects)
            {
                if (statusEffect.id == id)
                    return statusEffect;
            }

            return null;
        }

        public float GetRemain(string id)
        {
            if (!IsEffectedBy(id) || !remainTimePairs.ContainsKey(id))
                return 0f;

            return remainTimePairs[id];
        }
        
        private StatusEffect Get(string id)
        {
            foreach (var statusEffect in effects)
            {
                if (statusEffect.id == id)
                    return statusEffect;
            }

            return null;
        }

        private void ClearEffect(string id)
        {
            if (!IsEffectedBy(id))
                return;

            if (remainTimePairs.ContainsKey(id))
                remainTimePairs.Remove(id);

            currentEffects.Remove(GetCurrent(id));
        }

        private bool Contains(string id)
        {
            foreach (var statusEffect in effects)
            {
                if (statusEffect.id == id)
                    return true;
            }

            return false;
        }
    }
}