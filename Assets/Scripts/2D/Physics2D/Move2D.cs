using UnityEngine;

namespace GodUnityPlugin
{
    public class Move2D
    {
        private Rigidbody2D rigidbody2D;
        private Collider2D motionCollider;

        private Vector2 baseDirection;

        public Vector2 direction
        {
            get
            {
                Vector2 result = baseDirection.normalized;

                Vector2 normal = GetNormalBottom();

                if (Vector2.zero != normal)
                    result = result - (-normal * Vector3.Dot(result, -normal));

                return result;
            }
        }

        public float currentSpeed
        {
            get { return Vector3.Dot(rigidbody2D.velocity, direction); }
        }

        public Move2D(Rigidbody2D rigidbody2D, Collider2D collider)
        {
            this.rigidbody2D = rigidbody2D;
            this.motionCollider = collider;
        }

        public Collider2D GetCollider()
        {
            return motionCollider;
        }

        public void Move(Vector2 direction, float targetSpeed, float acceleration, float deceleration)
        {
            baseDirection = direction;

            float moveSpeed = 0.0f;

            if (targetSpeed < currentSpeed)
                moveSpeed = deceleration * Time.fixedDeltaTime;
            else
                moveSpeed = acceleration * Time.fixedDeltaTime;

            if (Mathf.Abs(targetSpeed - currentSpeed) < moveSpeed)
                if (targetSpeed > currentSpeed)
                    moveSpeed = targetSpeed - currentSpeed;
                else
                    moveSpeed = currentSpeed - targetSpeed;

            rigidbody2D.velocity += this.direction * moveSpeed;

            //Debug.DrawRay(position, this.direction, Color.cyan);
        }

        private Vector2 GetNormalBottom()
        {
            Vector2 normal = Vector2.zero;

            float min = float.MaxValue;

            ContactPoint2D[] contacts = new ContactPoint2D[10];

            for (int i = 0; i < contacts.Length; i++)
                contacts[i] = new ContactPoint2D();

            int count = motionCollider.GetContacts(contacts);

            if (count == 0)
                return normal;

            foreach (var contact in contacts)
            {
                if (!contact.collider)
                    continue;

                if (contact.point.y > min)
                    continue;

                min = contact.point.y;
                normal = contact.normal;
            }

            return normal;
        }
    }
}