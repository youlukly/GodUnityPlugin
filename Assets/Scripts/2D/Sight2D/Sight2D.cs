using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    [CreateAssetMenu(menuName = "Sight2D")]
    public class Sight2D : ScriptableObject
    {
        public float range;
        [Range(0.0f, 360f)] public float angle;
        public Vector2 baseDirection;

        public bool IsInSight(Vector2 origin, Vector2 target)
        {
            return GUPMath.IsInAngle(origin, target, angle, baseDirection) &&
                   GUPMath.IsInRange(origin, target, range);
        }

        public bool IsInSight(Vector2 origin, Vector2 target, Vector2 direction)
        {
            return GUPMath.IsInAngle(origin, target, angle, direction) &&
                   GUPMath.IsInRange(origin, target, range);
        }

        public bool IsInAngle(Vector2 origin, Vector2 target)
        {
            return GUPMath.IsInAngle(origin, target, angle, baseDirection);
        }

        public bool IsInAngle(Vector2 origin, Vector2 target, Vector2 direction)
        {
            return GUPMath.IsInAngle(origin, target, angle, direction);
        }

        public bool IsInRange(Vector2 origin, Vector2 target)
        {
            return GUPMath.IsInRange(origin, target, range);
        }

        public bool CheckObstacle(Vector2 origin, int layerMask)
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, baseDirection, range, layerMask);

            return raycastHit2D.collider != null;
        }

        public bool CheckObstacle(Vector2 origin, int layerMask, Collider2D ignore)
        {
            RaycastHit2D[] raycastHit2D = Physics2D.RaycastAll(origin, baseDirection, range, layerMask);

            if (raycastHit2D.Length == 0)
                return false;

            if (raycastHit2D.Length == 1)
                if (raycastHit2D[0].collider == null || raycastHit2D[0].collider == ignore)
                    return false;

            return true;
        }

        public bool CheckObstacle(Vector2 origin, int layerMask, out RaycastHit2D closest)
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, baseDirection, range, layerMask);

            closest = raycastHit2D;

            return raycastHit2D.collider != null;
        }

        public bool CheckObstacle(Vector2 origin, int layerMask, Collider2D ignore, out RaycastHit2D closest)
        {
            RaycastHit2D[] raycastHit2D = Physics2D.RaycastAll(origin, baseDirection, range, layerMask);

            closest = new RaycastHit2D();

            if (raycastHit2D.Length == 0)
                return false;

            foreach (var item in raycastHit2D)
            {
                if (item.collider == null)
                    continue;

                float min = float.MaxValue;

                if (min > item.distance)
                {
                    min = item.distance;
                    closest = item;
                }
            }

            if (raycastHit2D.Length == 1)
                if (raycastHit2D[0].collider == null || raycastHit2D[0].collider == ignore)
                    return false;

            return true;
        }

        public bool CheckObstacle(Vector2 origin, int layerMask, Collider2D[] ignores, out RaycastHit2D closest)
        {
            RaycastHit2D[] raycastHit2Ds = Physics2D.RaycastAll(origin, baseDirection, range, layerMask);

            closest = new RaycastHit2D();

            if (raycastHit2Ds.Length == 0)
                return false;

            closest = GetClosest(raycastHit2Ds);

            foreach (var hit in raycastHit2Ds)
                for (int i = 0; i < ignores.Length; i++)
                    if (hit.collider != ignores[i])
                        return true;

            return false;
        }


        public bool CheckObstacle(Vector2 origin, Vector2 dir, int layerMask)
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, dir, range, layerMask);

            return raycastHit2D.collider != null;
        }

        public bool CheckObstacle(Vector2 origin, Vector2 dir, int layerMask, Collider2D ignore)
        {
            RaycastHit2D[] raycastHit2Ds = Physics2D.RaycastAll(origin, dir, range, layerMask);

            RaycastHit2D closest = new RaycastHit2D();

            if (raycastHit2Ds.Length == 0)
                return false;

            closest = GetClosest(raycastHit2Ds);

            if (raycastHit2Ds.Length == 1)
                if (raycastHit2Ds[0].collider == null || raycastHit2Ds[0].collider == ignore)
                    return false;

            return true;
        }

        public bool CheckObstacle(Vector2 origin, Vector2 dir, int layerMask, Collider2D[] ignores)
        {
            RaycastHit2D[] raycastHit2Ds = Physics2D.RaycastAll(origin, dir, range, layerMask);

            RaycastHit2D closest = raycastHit2Ds[0];

            if (raycastHit2Ds.Length == 0)
                return false;

            closest = GetClosest(raycastHit2Ds);

            foreach (var hit in raycastHit2Ds)
                for (int i = 0; i < ignores.Length; i++)
                    if (hit.collider != ignores[i])
                        return true;

            return false;
        }

        public bool CheckObstacle(Vector2 origin, Vector2 dir, int layerMask, out RaycastHit2D closest)
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, dir, range, layerMask);

            closest = raycastHit2D;

            return raycastHit2D.collider != null;
        }

        public bool CheckObstacle(Vector2 origin, Vector2 dir, int layerMask, Collider2D ignore, out RaycastHit2D closest)
        {
            RaycastHit2D[] raycastHit2Ds = Physics2D.RaycastAll(origin, dir, range, layerMask);

            closest = new RaycastHit2D();

            if (raycastHit2Ds.Length == 0)
                return false;

            closest = GetClosest(raycastHit2Ds);

            if (closest.collider != ignore)
                return true;

            return false;
        }

        public bool CheckObstacle(Vector2 origin, Vector2 dir, int layerMask, Collider2D[] ignores,
            out RaycastHit2D closest)
        {
            RaycastHit2D[] raycastHit2Ds = Physics2D.RaycastAll(origin, dir, range, layerMask);

            closest = new RaycastHit2D();

            if (raycastHit2Ds.Length == 0)
                return false;

            closest = GetClosest(raycastHit2Ds);

            foreach (var hit in raycastHit2Ds)
                for (int i = 0; i < ignores.Length; i++)
                    if (hit.collider != ignores[i])
                        return true;

            return false;
        }

        private RaycastHit2D GetClosest(RaycastHit2D[] raycastHit2Ds)
        {
            RaycastHit2D closest = new RaycastHit2D();

            foreach (var item in raycastHit2Ds)
            {
                if (item.collider == null)
                    continue;

                float min = float.MaxValue;

                if (min > item.distance)
                {
                    min = item.distance;
                    closest = item;
                }
            }

            return closest;
        }
    }
}