using UnityEngine;
using UnityEngine.Events;

// serializable unity event that returns Vector3
namespace GodUnityPlugin
{
    [System.Serializable]
    public class Vector2UnityEvent : UnityEvent<Vector2> { }
}