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
        public KeyCode selectKey = KeyCode.G;

        // grid cell unity events, occur on select something
        [Header("Events")]
        public GridCellUnityEvent onSelect;

        [Header("Gizmos")]
        [SerializeField] private bool drawGizmos = true;
        public Color gizmoLineColor = new Color(0.65f, 0.65f, 0.25f, 0.75f);

        private Grid grid;
        private BoxCollider gridBox;
        private bool selected = false;
        private GridCell current;

        private void Awake()
        {
            grid = GetComponent<Grid>();

            gridBox = GetComponent<BoxCollider>();
            if (gridBox == null)
                gridBox = gameObject.AddComponent<BoxCollider>();
        }

        public bool IsSelected(out GridCell cell)
        {
            cell = current;

            return selected;
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

            if (Input.GetKey(selectKey) && Input.GetKeyDown(KeyCode.Mouse0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                CheckGridCell(ray);
            }
        }

        private void CheckGridCell(Ray ray)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != null && hit.collider == gridBox)
                {
                    GridCell cell = new GridCell();

                    if (grid.IsInCell(hit.point, out cell))
                    {
                        OnSelect(cell);
                        return;
                    }
                }
            }

            OnDeselect();
        }

        private void OnDeselect()
        {
            selected = false;
            current = new GridCell();
        }

        private void OnSelect(GridCell cell)
        {
            Debug.Log("select cell : name [" + cell.id + "], matrix [" + cell.columnIndex + "], [" + cell.rowIndex + "]");
            selected = true;
            current = cell;
            onSelect.Invoke(current);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!drawGizmos)
                return;

            Gizmos.color = gizmoLineColor;

            // draw on selected cell area
            if (selected)
            {
                Vector3[] vertices = grid.GetCellVertices(current);

                Vector3 a = vertices[0];
                Vector3 b = vertices[1];
                Vector3 c = vertices[2];
                Vector3 d = vertices[3];

                Gizmos.DrawLine(a, d);
                Gizmos.DrawLine(b, c);
            }
        }
#endif

    }
}