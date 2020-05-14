using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    public class StatusEffect
    {
        public string[] effectType;

        private Dictionary<string, string> effectPairs = new Dictionary<string, string>();

        public void AddEffect(string id, string type)
        {
            if (effectPairs.ContainsKey(id))
                return;

            effectPairs.Add(id, type);
        }

        public void RemoveEffect(string id)
        {
            if (!effectPairs.ContainsKey(id))
                return;

            effectPairs.Remove(id);
        }

        public void RemoveAllEffects()
        {
            effectPairs.Clear();
        }

        public void RemoveAllEffects(string type)
        {
            if (!effectPairs.ContainsValue(type))
                return;

            Dictionary<string, string> newPairs = new Dictionary<string, string>();

            foreach (var effectPair in effectPairs)
            {
                if (effectPair.Value == type)
                    continue;

                newPairs.Add(effectPair.Key, effectPair.Value);
            }

            effectPairs = newPairs;
        }

        public bool IsEffecedByType(string type)
        {
            return effectPairs.ContainsValue(type);
        }

        public bool IsEffectedByID(string id)
        {
            return effectPairs.ContainsKey(id);
        }
    }
}