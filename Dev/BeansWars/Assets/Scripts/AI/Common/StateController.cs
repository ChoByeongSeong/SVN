using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스테이트 컨트롤러는 유닛이 필요하다.
[RequireComponent(typeof(Unit))]

public class StateController : MonoBehaviour
{
    // 유닛
    [HideInInspector] public Unit unit;
    [HideInInspector] public CircleCollider2D cc2d;
    [HideInInspector] public Rigidbody2D rb2d;
    [HideInInspector] public Vector3 Position { get { return transform.position; } }

    // 모델
    [HideInInspector]public Transform    modelTr;
    [HideInInspector]public Animator     modelAnim;
    [HideInInspector] public AnimEventer  modelAnimEventer;

    // 상태
    protected State firstState;
    protected State currentState;
    protected State previusState;

    protected Coroutine updateStateRoutine;

    public virtual void Awake()
    {
        // 유닛을 가져온다
        unit = GetComponent<Unit>();
        cc2d = GetComponent<CircleCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    public virtual void Start()
    {
        // 사용할 모델의 컴포넌트를 설정한다.
        GameObject model = transform.Find("Model").gameObject;
        modelTr = model.GetComponent<Transform>();
        modelAnim = model.GetComponent<Animator>();
        modelAnimEventer = model.AddComponent<AnimEventer>();

        ChangeState(firstState);

        updateStateRoutine = StartCoroutine(Excute());

        ChaseUpdate();
    }

    public virtual void ChaseUpdate()
    {
    }

    public IEnumerator Excute()
    {
        yield return null;

        while (true)
        {
            if (unit.onAi)
            {
                yield return currentState.Update();
            }
            else
            {
                yield return null;
            }
        }
    }

    public void ChangeState(State newState)
    {
        if(currentState !=null)
        {
            currentState.Exit();
            previusState = currentState;
        }

        currentState = newState;
        currentState.Enter();
    }

    public void Restart()
    {
        if (updateStateRoutine != null)
        {
            StopCoroutine(updateStateRoutine);
            updateStateRoutine = null;
        }

        ChangeState(firstState);

        updateStateRoutine = StartCoroutine(Excute());
    }

    public void StartState(State state)
    {
        if (updateStateRoutine != null)
        {
            StopCoroutine(updateStateRoutine);
            updateStateRoutine = null;
        }

        ChangeState(state);

        updateStateRoutine = StartCoroutine(Excute());
    }

    public float GetRandomDelay()
    {
        float random = UnityEngine.Random.Range(0.3f, 0.6f);

        return random;
    }

}
