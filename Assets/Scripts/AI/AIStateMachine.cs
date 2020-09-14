using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GodUnityPlugin
{
    public class AIStateMachine : GUPStateMachine
    {
        public bool log = true;

        public PatternBase currentPattern { private set; get; }

        private List<PatternBase> origins = new List<PatternBase>();

        private Dictionary<PatternBase, float> currentPriority = new Dictionary<PatternBase, float>();

        private int priorityMult;

        private int priorityMid;

        public void Init(List<PatternBase> patterns)
        {
            SortPriorityOrigins(patterns);

            InitializePriority(origins);

            priorityMult = GetPriorityMult(origins);

            priorityMid = GetPriorityMid(origins);
        }

        public void Init(List<PatternBase> patterns, string initState)
        {
            SortPriorityOrigins(patterns);

            InitializePriority(origins);

            priorityMult = GetPriorityMult(origins);

            priorityMid = GetPriorityMid(origins);

            TransitionToState(initState);
        }

        public void Refresh()
        {
            List<PatternBase> patterns = new List<PatternBase>();

            foreach (var patternPair in currentPriority)
                patterns.Add(patternPair.Key);

            foreach (var pattern in patterns)
            {
                if (!pattern.IsExecutable())
                {
                    AddPriority(pattern);
                    continue;
                }

                StartPattern(pattern);

                return;
            }

            if (log)
                Debug.Log("cannot refresh pattern : no available pattern");
        }

        public void TransitionToPattern(string id)
        {
            PatternBase next = Get(id);

            if (next == null)
                Debug.LogError("no pattern detected : " + id);

            List<PatternBase> patterns = new List<PatternBase>();

            foreach (var patternPair in currentPriority)
                patterns.Add(patternPair.Key);

            foreach (var item in patterns)
            {
                if (item == next)
                    continue;

                AddPriority(item);
            }
       
            StartPattern(next);
        }

        public PatternBase GetCurrentPattern()
        {
            foreach (var pattern in currentPriority)
            {
                if (!pattern.Key.isRunning)
                    continue;

                return pattern.Key;
            }

            return null;
        }

        public void OnReset()
        {
            InitializePriority(origins);
        }

        public new PatternBase Get(string id)
        {
            foreach (var item in currentPriority)
            {
                if (!item.Key.id.Equals(id))
                    continue;

                return item.Key;
            }

            return null;
        }

        public void RegisterEnterEvent(System.Action call)
        {
            foreach (var pattern in origins)
                pattern.RegisterEnterEvent(call);
        }

        public void RegisterExitEvent(System.Action call)
        {
            foreach (var pattern in origins)
                pattern.RegisterExitEvent(call);
        }

        public void RegisterUpdateEvent(System.Action call)
        {
            foreach (var pattern in origins)
                pattern.RegisterUpdateEvent(call);
        }

        public void RegisterFixedUpdateEvent(System.Action call)
        {
            foreach (var pattern in origins)
                pattern.RegisterFixedUpdateEvent(call);
        }

        public void RegisterLateUpdateEvent(System.Action call)
        {
            foreach (var pattern in origins)
                pattern.RegisterLateUpdateEvent(call);
        }

        public void DeRegisterEnterEvent(System.Action call)
        {
            foreach (var pattern in origins)
                pattern.DeRegisterEnterEvent(call);
        }

        public void DeRegisterExitEvent(System.Action call)
        {
            foreach (var pattern in origins)
                pattern.DeRegisterExitEvent(call);
        }

        public void DeRegisterUpdateEvent(System.Action call)
        {
            foreach (var pattern in origins)
                pattern.DeRegisterUpdateEvent(call);
        }

        public void DeRegisterFixedUpdateEvent(System.Action call)
        {
            foreach (var pattern in origins)
                pattern.DeRegisterFixedUpdateEvent(call);
        }

        public void DeRegisterLateUpdateEvent(System.Action call)
        {
            foreach (var pattern in origins)
                pattern.DeRegisterLateUpdateEvent(call);
        }

        public override void ManualUpdate()
        {
            base.ManualUpdate();
            foreach (var pattern in currentPriority)
            {
                if (pattern.Key.isRunning)
                    continue;

                pattern.Key.Cooldown(Time.deltaTime);
            }
        }

        private void StartPattern(PatternBase next)
        {
            SubtractPriority(next);

            SortByPriority(currentPriority);

            if (currentPattern != null)
                Debug.Log("Previous Pattern : " + currentPattern.id + ", Next Pattern : " + next.id);
            else
                Debug.Log("Next Pattern : " + next.id);

            TransitionToState(next.id);
            currentPattern = next;
        }

        private void SortPriorityOrigins(List<PatternBase> patterns)
        {
            List<PatternBase> unsorted = new List<PatternBase>();

            foreach (var pattern in patterns)
                unsorted.Add(pattern);

            int count = unsorted.Count;

            for (int i = 0; i < count; i++)
            {
                PatternBase highest = GetHighestPriority(unsorted);
                Add(highest);
                unsorted.Remove(highest);
                origins.Add(highest);
            }
        }

        private void InitializePriority(List<PatternBase> origins)
        {
            currentPriority.Clear();

            foreach (var item in origins)
                currentPriority.Add(item, item.GetPriority());
        }

        private void SortByPriority(Dictionary<PatternBase, float> currentPriority)
        {
            Dictionary<PatternBase, float> unsorted = new Dictionary<PatternBase, float>();

            foreach (var pattern in currentPriority)
                unsorted.Add(pattern.Key, pattern.Value);

            currentPriority.Clear();

            int count = unsorted.Count;

            for (int i = 0; i < count; i++)
            {
                PatternBase highest = GetHighestPriority(unsorted);
                currentPriority.Add(highest, unsorted[highest]);
                unsorted.Remove(highest);
            }
        }

        private PatternBase GetHighestPriority(List<PatternBase> patterns)
        {
            int max = 0;

            PatternBase result = null;

            foreach (var pattern in patterns)
            {
                if (pattern.GetPriority() < max)
                    continue;

                max = pattern.GetPriority();

                result = pattern;
            }

            return result;
        }

        private PatternBase GetHighestPriority(Dictionary<PatternBase, float> patterns)
        {
            float max = 0;

            PatternBase result = null;

            foreach (var pattern in patterns)
            {
                if (pattern.Value < max)
                    continue;

                max = pattern.Value;

                result = pattern.Key;
            }

            return result;
        }

        private int GetPriorityMult(List<PatternBase> patterns)
        {
            int count = patterns.Count - 1;

            List<int> priorities = new List<int>();

            foreach (var item in patterns)
                priorities.Add(item.GetPriority());

            int[] differs = new int[count];

            for (int i = 0; i < count; i++)
            {
                int differ = priorities[i] - priorities[i + 1];
                differs[i] = differ;
            }

            int sum = 0;

            for (int i = 0; i < differs.Length; i++)
                sum += differs[i];

            return sum / differs.Length;
        }

        private int GetPriorityMid(List<PatternBase> patterns)
        {
            int sum = 0;

            foreach (var item in patterns)
                sum += item.GetPriority();

            return sum / patterns.Count;
        }

        private void AddPriority(PatternBase pattern)
        {
            float p = currentPriority[pattern];

            p += priorityMult * (pattern.GetPriority() / priorityMid);

            p = Mathf.Clamp(p, 0, pattern.GetPriority());

            currentPriority[pattern] = p;
        }

        private void SubtractPriority(PatternBase pattern)
        {
            float p = currentPriority[pattern];

            p -= priorityMid;

            p = Mathf.Clamp(p, 0, pattern.GetPriority());

            currentPriority[pattern] = p;
        }
    }
}