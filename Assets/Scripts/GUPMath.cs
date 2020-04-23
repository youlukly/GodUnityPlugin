using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    public static class GUPMath
    {
        public static float PercentToValue(float value, float reference)
        {
            return (reference * (value / 100f));
        }

        public static float PercentToValue(int value, int reference)
        {
            return (reference * (value / 100f));
        }

        public static float ValueToPercent(float value, float reference)
        {
            return 100f * value / reference;
        }

        public static float ValueToPercent(int value, int reference)
        {
            return 100f * value / reference;
        }

        public static float Least(float[] values)
        {
            float min = float.MaxValue;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] >= min)
                    continue;

                min = values[i];
            }

            return min;
        }

        public static float Greatest(float[] values)
        {
            float max = float.MinValue;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] <= max)
                    continue;

                max = values[i];
            }

            return max;
        }

        public static List<float> Leasts(float[] values)
        {
            float min = float.MaxValue;

            List<float> mins = new List<float>();

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > min)
                    continue;

                min = values[i];
            }

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > min)
                    continue;

                mins.Add(values[i]);
            }

            return mins;
        }

        public static List<float> Greatests(float[] values)
        {
            float max = float.MinValue;

            List<float> maxs = new List<float>();

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] <= max)
                    continue;

                max = values[i];
            }

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] < max)
                    continue;

                maxs.Add(values[i]);
            }

            return maxs;
        }

        public static int Least(int[] values)
        {
            int min = int.MaxValue;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] >= min)
                    continue;

                min = values[i];
            }

            return min;
        }

        public static int Greatest(int[] values)
        {
            int max = int.MinValue;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] <= max)
                    continue;

                max = values[i];
            }

            return max;
        }


        public static List<int> Leasts(int[] values)
        {
            int min = int.MaxValue;

            List<int> mins = new List<int>();

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > min)
                    continue;

                min = values[i];
            }

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > min)
                    continue;

                mins.Add(values[i]);
            }

            return mins;
        }

        public static List<int> Greatests(int[] values)
        {
            int max = int.MinValue;

            List<int> maxs = new List<int>();

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] <= max)
                    continue;

                max = values[i];
            }

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] < max)
                    continue;

                maxs.Add(values[i]);
            }

            return maxs;
        }

        public static bool IsGreater(float value, float reference)
        {
            return value > reference;
        }

        public static bool IsLesser(float value, float reference)
        {
            return value < reference;
        }

        public static bool IsEqual(float value, float reference)
        {
            return value == reference;
        }

        public static bool IsGreater(int value, int reference)
        {
            return value > reference;
        }

        public static bool IsLesser(int value, int reference)
        {
            return value < reference;
        }

        public static bool IsEqual(int value, int reference)
        {
            return value == reference;
        }

        public static bool IsInAngle(Vector2 origin, Vector2 point, float angle, Vector2 direction)
        {
            Vector2 dir = direction.normalized;

            float dot = Vector2.Dot(dir, (point - origin).normalized);
            float degDot = Vector2.Dot(dir, Quaternion.Euler(0, 0, angle / 2f) * dir);

            return dot >= degDot;
        }

        public static bool IsInRange(Vector2 origin, Vector2 point, float range)
        {
            return (point - origin).sqrMagnitude <= (range * range);
        }

        public static Vector2 ClampDirection(Vector2 origin, Vector2 point, float angle, Vector2 baseDirection)
        {
            Vector2 result = (point - origin).normalized;

            if (!IsInAngle(origin, point, angle, baseDirection))
            {
                if (point.y < origin.y)
                {
                    result = Quaternion.Euler(0, 0, -angle / 2f) * baseDirection;
                }
                else
                {
                    result = Quaternion.Euler(0, 0, angle / 2f) * baseDirection;
                }
            }

            return result;
        }

        public static GameObject GetClosest(GameObject[] gameObjects, Vector3 point)
        {
            GameObject result = null;
            float min = float.MaxValue;

            for (int i = 0; i < gameObjects.Length; i++)
            {
                float distance = (gameObjects[i].transform.position - point).sqrMagnitude;
                if (distance > min)
                    continue;

                min = distance;
                result = gameObjects[i];
            }

            return result;
        }

        public static GameObject GetClosest(GameObject[] gameObjects, Vector2 point)
        {
            GameObject result = null;
            float min = float.MaxValue;

            for (int i = 0; i < gameObjects.Length; i++)
            {
                Vector2 objPoint = gameObjects[i].transform.position;
                float distance = (objPoint - point).sqrMagnitude;
                if (distance > min)
                    continue;

                min = distance;
                result = gameObjects[i];
            }

            return result;
        }

        public static Vector3 GetClosestVector(List<Vector3> targets, Vector3 point)
        {
            Vector3 result = new Vector3();
            float min = float.MaxValue;

            for (int i = 0; i < targets.Count; i++)
            {
                float distance = (targets[i] - point).sqrMagnitude;
                if (distance > min)
                    continue;

                min = distance;
                result = targets[i];
            }

            return result;
        }

        public static Vector2 GetClosestVector(List<Vector2> targets, Vector2 point)
        {
            Vector2 result = new Vector2();
            float min = float.MaxValue;

            for (int i = 0; i < targets.Count; i++)
            {
                float distance = (targets[i] - point).sqrMagnitude;
                if (distance > min)
                    continue;

                min = distance;
                result = targets[i];
            }

            return result;
        }

        public static Vector3 GetRandomPointInsideTri(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 dAB = b - a;
            Vector3 dBC = c - b;

            float randomAB = UnityEngine.Random.value;
            float randomBC = UnityEngine.Random.value;

            Vector3 result = a + dAB * randomAB + dBC * randomBC;

            Vector3 dirAB = (b - a).normalized;
            Vector3 dirAC = (c - a).normalized;
            Vector3 dirPC = (c - result).normalized;

            Vector3 normal = Vector3.Cross(dirAB, dirAC).normalized;
            Vector3 dirH_AC = Vector3.Cross(normal, dirAC).normalized;

            float dot = Vector3.Dot(dirH_AC, dirPC);

            if (dot >= 0)
            {
                Vector3 centralPoint = (a + c) / 2;

                Vector3 symmetricPoint = 2 * centralPoint - result;

                result = symmetricPoint;
            }

            return result;
        }

        public static bool IsVertexInTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 point)
        {
            return IsInAcuteAngle(a, b, c, point) &&
                   IsInAcuteAngle(b, c, a, point) &&
                   IsInAcuteAngle(c, a, b, point);
        }

        public static bool IsInAcuteAngle(Vector3 a, Vector3 b, Vector3 c, Vector3 point)
        {
            Vector3 baseDirection = (b - a).normalized;
            Vector3 compareDirection = (c - a).normalized;
            Vector3 targetDirection = (point - a).normalized;

            Vector3 crossA = Vector3.Cross(baseDirection, compareDirection);
            Vector3 crossB = Vector3.Cross(targetDirection, compareDirection);

            return Vector3.Dot(crossA, crossB) >= 0;
        }

        public static List<Vector3> FindContactPointInCircle(float slope, float yIntercept, float radius,
            Vector3 circleCenter)
        {
            List<Vector3> results = null;
            float a, b;
            float sqrRange;
            sqrRange = Mathf.Pow(radius, 2);
            a = circleCenter.x;
            b = circleCenter.z;

            float xSqrCoefficient = slope * slope + 1;
            float xCoefficient = 2 * (-a + slope - b * slope);
            float constant = a * a + yIntercept * yIntercept - 2 * yIntercept * b + b * b - sqrRange;

            int D = 0;
            D = Discriminant(
                xSqrCoefficient,
                xCoefficient,
                constant
            );

            if (D == 2)
            {
                float x1, x2, y1, y2;

                x1 = (-1 * xCoefficient + Mathf.Sqrt(xCoefficient * xCoefficient - 4 * xSqrCoefficient * constant)) / 2 *
                     xSqrCoefficient;
                y1 = slope * x1 + yIntercept;
                x2 = (-1 * xCoefficient - Mathf.Sqrt(xCoefficient * xCoefficient - 4 * xSqrCoefficient * constant)) / 2 *
                     xSqrCoefficient;
                y2 = slope * x1 + yIntercept;

                Vector3 result = new Vector3(x1, 0, y1);
                results.Add(result);
                result = new Vector3(x2, 0, y2);
                results.Add(result);
            }
            else if (D == 1)
            {
                float x, y;
                x = (-1 * xCoefficient + Mathf.Sqrt(xCoefficient * xCoefficient - 4 * xSqrCoefficient * constant)) / 2 *
                    xSqrCoefficient;
                y = slope * x + yIntercept;

                Vector3 result = new Vector3(x, 0, y);
                results.Add(result);
            }
            else if (D == 0)
            {
                Debug.Log("there is no contact point");
            }

            return results;
        }

        public static float GetYintercept(Vector3 point1, Vector3 point2)
        {
            float p1x, p1y, p2x, p2y;
            p1x = point1.x;
            p1y = point1.z;
            p2x = point2.x;
            p2y = point2.z;

            float a, b;
            a = (p2y - p1y) / (p2x - p1x);
            b = p2y - a * p2x;

            return b;
        }

        public static float GetYintercept(Vector2 point1, Vector2 point2)
        {
            float p1x, p1y, p2x, p2y;
            p1x = point1.x;
            p1y = point1.y;
            p2x = point2.x;
            p2y = point2.y;

            float a, b;
            a = (p2y - p1y) / (p2x - p1x);
            b = p2y - a * p2x;

            return b;
        }

        public static float GetSlope(Vector3 point1, Vector3 point2)
        {
            float p1x, p1y, p2x, p2y;
            p1x = point1.x;
            p1y = point1.z;
            p2x = point2.x;
            p2y = point2.z;

            float a;
            a = (p2y - p1y) / (p2x - p1x);

            return a;
        }

        public static float GetSlope(Vector2 point1, Vector2 point2)
        {
            float p1x, p1y, p2x, p2y;
            p1x = point1.x;
            p1y = point1.y;
            p2x = point2.x;
            p2y = point2.y;

            float a;
            a = (p2y - p1y) / (p2x - p1x);

            return a;
        }

        public static bool IsInSameStraight(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
        {
            float a1 = GetSlope(p1, p2);
            float a2 = GetSlope(q1, q2);

            if (a1 != a2)
                return false;

            float b1 = GetYintercept(p1, p2);
            float b2 = GetYintercept(q1, q2);

            return true;
        }

        public static bool IsInSameStraight(Vector3 p1, Vector3 p2, Vector3 q1, Vector3 q2)
        {
            float a1 = GetSlope(p1, p2);
            float a2 = GetSlope(q1, q2);

            if (a1 != a2)
                return false;

            float b1 = GetYintercept(p1, p2);
            float b2 = GetYintercept(q1, q2);

            return true;
        }

        public static bool IsInSameLineSegment(Vector3 p1, Vector3 p2, Vector3 q1, Vector3 q2)
        {
            if ((p1 != q1 && p1 != q2) || (p2 != q1 && p2 != q2))
                return false;

            return IsInSameStraight(p1, p2, q1, q2);
        }

        public static bool IsInSameLineSegment(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
        {
            if ((p1 != q1 && p1 != q2) || (p2 != q1 && p2 != q2))
                return false;

            return IsInSameStraight(p1, p2, q1, q2);
        }

        public static bool IsInCircle(Vector3 point, float radius, Vector3 circleCenter)
        {
            float sqrRange;
            sqrRange = Mathf.Pow(radius, 2);
            float x, y, a, b;
            x = point.x;
            y = point.z;
            a = circleCenter.x;
            b = circleCenter.z;
            return sqrRange >= (x - a) * (x - a) + (y - b) * (y - b);
        }

        public static Vector3 FindContactPointBetweenLines(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
        {
            float a = GetSlope(a1, a2);
            float b = GetYintercept(a1, a2);

            float c = GetSlope(b1, b2);
            float d = GetYintercept(b1, b2);

            if (a == c)
                return Vector3.zero;

            float x = (d - b) / (a - c);
            float y = (a * x) + b;

            return new Vector3(x, 0, y);
        }

        public static Vector2 FindContactPointBetweenLines(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            float a = GetSlope(a1, a2);
            float b = GetYintercept(a1, a2);

            float c = GetSlope(b1, b2);
            float d = GetYintercept(b1, b2);

            if (a == c)
                return Vector2.zero;

            float x = (d - b) / (a - c);
            float y = (a * x) + b;

            return new Vector2(x, y);
        }

        public static int Discriminant(float xSqrCoefficient, float xCoefficient, float constant)
        {
            float result;
            result = Mathf.Pow(xCoefficient, 2) - 4 * xSqrCoefficient * constant;
            if (result > 0.0f)
                return 2;
            else if (result == 0.0f)
                return 1;
            else
                return 0;
        }
    }
}