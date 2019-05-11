using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.EventSystems;

public class StageEditor : MonoBehaviour
{
    // 유닛 리스트
    List<Unit> listUnit = new List<Unit>();

    // UI
    public ToggleGroup toggleGroup;
    public Toggle unitSelectTogglePrefab;
    public List<Toggle> toggles = new List<Toggle>();

    public Button btnStart;
    public Button btnReset;
    public Button btnUndo;

    public InputField ifMapId;
    public Button btnLoad;
    MapData mapData;

    public Button btnSave;
    public GameObject onSaveGo;

    public InputField ifStageID;
    public InputField ifStageDesc;
    public InputField ifStageCost;
    public Button btnOk;

    private void Start()
    {
        InitializeUI();

        // 세이브 버튼을 비활성화 해 놓는다.
        btnSave.enabled = false;

        // 확인창은 비활성화 한다.
        onSaveGo.SetActive(false);

        btnLoad.onClick.AddListener(() => 
        {
            int id = 0;
            if (!int.TryParse(ifMapId.text, out id))
            {
                Debug.LogError("맵 아이디를 확인하세요");
            }

            string filePath = Application.dataPath + "/Resources/Data/MapData/";
            string fileName = string.Format("map_{0}.json", id);

            if(!File.Exists(filePath + fileName))
            {
                Debug.LogError("없는 위치입니다.");
                return;
            }

            // 맵을 만든다.
            string data = File.ReadAllText(filePath + fileName);
            mapData = JsonConvert.DeserializeObject<MapData>(data);

            CreateMap();
        });

        btnSave.onClick.AddListener(() => {
            onSaveGo.SetActive(true);
        });

        btnOk.onClick.AddListener(() => {
            StageData();
        });
    }

    void StageData()
    {
        // 스토리 스테이지 데이타
        StoryStageData storyStageData = new StoryStageData();

        // 아이디를 입력한다.
        if(!int.TryParse(ifStageID.text, out storyStageData.id))
        {
            onSaveGo.SetActive(false);
            Debug.LogError("아이디를 확인하세요.");
            return;
        }

        // 타이틀을 입력한다.
        storyStageData.desc = ifStageDesc.text;

        // 코스트를 넣는다.
        if (!int.TryParse(ifStageCost.text, out storyStageData.cost))
        {
            onSaveGo.SetActive(false);
            Debug.LogError("코스트를 확인하세요.");
            return;
        }

        storyStageData.map_id = mapData.map_id;

        // 유닛 배열
        Unit[] arrUnit = GameObject.FindObjectsOfType<Unit>();

        for (int i = 0; i < arrUnit.Length; i++)
        {
            unit_info info = new unit_info();
            info.unit_name = arrUnit[i].status.name;
            info.color = UnitColor.PaseToInt(arrUnit[i].tag.ToString());
            info.x = arrUnit[i].transform.position.x;
            info.y = arrUnit[i].transform.position.y;
            info.z = 0;

            storyStageData.listUnitInfos.Add(info);
        }

        string path = Application.dataPath + "/Resources/Data/StoryStageData/";
        string fileName = string.Format("story_stage_{0}.json", storyStageData.id);

        if (File.Exists(path+fileName))
        {
            Debug.LogError("중복된 파일 명 입니다.");
            onSaveGo.SetActive(false);
            return;
        }

        string json = JsonConvert.SerializeObject(storyStageData);
        File.WriteAllText(path + fileName, json);

        onSaveGo.SetActive(false);
    }

    void CreateMap()
    {
        GameObject newGo = new GameObject();
        newGo.name = this.mapData.map_id.ToString();

        // 그라운드를 만든다.
        GameObject groundPrefab = PrefabsManager.GetInstance().dicGroundPrefabs[mapData.groundName];
        Instantiate(groundPrefab).transform.parent= newGo.transform;

        // 환경을 배치한다.
        for (int i = 0; i < mapData.listEnvirnment.Count; i++)
        {
            GameObject envirGo = PrefabsManager.GetInstance().dicEnvironmentPrefabs[mapData.listEnvirnment[i].name];
            Vector3 pos = new Vector3(mapData.listEnvirnment[i].x, mapData.listEnvirnment[i].y, 0);

            Instantiate(envirGo, pos, Quaternion.identity).transform.parent = newGo.transform;
        }

        // 초록 베이스
        {
            GameObject basePrefab = PrefabsManager.GetInstance().dicBasePrefabs[mapData.greenBaseName];
            var go = Instantiate(basePrefab, new Vector3(0, 0, 100), Quaternion.identity);
        }

        // 노랑 베이스
        {
            GameObject basePrefab = PrefabsManager.GetInstance().dicBasePrefabs[mapData.yellowBaseName];
            var go = Instantiate(basePrefab, new Vector3(0, 0, 100), Quaternion.identity);
        }

        // 그리드를 생성한다.
        var grid = GetComponent<YellowBean.Grid>();

        grid.gridWorldSize = new Vector2(mapData.grid_x, mapData.grid_y);
        grid.nodeRadius = mapData.node_radius;

        grid.CreateGrid();

        // 세이브 버튼을 활성화 한다.
        btnSave.enabled = true;
    }

    void InitializeUI()
    {
        foreach (var e in PrefabsManager.GetInstance().dicUnitPrefabs)
        {
            GameObject unitGo = e.Value;
            Unit unit = unitGo.GetComponent<Unit>();

            if (DataManager.GetInstance().HasUnitData(unit.name))
            {
                Toggle toggle = Instantiate(unitSelectTogglePrefab);
                toggle.transform.parent = toggleGroup.transform;
                toggle.group = toggleGroup;
                toggle.isOn = false;
                toggle.name = unit.name;

                toggle.GetComponentInChildren<Text>().text =
                DataManager.GetInstance().GetUnitData(unit.name).cost.ToString();
                toggles.Add(toggle);
            }

        }

        btnStart.onClick.AddListener(() =>
        {
            if (listUnit == null) return;

            foreach (var unit in listUnit)
            {
                unit.onAi = true;
            }
        });

        btnReset.onClick.AddListener(() =>
        {
            if (listUnit == null) return;

            foreach (var unit in listUnit)
            {
                Destroy(unit.gameObject);
            }
            listUnit.Clear();
        });

        btnUndo.onClick.AddListener(() =>
        {
            if (listUnit == null) return;

            if (listUnit.Count <= 0) return;

            Unit unit = listUnit[listUnit.Count - 1];
            Destroy(unit.gameObject);

            listUnit.Remove(unit);
        });
    }

    private void Update()
    {
        bool left = Input.GetMouseButtonDown(0);
        bool right = Input.GetMouseButtonDown(1);

        if ((left || right) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(inputRay, out hit))
            {
                YBEnum.eColorType colorType = YBEnum.eColorType.None;

                if (left) colorType = YBEnum.eColorType.Yellow;
                if (right) colorType = YBEnum.eColorType.Green;

                Toggle toggle = toggles.Find(t => t.isOn);
                if (toggle == null) return;

                YBEnum.eUnitName strToEnum = UnitName.ParseToEnum(toggle.name);
                listUnit.Add(UnitsPool.instance.CreateUnit(strToEnum, colorType, hit.point));
            }
        }
    }
}
