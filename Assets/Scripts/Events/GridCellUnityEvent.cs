using UnityEngine.Events;

// serializable unity event that returns grid cells
namespace GodUnityPlugin
{
    [System.Serializable]
    public class GridCellUnityEvent : UnityEvent<GridCell> { }
}