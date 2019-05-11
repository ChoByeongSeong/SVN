using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YellowBean
{
    public class Path
    {
        public Vector3[] nextPoints;
        public Line[] turnBoundaries;
        public int finishLineIndex;
        public int slowDownIndex;

        public void Initialize(Vector3[] waypoints, Vector3 startPos, float turnDst, float stoppingDst)
        {
            nextPoints = waypoints;
            turnBoundaries = new Line[nextPoints.Length];
            finishLineIndex = turnBoundaries.Length - 1;

            Vector2 previousPoint = V3ToV2(startPos);
            for (int i = 0; i < nextPoints.Length; i++)
            {
                Vector2 currentPoint = V3ToV2(nextPoints[i]);
                Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
                Vector2 turnBoundaryPoint = (i == finishLineIndex) ? currentPoint : currentPoint - dirToCurrentPoint * turnDst;
                turnBoundaries[i] = new Line(turnBoundaryPoint, previousPoint - dirToCurrentPoint * turnDst);
                previousPoint = turnBoundaryPoint;
            }

            float dstFromEndPoint = 0;
            for (int i = nextPoints.Length - 1; i > 0; i--)
            {
                dstFromEndPoint += Vector3.Distance(nextPoints[i], nextPoints[i - 1]);
                if (dstFromEndPoint > stoppingDst)
                {
                    slowDownIndex = i;
                    break;
                }
            }
        }

        Vector2 V3ToV2(Vector3 v3)
        {
            return new Vector2(v3.x, v3.y);
        }

        public void DrawWithGizmos()
        {
            Gizmos.color = Color.red;
            foreach (Vector3 p in nextPoints)
            {
                Gizmos.DrawSphere(p, .1f);
            }

            Gizmos.color = Color.white;
            foreach (Line l in turnBoundaries)
            {
                l.DrawWithGizmos(1);
            }
        }
    }
}