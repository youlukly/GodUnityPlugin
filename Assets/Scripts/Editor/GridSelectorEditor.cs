using UnityEngine;
using UnityEditor;

namespace GodUnityPlugin
{
    [CustomEditor(typeof(GridSelector))]
    public class GridSelectorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GridSelector selector = target as GridSelector;

            if (GUILayout.Button("Update SceneView"))
                selector.StartSceneViewUpdate();
        }
    }
}