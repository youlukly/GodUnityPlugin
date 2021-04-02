using UnityEngine;

namespace GodUnityPlugin
{
    public class GridSelector 
    {
        // grid cell unity events, respond with user input
        public GridCellUnityEvent onDownCell = new GridCellUnityEvent();
        public Vector3UnityEvent onDown = new Vector3UnityEvent();
        public GridCellUnityEvent onDragCell = new GridCellUnityEvent();
        public Vector3UnityEvent onDrag = new Vector3UnityEvent();
        public GridCellUnityEvent onUpCell = new GridCellUnityEvent();
        public Vector3UnityEvent onUp = new Vector3UnityEvent();
        public GridCellUnityEvent onOverCell = new GridCellUnityEvent();
        public Vector3UnityEvent onOver = new Vector3UnityEvent();

        public bool drawGizmos = true;
        public Color gizmoDownColor = Color.green;
        public Color gizmoDragColor = Color.yellow;
        public Color gizmoUpColor = Color.white;
        public Color gizmoOverColor = Color.gray;
        public Color gizmoFailColor = Color.red;

        private Camera camera;
        private BoxCollider boxCollider;

        private GUPGrid grid;

        public GridSelector(GUPGrid grid,BoxCollider boxCollider,Camera camera)
        {
            this.grid = grid;

            this.boxCollider = boxCollider;

            this.camera = camera;
        }

        public void SynchronizeCollider()
        {
            Bounds bounds = new Bounds(grid.Center, new Vector2(grid.Width, grid.Height));

            boxCollider.size = bounds.size;
            boxCollider.isTrigger = true;
        }

        public void ManualUpdate()
        {
            GridCell selected;
            Vector3 point;

            Vector3 mousePos = Input.mousePosition;

            if (camera.orthographicSize == 0 && IsInFrustum(camera,mousePos))
                return;

            Ray ray = camera.ScreenPointToRay(mousePos);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            bool isInCell = false;

            if (Input.GetMouseButtonDown(0))
            {
                if (CheckGridCell(hits, out selected))
                {
                    onDownCell.Invoke(selected);
#if UNITY_EDITOR
                    DrawGizmoCell(selected, gizmoDownColor, 0.75f);
#endif
                    isInCell = true;
                }

                if (CheckGridPoint(hits, out point))
                {
                    onDown.Invoke(point);

                    Color pointColor = isInCell ? gizmoDownColor : gizmoFailColor;

#if UNITY_EDITOR
                    DrawGizmoPoint(point, Color.Lerp(pointColor, Color.black, 0.35f), 0.5f);
#endif
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if (CheckGridCell(hits, out selected))
                {
                    onDragCell.Invoke(selected);
#if UNITY_EDITOR
                    DrawGizmoCell(selected, gizmoDragColor);
#endif
                    isInCell = true;
                }

                if (CheckGridPoint(hits, out point))
                {
                    onDrag.Invoke(point);

                    Color pointColor = isInCell ? gizmoDragColor : gizmoFailColor;

#if UNITY_EDITOR
                    DrawGizmoPoint(point, Color.Lerp(pointColor, Color.black, 0.35f));
#endif
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (CheckGridCell(hits, out selected))
                {
                    onUpCell.Invoke(selected);
#if UNITY_EDITOR
                    DrawGizmoCell(selected, gizmoUpColor, 0.75f);
#endif
                    isInCell = true;
                }

                if (CheckGridPoint(hits, out point))
                {
                    onUp.Invoke(point);

                    Color pointColor = isInCell ? gizmoUpColor : gizmoFailColor;

#if UNITY_EDITOR
                    DrawGizmoPoint(point, Color.Lerp(pointColor, Color.black, 0.35f), 0.5f);
#endif
                }
            }
            else 
            {
                if (CheckGridCell(hits, out selected))
                {
                    onOverCell.Invoke(selected);
#if UNITY_EDITOR
                    DrawGizmoCell(selected, gizmoUpColor);
#endif
                    isInCell = true;
                }

                if (CheckGridPoint(hits, out point))
                {
                    onOver.Invoke(point);

                    Color pointColor = isInCell ? gizmoOverColor : gizmoFailColor;

#if UNITY_EDITOR
                    DrawGizmoPoint(point, Color.Lerp(pointColor, Color.black, 0.35f));
#endif
                }
            }
        }

        private bool CheckGridCell(RaycastHit[] hits, out GridCell cell)
        {
            cell = new GridCell();

            Vector3 point;

            if (!CheckGridPoint(hits, out point))
                return false;

            return grid.IsInGrid(point, out cell);
        }

        private bool CheckGridPoint(RaycastHit[] hits, out Vector3 point)
        {
            point = new Vector3();

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != null && hit.collider == boxCollider)
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

        private bool IsInFrustum(Camera camera,Vector3 position)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(camera);
            foreach (var plane in planes)
            {
                if (plane.GetDistanceToPoint(position) < 0)
                    return false;
            }
            return true;
        }

#if UNITY_EDITOR
        private void DrawGizmoCell(GridCell cell, Color color)
        {
            if (!drawGizmos)
                return;

            Debug.DrawLine(cell.vertices[0], cell.vertices[3], color);
            Debug.DrawLine(cell.vertices[1], cell.vertices[2], color);
        }

        private void DrawGizmoCell(GridCell cell, Color color, float duration)
        {
            if (!drawGizmos)
                return;

            Debug.DrawLine(cell.vertices[0], cell.vertices[3], color, duration);
            Debug.DrawLine(cell.vertices[1], cell.vertices[2], color, duration);
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

            float mag = Mathf.Clamp(grid.CellWidth / 5f, grid.CellWidth / 5f, grid.CellHeight / 5f);

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