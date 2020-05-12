#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace GodUnityPlugin
{
    public class PointIndicator : MonoBehaviour
    {
        public enum IndicateType
        {
            None,
            Float,
            Bar,
            Both
        }

        public PointController pointController;
        public IndicateType type;

        public Vector2 pos;
        public Color color;
        public float lengthMult = 1.0f;

        private void OnDrawGizmos()
        {
            Vector2 nowPos = transform.position;

            if (type == IndicateType.Both || type == IndicateType.Bar)
            {
                Vector2 start = nowPos + pos - new Vector2((pointController.Current / 2.0f * lengthMult), 0f);
                Vector2 end = nowPos + pos + new Vector2((pointController.Current / 2.0f * lengthMult), 0f);
                Handles.color = color;
                Handles.DrawLine(start, end);
            }

            if (type == IndicateType.Both || type == IndicateType.Float)
                Handles.Label(nowPos + pos + new Vector2(0, 0.25f), pointController.Current.ToString());
        }
    }
}

#endif