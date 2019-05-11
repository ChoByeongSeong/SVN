using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using YellowBean;

public class State_ChargerChase : State
{
    StateController_Charger sc;

    public Path path;
    readonly float turnDst = 0.01f;
    readonly float stoppingDst = .1f;

    Coroutine follow;

    public State_ChargerChase(StateController controller) : base(controller)
    {
        sc = (controller as StateController_Charger);
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override IEnumerator Update()
    {
        // 적을 찾는다.
        List<Unit> enemies = FindEnemiesWithTag();

        // 조건. 적이 없다면.
        if (enemies.Count <= 0)
        {
            yield return new WaitForSeconds(sc.GetRandomDelay());
            yield break;
        }

        // 가장 가까운 적을 찾는다.
        sc.target = GetClosest(enemies);

        // 사거리내에 없다면.
        // 사이에 장애물이 있는지 확인한다.
        if (CheckAround())
        {
            PathRequestManager.RequestPath(new PathRequest(controller.Position, sc.target.transform.position, OnPathFound));
        }

        else
        {
            if (follow != null)
            {
                controller.StopCoroutine(follow);
                follow = null;
            }
            follow = sc.StartCoroutine(this.FollowTarget());
        }

        yield return new WaitForSeconds(sc.GetRandomDelay());

        sc.ChangeState(sc.chaseSate);
    }

    public override void Exit()
    {
        base.Exit();

        if (follow != null)
        {
            controller.StopCoroutine(follow);
            follow = null;
        }
    }

    List<Unit> FindEnemiesWithTag()
    {
        return GameObject.FindObjectsOfType<Unit>()
            .Where(enemy => enemy.alive)
            .Where(enemy => enemy.tag != controller.tag)
            .ToList();
    }

    Unit GetClosest(List<Unit> enemies)
    {
        // 가까운 적을 찾는다.
        enemies = enemies
            .OrderBy(e => (controller.Position - e.transform.position).sqrMagnitude)
            .ToList();

        // 시야 범위 안에.
        // 원거리 유닛이나, 스페셜 유닛이 있는지 확인한다.
        Unit targetUnit =
            enemies.Where(e => (e.transform.position - sc.transform.position).sqrMagnitude < sc.viewRange * sc.viewRange)
                   .Where(e => e.status.type == (int)YBEnum.eUnitType.Range || e.status.type == (int)YBEnum.eUnitType.Special)
                   .FirstOrDefault();

        if (targetUnit != null)
        {
            return targetUnit;
        }

        return enemies[0];
    }

    bool CheckAround()
    {
        // 타겟의 위치.
        Vector3 targetPos = sc.target.transform.position;

        // 검사할 레이어
        LayerMask layerMask = LayerMask.GetMask("Unwalkable");

        // 사이에 장애물이 있는지 검사한다.
        if (Physics2D.Linecast(controller.Position, targetPos, layerMask))
        {
            return true;
        }

        // 주위에 장애물이 있는지 검사한다.
        if (Physics2D.OverlapCircle(controller.Position, controller.cc2d.radius * 1.2f, layerMask))
        {
            return true;
        }

        return false;
    }

    void OnPathFound(Vector3[] waypoints, bool pathsuccessful)
    {
        if (!sc.unit.alive) return;
        if (!onState) return;
        if (!sc.unit.onAi) return;

        // 만약 길을 찾았다면
        if (pathsuccessful)
        {

            if (path != null)
            {
                PathRequestManager.instance.pathPool.PushPath(path);
                path = null;
            }
            path = PathRequestManager.instance.pathPool.GetPath(waypoints, controller.Position, turnDst, stoppingDst);

            if (follow != null)
            {
                controller.StopCoroutine(follow);
                follow = null;
            }

            follow = sc.StartCoroutine(this.FollowPath());
        }
    }

   


    IEnumerator FollowTarget()
    {
        if (!sc.unit.alive) yield break;
        if (!onState) yield break;
        if (!sc.unit.onAi) yield break;

        controller.modelAnim.Play("walk");

        while (true)
        {
            Vector3 targetPos = sc.target.transform.position;


            controller.transform.position = Vector3.MoveTowards(
                controller.Position,
                targetPos,
                controller.unit.status.move_speed * Time.deltaTime);

            // 왼쪽이 0
            int dir = (targetPos.x - controller.Position.x < 0) ? 0 : 1;
            controller.modelTr.rotation = Quaternion.Euler(new Vector3(0, 180 * dir, 0));

            if (!sc.target.alive)
            {
                sc.Restart();
                break;
            }

            yield return null;
        }
    }

    IEnumerator FollowPath()
    {
        controller.modelAnim.Play("walk");

        bool followingPath = true;
        int pathIndex = 0;

        while (followingPath)
        {
            Vector2 pos2D = new Vector2(controller.Position.x, controller.Position.y);

            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                controller.transform.position = Vector3.MoveTowards(
                    controller.Position,
                    path.nextPoints[pathIndex],
                    controller.unit.status.move_speed * Time.deltaTime);

                // 왼쪽이 0
                int dir = (path.nextPoints[pathIndex].x - controller.Position.x < 0) ? 0 : 1;
                controller.modelTr.rotation = Quaternion.Euler(new Vector3(0, 180 * dir, 0));
            }

            if (!sc.target.alive)
            {
                sc.Restart();
                break;

            }
            yield return null;
        }
    }
}

