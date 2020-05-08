using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GodUnityPlugin;

public class GridMover : MonoBehaviour
{
    public GUPGridSelector gridSelector;
    public GUPGrid grid;

    public GUPGridCell origin;

    private float buffer = 0.0f;

    private void Awake()
    {
        buffer = transform.position.y;

        gridSelector.onDownCell.AddListener(OnDownCell);
        gridSelector.onDrag.AddListener(OnDrag);
        gridSelector.onUp.AddListener(OnUp);
    }

    private void OnDownCell(GUPGridCell cell)
    {
        SetGameObjectPosToCell(cell);
    }

    private void OnDrag(Vector3 point)
    {
        Vector3 heightBuffer = grid.normal * buffer;

        transform.position = point + heightBuffer;
    }

    private void OnUp(Vector3 point)
    {
        GUPGridCell cell;

        if (grid.IsInCell(point, out cell))
            SetGameObjectPosToCell(cell);
        else
            SetGameObjectPosToOrigin();
    }

    private void SetGameObjectPosToCell(GUPGridCell cell)
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
