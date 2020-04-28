using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

namespace GodUnityPlugin
{
    [RequireComponent(typeof(Grid))]
    public class GridSelector : MonoBehaviour
    {
        //[Header("Events")]
        //public UnityEvent<Grid.GridCell> onSelect;

        [Header("Gizmos")]
        [SerializeField] private bool drawGizmos = true;
        [Range(1, 9)]
        [SerializeField] private int gizmolineCount; 
        public Color gizmoLineColor = new Color(0.65f, 0.65f, 0.25f, 0.75f);

        private Grid grid;

        private BoxCollider gridBox;

        private bool selected;
        private Grid.GridCell current;

        private Grid Grid
        {
            get
            {
                if (grid == null)
                    grid = GetComponent<Grid>();

                return grid;
            }
        }

        private BoxCollider GridBox
        {
            get
            {
                gridBox = GetComponent<BoxCollider>();

                if (gridBox == null)
                    gridBox = grid.gameObject.AddComponent<BoxCollider>();

                return gridBox;
            }
        }

        public bool IsSelected(out Grid.GridCell cell)
        {
            cell = current;

            return selected;
        }

        public void StartSceneViewUpdate()
        {
            SceneView.duringSceneGui += UpdateSceneView;
        }

        private void SynchronizeCollider()
        {
            Bounds bounds = new Bounds(Grid.center, new Vector2(Grid.Width, Grid.Height));

            GridBox.size = bounds.size;
        }

        private void Reset()
        {
            SceneView.duringSceneGui += UpdateSceneView;
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= UpdateSceneView;
            DestroyBox();
        }

        private void DestroyBox()
        {
            if (gridBox != null)
                if (Application.isPlaying)
                    Destroy(gridBox);
                else
                    DestroyImmediate(gridBox);
        }

        private void UpdateSceneView(SceneView sceneView)
        {
            SynchronizeCollider();

            if (Event.current.type != EventType.MouseDown || Event.current.button != 0)
                return;

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            CheckGridCell(ray);
        }

        private void Update()
        {
            SynchronizeCollider();

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                CheckGridCell(ray);
            }
        }

        private void CheckGridCell(Ray ray)
        {
            Debug.DrawRay(ray.origin, ray.direction * 999f, Color.red, 2.0f);

            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider == null || hit.collider != GridBox)
                    return;

                Grid.GridCell cell = new Grid.GridCell();

                if (Grid.IsInCell(hit.point,out cell) )
                { 
                    Debug.Log("select cell : name [" + cell.id + "], matrix [" + cell.columnIndex + "], [" + cell.rowIndex + "]");
                    selected = true;
                    current = cell;
                    return;
                }

            }

            selected = false;
            current = new Grid.GridCell();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!drawGizmos)
                return;

            if(selected)
            Gizmos.DrawCube(current.center, new Vector3(current.width, current.height,0.1f));
        }
#endif

    }
}