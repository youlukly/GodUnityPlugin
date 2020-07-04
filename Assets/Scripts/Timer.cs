using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GodUnityPlugin
{
    public class Timer : MonoBehaviour
    {
        public float globalTimerSpeedMult { get; set; } = 1f;

        private Dictionary<string, float> timerPair = new Dictionary<string, float>();

        public void StartTimer(string key)
        {
            StartTimer(key,0.0f);
        }

        public void StartTimer(string key,float value)
        {
            if (timerPair.ContainsKey(key))
                return;

            timerPair.Add(key, value);
        }

        public void SetTimer(string key,float value)
        {
            if (!timerPair.ContainsKey(key))
                StartTimer(key, value);
            else
                timerPair[key] = value;
        }

        public void AddTime(string key, float value)
        {
            if (!timerPair.ContainsKey(key))
                return;

            timerPair[key] += value;
        }

        public float GetTimer(string key)
        {
            if (!timerPair.ContainsKey(key))
            {
                StartTimer(key);
                return 0.0f;
            }

            return timerPair[key];
        }

        public bool TryGetTimer(string key,out float time)
        {
            time = 0.0f;

            if (!timerPair.ContainsKey(key))
                return false;

            time = timerPair[key];

            return true;
        }

        public void RemoveTimer(string key)
        {
            if (!timerPair.ContainsKey(key))
                return;

            timerPair.Remove(key);
        }

        public bool IsElapsed(string key,float time)
        {
            if (!timerPair.ContainsKey(key))
                return false;

            return timerPair[key] >= time;
        }

        private void Update()
        {
            if (timerPair.Count <= 0)
                return;

            foreach (var pair in timerPair.Keys.ToList())
                timerPair[pair] += Time.deltaTime * globalTimerSpeedMult;
        }

    }
}