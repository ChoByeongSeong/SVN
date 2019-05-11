using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stone : Projectile
{
    [Range(0, 2)]
    public float colRad;
    public float attackRange;

    Transform modelTransform;
    SpriteRenderer modelRenderer;

    Vector3 velocity;

    bool alive;
    public float aliveTime;
    public int destroySpeed;
    public int sortingOrder;

    private void Awake()
    {
        // 모델 게임오브젝트를 찾는다.
        GameObject model = transform.Find("Model").gameObject;
        modelTransform = model.transform;
        modelRenderer = model.GetComponent<SpriteRenderer>();
    }

    public void Initialize(Unit owner, Vector2 startPos, Vector2 endPos)
    {
        base.owner = owner;
        tag = owner.tag;

        // 타입 초기화
        alive = true;
        transform.position = startPos;
        modelRenderer.sortingOrder = int.MaxValue;
        modelRenderer.color = Color.white;

        // 위치를 저장한다.
        base.startPos = startPos;
        base.endPos = endPos;

        StopCoroutine("Shoot");
        StartCoroutine("Shoot");
    }

    private void Shoot()
    {
        // 생성 소멸에 관한 코루틴
        // Y 값이 낮아지면
        // 최대 5초
        StopCoroutine("Destroy");
        StartCoroutine("Destroy");

        // 이동 회전에 관한 루틴
        StopCoroutine("Fire");
        StartCoroutine("Fire");

        // 충돌에 관한 루틴
        StopCoroutine("Collusion");
        StartCoroutine("Collusion");
    }

    IEnumerator Collusion()
    {
        /* 목적지 근처일 때 만 충 돌 체크 한다.
         */

        while (alive)
        {
            // 떨어질 때만 계산한다.
            if (velocity.y < 0)
            {
                // 유닛 마스크
                Collider2D col = Physics2D.OverlapCircle(transform.position, colRad, LayerMask.GetMask("Unit"));

                // 충돌했다.
                if (col != null)
                {
                    // 적이랑 충돌했다.
                    if (tag != col.gameObject.tag)
                    {
                        alive = false;

                        // 적을 찾는다.
                        Unit target = col.GetComponent<Unit>();

                        // 적의 위치를 찾는다.
                        Vector3 targetPos = target.transform.position;

                        // 적 위치에 이펙트를 처리한다.
                        EffectsPool.instace.CreateEffect("Effect_DownSmoke", targetPos, 1);

                        // 공격범위를 설정한다.
                        float attackRadius = attackRange;

                        // 다시 한번 충돌 검사를 한다.
                        LayerMask unitMask = LayerMask.GetMask("Unit");

                        // 적들을 가져온다.
                        List<Collider2D> cols = Physics2D.OverlapCircleAll(targetPos, attackRadius, unitMask)
                            .Where(c => c.tag != owner.tag)
                            .ToList();

                        for (int i = 0; i < cols.Count; i++)
                        {
                            Unit enemy = cols[i].GetComponent<Unit>();

                            Vector2 dir = (enemy.transform.position - targetPos).normalized;
                            enemy.rb2d.AddForce(dir * 15000);

                            Attacker attacker = new Attacker
                            {
                                damage = owner.status.attack_damage
                            };

                            enemy.Hit(attacker);
                        }

                        //// 타겟에게 데미지를 준다.
                        //Unit target = col.GetComponent<Unit>();

                        //Attacker attacker = new Attacker
                        //{
                        //    damage = owner.status.attack_damage
                        //};

                        //target.Hit(attacker);
                    }
                }
            }

            yield return new WaitForSeconds(0.015f);
        }
    }

    IEnumerator Destroy()
    {
        /* y 값이 스톱 Y보다 작아지면.
         * 움직임을 멈춘다.
         * 
         * 발사된지 일정시간이 지나가면 멈춘다.
         */

        alive = true;
        // float currentTime = 0f;

        yield return null;

        // 타겟포지션에 도착 할 때까지 살아 있는다.
        while (alive)
        {
            if (transform.position.y <= endPos.y - 0.3f && velocity.y < 0)
            {
                alive = false;

                // 떨어진 위치를 찾는다.
                Vector3 targetPos = transform.position;

                // 적 위치에 이펙트를 처리한다.
                EffectsPool.instace.CreateEffect("Effect_DownSmoke", targetPos, 0.7f);

                // 공격범위를 설정한다.
                float attackRadius = 1.5f;

                // 다시 한번 충돌 검사를 한다.
                LayerMask unitMask = LayerMask.GetMask("Unit");

                // 적들을 가져온다.
                List<Collider2D> cols = Physics2D.OverlapCircleAll(targetPos, attackRadius, unitMask)
                    .Where(c => c.tag != owner.tag)
                    .ToList();

                for (int i = 0; i < cols.Count; i++)
                {
                    Unit enemy = cols[i].GetComponent<Unit>();

                    Vector2 dir = (enemy.transform.position - targetPos).normalized;
                    enemy.rb2d.AddForce(dir * 15000);

                    Attacker attacker = new Attacker
                    {
                        damage = owner.status.attack_damage
                    };

                    enemy.Hit(attacker);
                }

            }

            yield return new WaitForSeconds(0.015f);
        }

        // 도착 했다면
        //while (true)
        //{
        //    if (currentTime > aliveTime)
        //    {
        //        break;
        //    }

        //    currentTime += Time.deltaTime;

        //    yield return new WaitForSeconds(0.015f);
        //}

        while (true)
        {
            modelRenderer.color -= new Color(0, 0, 0, destroySpeed * Time.deltaTime);
            yield return null;

            if (modelRenderer.color.a <= 0.1f)
            {
                ProjectilesPool.instance.DestroyStone(this);
                break;
            }
        }
    }

    IEnumerator Fire()
    {
        // 목적지 값이 다를수도 있기 때문에.
        // 임시로 endPos를 설정한다.
        Vector3 newEndPos = endPos;

        velocity = Vector3.zero;

        // 근의 공식을 구하기 위한 변수

        // 거리에 따른 보정 + 높이에 따른 보정
        float dX = Mathf.Abs(startPos.x - newEndPos.x) * 0.3f;
        float dy = (newEndPos.y - startPos.y) * 0.3f;
        dy = dy < 0 ? 0 : dy;

        float maxH = 2 + dX + dy;

        float dextH = newEndPos.y - startPos.y;
        float g = 9.8f;

        velocity.y = Mathf.Sqrt(2 * g * maxH);

        float a = g;
        float b = -2 * velocity.y;
        float c = 2 * dextH;

        float sqrt = Mathf.Sqrt((b * b) - (4 * a * c));
        float endTime = (-b + sqrt) / (2 * a);

        velocity.x = -(startPos.x - newEndPos.x) / endTime;

        // 임시 에러 처리.
        {
            if (float.IsNaN(velocity.x))
            {
                float dstX = newEndPos.x - startPos.x;
                dy = Mathf.Abs(newEndPos.y - startPos.y) * 0.55f;
                dy = dy < 1 ? 1 : dy;

                newEndPos.y = startPos.y;
                newEndPos.x += dstX / 3;

                // 거리에 따른 보정 + 높이에 따른 보정
                dX = Mathf.Abs(startPos.x - newEndPos.x) * 0.55f;
                dX = dX < 1 ? 1 : dX;

                maxH = 3.2f + dX + dy;

                // 근의 공식을 구하기 위한 변수
                dextH = newEndPos.y - startPos.y;
                g = 9.8f;

                velocity.y = Mathf.Sqrt(2 * g * maxH);

                a = g;
                b = -2 * velocity.y;
                c = 2 * dextH;

                sqrt = Mathf.Sqrt((b * b) - (4 * a * c));
                endTime = (-b + sqrt) / (2 * a);

                velocity.x = -(startPos.x - newEndPos.x) / endTime;
            }
        }

        float rotateSpeed = Random.Range(100, 300);
        int modelDir = (modelTransform.rotation.y ==0)? -1 : 1;
        
        while (alive)
        {
            var dir = velocity.normalized;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // modelTransform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            modelTransform.Rotate(new Vector3(0, 0, rotateSpeed * modelDir *Time.deltaTime));

            // 화살의 이동
            {
                velocity.y -= g * Time.deltaTime;
                Vector3 newPos = transform.position + velocity * Time.deltaTime;

                if (!float.IsNaN(newPos.x))
                    transform.position = newPos;
            }

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, colRad);
    }
}
