using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    public class GUPStateMachine : MonoBehaviour
    {
        private List<IGUPState> states = new List<IGUPState>();

        public IGUPState current { get; private set; }

        public void Init(List<IGUPState> states)
        {
            this.states = states;
        }

        public void Init(List<IGUPState> states, string id)
        {
            this.states = states;

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

        public void TransitionToState(string id)
        {
            IGUPState next = Get(id);
            Debug.Assert(next != null);

            if (current != null)
                current.OnExit();

            current = next;

            current.OnEnter();
        }

        protected virtual void Update()
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

        protected virtual void LateUpdate()
        {
            if (current == null)
                return;

            current.OnLateUpdate();
        }

        protected virtual void FixedUpdate()
        {
            if (current == null)
                return;

            current.OnFixedUpdate();
        }
    }
}