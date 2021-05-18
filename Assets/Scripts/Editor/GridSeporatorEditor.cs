﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GodUnityPlugin;

namespace GodUnityPlugin
{
    [CustomEditor(typeof(GridSeperator))]
    public class GridSeporatorEditor : Editor
    {
        private void OnSceneGUI()
        {
            GridSeperator seperator = target as GridSeperator;

            if (seperator == null || seperator.gameObject == null)
                return;

            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.A)
            {
                GridCell selected;

                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

                RaycastHit[] hits = Physics.RaycastAll(ray);

                if (CheckGridCell(seperator, hits, out selected))
                {
                    Debug.Log("added ignore cell : " + selected.id);
                    seperator.AddIgnoreCellIndices(selected.index);
                }
            }
            else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.R)
            {
                GridCell selected;

                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

                RaycastHit[] hits = Physics.RaycastAll(ray);

                if (CheckGridCell(seperator, hits, out selected))
                {
                    Debug.Log("removed ignore cell : " + selected.id);
                    seperator.RemoveIgnoreCellIndices(selected.index);
                }
            }
        }

        private bool CheckGridCell(GridSeperator seperator, RaycastHit[] hits, out GridCell cell)
        {
            cell = new GridCell();

            Vector3 point;

            if (!CheckGridPoint(seperator, hits, out point))
                return false;

            return seperator.grid.IsInGrid(point, out cell, false);
        }

        private bool CheckGridPoint(GridSeperator seperator, RaycastHit[] hits, out Vector3 point)
        {
            point = new Vector3();

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != null && hit.collider == seperator.boxCollider)
                {
                    point = hit.point;
                    return true;
                }
            }

            return false;
        }
    }
}