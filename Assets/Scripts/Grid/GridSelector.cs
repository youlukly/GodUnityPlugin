using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

namespace GodUnityPlugin
{
    [ExecuteAlways]
    [RequireComponent(typeof(Grid))]
    public class GridSelector : MonoBehaviour
    {
        // which key you should press to select function work
        //public KeyCode selectKey = KeyCode.G;

        public bool allowInput = true;

        // grid cell unity events, respond with user input
        [Header("Events")]
        public GridCellUnityEvent onDown;
        public Vector3UnityEvent onDrag;
        public GridCellUnityEvent onUp;
        public GridCellUnityEvent onOver;

        [Header("Gizmos")]
        [SerializeField] private bool drawGizmos = true;
        public Color gizmoLineColor = new Color(0.65f, 0.65f, 0.25f, 0.75f);

        private Grid grid;
        private BoxCollider gridBox;

        private void Awake()
        {
            grid = GetComponent<Grid>();

            gridBox = GetComponent<BoxCollider>();
            if (gridBox == null)
                gridBox = gameObject.AddComponent<BoxCollider>();
        }

        private void SynchronizeCollider()
        {
            Bounds bounds = new Bounds(grid.center, new Vector2(grid .Width, grid.Height));

            gridBox.size = bounds.size;
        }

        private void DestroyBox()
        {
            if (gridBox != null)
                if (Application.isPlaying)
                    Destroy(gridBox);
                else
                    DestroyImmediate(gridBox);
        }

        private void Update()
        {
            SynchronizeCollider();

            if (!allowInput)
                return;

            GridCell selected;
            Vector3 point;

            if (Input.GetMouseButtonDown(0))
            {
                if (CheckGridCell(out selected))
                    onDown.Invoke(selected);
            }
            else if (Input.GetMouseButton(0))
            {
                if (CheckGridCell(out point))
                    onDrag.Invoke(point);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (CheckGridCell(out selected))
                    onUp.Invoke(selected);
            }
            else
            {
                if (CheckGridCell(out selected))
                    onOver.Invoke(selected);
            }
        }

        private bool CheckGridCell(out GridCell cell)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            cell = new GridCell();

            RaycastHit[] hits = Physics.RaycastAll(ray);

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

        private bool CheckGridCell(out Vector3 point)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            point = new Vector3();

            RaycastHit[] hits = Physics.RaycastAll(ray);

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
        //private void OnDrawGizmos()
        //{
        //    if (!drawGizmos)
        //        return;

        //    Gizmos.color = gizmoLineColor;

        //    draw on selected cell area
        //    if (selected)
        //    {
        //        Vector3[] vertices = grid.GetCellVertices(current);

        //        Vector3 a = vertices[0];
        //        Vector3 b = vertices[1];
        //        Vector3 c = vertices[2];
        //        Vector3 d = vertices[3];

        //        Gizmos.DrawLine(a, d);
        //        Gizmos.DrawLine(b, c);
        //    }
        //}
#endif

    }
}