using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin.Examples
{
    public class GridMover : MonoBehaviour
    {
        public GridSelector gridSelector;
        public GodUnityPlugin.GUPGrid grid;

        public GridCell origin;

        private float buffer = 0.0f;

        private void Awake()
        {
            buffer = transform.position.y;

            gridSelector.onDownCell.AddListener(OnDownCell);
            gridSelector.onDrag.AddListener(OnDrag);
            gridSelector.onUp.AddListener(OnUp);
        }

        private void OnDownCell(GridCell cell)
        {
            SetGameObjectPosToCell(cell);
        }

        private void OnDrag(Vector3 point)
        {
            Vector3 heightBuffer = grid.GetNormal() * buffer;

            transform.position = point + heightBuffer;
        }

        private void OnUp(Vector3 point)
        {
            GridCell cell;

            if (grid.IsInGrid(point, out cell))
                SetGameObjectPosToCell(cell);
            else
                SetGameObjectPosToOrigin();
        }

        private void SetGameObjectPosToCell(GridCell cell)
        {
            Vector3 heightBuffer = cell.normal * buffer;

            transform.position = cell.center + heightBuffer;
            transform.rotation = grid.transform.rotation;

            origin = cell;
        }

        private void SetGameObjectPosToOrigin()
        {
            SetGameObjectPosToCell(origin);
        }
    }
}