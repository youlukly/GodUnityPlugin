using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

namespace GodUnityPlugin
{
    [RequireComponent(typeof(Grid))]
    public class GridSelector : MonoBehaviour
    {
        public bool allowInput = true;
        public bool autoSynchronize = false;

        public float edgeBuffer = 3.0f;

        // grid cell unity events, respond with user input
        [Header("Events")]
        public GridCellUnityEvent onDownCell;
        public Vector3UnityEvent onDown;
        public GridCellUnityEvent onDragCell;
        public Vector3UnityEvent onDrag;
        public GridCellUnityEvent onUpCell;
        public Vector3UnityEvent onUp;
        public GridCellUnityEvent onOverCell;
        public Vector3UnityEvent onOver;

        [Header("Gizmos")]
        [SerializeField] private bool drawGizmos = true;
        public Color gizmoDownColor = Color.green;
        public Color gizmoDragColor = Color.yellow;
        public Color gizmoUpColor = Color.white;
        public Color gizmoOverColor = Color.gray;
        public Color gizmoFailColor = Color.red;

        private Grid grid;
        private BoxCollider gridBox;

        public void SynchronizeCollider()
        {
            Bounds bounds = new Bounds(grid.center, new Vector2(grid.Width, grid.Height));

            gridBox.size = bounds.size;
        }

        private void Awake()
        {
            grid = GetComponent<Grid>();

            gridBox = GetComponent<BoxCollider>();
            if (gridBox == null)
                gridBox = gameObject.AddComponent<BoxCollider>();
        }

        private void Update()
        {
            if (!allowInput)
                return;

            if (autoSynchronize)
                SynchronizeCollider();

            GridCell selected;
            Vector3 point;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            bool isInCell = false;

            if (Input.GetMouseButtonDown(0))
            {
                if (CheckGridCell(hits, out selected))
                {
                    onDownCell.Invoke(selected);
                    DrawGizmoCell(selected, gizmoDownColor, 0.75f);
                    isInCell = true;
                }

                if (CheckGridPoint(hits, out point))
                {
                    onDown.Invoke(point);

                    Color pointColor = isInCell ? gizmoDownColor : gizmoFailColor;

                    DrawGizmoPoint(point, Color.Lerp(pointColor, Color.black, 0.35f), 0.5f);
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if (CheckGridCell(hits, out selected))
                {
                    onDragCell.Invoke(selected);
                    DrawGizmoCell(selected, gizmoDragColor);
                    isInCell = true;
                }

                if (CheckGridPoint(hits, out point))
                {
                    onDrag.Invoke(point);

                    Color pointColor = isInCell ? gizmoDragColor : gizmoFailColor;

                    DrawGizmoPoint(point, Color.Lerp(pointColor, Color.black, 0.35f));
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (CheckGridCell(hits, out selected))
                {
                    onUpCell.Invoke(selected);
                    DrawGizmoCell(selected, gizmoUpColor, 0.75f);
                    isInCell = true;
                }

                if (CheckGridPoint(hits, out point))
                {
                    onUp.Invoke(point);

                    Color pointColor = isInCell ? gizmoUpColor : gizmoFailColor;

                    DrawGizmoPoint(point, Color.Lerp(pointColor, Color.black, 0.35f), 0.5f);
                }
            }
            else 
            {
                if (CheckGridCell(hits, out selected))
                {
                    onOverCell.Invoke(selected);
                    DrawGizmoCell(selected, gizmoUpColor);
                    isInCell = true;
                }

                if (CheckGridPoint(hits, out point))
                {
                    onOver.Invoke(point);

                    Color pointColor = isInCell ? gizmoOverColor : gizmoFailColor;

                    DrawGizmoPoint(point, Color.Lerp(pointColor, Color.black, 0.35f));
                }
            }
        }

        private bool CheckGridCell(RaycastHit[] hits, out GridCell cell)
        {
            cell = new GridCell();

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != null && hit.collider == gridBox)
                {
                    if (grid.IsInCell(hit.point, out cell))
                        return true;
                }
            }

            return false;
        }

        private bool CheckGridPoint(RaycastHit[] hits, out Vector3 point)
        {
            point = new Vector3();

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != null && hit.collider == gridBox)
                {
                    point = hit.point;
                    return true;
                }
            }

            return false;
        }

        private void OnSelect(GridCell cell)
        {
            Debug.Log("select cell : name [" + cell.id + "], matrix [" + cell.columnIndex + "], [" + cell.rowIndex + "]");
        }

#if UNITY_EDITOR
        private void DrawGizmoCell(GridCell cell, Color color)
        {
            if (!drawGizmos)
                return;

            Vector3[] vertices = GetGizmoVertices(cell);

            Debug.DrawLine(vertices[0], vertices[3], color);
            Debug.DrawLine(vertices[1], vertices[2], color);
        }

        private void DrawGizmoCell(GridCell cell, Color color, float duration)
        {
            if (!drawGizmos)
                return;

            Vector3[] vertices = GetGizmoVertices(cell);

            Debug.DrawLine(vertices[0], vertices[3], color, duration);
            Debug.DrawLine(vertices[1], vertices[2], color, duration);
        }

        private void DrawGizmoPoint(Vector3 point, Color color)
        {
            if (!drawGizmos)
                return;

            Vector3[] vertices = GetGizmoVertices(point);

            Debug.DrawLine(vertices[0], vertices[3], color);
            Debug.DrawLine(vertices[1], vertices[2], color);
        }

        private void DrawGizmoPoint(Vector3 point, Color color, float duration)
        {
            if (!drawGizmos)
                return;

            Vector3[] vertices = GetGizmoVertices(point);

            Debug.DrawLine(vertices[0], vertices[3], color, duration);
            Debug.DrawLine(vertices[1], vertices[2], color, duration);
        }

        private Vector3[] GetGizmoVertices(GridCell cell)
        {
            Vector3 a = cell.vertices[0];
            Vector3 b = cell.vertices[1];
            Vector3 c = cell.vertices[2];
            Vector3 d = cell.vertices[3];

            return new Vector3[] { a, b, c, d };
        }

        private Vector3[] GetGizmoVertices(Vector3 point)
        {
            Vector3 a = grid.vertices[0];
            Vector3 b = grid.vertices[1];
            Vector3 c = grid.vertices[2];
            Vector3 d = grid.vertices[3];

            float mag = Mathf.Clamp(grid.AbsCellWidth / 5f, grid.AbsCellWidth / 5f, grid.AbsCellHeight / 5f);

            Vector3 dir1 = (a - d).normalized * mag;
            Vector3 dir2 = (b - c).normalized * mag;

            Vector3 pA = point + dir1;
            Vector3 pB = point + dir2;
            Vector3 pC = point - dir2;
            Vector3 pD = point - dir1;

            return new Vector3[] { pA, pB, pC, pD };
        }
#endif

    }
}