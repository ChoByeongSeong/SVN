using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrefabsManager
{
    /*
     * 유닛 프리펩을 가지고 있다.
     * 투사체 프리펩을 가지고 있다.
     * 모델 프리펩을 가지고 있다.
     * 이펙트 프리펩을 가지고 있다.
     */
    static PrefabsManager instance;

    private PrefabsManager()
    {
        Initialize();
    }

    public static PrefabsManager GetInstance()
    {
        if (PrefabsManager.instance == null)
        {
            PrefabsManager.instance = new PrefabsManager();
        }

        return PrefabsManager.instance;
    }

    public Dictionary<string, GameObject> dicUnitPrefabs = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> dicModelPrefabs = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> dicProjectilePrefabs = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> dicEffectPrefabs = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> dicEnvironmentPrefabs = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> dicGroundPrefabs = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> dicBasePrefabs = new Dictionary<string, GameObject>();

    public void Initialize()
    {
        LoadUnitPrefabs();
        LoadProjectilePrefabs();
        LoadModelPrefabs();
        LoadEffectPrefabs();
        LoadEnvironmentPrefabs();
        LoadGroundPrefabs();
        LoadBasePrefabs();
    }

    public GameObject GetUnitPrefab(YBEnum.eUnitName name)
    {
        string strName = name.ToString();

        if (!dicUnitPrefabs.ContainsKey(strName))
        {
            Debug.LogError("잘못된 키 입니다.");
            return null;
        }

        return dicUnitPrefabs[strName];
    }

    public GameObject GetProjectilePrefab(YBEnum.eProjectileType type)
    {
        string typeName = type.ToString();

        if (!dicProjectilePrefabs.ContainsKey(typeName))
        {
            Debug.LogError("잘못된 키 입니다.");
            return null;
        }

        return dicProjectilePrefabs[typeName];
    }

    public GameObject GetModelPrefab(YBEnum.eUnitName name, YBEnum.eColorType colorType)
    {
        string newName = string.Format("{0}_{1}", name.ToString(), colorType.ToString());

        return dicModelPrefabs[newName];
    }

    void LoadUnitPrefabs()
    {
        GameObject[] units = Resources.LoadAll<GameObject>("Prefabs/Units");

        foreach (var e in units)
        {
            dicUnitPrefabs.Add(e.name, e);
        }
    }

    void LoadProjectilePrefabs()
    {
        GameObject[] projectiles = Resources.LoadAll<GameObject>("Prefabs/Projectiles");

        foreach (var e in projectiles)
        {
            dicProjectilePrefabs.Add(e.name, e);
        }
    }

    void LoadModelPrefabs()
    {
        GameObject[] models = Resources.LoadAll<GameObject>("Prefabs/Models");

        foreach (var e in models)
        {
            dicModelPrefabs.Add(e.name, e);
        }
    }

    void LoadEffectPrefabs()
    {
        GameObject[] effects = Resources.LoadAll<GameObject>("Prefabs/Effects");

        foreach (var e in effects)
        {
            dicEffectPrefabs.Add(e.name, e);
        }
    }

    void LoadEnvironmentPrefabs()
    {
        GameObject[] environment = Resources.LoadAll<GameObject>("Prefabs/Environment");

        foreach (var e in environment)
        {
            dicEnvironmentPrefabs.Add(e.name, e);
        }
    }

    void LoadGroundPrefabs()
    {
        GameObject[] arrGround = Resources.LoadAll<GameObject>("Prefabs/Ground");

        foreach (var e in arrGround)
        {
            dicGroundPrefabs.Add(e.name, e);
        }
    }

    void LoadBasePrefabs()
    {
        GameObject[] arrBase = Resources.LoadAll<GameObject>("Prefabs/Base");

        foreach (var e in arrBase)
        {
            dicBasePrefabs.Add(e.name, e);
        }
    }

}
