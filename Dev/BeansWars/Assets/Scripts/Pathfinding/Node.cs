﻿using UnityEngine;
using System.Collections;

namespace YellowBean
{
    public class Node : IHeapItem<Node>
    {

        public bool walkable;
        public Vector3 worldPosition;
        public int gridX;
        public int gridY;
        public int movementPenalty;

        public int gCost;
        public int hCost;
        public Node parent;
        int heapIndex;

        public Node(bool walkable, Vector3 worldPos, int gridX, int gridY, int penalty)
        {
            this.walkable = walkable;
            worldPosition = worldPos;
            this.gridX = gridX;
            this.gridY = gridY;
            movementPenalty = penalty;
        }

        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        public int HeapIndex
        {
            get
            {
                return heapIndex;
            }
            set
            {
                heapIndex = value;
            }
        }

        public int CompareTo(Node nodeToCompare)
        {
            int compare = fCost.CompareTo(nodeToCompare.fCost);
            if (compare == 0)
            {
                compare = hCost.CompareTo(nodeToCompare.hCost);
            }
            return -compare;
        }
    }
}