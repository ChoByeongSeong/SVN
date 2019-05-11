using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

namespace YellowBean
{
    [RequireComponent(typeof(Pathfinding))]
    public class PathRequestManager : MonoBehaviour
    {
        Queue<PathResult> results = new Queue<PathResult>();

        public static PathRequestManager instance;
        Pathfinding pathfinding;

        public PathPool pathPool = new PathPool(100);

        void Awake()
        {
            instance = this;
            pathfinding = GetComponent<Pathfinding>();
        }

        void Update()
        {

            if (results.Count > 0)
            {
                int itemsInQueue = results.Count;
                lock (results)
                {
                    for (int i = 0; i < itemsInQueue; i++)
                    {
                        PathResult result = results.Dequeue();
                        result.callback(result.path, result.success);
                    }
                }
            }

        }

        public static void RequestPath(PathRequest request)
        {
            ThreadStart threadStart = delegate
            {
                instance.pathfinding.FindPath(request, instance.FinishedProcessingPath);
            };
            threadStart.Invoke();
        }

        public void FinishedProcessingPath(PathResult result)
        {
            lock (results)
            {
                results.Enqueue(result);
            }
        }
    }
}

    public struct PathResult
    {
        public Vector3[] path;
        public bool success;
        public Action<Vector3[], bool> callback;

        public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
        {
            this.path = path;
            this.success = success;
            this.callback = callback;
        }

    }

    public struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 startPos, Vector3 endPos, Action<Vector3[], bool> callback)
        {
            pathStart = startPos;
            pathEnd = endPos;
            this.callback = callback;
        }

    }

