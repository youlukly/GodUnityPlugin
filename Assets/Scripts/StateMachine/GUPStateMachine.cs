using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    public class GUPStateMachine
    {
        private List<IGUPState> states = new List<IGUPState>();

        public IGUPState current { get; private set; }

        public GUPStateMachine()
        {

        }

        public GUPStateMachine(List<IGUPState> states)
        {
            this.states = states;
        }

        public GUPStateMachine(List<IGUPState> states, string initState)
        {
            this.states = states;
            TransitionToState(initState);
        }

        public IGUPState Get(string id)
        {
            foreach (IGUPState state in states)
            {
                if (state.id != id)
                    continue;

                return state;
            }

            return null;
        }

        public void Add(IGUPState state)
        {
            if (states.Contains(state))
                return;

            states.Add(state);
        }

        public void Remove(IGUPState state)
        {
            if (!states.Contains(state))
                return;

            states.Remove(state);
        }

        public void Clear()
        {
            states.Clear();
        }

        public void TransitionToState(string id)
        {
            IGUPState next = Get(id);
            Debug.Assert(next != null);

            if (current != null)
                current.OnExit();

            current = next;

            current.OnEnter();
        }

        public virtual void ManualUpdate()
        {
            if (current == null)
                return;

            current.OnUpdate();

            string id;

            if (current.IsTransition(out id))
            {
                TransitionToState(id);
                return;
            }
        }

        public virtual void ManualLateUpdate()
        {
            if (current == null)
                return;

            current.OnLateUpdate();
        }

        public virtual void ManualFixedUpdate()
        {
            if (current == null)
                return;

            current.OnFixedUpdate();
        }
    }
}