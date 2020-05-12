using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    public class StatusEffect
    {
        public enum EffectType
        {
            ResistStagger,
            ResistKnockback,
            Burn,
            Frozen,
            Electrocuted,
            Poisoned,
            Defense,
        }

        private Dictionary<string, EffectType> effectPairs = new Dictionary<string, EffectType>();

        public void AddEffect(string effectName, EffectType type)
        {
            if (effectPairs.ContainsKey(effectName))
                return;

            effectPairs.Add(effectName, type);
        }

        public void RemoveEffect(string effectName)
        {
            if (!effectPairs.ContainsKey(effectName))
                return;

            effectPairs.Remove(effectName);
        }

        public void RemoveAllEffects()
        {
            effectPairs.Clear();
        }

        public void RemoveAllEffects(EffectType type)
        {
            if (!effectPairs.ContainsValue(type))
                return;

            Dictionary<string, EffectType> newPairs = new Dictionary<string, EffectType>();

            foreach (var effectPair in effectPairs)
            {
                if (effectPair.Value == type)
                    continue;

                newPairs.Add(effectPair.Key, effectPair.Value);
            }

            effectPairs = newPairs;
        }

        public bool IsEffecedByType(EffectType type)
        {
            return effectPairs.ContainsValue(type);
        }

        public bool IsEffectedByID(string effectName)
        {
            return effectPairs.ContainsKey(effectName);
        }
    }
}