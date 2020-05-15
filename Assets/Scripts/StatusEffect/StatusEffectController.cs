using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GodUnityPlugin
{
    public class StatusEffectController : MonoBehaviour
    {
        protected Dictionary<string, IStatusEffect> effectPairs = new Dictionary<string, IStatusEffect>();

        public void AddEffect(string id,IStatusEffect effect)
        {
            if (effectPairs.ContainsKey(id))
                return;

            effectPairs.Add(id, effect);
            effect.OnStartEffect();
        }

        public void AddEffect(string id, IStatusEffect effect,bool overlap)
        {
            if (!overlap && effectPairs.ContainsValue(effect))
                return;

            AddEffect(id, effect);
        }

        public void RemoveEffect(string id)
        {
            if (!effectPairs.ContainsKey(id))
                return;

            effectPairs[id].OnFinishEffect();
            effectPairs.Remove(id);
        }

        public void RemoveAllEffects()
        {
            foreach (var pairKey in effectPairs.Keys.ToList())
            {
                effectPairs[pairKey].OnFinishEffect();
                effectPairs.Remove(pairKey);
            }
        }

        public void RemoveAllEffects(IStatusEffect effect)
        {
            if (!effectPairs.ContainsValue(effect))
                return;

            foreach (var pairKey in effectPairs.Keys.ToList())
            {
                if (effectPairs[pairKey] != effect)
                    continue;

                effectPairs[pairKey].OnFinishEffect();
                effectPairs.Remove(pairKey);
            }
        }

        public bool IsEffected(IStatusEffect effect)
        {
            return effectPairs.ContainsValue(effect);
        }

        public bool IsEffected(string id)
        {
            return effectPairs.ContainsKey(id);
        }

        public IStatusEffect Get(string id)
        {
            if (!IsEffected(id))
                return null;

            return effectPairs[id];
        }

        public List<IStatusEffect> Get(IStatusEffect effect)
        {
            if (!IsEffected(effect))
                return null;

            List<IStatusEffect> results = new List<IStatusEffect>();

            foreach (var pairKey in effectPairs.Keys.ToList())
            {
                IStatusEffect currentEffect = effectPairs[pairKey];

                if (currentEffect != effect)
                    continue;

                results.Add(currentEffect);
            }

            return results;
        }

        private void Update()
        {
            foreach (var pairKey in effectPairs.Keys.ToList())
            {
                IStatusEffect effect = effectPairs[pairKey];

                effect.OnUpdateEffect();
                if (effect.IsRequireFinish())
                    RemoveEffect(pairKey);
            }
        }
    }
}