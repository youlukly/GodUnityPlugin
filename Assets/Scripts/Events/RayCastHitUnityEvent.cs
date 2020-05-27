using UnityEngine;
using UnityEngine.Events;

// serializable unity event that returns RayCastHit
namespace GodUnityPlugin
{
    [System.Serializable]
    public class RayCastHitUnityEvent : UnityEvent<RaycastHit> { }
}