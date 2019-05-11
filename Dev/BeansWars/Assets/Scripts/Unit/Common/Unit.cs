using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

public class Unit : MonoBehaviour
{
    // 데이터
    public UnitData status;

    // 변수
    public bool alive = true;
    public bool onAi;
    public float size;

    public bool isTest;

    // 컴포넌트
    public CircleCollider2D cc;
    public Rigidbody2D rb2d;

    // 궁수의 표적이 된다.
    public int targetedCnt;

    // Display
    public bool displayStatus = true;

    // 코루틴
    Coroutine destroy;

    // 모델 트랜스폼
    Transform modelTr;

    Transform ModelTr
    {
        get
        {
            if (modelTr == null)
            {
                modelTr = transform.Find("Model").transform;
            }

            return modelTr;
        }
    }
          
    private void Awake()
    {
        if (status != null)
        {
            status = new UnitData(status);
        }

        cc = GetComponent<CircleCollider2D>();
        cc.radius = size;

        rb2d = GetComponent<Rigidbody2D>();
    }

    private void OnDrawGizmos()
    {
        // 상태를 표시한다.
        if (displayStatus && status != null)
        {
            Gizmos.DrawWireSphere(transform.position, status.attack_range);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, size);
    }

    public void Hit(Attacker attacker)
    {
        if (status.name.CompareTo("Defender") == 0)
        {
            int rnd = UnityEngine.Random.Range(0, 100);
            if (rnd < status.armor)
            {
                // 막는 곳

                var rtShield = ModelTr.Find("body").Find("shield");

                bool isReverse = modelTr.rotation.y != 0;

                // 적 위치에 이펙트를 처리한다.
                Vector3 shiedlPos = rtShield.position + new Vector3(0, 1f, 0);
                EffectsPool.instace.CreateEffect("Defence", shiedlPos, 1, isReverse);

                return;
            }
        }

        // 맞는 곳
        var range = UnityEngine.Random.Range(0, 100);

        if (range <= 15)
        {
            string sfxName = "";

            sfxName += (tag.CompareTo("Yellow") == 0) ? string.Format("y_damage{0}", UnityEngine.Random.Range(0, 10)) : string.Format("g_damage{0}", UnityEngine.Random.Range(0, 9));
            if (YellowBean.SoundManager.Instance != null)
            {
                YellowBean.SoundManager.Instance.PlaySFX(sfxName);
            }
        }
            
        status.hp -= attacker.damage;

        //status.hp--;

        if (status.hp > 0) return;

        if (destroy == null)
        {
            destroy = StartCoroutine(Destroy());
        }
    }

    IEnumerator Destroy()
    {
        alive = false;

        yield return null;

        // 죽는 곳
        // 사운드 색_이름{인덱스}
        var range = UnityEngine.Random.Range(0, 100);

        if (range <= 15)
        {
            string sfxName = "";

            sfxName += (tag.CompareTo("Yellow") == 0) ? string.Format("y_dead{0}", UnityEngine.Random.Range(0, 8)) : string.Format("g_dead{0}", UnityEngine.Random.Range(0, 5));
            if (YellowBean.SoundManager.Instance != null)
            {
                YellowBean.SoundManager.Instance.PlaySFX(sfxName);
            }
        }
            
        EffectsPool.instace.CreateEffect("DeadEffect_" + tag.ToString(), transform.position, this.size);

        gameObject.SetActive(false);

        UnitsPool.instance.listUnitDeath.Add(this);
    }

    public virtual void Test()
    {
        Debug.Log("<color=red>테스트!!!</color>");
    }
}
