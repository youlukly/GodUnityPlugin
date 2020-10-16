using UnityEngine;
using UnityEngine.Events;

// serializable unity event that returns bool
namespace GodUnityPlugin
{
    [System.Serializable]
    public class BoolUnityEvent : UnityEvent<bool> { }
}