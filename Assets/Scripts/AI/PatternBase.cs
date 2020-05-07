using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GodUnityPlugin
{
    public abstract class PatternBase : IGUPState
    {
        private float remainCoolTime = 0.0f;
        private float randomCooldown = 0.0f;

        private PatternData data;

        public bool isRunning { private set; get; }

        private List<System.Func<bool>> conditions = new List<System.Func<bool>>();

        private System.Action onEnterPattern;
        private System.Action onExitPattern;
        private System.Action onUpdatePattern;
        private System.Action onFixedUpdatePattern;
        private System.Action onLateUpdatePattern;

        public string id => data.ID;

        public PatternBase(PatternData data)
        {
            this.data = data;
            randomCooldown = Random.Range(data.CooldownMin, data.CooldownMax);
            remainCoolTime = 0.0f;
        }

        #region PatternBase

        public bool IsTransition(out string ID)
        {
            ID = null;

            return false;
        }

        public bool IsExecutable()
        {
            if (remainCoolTime > 0.0f)
                return false;

            foreach (var condition in conditions)
                if (!condition.Invoke())
                    return false;

            return IsAvailable();
        }

        public virtual void Cooldown(float value)
        {
            if (remainCoolTime <= 0.0f)
            {
                remainCoolTime = 0.0f;
                return;
            }

            remainCoolTime -= value;
        }

        public int GetPriority()
        {
            return data.Priority;
        }

        public void RegisterCondition(System.Func<bool> condition)
        {
            conditions.Add(condition);
        }

        public void DeRegisterCondition(System.Func<bool> condition)
        {
            conditions.Remove(condition);
        }

        public void RegisterEnterEvent(System.Action call)
        {
            onEnterPattern += call;
        }

        public void RegisterExitEvent(System.Action call)
        {
            onExitPattern += call;
        }

        public void RegisterUpdateEvent(System.Action call)
        {
            onUpdatePattern += call;
        }

        public void RegisterFixedUpdateEvent(System.Action call)
        {
            onFixedUpdatePattern += call;
        }

        public void RegisterLateUpdateEvent(System.Action call)
        {
            onLateUpdatePattern += call;
        }

        public void DeRegisterEnterEvent(System.Action call)
        {
            onEnterPattern -= call;
        }

        public void DeRegisterExitEvent(System.Action call)
        {
            onExitPattern -= call;
        }

        public void DeRegisterUpdateEvent(System.Action call)
        {
            onUpdatePattern -= call;
        }

        public void DeRegisterFixedUpdateEvent(System.Action call)
        {
            onFixedUpdatePattern -= call;
        }

        public void DeRegisterLateUpdateEvent(System.Action call)
        {
            onLateUpdatePattern -= call;
        }

        protected abstract bool IsAvailable();

        protected abstract void OnEnterPattern();

        protected abstract void OnExitPattern();

        protected abstract void OnFixedUpdatePattern();

        protected abstract void OnLateUpdatePattern();

        protected abstract void OnUpdatePattern();

        public void OnEnter()
        {
            if (onEnterPattern != null)
                onEnterPattern.Invoke();
            Execute();
            OnEnterPattern();
        }

        public void OnFixedUpdate()
        {
            if (onFixedUpdatePattern != null)
                onFixedUpdatePattern.Invoke();
            OnFixedUpdatePattern();
        }

        public void OnUpdate()
        {
            if (onUpdatePattern != null)
                onUpdatePattern.Invoke();
            OnUpdatePattern();
        }

        public void OnLateUpdate()
        {
            if (onLateUpdatePattern != null)
                onLateUpdatePattern.Invoke();
            OnLateUpdatePattern();
        }

        public void OnExit()
        {
            if (onExitPattern != null)
                onExitPattern.Invoke();
            Escape();
            OnExitPattern();
        }

        private void Execute()
        {
            if (isRunning)
                return;

            isRunning = true;
            randomCooldown = Random.Range(data.CooldownMin, data.CooldownMax);
            remainCoolTime = randomCooldown;
        }

        private void Escape()
        {
            if (!isRunning)
                return;

            isRunning = false;
        }

        #endregion
    }
}