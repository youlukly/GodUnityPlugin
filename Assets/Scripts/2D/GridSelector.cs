using UnityEngine;
using UnityEditor;

namespace GodUnityPlugin
{
    [RequireComponent(typeof(Grid))]
    public class GridSelector : MonoBehaviour
    {
        private Grid grid;

        private BoxCollider2D gridBox;

        public Grid Grid
        {
            get
            {
                if (grid == null)
                    grid = GetComponent<Grid>();

                return grid;
            }
        }

        public BoxCollider2D GridBox
        {
            get
            {
                if (gridBox == null)
                    CreateBox();

                return gridBox;
            }
        }

        private void CreateBox()
        {
            gridBox = grid.gameObject.AddComponent<BoxCollider2D>();

            float gridWidth = grid.cellWidth * grid.AbsCellCountX;
            float gridHeight = grid.cellHeight * grid.AbsCellCountY;

            gridBox.size = new Vector2(gridWidth,gridHeight);
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += UpdateSceneView;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= UpdateSceneView;
        }

        private void UpdateSceneView(SceneView sceneView)
        {
            if (Grid == null || gridBox == null)
                return;

            if (Event.current.type != EventType.MouseDown || Event.current.button != 0)
                return;

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            Debug.DrawRay(ray.origin,ray.direction * 999f, Color.red,2.0f);

            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != GridBox)
                    return;

                if (Grid.cells == null)
                    return;

                for (int i = 0; i < Grid.cells.Length; i++)
                {
                    for (int j = 0; j < Grid.cells[i].Length; j++)
                    {
                        if (Grid.cells[i][j].Contains(hit.point))
                        {
                            Debug.Log("select cell : [" + i + "], [" + j + "]");
                            return;
                        }
                    }
                }
            }

        }

        private void Update()
        {
            if (Grid == null || GridBox == null)
                return;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                Debug.DrawRay(ray.origin, ray.direction * 999f, Color.red, 2.0f);

                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != GridBox)
                        return;

                    if (Grid.cells == null)
                        return;

                    for (int i = 0; i < Grid.cells.Length; i++)
                    {
                        for (int j = 0; j < Grid.cells[i].Length; j++)
                        {
                            if (Grid.cells[i][j].Contains(hit.point))
                            {
                                Debug.Log("select cell : [" + i + "], [" + j + "]");
                                return;
                            }

                        }
                    }
                }

            }

        }
    }
}