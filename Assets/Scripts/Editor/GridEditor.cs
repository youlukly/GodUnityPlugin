//using UnityEngine;
//using UnityEditor;

//namespace GodUnityPlugin
//{
//    [CustomEditor(typeof(Grid))]
//    public class GridEditor : Editor
//    {
//        public override void OnInspectorGUI()
//        {
//            DrawDefaultInspector();

//            Grid grid = target as Grid;

//            if (GUILayout.Button("Update Cell Matrix"))
//                grid.UpdateCellMatrix();
//        }
//    }
//}