//bool CheckSight(Vector3 path)
//{
//    // true 를 반환하면. 이동이 멈춘다.

//    float dashRange = 2f;
//    float attackRange = sc.unit.status.attack_range;

//    // 대쉬 범위 안에 모든 적 유닛을 가져온다.
//    List<Collider2D> others =
//        Physics2D.OverlapCircleAll(sc.Position, dashRange, LayerMask.GetMask("Unit"))
//        .Where(c => c.tag != sc.tag)
//        .ToList();

//    return false;
//}
//// 만약. 콜라이더 안에 타겟이 있다면.
//// 공격하거나.
//// 이동한다.
//for (int i = 0; i < others.Length; i++)
//{
//    Collider2D other = others[i];

//    if (other == sc.target)
//    {
//        float taregetRadius = (other as CircleCollider2D).radius;

//        // 적과 내 거리.
//        float sqrDst = (controller.Position - sc.target.transform.position).sqrMagnitude;

//        // 내 공격 길이 + 적 반지름.
//        attackRange = attackRange + taregetRadius;

//        // 사거리 안에 있다면.
//        // 공격 스테이지로 전환한다.
//        if (sqrDst < attackRange * attackRange)
//        {
//            sc.StartState(sc.attackState);
//            return true;
//        }

//        return false;
//    }
//}

//// 내가 대쉬할 수 없으면. 이동만 한다.
//if (!sc.dashState.IsAttackable())
//    return false;

//// 콜라이더 안에 타겟이 없다면.
//// 대쉬하거나.
//// 이동한다.
//for (int i = 0; i < others.Length; i++)
//{
//    Collider2D other = others[i];

//    if (other.tag == sc.tag) continue;

//    Vector2 dirToPath = (path - sc.Position).normalized;
//    Vector3 targetPosition = other.transform.position;
//    Vector2 dirToTarget = (targetPosition - sc.Position).normalized;

//    // 타겟과의 각도를 비교한다.
//    float angleDeg = Mathf.Acos(Vector3.Dot(dirToPath, dirToTarget)) * Mathf.Rad2Deg;

//    if (angleDeg < 20)
//    {
//        // 자신과 타겟 사이에 장애물이 없으면,
//        if (!Physics2D.Linecast(sc.Position, path, LayerMask.GetMask("Unwalkable")))
//        {
//            sc.dashDir = dirToPath;
//            sc.StartState(sc.dashState);
//            return true;
//        }
//    }
//}

//return false;