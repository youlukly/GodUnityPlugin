using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GodUnityPlugin
{
    public class PointController : MonoBehaviour
    {
        public string id;

        [System.Serializable]
        public struct PercentEvent
        {
            [System.Serializable]
            public struct Gauge
            {
                [Range(0, 100)] public float percentage;
                public bool inclusive;
            }

            public string id;
            public Gauge max;
            public Gauge min;

            public UnityEvent onPhase;
            public UnityEvent onExit;
            public UnityEvent onEnter;
        }

        [System.Serializable]
        public struct ValueEvent
        {
            [System.Serializable]
            public struct Gauge
            {
                public float value;
                public bool inclusive;
            }

            public string id;
            public Gauge max;
            public Gauge min;

            public UnityEvent onPhase;
            public UnityEvent onExit;
            public UnityEvent onEnter;
        }

        public UnityEvent onValueChanged;
        public UnityEvent onValueIncreased;
        public UnityEvent onValueDecreased;

        public List<PercentEvent> percentEvents;
        public List<ValueEvent> valueEvents;
      
        public float initialValue;
        public float maxValue = 100f;
        public float minValue = 0.0f;

        public float Current => value;
        public float CurrentRatio => value / maxValue;

        private float value;

        public static PointController Get(GameObject target, string id)
        {
            PointController[] pointControllers = target.transform.root.GetComponentsInChildren<PointController>();

            PointController result = null;

            foreach (var pointController in pointControllers)
            {
                if (pointController.id == id)
                {
                    result = pointController;
                    break;
                }
            }

            return result;
        }

        public void Add(float value)
        {
            float prev = this.value;

            this.value = Mathf.Clamp(this.value += value, minValue, maxValue);

            float next = this.value;

            int compareIndicator = prev.CompareTo(next);
            if (compareIndicator != 0)
            {
                onValueChanged?.Invoke();
                if (compareIndicator < 0)
                {
                    onValueIncreased?.Invoke();
                }
                else
                {
                    onValueDecreased?.Invoke();
                }
            }

            if (percentEvents != null)
                foreach (var percentEvent in percentEvents)
                {
                    if (IsInBetween(this.value, percentEvent.max, percentEvent.min))
                        percentEvent.onPhase.Invoke();

                    if (!IsInBetween(prev, percentEvent.max, percentEvent.min) &&
                        IsInBetween(next, percentEvent.max, percentEvent.min))
                        percentEvent.onEnter.Invoke();

                    if (IsInBetween(prev, percentEvent.max, percentEvent.min) &&
                        !IsInBetween(next, percentEvent.max, percentEvent.min))
                        percentEvent.onExit.Invoke();
                }

            if (valueEvents != null)
                foreach (var valueEvent in valueEvents)
                {
                    if (IsInBetween(this.value, valueEvent.max, valueEvent.min))
                        valueEvent.onPhase.Invoke();

                    if (!IsInBetween(prev, valueEvent.max, valueEvent.min) &&
                        IsInBetween(next, valueEvent.max, valueEvent.min))
                        valueEvent.onEnter.Invoke();

                    if (IsInBetween(prev, valueEvent.max, valueEvent.min) &&
                        !IsInBetween(next, valueEvent.max, valueEvent.min))
                        valueEvent.onExit.Invoke();
                }
        }

        public void Set(float value)
        {
            this.value = Mathf.Clamp(value, minValue, maxValue);
        }

        public bool IsCurrentValueGreater(float value)
        {
            return GUPMath.IsGreater(Current, value);
        }

        public bool IsCurrentValueLesser(float value)
        {
            return GUPMath.IsLesser(Current, value);
        }

        private void OnEnable()
        {
            value = initialValue;
        }

        public void Reset()
        {
            value = initialValue;
        }

        private bool IsInBetween(float value, ValueEvent.Gauge max, ValueEvent.Gauge min)
        {
            if (max.inclusive)
            {
                if (value > max.value)
                    return false;
            }
            else if (value >= max.value)
                return false;

            if (min.inclusive)
            {
                if (value < min.value)
                    return false;
            }
            else if (value <= min.value)
                return false;

            return true;
        }

        private bool IsInBetween(float value, PercentEvent.Gauge max, PercentEvent.Gauge min)
        {
            if (max.inclusive)
            {
                if (value > (maxValue * max.percentage / 100f))
                    return false;
            }
            else if (value >= (maxValue * max.percentage / 100f))
                return false;

            if (min.inclusive)
            {
                if (value < (maxValue * min.percentage / 100f))
                    return false;
            }
            else if (value <= (maxValue * min.percentage / 100f))
                return false;

            return true;
        }
    }
}