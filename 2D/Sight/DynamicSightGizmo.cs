using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GodUnityPlugin
{
    public class DynamicSightGizmo : MonoBehaviour
    {
        private DynamicSightData dynamicSightData;
        private Forward forward;
        private DynamicSight dynamicSight;

        public void Init(DynamicSight dynamicSight, DynamicSightData dynamicSightData, Forward forward)
        {
            this.dynamicSightData = dynamicSightData;
            this.forward = forward;
            this.dynamicSight = dynamicSight;
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            if (dynamicSightData.Sights == null || dynamicSightData.Sights.Length == 0)
                return;

            for (int i = 0; i < dynamicSightData.Sights.Length; i++)
            {
                DynamicSight.Sight sight = dynamicSightData.Sights[i];

                Vector2 dir = Quaternion.Euler(0, 0, -sight.sight2D.angle * 0.5f) *
                              forward.NormalizeToForward(sight.sight2D.baseDirection);

                Color c = sight.onNormal;

                if (dynamicSight.Target)
                    if (dynamicSight.IsAlerted())
                        c = sight.onAlert;
                    else if (dynamicSight.InSight(sight))
                        c = sight.onDetect;

                Handles.color = c;

                if (transform == null)
                    Debug.Log("transform is null ");

                if (sight.sight2D == null)
                    Debug.Log("sight is null ");

                Handles.DrawSolidArc(transform.position, Vector3.forward, dir, sight.sight2D.angle,
                    sight.sight2D.range);
            }
        }

#endif
    }
}