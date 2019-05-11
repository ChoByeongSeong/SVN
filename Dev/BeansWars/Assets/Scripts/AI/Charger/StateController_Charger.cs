using UnityEngine;
using System.Collections;

public class StateController_Charger : StateController 
{
    // 유닛
    // 현재상태, 지난상태ㄴ

    // 상태 공용 데이터
    public Unit target;
    public Vector3 dashDir;
    public float viewRange;

    // 가지고 있는 상태
    public State_ChargerChase chaseSate;
    public State_ChargerDash dashState;
    public State_ChargerAttack attackState;

    // 길을 확인하는 변수
    public bool displayPath = true;

    public override void Awake()
    {
        base.Awake();

        // 스테이트를 생성한다.
        chaseSate = new State_ChargerChase(this);
        dashState = new State_ChargerDash(this);
        attackState = new State_ChargerAttack(this);

        firstState = chaseSate;
    }

    public override void Start()
    {
        base.Start();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, viewRange);

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
