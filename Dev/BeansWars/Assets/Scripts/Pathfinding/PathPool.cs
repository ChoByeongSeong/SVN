using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace YellowBean
{
    public class PathPool
    {
        public Stack<Path> pathStack;
        int stackNum;

        public PathPool(int num)
        {
            stackNum = num;

            pathStack = new Stack<Path>();

            for (int i = 0; i < stackNum; i++)
            {
                pathStack.Push(new Path());
            }
        }

        public Path GetPath(Vector3[] waypoints, Vector3 startPos, float turnDst, float stoppingDst)
        {
            if (pathStack.Count <= 0)
            {
                for (int i = 0; i < stackNum; i++)
                {
                    pathStack.Push(new Path());
                }
            }

            Path newPath = pathStack.Pop();
            newPath.Initialize(waypoints, startPos, turnDst, stoppingDst);

            return newPath;
        }

        public void PushPath(Path path)
        {
            pathStack.Push(path);
        }
    }
}