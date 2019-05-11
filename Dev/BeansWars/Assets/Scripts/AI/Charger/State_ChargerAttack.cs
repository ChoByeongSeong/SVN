using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using YellowBean;

public class State_ChargerAttack : State
{
    // 컨트롤러
    StateController_Charger sc;

    // 공격시간
    float currentTime = float.MaxValue;

    // 공격 모션이 끝났음을 확인한다.
    bool attackAnimComplete;

    // 공격 쿨타임을 계산한다.
    Coroutine checkTime;

    public State_ChargerAttack(StateController controller) : base(controller)
    {
        sc = (controller as StateController_Charger);
    }

    public override void Enter()
    {
        base.Enter();

        // 초기화
        controller.modelAnimEventer.attack = Attack;
        controller.modelAnimEventer.onComplete = OnComplete;

        attackAnimComplete = false;
    }

    public override IEnumerator Update()
    {
        if (!sc.target.alive)
        {
            sc.Restart();
            yield break;
        }

        sc.modelAnim.Play("idle");

        // 공격 가능한 시간이 되면.
        if (currentTime >= controller.unit.status.attack_speed)
        {
            if (checkTime != null)
            {
                controller.StopCoroutine(checkTime);
                checkTime = null;
            }

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
        Attacker attacker = new Attacker();
        attacker.damage = 100;
        sc.target.Hit(attacker);
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