using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    public class Teleportation2D
    {
        private GameObject teleporter;
        private float emptyHeightMin = 3.0f;
        private float emptyWidthMin = 1f;
        private float teleportRangeMax = 11.0f;
        private float teleportHeightMax = 11.0f;
        private int randomNearPointMax = 3;
        private int rayCount = 5;
        private LayerMask teleportables;
        private LayerMask obstacles;
        private bool drawGizmo = false;

        public Teleportation2D(GameObject teleporter, float emptyHeightMin, float emptyWidthMin, float teleportRangeMax,
            float teleportHeightMax,
            int randomNearPointMax, int rayCount, LayerMask teleportables, LayerMask obstacles)
        {
            this.teleporter = teleporter;
            this.emptyHeightMin = emptyHeightMin;
            this.emptyWidthMin = emptyWidthMin;
            this.teleportRangeMax = teleportRangeMax;
            this.teleportHeightMax = teleportHeightMax;
            this.randomNearPointMax = randomNearPointMax;
            this.rayCount = rayCount;
            this.teleportables = teleportables;
            this.obstacles = obstacles;
        }

        public void DrawGizmo(bool value)
        {
            drawGizmo = value;
        }

        public bool CheckTeleportablePointsExist(Vector2 target)
        {
            List<Vector2> teleportPoints = GetTeleportablePoints(target);
            return teleportPoints.Count > 0;
        }

        public void TelePort(Vector2 target)
        {
            List<Vector2> teleportPoints = GetTeleportablePoints(target);

            Vector2 teleportPoint = teleporter.transform.position;

            if (teleportPoints != null || teleportPoints.Count > 0)
            {
                List<Vector2> validPoints = new List<Vector2>();

                foreach (var point in teleportPoints)
                {
                    if (IsInCollider(point))
                        continue;

                    validPoints.Add(point);
                }

                if (validPoints.Count > 0)
                {
                    List<Vector2> nearests = new List<Vector2>();

                    for (int i = 0; i < randomNearPointMax; i++)
                    {
                        if (validPoints.Count - 1 < i)
                            break;

                        if (validPoints.Count <= 0)
                            break;

                        Vector2 nearest = Vector2.zero;

                        float min = float.MaxValue;

                        foreach (var point in validPoints)
                        {
                            float dist = (point - target).sqrMagnitude;

                            if (dist > min)
                                continue;

                            min = dist;
                            nearest = point;
                        }

                        if (nearest == Vector2.zero)
                            continue;

                        nearests.Add(nearest);
                        validPoints.Remove(nearest);
                    }

                    if (drawGizmo)
                        if (nearests != null && nearests.Count > 0)
                            for (int i = 0; i < nearests.Count; i++)
                                DebugLocation(nearests[i], Color.blue);

                    Debug.Log(nearests.Count);
                    teleportPoint = nearests[Random.Range(0, nearests.Count - 1)];
                }
            }

            teleporter.transform.position = teleportPoint;
        }

        private List<Vector2> GetTeleportablePoints(Vector2 targetPos)
        {
            List<Vector2> teleportPoints = new List<Vector2>();

            Vector2 originPos = teleporter.transform.position;

            Vector2 targetDir = (targetPos - originPos).normalized;

            for (int j = 0; j < rayCount; j++)
            {
                float interval = teleportRangeMax / (rayCount - 1);

                Vector2 rayPoint = new Vector2(originPos.x + (interval * j) * targetDir.x,
                    originPos.y + (teleportHeightMax / 2.0f));

                RaycastHit2D[] hits = Physics2D.RaycastAll(rayPoint, Vector2.down, teleportHeightMax, teleportables);

                if (hits == null)
                    continue;

                foreach (var hit in hits)
                {
                    if (hit.collider.OverlapPoint(rayPoint))
                        continue;

                    teleportPoints.Add(hit.point);
                }
            }

            return teleportPoints;
        }

        private bool IsInCollider(Vector2 point)
        {
            Collider2D overlap = Physics2D.OverlapBox(point + new Vector2(0.0f, 0.02125f + (emptyHeightMin * 0.5f)),
                new Vector2(emptyWidthMin, emptyHeightMin), 0.0f, obstacles);

            if (drawGizmo)
            {
                Color color = overlap ? Color.red : Color.green;
                DebugLocation(point, color);
            }

            return overlap == true;
        }

        private void DebugLocation(Vector2 point, Color color)
        {
#if UNITY_EDITOR

            Vector2 center = point + new Vector2(0.0f, 0.02125f + (emptyHeightMin * 0.5f));

            Vector2 a = point + new Vector2(0.0f, 0.02125f + (emptyHeightMin * 0.5f)) +
                        new Vector2(-emptyWidthMin * 0.5f, emptyHeightMin * 0.5f);
            Vector2 b = point + new Vector2(0.0f, 0.02125f + (emptyHeightMin * 0.5f)) +
                        new Vector2(-emptyWidthMin * 0.5f, -emptyHeightMin * 0.5f);
            Vector2 c = point + new Vector2(0.0f, 0.02125f + (emptyHeightMin * 0.5f)) +
                        new Vector2(+emptyWidthMin * 0.5f, -emptyHeightMin * 0.5f);
            Vector2 d = point + new Vector2(0.0f, 0.02125f + (emptyHeightMin * 0.5f)) +
                        new Vector2(emptyWidthMin * 0.5f, emptyHeightMin * 0.5f);

            Debug.DrawLine(a, b, color, 2.0f);
            Debug.DrawLine(b, c, color, 2.0f);
            Debug.DrawLine(c, d, color, 2.0f);
            Debug.DrawLine(d, a, color, 2.0f);

#endif
        }
    }
}