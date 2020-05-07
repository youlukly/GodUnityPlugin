using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GodUnityPlugin
{
    public class Forward
    {
        public interface IFlip
        {
            void OnFlip();
        }

        public enum HorizontalDirection
        {
            Left,
            Right,
        }

        private HorizontalDirection startDirection;

        private List<IFlip> flips = new List<IFlip>();
        private List<GameObject> flippingObjects = new List<GameObject>();

        private bool flipX;

        private Transform transform;

        public UnityEvent onFlipX;

        public void Init(Transform transform, HorizontalDirection startDirection)
        {
            flipX = false;

            this.startDirection = startDirection;

            float angle = startDirection == HorizontalDirection.Left ? 0.0f : 180.0f;

            this.transform = transform;

            transform.localEulerAngles = new Vector3(0.0f, angle, 0.0f);
        }

        public void AddFlipping(GameObject flippingObject)
        {
            if (!flippingObjects.Contains(flippingObject))
                flippingObjects.Add(flippingObject);
        }

        public void RemoveFlipping(GameObject flippingObject)
        {
            if (flippingObjects.Contains(flippingObject))
                flippingObjects.Remove(flippingObject);
        }

        public void AddListener(IFlip flip)
        {
            if (!flips.Contains(flip))
                flips.Add(flip);
        }

        public void RemoveListener(IFlip flip)
        {
            if (flips.Contains(flip))
                flips.Remove(flip);
        }

        public void Flip()
        {
            flipX = !flipX;

            transform.Rotate(0.0f, 180.0f, 0.0f);

            if (flippingObjects != null && flippingObjects.Count > 0)
                foreach (var flippingObject in flippingObjects)
                    flippingObject.transform.Rotate(0.0f, 180.0f, 0.0f);

            if (flips != null && flips.Count > 0)
                foreach (var flip in flips)
                    flip.OnFlip();

            if (onFlipX != null)
                onFlipX.Invoke();
        }

        public Vector2 Get()
        {
            return startDirection == HorizontalDirection.Left
                ? (flipX ? Vector2.right : Vector2.left)
                : (flipX ? Vector2.left : Vector2.right);
        }

        public Vector2 NormalizeToForward(Vector2 baseDirection)
        {
            return new Vector2(baseDirection.x * Get().x, baseDirection.y).normalized;
        }

        public bool IsHeadingForward(Vector2 dir)
        {
            float x = dir.x * Get().x;

            return x > 0;
        }

        public void LookAt(Vector2 target)
        {
            if (Get() == Vector2.left)
            {
                if (target.x > transform.position.x)
                    Flip();
            }
            else if (Get() == Vector2.right)
            {
                if (target.x < transform.position.x)
                    Flip();
            }
        }

        public void LookAt(GameObject target)
        {
            LookAt(target.transform.position);
        }

        public bool IsFacing(Vector2 target)
        {
            if (Get() == Vector2.left)
                return target.x < transform.position.x;

            return target.x > transform.position.x;
        }

        public bool IsFacing(GameObject target)
        {
            if (target == null)
                return false;

            return IsFacing(target.transform.position);
        }

        public bool IsFacingLeft()
        {
            if (startDirection == HorizontalDirection.Left)
            {
                return !flipX;
            }

            return flipX;
        }

        public bool IsFacingRight()
        {
            if (startDirection == HorizontalDirection.Right)
            {
                return !flipX;
            }

            return flipX;
        }
    }
}