using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class State_WarlockAttack : State
{
    StateController_Warlock sc;

    float currentTime = float.MaxValue;

    bool attackAnimComplete;

    Coroutine checkTime;

    public State_WarlockAttack(StateController controller) : base(controller)
    {
        sc = (controller as StateController_Warlock);
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
        if (!sc.target.alive)
        {
            attackAnimComplete = true;
            return;
        }

        controller.modelAnim.Play("attack");
    }

    void Attack()
    {
        currentTime = 0f;
        if (YellowBean.SoundManager.Instance != null)
        {
            YellowBean.SoundManager.Instance.PlaySFX("wizard");
            var range = UnityEngine.Random.Range(0, 100);

            if (range <= 20)
            {
                if (sc.tag.CompareTo("Yellow") == 0)
                {
                    YellowBean.SoundManager.Instance.PlaySFX(string.Format("y_attack{0}", Random.Range(0, 21)));
                }
                else
                {
                    YellowBean.SoundManager.Instance.PlaySFX(string.Format("g_attack{0}", Random.Range(0, 21)));
                }
            }
        }

        if (!sc.target.alive)
        {
            return;
        }

        Vector3 startPos = sc.unit.transform.position;
        Vector3 endPos = sc.target.transform.position;
        ProjectilesPool.instance.ShootMagic(sc.unit, sc.target, startPos, endPos);
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