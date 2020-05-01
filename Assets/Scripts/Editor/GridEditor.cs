using UnityEngine;
using UnityEditor;

namespace GodUnityPlugin
{
    [CustomEditor(typeof(Grid))]
    public class GridEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Grid grid = target as Grid;

            if(!grid.autoCellUpdate)
            if (GUILayout.Button("Update Cell Matrix Manually"))
                grid.UpdateCellMatrix();
        }

        //// draw the grid :) 
        //private void CustomOnSceneGUI(SceneView sceneview)
        //{
        //    Grid grid = target as Grid;

        //    Handles.matrix = grid.transform.localToWorldMatrix;

        //    Handles.color = grid.gizmoLineColor;

        //    // draw the horizontal lines
        //    for (int x = 0; x < grid.AbsCellCountY + 1; x++)
        //    {
        //        if (x == 0 || x == grid.AbsCellCountY)
        //            Gizmos.color = grid.gizmoLineColor;
        //        else
        //            Gizmos.color = Color.Lerp(Color.white, grid.gizmoLineColor, 0.5f);

        //        float y = grid.yMax - (x * grid.cellHeight);

        //        Vector3 pos1 = new Vector3(grid.xMin, y, 0) * grid.gridScale;
        //        Vector3 pos2 = new Vector3(grid.xMax, y, 0) * grid.gridScale;

        //        Handles.DrawLine((grid.gridOffset + pos1), (grid.gridOffset + pos2));
        //    }

        //    // draw the horizontal lines
        //    for (int y = 0; y < grid.AbsCellCountX + 1; y++)
        //    {
        //        if (y == 0 || y == grid.AbsCellCountX)
        //            Gizmos.color = grid.gizmoLineColor;
        //        else
        //            Gizmos.color = Color.Lerp(Color.white, grid.gizmoLineColor, 0.5f);


        //        float x = grid.xMin + (y * grid.cellWidth);

        //        Vector3 pos1 = new Vector3(x, grid.yMax, 0) * grid.gridScale;
        //        Vector3 pos2 = new Vector3(x, grid.yMin, 0) * grid.gridScale;

        //        Handles.DrawLine((grid.gridOffset + pos1), (grid.gridOffset + pos2));
        //    }
        //}
    }
}