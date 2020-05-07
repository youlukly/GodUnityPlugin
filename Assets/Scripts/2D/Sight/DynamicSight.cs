using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    public class DynamicSight
    {
        [System.Serializable]
        public struct Sight
        {
            public Sight2D sight2D;
            public Color onAlert;
            public Color onDetect;
            public Color onNormal;
            [Range(0, 100)] public float sensitivity;
            public bool checkObstacles;
            public LayerMask sightObstacles;
        }

        private DynamicSightData dynamicSightData;
        private Forward forward;

        private Transform transform;

        private float alertTime;

        private float randomAlertTime;

        private float intervalTime;

        public GameObject Target { get; set; }
        public float AlertIncreaseSensitivity { get; set; }
        public float AlertDecreaseSensitivity { get; set; }

        public float DistToTarget
        {
            get
            {
                float dist = -1.0f;

                if (Target)
                    dist = Vector2.Distance(GetTargetPos(), transform.position);

                return dist;
            }
        }

        public DynamicSight(GameObject target, Forward forward, DynamicSightData dynamicSightData, Transform origin,
            bool drawGizmo)
        {
            this.Target = target;
            this.forward = forward;
            this.dynamicSightData = dynamicSightData;
            transform = origin;
            randomAlertTime = Random.Range(dynamicSightData.AlertTimeMin, dynamicSightData.AlertTimeMax);
            AlertIncreaseSensitivity = 1.0f;
            AlertDecreaseSensitivity = 1.0f;

            if (drawGizmo)
            {
                DynamicSightGizmo dynamicSightGizmo = origin.gameObject.AddComponent<DynamicSightGizmo>();
                dynamicSightGizmo.Init(this, dynamicSightData, forward);
            }
        }

        public DynamicSight(Forward forward, DynamicSightData dynamicSightData, Transform origin, bool drawGizmo)
        {
            this.forward = forward;
            this.dynamicSightData = dynamicSightData;
            transform = origin;
            randomAlertTime = Random.Range(dynamicSightData.AlertTimeMin, dynamicSightData.AlertTimeMax);
            AlertIncreaseSensitivity = 1.0f;
            AlertDecreaseSensitivity = 1.0f;

            if (drawGizmo)
            {
                DynamicSightGizmo dynamicSightGizmo = origin.gameObject.AddComponent<DynamicSightGizmo>();
                dynamicSightGizmo.Init(this, dynamicSightData, forward);
            }
        }

        public bool IsTargetInSight()
        {
            float sensitivity = 0.0f;

            return IsTargetInSight(out sensitivity);
        }

        public bool IsSearching()
        {
            if (!Target || !Target.activeSelf)
                return false;

            if (dynamicSightData.Sights == null || dynamicSightData.Sights.Length == 0)
                return false;

            if (IsAlerted())
                return false; 

            return alertTime > 0;
        }

        public void AddAlert(float value)
        {
            //민감도에 따라 경계 게이지 증가

            if (value < 0.0f)
                alertTime += value * AlertDecreaseSensitivity;
            else
                alertTime += value * AlertIncreaseSensitivity;

            Mathf.Clamp(alertTime, 0.0f, dynamicSightData.AlertTimeMax);

            if (alertTime >= randomAlertTime)
            {
                StartAlert();
            }
        }

        public void StartAlert()
        {
            alertTime = randomAlertTime;
            intervalTime = dynamicSightData.AlertExitInterval;
        }

        public void AddAlertRaw(float value)
        {
            //민감도 상관없이 경계 게이지 증가
            alertTime += value;
            Mathf.Clamp(alertTime, 0.0f, dynamicSightData.AlertTimeMax);
        }

        public bool InSight(Sight sight)
        {
            if (
                sight.sight2D.IsInSight(transform.position, GetTargetPos(),
                    forward.NormalizeToForward(sight.sight2D.baseDirection))
            )
            {
                if (sight.checkObstacles)
                    return !CheckWallBetweenTarget(sight.sight2D, sight.sightObstacles);

                return true;
            }

            return false;
        }

        public bool CheckWallBetweenTarget(Sight2D sight2D, LayerMask obstacles)
        {
            Vector2 dir = GetTargetPos() - new Vector2(transform.position.x, transform.position.y);

            float dist = (transform.position - Target.transform.position).magnitude;

            RaycastHit2D raycastHit2D;

            if (sight2D.CheckObstacle(transform.position, dir, obstacles, out raycastHit2D))
                return dist > raycastHit2D.distance;

            return false;
        }

        public Vector2 GetTargetPos()
        {
            if (!Target || !Target.activeSelf)
                return new Vector2();

            return Target.transform.position;
        }

        public void ManualUpdate()
        {
            Search();
        }

        private void Search()
        {
            float sensitivity = 1.0f;

            if (IsTargetInSight(out sensitivity))
            {
                AddAlert(Time.deltaTime * sensitivity);
            }
            else if (!IsTargetInSight(out sensitivity))
            {
                if (intervalTime < 0.0f)
                {
                    randomAlertTime = Random.Range(dynamicSightData.AlertTimeMin, dynamicSightData.AlertTimeMax);
                    intervalTime = 0.0f;
                    alertTime = 0.0f;
                }
                else if (intervalTime > 0.0f)
                    intervalTime -= Time.deltaTime * AlertDecreaseSensitivity;

                if (alertTime > 0.0f)
                    AddAlert(-Time.deltaTime);
                else if (alertTime < 0.0f)
                    alertTime = 0.0f;
            }
        }

        public bool IsAlerted()
        {
            if (Target && Target.activeSelf)
                return intervalTime > 0.0f;
            else
                return false;
        }

        private bool IsTargetInSight(out float sensitivity)
        {
            sensitivity = 0.0f;

            if (!Target || !Target.activeSelf)
                return false;

            if (dynamicSightData.Sights == null || dynamicSightData.Sights.Length == 0)
                return false;

            for (int i = 0; i < dynamicSightData.Sights.Length; i++)
            {
                Sight sight = dynamicSightData.Sights[i];

                if (InSight(sight) && sensitivity < sight.sensitivity)
                {
                    sensitivity = sight.sensitivity;
                }
            }

            return sensitivity != 0f;
        }
    }
}