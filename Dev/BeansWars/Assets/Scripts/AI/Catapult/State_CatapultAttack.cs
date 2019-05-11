using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class State_CatapultAttack : State
{
    StateController_Catapult sc;

    float currentTime = float.MaxValue;

    bool attackAnimComplete;

    Coroutine checkTime;

    public State_CatapultAttack(StateController controller) : base(controller)
    {
        sc = (controller as StateController_Catapult);
    }

    public override void Enter()
    {
        base.Enter();

        // 초기화
        sc.modelAnimEventer.attack = Attack;
        sc.modelAnimEventer.onComplete = OnComplete;

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

        controller.modelAnim.Play("idle");

        // 공격 가능한 시간이 되면.
        if (currentTime >= controller.unit.status.attack_speed)
        {
            if (checkTime != null)
            {
                controller.StopCoroutine(checkTime);
                checkTime = null;
            }

            currentTime = 0f;

            // 공격을 한다.
            AttackStart();

            // 공격이 끝날때 까지 기다린다.
            yield return new WaitUntil(() => attackAnimComplete);

            // 공격이 끝나면 다시 시간을 계산한다.
            checkTime = sc.StartCoroutine(CheckTime());
        }

        // 스테이지를 변경하고 끝낸다.
        controller.ChangeState(sc.chaseSate);

        yield break;
    }

    public override void Exit()
    {
        base.Exit();
    }

    void AttackStart()
    {
        controller.modelAnim.Play("attack");
    }

    void Attack()
    {
        if (YellowBean.SoundManager.Instance != null)
        {
            YellowBean.SoundManager.Instance.PlaySFX("catapult");
        }

        if (!sc.target.alive)
        {
            return;
        }

        Vector3 startPos = sc.stoneTr.transform.position;
        Vector3 endPos = sc.target.transform.position;
        ProjectilesPool.instance.ShootStone(sc.unit, startPos, endPos);
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
}