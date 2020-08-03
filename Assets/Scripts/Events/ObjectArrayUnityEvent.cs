using UnityEngine;
using UnityEngine.Events;

// serializable unity event that returns array of object
namespace GodUnityPlugin
{
    [System.Serializable]
    public class ObjectArrayUnityEvent : UnityEvent<object[]> { }
}