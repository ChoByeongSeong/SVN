using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class State_WarlockSummon : State
{
    StateController_Warlock sc;

    // 소환관련 시간.
    readonly float limitTime = 4.5f;
    float currentTime = float.MaxValue;

    bool attackAnimComplete;

    Coroutine checkTime;

    public State_WarlockSummon(StateController controller) : base(controller)
    {
        sc = (controller as StateController_Warlock);
    }

    public override void Enter()
    {
        base.Enter();

        // 초기화
        controller.modelAnimEventer.attack = Summon;
        controller.modelAnimEventer.onComplete = OnComplete;

        attackAnimComplete = false;
    }

    public override IEnumerator Update()
    {
        /* 공격 가능한 시간이 되면 공격하고.
         * 추격 스테이트로 간다.
         * 
         * 공격이 불가능 하다면.
         * 시간을 더하고 추격 스테이트로 간다.
         */

        if (!sc.target.alive)
        {
            sc.Restart();
            yield break;
        }

        if (checkTime != null)
        {
            controller.StopCoroutine(checkTime);
            checkTime = null;
        }

        currentTime = 0f;

        // 공격을 한다.
        SummonStart();

        // 공격이 끝날때 까지 기다린다.
        yield return new WaitUntil(() => attackAnimComplete);

        // 공격이 끝나면 다시 시간을 계산한다.
        checkTime = sc.StartCoroutine(CheckTime());

        // 스테이지를 변경하고 끝낸다.
        controller.ChangeState(sc.chaseSate);

        yield break;
    }

    public override void Exit()
    {
        base.Exit();
    }

    void SummonStart()
    {
        controller.modelAnim.Play("attack");
    }

    void Summon()
    {
        YBEnum.eColorType color = UnitColor.PaseToEnum(sc.tag.ToString());

        // 소환을 한다.
        UnitsPool.instance.SummonMinion(sc.tag, sc.Position);
        UnitsPool.instance.SummonMinion(sc.tag, sc.Position);
        UnitsPool.instance.SummonMinion(sc.tag, sc.Position);
    }

    void OnComplete()
    {
        attackAnimComplete = true;

        controller.modelAnim.Play("idle");
    }

    IEnumerator CheckTime()
    {
        while (true)
        {
            currentTime += Time.deltaTime;

            yield return null;
        }
    }

    public bool IsSummonable()
    {
        return currentTime >= limitTime;
    }
}