using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventer : MonoBehaviour
{
    public System.Action attack;
    public System.Action onComplete;
    public System.Action dashEnd;
    public System.Action defence;

    public void Attack()
    {
        if (attack != null) attack();
    }

    public void Defence()
    {
        if (defence != null) defence();
    }

    public void DashEnd()
    {
        if (dashEnd != null) dashEnd();
    }

    public void OnComplete()
    {
        if (onComplete != null) onComplete();
    }
}
