using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin.Examples
{
    public class PickerAI : MonoBehaviour
    {
        public AIStateMachine aIStateMachine;
        public PickUpManager pickUpManager;

        public PatternData idleData;
        public PatternData moveData;
        public PatternData searchData;

        public float patternMaxTime = 5.0f;

        public Renderer meshRenderer;

        public PickUp currentTarget { get; set; }

        private float currentTime = 0.0f;

        private void Awake()
        {
            AIIdlePattern idle = new AIIdlePattern(idleData, this);
            AIMovePattern move = new AIMovePattern(moveData, this);
            AISearchPattern search = new AISearchPattern(searchData, this);

            List<PatternBase> patterns = new List<PatternBase>();

            patterns.Add(idle);
            patterns.Add(move);
            patterns.Add(search);

            aIStateMachine.Init(patterns, idle.id);
        }

        public PickUp GetNextPickUp()
        {
            List<PickUp> pickUps = pickUpManager.GetActivePickUps();

            if (pickUps.Count == 0)
                return null;

            int ran = Random.Range(0, pickUps.Count - 1);

            return pickUps[ran];
        }

        public void Refresh()
        {
            aIStateMachine.Refresh();
        }

    }
}