using UnityEngine;
using UnityEngine.Events;

// serializable unity event that returns RayCastHits
namespace GodUnityPlugin
{
    [System.Serializable]
    public class RayCastHitsUnityEvent : UnityEvent<RaycastHit[]> { }
}