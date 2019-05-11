using UnityEngine;
using System.Collections;

public class StateController_Defender : StateController 
{
    // 유닛
    // 현재상태, 지난상태

    // 상태 공용 데이터
    public Unit target;

    public float speedReductionAmount = 0.5f;
    public float massIncreaseAmount = 2f;
    public float armorAmount = 30f;

    [HideInInspector]
    public float speedPercent;
    public float MoveSpeed
    {
        get
        {
            return unit.status.move_speed* speedPercent;
        }
    }

    // 가지고 있는 상태
    public State_DefenderChase chaseSate;

    // 길을 확인하는 변수
    public bool displayPath = true;

    public override void Awake()
    {
        base.Awake();

        // 스테이트를 생성한다.
        chaseSate = new State_DefenderChase(this);

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
