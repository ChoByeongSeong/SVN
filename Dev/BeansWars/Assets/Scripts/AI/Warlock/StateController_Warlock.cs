using UnityEngine;
using UnityEditor;
using System.Collections;

public class StateController_Warlock : StateController
{
    // 유닛
    // 현재상태, 지난상태

    // 상태 공용 데이터
    public Unit target;

    // 가지고 있는 상태
    public State_WarlockChase chaseSate;
    public State_WarlockAttack attackState;
    public State_WarlockSummon summonState;

    // 길을 확인하는 변수
    public bool displayPath = true;

    public override void Awake()
    {
        base.Awake();

        // 스테이트를 생성한다.
        chaseSate = new State_WarlockChase(this);
        attackState = new State_WarlockAttack(this);
        summonState = new State_WarlockSummon(this);

        // 첫번째 스테이트를 설정한다.
        firstState = chaseSate;
    }

    public override void Start()
    {
        base.Start();
    }

    private void OnDrawGizmos()
    {
        if (chaseSate == null) return;

        if (chaseSate.path != null && displayPath)
        {
            chaseSate.path.DrawWithGizmos();
        }
    }

    public override void ChaseUpdate()
    {
        StartCoroutine(ChaseUpdateImpl());
    }

    IEnumerator ChaseUpdateImpl()
    {
        yield return chaseSate.Update();
    }
}