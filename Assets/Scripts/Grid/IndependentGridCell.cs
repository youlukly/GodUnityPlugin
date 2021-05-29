using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    [RequireComponent(typeof(BoxCollider))]
    public class IndependentGridCell : MonoBehaviour
    {
        [SerializeField] private string id = "cell";
        [SerializeField] private float width = 2.0f;
        [SerializeField] private float height = 2.0f;
        [SerializeField] private bool drawGizmos = true;
        [SerializeField] private Color gizmoLineColor = new Color(0.25f, 0.1f, 0.25f, 1f);

        private BoxCollider box;
        private GridCell cell;

        private void OnValidate()
        {
            SynchronizeCell();
        }

        private void Reset()
        {
            cell = new GridCell();

            SynchronizeCell();
        }

        private void Awake()
        {
            SynchronizeCell();
        }

        public GridCell GetCell()
        {
            return cell;
        }

        public bool IsInCell(Vector3 point)
        {
            return cell.IsInCell(point);
        }

        private void SynchronizeCell()
        {
            if (box == null)
                box = GetComponent<BoxCollider>();

            box.isTrigger = true;

            Bounds bounds = new Bounds(transform.position, new Vector2(width,height));

            box.size = bounds.size;

            Quaternion euler = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);

            cell.center = transform.position;
            cell.normal = GetNormal();
            cell.vertices = GetVertices();
            cell.width = width;
            cell.height = height;
            cell.rowIndex = -1;
            cell.columnIndex = -1;
        }

        // returns vertices of the cell. always returns 4 values with matrix order
        private Vector3[] GetVertices()
        {
            Quaternion quaternionEuler = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);

            Quaternion inverse = Quaternion.Inverse(quaternionEuler);

            Vector3 cellCenter = quaternionEuler * transform.position;

            float xMin = cellCenter.x - (width / 2.0f);
            float xMax = cellCenter.x + (width / 2.0f);
            float yMin = cellCenter.y - (height / 2.0f);
            float yMax = cellCenter.y + (height / 2.0f);

            Vector3 a = inverse * new Vector3(xMin, yMax, cellCenter.z);
            Vector3 b = inverse * new Vector3(xMax, yMax, cellCenter.z);
            Vector3 c = inverse * new Vector3(xMin, yMin, cellCenter.z);
            Vector3 d = inverse * new Vector3(xMax, yMin, cellCenter.z);

            Vector3[] vertices = new Vector3[] { a, b, c, d };

            return vertices;
        }

        private Vector3 GetNormal()
        {
            Vector3[] vert = GetVertices();

            Vector3 a = vert[0];
            Vector3 b = vert[1];
            Vector3 c = vert[2];

            Vector3 side1 = b - a;
            Vector3 side2 = c - a;

            Vector3 perp = Vector3.Cross(side1, side2);

            return perp.normalized;
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!drawGizmos)
                return;

            Gizmos.color = Color.Lerp(Color.black, gizmoLineColor, 0.4f);

            Gizmos.DrawLine(cell.vertices[0], cell.vertices[1]);
            Gizmos.DrawLine(cell.vertices[1], cell.vertices[3]);
            Gizmos.DrawLine(cell.vertices[3], cell.vertices[2]);
            Gizmos.DrawLine(cell.vertices[2], cell.vertices[0]);

            Gizmos.DrawLine(cell.vertices[0], cell.vertices[3]);
            Gizmos.DrawLine(cell.vertices[1], cell.vertices[2]);
            Gizmos.DrawRay(cell.center, cell.normal);
            UnityEditor.Handles.Label(cell.center, cell.index.ToString());
        }
#endif
    }
}