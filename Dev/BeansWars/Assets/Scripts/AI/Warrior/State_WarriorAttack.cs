using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class State_WarriorAttack : State
{
    StateController_Warrior sc;

    float currentTime = float.MaxValue;

    bool attackAnimComplete;

    Coroutine checkTime;

    public State_WarriorAttack(StateController controller) : base(controller)
    {
        sc = (controller as StateController_Warrior);
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

        if (YellowBean.SoundManager.Instance != null)
        {
            YellowBean.SoundManager.Instance.PlaySFX("sword");
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
    }

    void Attack()
    {
        if (!sc.target.alive)
        {
            return;
        }

        Attacker attacker;
        attacker.damage = sc.unit.status.attack_damage;

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