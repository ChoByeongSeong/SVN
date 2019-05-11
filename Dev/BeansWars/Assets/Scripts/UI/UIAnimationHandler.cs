using UnityEngine;
using System.Collections;
using System;

public class UIAnimationHandler : MonoBehaviour
{
    Animator anim;
    Action onComplete;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Play(string animName, Action onComplete)
    {
        anim.Play(animName);

        this.onComplete = onComplete;
    }

    void OnComplete()
    {
        if(onComplete != null) onComplete();
    }
}
