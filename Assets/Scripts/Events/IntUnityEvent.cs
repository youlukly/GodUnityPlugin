using UnityEngine;
using UnityEngine.Events;

// serializable unity event that returns int
namespace GodUnityPlugin
{
    [System.Serializable]
    public class IntUnityEvent : UnityEvent<int> { }
}