using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GodUnityPlugin;

namespace GodUnityPlugin
{
    [CustomEditor(typeof(GridSeparator))]
    public class GridSeparatorEditor : Editor
    {
        private void OnSceneGUI()
        {
            GridSeparator separator = target as GridSeparator;

            if (separator == null || separator.gameObject == null)
                return;

            separator.UpdateData();

            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.A)
                {
                    GridCell selected;

                    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

                    RaycastHit[] hits = Physics.RaycastAll(ray);

                    if (CheckGridCell(separator, hits, out selected))
                    {
                        string id = separator.GetCurrentGroupName();

                        if (!string.IsNullOrEmpty(id))
                        {
                            Debug.Log("added cell " + selected.index + " to group : " + id);
                            separator.TrySeparateCell(id, selected.index);
                        }
                    }
                }
                else if (Event.current.keyCode == KeyCode.R)
                {
                    GridCell selected;

                    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

                    RaycastHit[] hits = Physics.RaycastAll(ray);

                    if (CheckGridCell(separator, hits, out selected))
                    {
                        string id = separator.GetCurrentGroupName();

                        if (!string.IsNullOrEmpty(id))
                        {
                            Debug.Log("removed cell " + selected.index + " from group : " + id);
                            separator.TryReconnectCell(id, selected.index);
                        }
                    }
                }
                else if (Event.current.keyCode == KeyCode.N)
                {
                    separator.TargetNextGroup();
                }
            }
        }

        private bool CheckGridCell(GridSeparator separator, RaycastHit[] hits, out GridCell cell)
        {
            cell = new GridCell();

            Vector3 point;

            if (!CheckGridPoint(separator, hits, out point))
                return false;

            return separator.grid.IsInAnyGrid(point, out cell);
        }

        private bool CheckGridPoint(GridSeparator separator, RaycastHit[] hits, out Vector3 point)
        {
            point = new Vector3();

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != null && hit.collider == separator.boxCollider)
                {
                    point = hit.point;
                    return true;
                }
            }

            return false;
        }
    }
}