using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectilesPool : MonoBehaviour
{
    public static ProjectilesPool instance;

    public int arrowCnt = 30;
    public int magicCnt = 30;
    public int stoneCnt = 10;

    Stack<Arrow> arrowPool = new Stack<Arrow>();
    Stack<Magic> magicPool = new Stack<Magic>();
    Stack<Stone> stonePool = new Stack<Stone>();

    Transform arrowPoolTr;
    Transform magicPoolTr;
    Transform stonePoolTr;

    private void Awake()
    {
        instance = this;
        Initialize();
    }

    private void Initialize()
    {
        // 화살들이 위치할 공간을 만든다.
        arrowPoolTr = new GameObject().transform;
        arrowPoolTr.gameObject.name = "ArrowPool";
        arrowPoolTr.parent = transform;

        // 화살을 만든다.
        CreateArrow();

        magicPoolTr = new GameObject().transform;
        magicPoolTr.gameObject.name = "MagicPool";
        magicPoolTr.parent = transform;

        CreateMagic();

        // 화살들이 위치할 공간을 만든다.
        stonePoolTr = new GameObject().transform;
        stonePoolTr.gameObject.name = "StonePool";
        stonePoolTr.parent = transform;

        CreateStone();
    }

    private void CreateArrow()
    {
        // 화살 프리펩을 가져온다.
        GameObject arrowPrefab = PrefabsManager.GetInstance().GetProjectilePrefab(YBEnum.eProjectileType.Arrow);

        // 프리펩을 만든다.
        for (int i = 0; i < arrowCnt; i++)
        {
            GameObject arrowGo = Instantiate(arrowPrefab);
            arrowGo.SetActive(false);
            arrowGo.transform.parent = arrowPoolTr;

            Arrow arrow = arrowGo.GetComponent<Arrow>();

            arrowPool.Push(arrow);
        }
    }

    private void CreateMagic()
    {
        // 매직 프리펩을 가져온다.
        GameObject magicPrefab = PrefabsManager.GetInstance().GetProjectilePrefab(YBEnum.eProjectileType.Magic);

        // 프리펩을 만든다.
        for (int i = 0; i < magicCnt; i++)
        {
            GameObject magicGo = Instantiate(magicPrefab);
            magicGo.SetActive(false);
            magicGo.transform.parent = magicPoolTr;

            Magic magic = magicGo.GetComponent<Magic>();

            magicPool.Push(magic);
        }
    }

    private void CreateStone()
    {
        // 화살 프리펩을 가져온다.
        GameObject stonePrefab = PrefabsManager.GetInstance().GetProjectilePrefab(YBEnum.eProjectileType.Stone);

        // 프리펩을 만든다.
        for (int i = 0; i < stoneCnt; i++)
        {
            GameObject stoneGo = Instantiate(stonePrefab);
            stoneGo.SetActive(false);
            stoneGo.transform.parent = stonePoolTr;

            Stone stone = stoneGo.GetComponent<Stone>();

            stonePool.Push(stone);
        }
    }

    public void DestroyArrow(Arrow arrow)
    {
        arrow.gameObject.SetActive(false);
        arrow.transform.parent = arrowPoolTr;

        arrowPool.Push(arrow);
    }

    public void DestroyMagic(Magic magic)
    {
        magic.gameObject.SetActive(false);
        magic.transform.parent = magicPoolTr;

        magicPool.Push(magic);
    }

    public void DestroyStone(Stone stone)
    {
        stone.gameObject.SetActive(false);
        stone.transform.parent = stonePoolTr;

        stonePool.Push(stone);
    }

    public void ShootArrow(Unit owner, Vector3 startPos, Vector3 endPos)
    {
        if(arrowPool.Count <= 0)
        {
            CreateArrow();
        }

        Arrow arrow = arrowPool.Pop();
        arrow.gameObject.SetActive(true);

        arrow.Initialize(owner, startPos, endPos);
    }

    public void ShootMagic(Unit owner, Unit target, Vector3 startPos, Vector3 endPos)
    {
        if (magicPool.Count <= 0)
        {
            CreateMagic();
        }

        Magic magic = magicPool.Pop();
        magic.gameObject.SetActive(true);

        magic.Initialize(owner, target,startPos, endPos);
    }

    public void ShootStone(Unit owner, Vector3 startPos, Vector3 endPos)
    {
        if (stonePool.Count <= 0)
        {
            CreateStone();
        }

        Stone stone = stonePool.Pop();
        stone.gameObject.SetActive(true);

        stone.Initialize(owner, startPos, endPos);
    }
}