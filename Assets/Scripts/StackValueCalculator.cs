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

        public void AddStackValue(string id, string valueName, float value)
        {
            if (!stackValuePairs.ContainsKey(id))
                stackValuePairs.Add(id, new Dictionary<string, float>());

            if (!stackValuePairs[id].ContainsKey(valueName))
                stackValuePairs[id].Add(valueName, value);

            stackValuePairs[id][valueName] = value;
        }

        public void RemoveStackValue(string id, string valueName)
        {
            if (!stackValuePairs.ContainsKey(id))
                return;

            if (!stackValuePairs[id].ContainsKey(valueName))
                return;

            stackValuePairs[id].Remove(valueName);
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

        public bool TryGetRawValues(string id, out float[] values)
        {
            values = null;

            if (!stackValuePairs.ContainsKey(id))
                return false;

            values = new float[stackValuePairs[id].Count];

            int i = 0;
            foreach (var stackValuePair in stackValuePairs[id])
            {
                values[i] = stackValuePair.Value;
                i++;
            }

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
                value += defaultValue * stackValuePair.Value;

            return true;
        }

        public bool TryGetCompoundMultiplicationValue(string id, float defaultValue, out float value)
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