using UnityEngine;
using UnityEngine.Events;

// serializable unity event that returns object
namespace GodUnityPlugin
{
    [System.Serializable]
    public class ObjectUnityEvent : UnityEvent<object> { }
}