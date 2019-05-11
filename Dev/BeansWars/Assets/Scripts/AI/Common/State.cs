using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public class State
{
    protected bool onState;
    protected StateController controller;

    public State(StateController controller)
    {
        this.controller = controller;
    }

    public virtual void Enter()
    {
        onState = true;
    }

    public virtual IEnumerator Update()
    {
        return null;
    }

    public virtual void Exit()
    {
        onState = false;
    }
}