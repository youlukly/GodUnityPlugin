using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin.Examples
{
    public class PickUp : MonoBehaviour
    {
        public Renderer meshRenderer;

        private const float pickUpTimer = 5.0f;

        private float currentTime = 0.0f;

        private void OnTriggerEnter(Collider other)
        {
            PickerAI ai = other.GetComponent<PickerAI>();

            if (!ai)
                return;

            gameObject.SetActive(false);
            ai.currentTarget = null;

            currentTime = 0.0f;
        }

        public void Regenerate()
        {
            if (gameObject.activeSelf)
                return;

            currentTime += Time.deltaTime;

            if (currentTime >= pickUpTimer)
            {
                gameObject.SetActive(true);
                currentTime = 0.0f;
            }
        }
    }
}