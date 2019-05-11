using UnityEngine;
using System.Collections;

public class StateController_Golem : StateController 
{
    // 유닛
    // 현재상태, 지난상태

    // 상태 공용 데이터
    public Unit target;

    // 가지고 있는 상태

    // 길을 확인하는 변수
    public bool displayPath = true;
    public State_GolemChase chaseSate;
    public State_GolemAttack attackState;

    public override void Awake()
    {
        base.Awake();

        // 스테이트를 생성한다.
        chaseSate = new State_GolemChase(this);
        attackState = new State_GolemAttack(this);

        firstState = chaseSate;
    }

    public override void Start()
    {
        base.Start();
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
