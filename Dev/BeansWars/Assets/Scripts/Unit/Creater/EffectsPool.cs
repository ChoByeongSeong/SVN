using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EffectsPool : MonoBehaviour
{
    public static EffectsPool instace;

    public int effectCnt = 30;
    Transform effectPoolTr;

    Dictionary<string, Stack<GameObject>> dicEffect = new Dictionary<string, Stack<GameObject>>();

    // Use this for initialization
    void Awake()
    {
        instace = this;

        Initialize();
    }

    private void Initialize()
    {
        effectPoolTr = new GameObject().transform;
        effectPoolTr.gameObject.name = "EffectPool";
        effectPoolTr.parent = transform;

        // 프리펩 매니저에 있는 이펙트 프리펩을 가져온다.
        var newDicEffect = PrefabsManager.GetInstance().dicEffectPrefabs;

        // 프리펩들을 돌면서 인스턴스를 생성한다.
        foreach (KeyValuePair<string, GameObject> e in newDicEffect)
        {
            // 같은 이름의 이펙트가 없다면.
            if (!this.dicEffect.ContainsKey(e.Key))
            {
                // 리스트를 생성하고.
                this.dicEffect.Add(e.Key, new Stack<GameObject>());

                // 리스트에 풀을 생성한다.
                for (int i = 0; i < effectCnt; i++)
                {
                    GameObject effect = GameObject.Instantiate(e.Value);
                    effect.transform.parent = effectPoolTr;
                    effect.SetActive(false);

                    this.dicEffect[e.Key].Push(effect);
                }
            }
        }
    }

    public void CreateEffect(string name, Vector2 pos, float size, bool isReverse = false)
    {
        StartCoroutine(CreateEffectImpl(name, pos, size, isReverse));
    }

    IEnumerator CreateEffectImpl(string name, Vector2 pos, float size, bool isReverse)
    {
        // 딕셔너리에 키가 있는지 확인한다.
        if (!this.dicEffect.ContainsKey(name))
        {
            yield break;
        }

        // 입력받은 키의 밸류.
        // 즉 스택.
        Stack<GameObject> effects = this.dicEffect[name];

        // 스택이 0이라면.
        // 새로 생성해서 넣어놔야 한다.
        if (effects.Count <= 0)
        {
            for (int i = 0; i < effectCnt; i++)
            {
                // 프리펩은 프리펩 매니저를 통해 가져온다.
                GameObject effect = GameObject.Instantiate(PrefabsManager.GetInstance().dicEffectPrefabs[name]);
                effect.transform.parent = effectPoolTr;
                effect.SetActive(false);

                effects.Push(effect);
            }
        }

        // 이펙트를 하나 꺼내고.
        // 설정한다.
        // 반환할 이펙트
        GameObject newEffect = null;
        newEffect = effects.Pop();

        float newSize = Mathf.Clamp(size * 1.3f, 0.8f, 4f);

        pos.y -= size;
        newEffect.transform.localScale = new Vector3(newSize, newSize, newSize);
        newEffect.transform.position = pos;

        if (isReverse)
        {
            var trParticle = newEffect.transform.Find("Particle System");

            if (trParticle != null) trParticle.localScale = new Vector3(-1, 1, 1);
        }

        newEffect.SetActive(true);

        yield return new WaitForSeconds(3);

        newEffect.SetActive(false);
        effects.Push(newEffect);
    }
}
