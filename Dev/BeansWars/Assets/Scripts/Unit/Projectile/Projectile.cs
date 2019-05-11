using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public Unit owner;

    [HideInInspector]
    public Vector3 startPos;
    [HideInInspector]
    public Vector3 endPos;
}
