using UnityEngine;
using UnityEngine.Events;

// serializable unity event that returns string
namespace GodUnityPlugin
{
    [System.Serializable]
    public class StringUnityEvent : UnityEvent<string> { }
}