using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using YellowBean;

public class State_ChargerDash : State
{
    // 컨트롤러
    StateController_Charger sc;

    // 공격 모션이 끝났음을 확인한다.
    Coroutine dash;
    bool attackAnimComplete;
    bool dashing;

    // 공격 쿨타임을 계산한다.
    float currentTime = float.MaxValue;
    readonly float limitTime = 5f;

    Coroutine checkTime;

    public State_ChargerDash(StateController controller) : base(controller)
    {
        sc = (controller as StateController_Charger);
    }

    public override void Enter()
    {
        base.Enter();

        // 초기화
        controller.modelAnimEventer.attack = Attack;
        controller.modelAnimEventer.onComplete = OnComplete;
        controller.modelAnimEventer.dashEnd = DashEnd;

        attackAnimComplete = false;
        dashing = false;
    }

    public override IEnumerator Update()
    {
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

        currentTime = 0;

        AttackStart();

        yield return new WaitUntil(() => attackAnimComplete);

        // 공격이 끝나면 다시 시간을 계산한다.
        checkTime = sc.StartCoroutine(CheckTime());

        sc.ChangeState(sc.chaseSate);

        yield break;
    }

    public override void Exit()
    {
        base.Exit();

        if (dash != null)
        {
            sc.StopCoroutine(dash);
            dash = null;
        }

        controller.modelAnimEventer.attack = null;
        controller.modelAnimEventer.onComplete = null;
    }

    void AttackStart()
    {
        sc.modelAnim.Play("dash");
    }

    void Attack()
    {
        if (dash != null)
        {
            sc.StopCoroutine(dash);
            dash = null;
        }
        dash = sc.StartCoroutine(Dash());
    }

    IEnumerator Dash()
    {
        dashing = true;

        while (dashing)
        {
            Vector3 dashDir = sc.dashDir + controller.Position;

            controller.transform.position = Vector3.MoveTowards(
                controller.Position,
                dashDir,
                controller.unit.status.move_speed * 4f * Time.deltaTime);

            // 왼쪽이 0
            int dir = (dashDir.x - controller.Position.x < 0) ? 0 : 1;
            controller.modelTr.rotation = Quaternion.Euler(new Vector3(0, 180 * dir, 0));

            if (!sc.target.alive)
            {
                sc.Restart();
                break;
            }

            yield return null;
        }
    }

    void DashEnd()
    {
        dashing = false;
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

    public bool IsAttackable()
    {
        return currentTime >= limitTime;
    }
}