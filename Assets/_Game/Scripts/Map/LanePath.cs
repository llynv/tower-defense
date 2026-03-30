using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Game.Map
{
    public sealed class LanePath
    {
        private readonly Vector3[] points;
        private readonly float[] cumulativeDistances;

        public LanePath(IReadOnlyList<Vector3> waypoints)
        {
            if (waypoints == null || waypoints.Count == 0)
                throw new ArgumentException("LanePath requires at least one waypoint.", nameof(waypoints));

            points = new Vector3[waypoints.Count];
            for (int i = 0; i < waypoints.Count; i++)
                points[i] = waypoints[i];

            cumulativeDistances = new float[points.Length];
            cumulativeDistances[0] = 0f;

            for (int i = 1; i < points.Length; i++)
                cumulativeDistances[i] = cumulativeDistances[i - 1] + Vector3.Distance(points[i - 1], points[i]);
        }

        public float TotalLength => cumulativeDistances[cumulativeDistances.Length - 1];

        public int PointCount => points.Length;

        public Vector3 EvaluatePosition(float progress)
        {
            if (points.Length == 1)
                return points[0];

            float clampedProgress = Mathf.Clamp01(progress);
            float targetDistance = clampedProgress * TotalLength;

            for (int i = 1; i < points.Length; i++)
            {
                if (targetDistance <= cumulativeDistances[i])
                {
                    float segmentStart = cumulativeDistances[i - 1];
                    float segmentLength = cumulativeDistances[i] - segmentStart;

                    if (segmentLength < Mathf.Epsilon)
                        return points[i];

                    float t = (targetDistance - segmentStart) / segmentLength;
                    return Vector3.Lerp(points[i - 1], points[i], t);
                }
            }

            return points[points.Length - 1];
        }
    }
}
