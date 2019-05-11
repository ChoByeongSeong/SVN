using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ProjectilesPool))]
[RequireComponent(typeof(EffectsPool))]

public class UnitsPool : MonoBehaviour
{
    public static UnitsPool instance;

    Stack<Unit> stackYellowMinion = new Stack<Unit>();
    Stack<Unit> stackGreenMinion = new Stack<Unit>();
    Transform minionPool;

    public List<Unit> listUnitDeath = new List<Unit>();

    IEnumerator EmptyDeathUnitList()
    {
        while (true)
        {
            for (int i = 0; i < listUnitDeath.Count; i++)
            {
                Destroy(listUnitDeath[i].gameObject);
            }
            listUnitDeath.Clear();

            yield return new WaitForSeconds(15f);
        }
    }

    private void Awake()
    {
        instance = this;

        // 풀을 생성한다.
        var goMinionPool = new GameObject();
        goMinionPool.name = "Minion Pool";
        goMinionPool.transform.SetParent(this.transform);

        minionPool = goMinionPool.transform;

        // 미니언을 만들어 놓는다.
        CreateMinion();

        // 휴지통
        StartCoroutine(EmptyDeathUnitList());
    }

    public Unit CreateUnit(YBEnum.eUnitName unitName, YBEnum.eColorType colorType, Vector2 position)
    {
        // 유닛 프리펩과 모델을 가져온다.
        GameObject unitPrefab = PrefabsManager.GetInstance().GetUnitPrefab(unitName);
        GameObject modelPrefab = PrefabsManager.GetInstance().GetModelPrefab(unitName, colorType);

        // 유닛을 생성한다.
        GameObject unitGo = Instantiate(unitPrefab, Vector3.zero, Quaternion.identity);

        // 모델을 생성한다.
        // 이름을 모델로 변경한다.
        // 트랜스폼을 조정한다.
        GameObject modelGo = Instantiate(modelPrefab);
        modelGo.name = "Model";
        modelGo.transform.parent = unitGo.transform;

        // 유닛의 위치를 변경한다.
        unitGo.transform.position = position;

        // 유닛의 태그를 설정한다.
        unitGo.tag = colorType.ToString();

        // 유닛 정보를 초기화 한다.
        Unit unit = unitGo.GetComponent<Unit>();

        if (unit.status == null)
        {
            UnitData data = DataManager.GetInstance().GetUnitData(unitName.ToString());

            // 유닛 데이터를 복사해서 생성한다.
            unit.status = new UnitData(data);
        }

        return unit;
    }

    void CreateMinion(string inputTag)
    {
        for (int i = 0; i < 30; i++)
        {
            // 미니언을 비 활성화된 상태로 생성한다.
            Unit minion = CreateUnit(YBEnum.eUnitName.Minion, UnitColor.PaseToEnum(inputTag), Vector3.zero);
            minion.onAi = false;

            // 게임 오브젝트를 비활성화 한다.
            minion.gameObject.SetActive(false);

            // 트랜스폼을 변경한다.
            minion.transform.SetParent(minionPool);


            if (inputTag.CompareTo("Yellow") == 0)
            {                
                // 스택에 추가한다.
                stackYellowMinion.Push(minion);
            }
            else
            {
                // 스택에 추가한다.
                stackGreenMinion.Push(minion);
            }

        }
    }


    void CreateMinion()
    {
        for (int i = 0; i < 150; i++)
        {
            // 미니언을 비 활성화된 상태로 생성한다.
            Unit minion = CreateUnit(YBEnum.eUnitName.Minion, YBEnum.eColorType.Yellow, Vector3.zero);
            minion.onAi = false;

            // 게임 오브젝트를 비활성화 한다.
            minion.gameObject.SetActive(false);

            // 트랜스폼을 변경한다.
            minion.transform.SetParent(minionPool);

            // 스택에 추가한다.
            stackYellowMinion.Push(minion);
        }

        for (int i = 0; i < 30; i++)
        {
            // 미니언을 비 활성화된 상태로 생성한다.
            Unit minion = CreateUnit(YBEnum.eUnitName.Minion, YBEnum.eColorType.Green, Vector3.zero);
            minion.onAi = false;

            // 게임 오브젝트를 비활성화 한다.
            minion.gameObject.SetActive(false);

            // 트랜스폼을 변경한다.
            minion.transform.SetParent(minionPool);

            // 스택에 추가한다.
            stackGreenMinion.Push(minion);
        }
    }

    public void SummonMinion(string tag, Vector3 pos)
    {
        Unit minion = null;

        if (tag.CompareTo("Yellow") == 0)
        {
            if (stackYellowMinion.Count <= 0)
            {
                CreateMinion(tag);
            }

            minion = stackYellowMinion.Pop();
        }
        else {

            if (stackGreenMinion.Count <= 0)
            {
                CreateMinion(tag);
            }

            minion = stackGreenMinion.Pop();
        }

        minion.transform.position = pos;
        minion.gameObject.SetActive(true);

        StartCoroutine(SummonImpl(minion));
    }

    IEnumerator SummonImpl(Unit unit)
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.2f));

        unit.onAi = true;
    }


}
