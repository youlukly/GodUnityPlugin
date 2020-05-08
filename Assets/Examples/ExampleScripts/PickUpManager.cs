using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin.Examples
{
    public class PickUpManager : MonoBehaviour
    {
        public PickUp[] pickUps;
        public float pickUpRegenTime = 2.0f;

        public List<PickUp> GetActivePickUps()
        {
            List<PickUp> results = new List<PickUp>();

            foreach (PickUp pickUp in pickUps)
            {
                if (!pickUp.gameObject.activeSelf)
                    continue;

                results.Add(pickUp);
            }

            return results;
        }

        private void Awake()
        {
        }

        private void Update()
        {
            for (int i = 0; i < pickUps.Length; i++)
                pickUps[i].Regenerate();
        }
    }
}   