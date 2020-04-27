using UnityEngine;
using UnityEditor;

namespace GodUnityPlugin
{
    [RequireComponent(typeof(Grid))]
    public class GridSelector : MonoBehaviour
    {
        private Grid grid;

        private BoxCollider gridBox;

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

        private void Awake()
        {
            SynchronizeCollider();
        }

        public void SynchronizeCollider()
        {
            Bounds bounds = new Bounds(Grid.center, new Vector2(Grid.Width, Grid.Height));

            GridBox.size = bounds.size;
        }

        private void OnDestroy()
        {
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

        //private void UpdateSceneView(SceneView sceneView)
        //{
        //    if (grid == null || gridBox == null)
        //        return;

        //    if (Event.current.type != EventType.MouseDown || Event.current.button != 0)
        //        return;

        //    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        //    Debug.DrawRay(ray.origin, ray.direction * 999f, Color.red, 2.0f);

        //    RaycastHit hit = new RaycastHit();

        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        if (hit.collider != gridBox)
        //            return;

        //        if (grid.cells == null)
        //            return;

        //        for (int i = 0; i < grid.cells.Length; i++)
        //        {
        //            for (int j = 0; j < grid.cells[i].Length; j++)
        //            {
        //                if (grid.cells[i][j].Contains(hit.point))
        //                {
        //                    Debug.Log("select cell : [" + i + "], [" + j + "]");
        //                    return;
        //                }
        //            }
        //        }
        //    }

        //}

        private void Update()
        {
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

        private void OnDrawGizmos()
        {
           
        }
    }
}