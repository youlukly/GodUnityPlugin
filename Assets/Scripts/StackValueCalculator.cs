using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    public class StackValueCalculator
    {
        private Dictionary<string, Dictionary<string, float>> stackValuePairs = new Dictionary<string, Dictionary<string, float>>();

        public StackValueCalculator(params string[] initialStackValues)
        {
            foreach (var initialStackValue in initialStackValues)
                stackValuePairs.Add(initialStackValue, new Dictionary<string, float>());
        }

        public void AddStackValue(string id, string effectName, float value)
        {
            if (!stackValuePairs.ContainsKey(id))
                stackValuePairs.Add(id, new Dictionary<string, float>());

            if (!stackValuePairs[id].ContainsKey(effectName))
                stackValuePairs[id].Add(effectName, value);

            stackValuePairs[id][effectName] = value;
        }

        public void RemoveStackValue(string id, string effectName)
        {
            if (!stackValuePairs.ContainsKey(id))
                return;

            if (!stackValuePairs[id].ContainsKey(effectName))
                return;

            stackValuePairs[id].Remove(effectName);
        }

        public void RemoveStackValue(string id)
        {
            if (!stackValuePairs.ContainsKey(id))
                return;

            stackValuePairs.Remove(id);
        }

        public void RemoveAllStackValues(string id)
        {
            stackValuePairs[id].Clear();
        }

        public void RemoveAllStackValues()
        {
            stackValuePairs.Clear();
        }

        public bool TryGetArithmeticValue(string id, out float value)
        {
            value = 0.0f;

            if (!stackValuePairs.ContainsKey(id))
                return false;

            foreach (var stackValuePair in stackValuePairs[id])
                value += stackValuePair.Value;

            return true;
        }

        public bool TryGetMultiplicationValue(string id, out float value)
        {
            value = 0.0f;

            if (!stackValuePairs.ContainsKey(id))
                return false;

            foreach (var stackValuePair in stackValuePairs[id])
                value *= stackValuePair.Value;

            return true;
        }

        public bool TryGetArithmeticValue(string id, float defaultValue, out float value)
        {
            value = defaultValue;

            if (!stackValuePairs.ContainsKey(id))
                return false;

            foreach (var stackValuePair in stackValuePairs[id])
                value += stackValuePair.Value;

            return true;
        }

        public bool TryGetMultiplicationValue(string id, float defaultValue, out float value)
        {
            value = defaultValue;

            if (!stackValuePairs.ContainsKey(id))
                return false;

            foreach (var stackValuePair in stackValuePairs[id])
                value += value * stackValuePair.Value;

            return true;
        }
    }
}