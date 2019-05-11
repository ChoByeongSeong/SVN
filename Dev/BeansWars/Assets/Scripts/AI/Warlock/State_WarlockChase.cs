using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using YellowBean;

public class State_WarlockChase : State
{
    StateController_Warlock sc;

    public Path path;
    readonly float turnDst = 0;
    readonly float stoppingDst = .1f;

    Coroutine follow;

    public State_WarlockChase(StateController controller) : base(controller)
    {
        sc = (controller as StateController_Warlock);
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
        Unit target = GetClosest(enemies);

        sc.target = target;

        // 조건. 사거리 내에 있는지 확인한다.
        // 사거리 내에 있다면, 상태를 변경한다.
        if (IsInAttackrange(sc.target))
        {
            // 왼쪽이 0
            int dir = (sc.target.transform.position.x - controller.Position.x < 0) ? 0 : 1;
            controller.modelTr.rotation = Quaternion.Euler(new Vector3(0, 180 * dir, 0));

            // 소환 하거나 공격한다.
            if (sc.summonState.IsSummonable())
            {
                sc.ChangeState(sc.summonState);
                yield break;
            }

            else
            {
                sc.ChangeState(sc.attackState);
                yield break;
            }
        }

        // 사거리내에 없다면.
        // 사이에 장애물이 있는지 확인한다.
        if (CheckAround())
        {
            PathRequestManager.RequestPath(new PathRequest(controller.Position, target.transform.position, OnPathFound));
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
        var range = UnityEngine.Random.Range(0, 100);

        if (range <= 10)
        {
            if (YellowBean.SoundManager.Instance != null)
            {
                if (sc.tag.CompareTo("Yellow") == 0)
                {
                    YellowBean.SoundManager.Instance.PlaySFX(string.Format("y_battle{0}", UnityEngine.Random.Range(0, 7)));
                }
                else
                {
                    YellowBean.SoundManager.Instance.PlaySFX(string.Format("g_battle{0}", UnityEngine.Random.Range(0, 9)));
                }
            }
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
        /* 일정 시야 범위 안에 있는 녀석 증에
         * 타겟티드 포인트가 낮은녀석이 타겟이다.
         */
        enemies = enemies
            .OrderBy(e => (controller.Position - e.transform.position).sqrMagnitude)
            .ToList();

        // 가까운 순 중에.
        // 시야 범위안에.
        // 타겟티드 포인트가 적은순.
        List<Unit> inRangeEnemies = enemies.Where(e => IsInAttackrange(e)).ToList();

        for (int i = 0; i < inRangeEnemies.Count; i++)
        {
            if(inRangeEnemies[i].targetedCnt < 3)
            {
                if(sc.target !=null)
                {
                    sc.target.targetedCnt--;
                }

                inRangeEnemies[i].targetedCnt++;
                return inRangeEnemies[i];
            }
        }

        if (sc.target != null)
        {
            sc.target.targetedCnt--;
        }
        return enemies.FirstOrDefault();
    }

    bool IsInAttackrange(Unit target)
    {
        // 타겟의 위치.
        Vector3 targetPos = target.transform.position;

        // 타겟의 콜라이더 크기.
        // 콜라이더가 없다면, 0이다.
        CircleCollider2D circleCollider2D = target.GetComponent<CircleCollider2D>();
        float taregetRadius = (circleCollider2D != null) ? circleCollider2D.radius : 0;

        // 적과 내 거리.
        float sqrDst = (controller.Position - targetPos).sqrMagnitude;

        // 내 공격 길이 + 적 반지름.
        float attackRange = controller.unit.status.attack_range + taregetRadius;

        // 사거리 안에 있다면.
        // 공격 스테이지로 전환한다.
        if (sqrDst < attackRange * attackRange)
        {
            return true;
        }

        return false;
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