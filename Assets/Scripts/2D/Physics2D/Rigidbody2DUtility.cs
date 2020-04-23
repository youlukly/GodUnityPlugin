using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    public class Rigidbody2DUtility
    {
        public Rigidbody2D rigidbody2D { private set; get; }

        private Forward forward;

        private float originGravity;
        private float originDrag;

        public Rigidbody2DUtility(Rigidbody2D rigidbody2D, Forward forward)
        {
            this.forward = forward;
            this.rigidbody2D = rigidbody2D;

            originGravity = rigidbody2D.gravityScale;
            originDrag = rigidbody2D.drag;
        }

        public void SetZeroGravityScale()
        {
            rigidbody2D.gravityScale = 0.0f;
        }

        public void SetOriginalGravity()
        {
            rigidbody2D.gravityScale = originGravity;
        }

        public void SetOriginalDrag()
        {
            rigidbody2D.drag = originDrag;
        }

        public void Stop()
        {
            rigidbody2D.velocity = Vector2.zero;
        }

        public void Brake()
        {
            rigidbody2D.AddForce(-rigidbody2D.velocity * Vector2.right * rigidbody2D.mass, ForceMode2D.Impulse);
        }

        public void MoveOnVelocityX(float velocityX)
        {
            rigidbody2D.velocity = new Vector2(velocityX, 0.0f);
        }

        public void MoveOnVelocityY(float velocityY)
        {
            rigidbody2D.velocity = new Vector2(0.0f, velocityY);
        }

        public void AddForceUp(float magnitude)
        {
            rigidbody2D.AddForce(Vector2.up * magnitude, ForceMode2D.Impulse);
        }

        public void AddCalibratedForceUp(float magnitude)
        {
            rigidbody2D.AddForce(Vector2.up * magnitude * rigidbody2D.mass, ForceMode2D.Impulse);
        }

        public void AddForceDown(float magnitude)
        {
            rigidbody2D.AddForce(Vector2.down * magnitude, ForceMode2D.Impulse);
        }

        public void AddCalibratedForceDown(float magnitude)
        {
            rigidbody2D.AddForce(Vector2.down * magnitude * rigidbody2D.mass, ForceMode2D.Impulse);
        }

        public void AddForceLeft(float magnitude)
        {
            rigidbody2D.AddForce(Vector2.left * magnitude, ForceMode2D.Impulse);
        }

        public void AddCalibratedForceLeft(float magnitude)
        {
            rigidbody2D.AddForce(Vector2.left * magnitude * rigidbody2D.mass, ForceMode2D.Impulse);
        }

        public void AddForceRight(float magnitude)
        {
            rigidbody2D.AddForce(Vector2.right * magnitude, ForceMode2D.Impulse);
        }

        public void AddCalibratedForceRight(float magnitude)
        {
            rigidbody2D.AddForce(Vector2.right * magnitude * rigidbody2D.mass, ForceMode2D.Impulse);
        }

        public void AddRelativeForce(Vector2 force, ForceMode2D mode)
        {
            rigidbody2D.AddRelativeForce(force, mode);
        }

        public void AddCalibratedRelativeForce(Vector2 force, ForceMode2D mode)
        {
            rigidbody2D.AddRelativeForce(force * rigidbody2D.mass, mode);
        }

        public void AddForce(Vector2 force, ForceMode2D mode)
        {
            rigidbody2D.AddForce(force, mode);
        }

        public void AddCalibratedForceImpulse(Vector2 force)
        {
            rigidbody2D.AddForce(force * rigidbody2D.mass, ForceMode2D.Impulse);
        }

        public void AddCalibratedForce(Vector2 force, ForceMode2D mode)
        {
            rigidbody2D.AddForce(force * rigidbody2D.mass, mode);
        }

        public void AddCalibratedForwardForce(float power)
        {
            rigidbody2D.AddForce(forward.Get() * power * rigidbody2D.mass, ForceMode2D.Impulse);
        }

        public void AddForceBackward(float magnitude)
        {
            AddForce(-forward.Get() * magnitude, ForceMode2D.Impulse);
        }

        public void AddForceForward(float magnitude)
        {
            AddForce(forward.Get() * magnitude, ForceMode2D.Impulse);
        }

        public void AddCalibratedForceBackward(float magnitude)
        {
            AddCalibratedForce(-forward.Get() * magnitude, ForceMode2D.Impulse);
        }

        public void MovePositionForward(float distance)
        {
            float x = forward.Get() == Vector2.left ? -distance : distance;

            rigidbody2D.MovePosition(rigidbody2D.position + new Vector2(x, 0.0f));
        }

        public Vector2 GetNormal(Collider2D motionCollider, Vector2 dir)
        {
            Vector2 normal = Vector2.zero;

            float min = float.MaxValue;

            ContactPoint2D[] contacts = new ContactPoint2D[10];

            int count = motionCollider.GetContacts(contacts);

            if (count == 0)
                return normal;

            foreach (var contact in contacts)
            {
                if (!contact.collider)
                    continue;

                float angle = Vector2.Angle(dir, (contact.point - rigidbody2D.position).normalized);

                if (angle >= min)
                    continue;

                min = angle;

                normal = contact.normal;
            }

            return normal;
        }

        public Vector2 GetNormal(Collider2D[] motionColliders, Vector2 dir)
        {
            Vector2 normal = Vector2.zero;

            float min = float.MaxValue;

            List<ContactPoint2D> contacts = new List<ContactPoint2D>();

            int count = 0;

            foreach (var collider in motionColliders)
            {
                List<ContactPoint2D> tempContacts = new List<ContactPoint2D>();

                count += collider.GetContacts(tempContacts);

                contacts.AddRange(tempContacts);
            }

            if (count == 0)
                return normal;

            foreach (var contact in contacts)
            {
                if (!contact.collider)
                    continue;

                float angle = Vector2.Angle(dir, (contact.point - rigidbody2D.position).normalized);

                if (angle >= min)
                    continue;

                min = angle;

                normal = contact.normal;
            }

            return normal;
        }

        public Vector2 GetNormalizedDirection(Collider2D motionCollider, Vector2 baseDirection)
        {
            Vector2 result = baseDirection.normalized;

            Vector2 normal = GetNormal(motionCollider, baseDirection.normalized);

            if (Vector2.zero != normal)
                result = result - (-normal * Vector3.Dot(result, -normal));

            return result;
        }

        public Vector2 GetNormalizedDirection(Collider2D[] motionColliders, Vector2 baseDirection)
        {
            Vector2 result = baseDirection.normalized;

            Vector2 normal = GetNormal(motionColliders, baseDirection.normalized);

            if (Vector2.zero != normal)
                result = result - (-normal * Vector3.Dot(result, -normal));

            return result;
        }
    }
}