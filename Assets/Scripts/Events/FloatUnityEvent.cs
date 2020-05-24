using UnityEngine;
using UnityEngine.Events;

// serializable unity event that returns float
namespace GodUnityPlugin
{
    [System.Serializable]
    public class FloatUnityEvent : UnityEvent<float> { }
}