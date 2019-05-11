using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

/* 20181231 유닛 데이터 로드부분을 제거.
 * 차후에 다른 곳에 유닛데이터를 표시할 때 필요할 것 같아서 주석처리함.
 */

public class DataManager
{
    static DataManager instance;
    public UserInfo userInfo;

    Dictionary<string, UnitData> dicUnitData = new Dictionary<string, UnitData>();
    public Dictionary<int, MapData> dicMapData = new Dictionary<int, MapData>();
    public Dictionary<int, MapCost> dicMapCost = new Dictionary<int, MapCost>();
    public Dictionary<int, StoryStageData> dicStoryStageData = new Dictionary<int, StoryStageData>();
    public Dictionary<int, CountryData> dicCountryData = new Dictionary<int, CountryData>();

    // <int, 새로만든 구조체> dic~~

    private DataManager()
    {
        Initialize();

        PlayData.Init();
    }

    public static DataManager GetInstance()
    {
        if (DataManager.instance == null)
        {
            DataManager.instance = new DataManager();
        }

        return DataManager.instance;
    }

    public void Initialize()
    {
        LoadUserData();
        LoadUnitData();
        LoadMapData();
        LoadStoryStageData();
        LoadCountryData();
    }

    public void LoadCountryData()
    {
        //국가 데이터 로드
        string filePath = "Data/CountryData/country";
        var ta = Resources.Load<TextAsset>(filePath);
        dicCountryData = JsonConvert.DeserializeObject<CountryData[]>(ta.text).ToDictionary(a => a.id);
    }

    public void LoadMapData()
    {
        // 맵 데이터 로드
        {
            string filePath = "Data/MapData";
            TextAsset[] ta = Resources.LoadAll<TextAsset>(filePath);

            for (int i = 0; i < ta.Length; i++)
            {
                string data = ta[i].text;
                MapData mapData = JsonConvert.DeserializeObject<MapData>(data);

                dicMapData.Add(mapData.map_id, mapData);
            }
        }

        // 맵 코스트 로드
        {
            string filePath = "Data/MapCost";
            MapCost[] arrMapCost = Resources.LoadAll<MapCost>(filePath);

            for (int i = 0; i < arrMapCost.Length; i++)
            {
                dicMapCost.Add(int.Parse(arrMapCost[i].name), arrMapCost[i]);
            }
        }
    }

    public void LoadStoryStageData()
    {
        string filePath = "Data/StoryStageData";
        TextAsset[] ta = Resources.LoadAll<TextAsset>(filePath);

        for (int i = 0; i < ta.Length; i++)
        {
            string data = ta[i].text;
            StoryStageData storyStageData = JsonConvert.DeserializeObject<StoryStageData>(data);

            dicStoryStageData.Add(storyStageData.id, storyStageData);
        }
    }

    public void LoadUserData()
    {
        string filePath = Application.persistentDataPath;
        string fileName = "UserData.json";

        // 파일이 존재 하면.
        // 로드해서 가져온다.
        if (File.Exists(filePath + fileName))
        {
            string data = File.ReadAllText(filePath + fileName);
            userInfo = JsonConvert.DeserializeObject<UserInfo>(data);
        }

        // 파일이 존재하지 않으면.
        // 생성하고 저장한다.
        else
        {
            CreateUserData();
        }
    }

    public void CreateUserData()
    {
        string filePath = Application.persistentDataPath;
        string fileName = "UserData.json";

        // 유저 인포를 생성한다.
        userInfo = new UserInfo();
        userInfo.score = 0;

        for (int i = 0; i < userInfo.arrStageInfo.Length; i++)
        {
            // 스테이지 인포를 생성한다.
            var stageInfo = new StoryStageInfo();
            stageInfo.stageId = i + 1;
            stageInfo.opened = false;
            stageInfo.starCnt = 0;

            userInfo.arrStageInfo[i] = stageInfo;
        }
        userInfo.arrStageInfo[0].opened = true;


        // 저장한다.
        string json = JsonConvert.SerializeObject(userInfo);
        File.WriteAllText(filePath + fileName, json);
    }

    public void SaveUserInfo()
    {
        string filePath = Application.persistentDataPath;
        string fileName = "UserData.json";

        // 저장한다.
        string json = JsonConvert.SerializeObject(userInfo);
        File.WriteAllText(filePath + fileName, json);

        Debug.LogFormat("{0}{1}", filePath, fileName);
    }

    public int GetStarCnt()
    {
        int starCnt = 0;

        foreach (var stageInfo in userInfo.arrStageInfo)
        {
            if (stageInfo.opened)
            {
                starCnt += stageInfo.starCnt;
            }
        }

        return starCnt;
    }

    public void LoadUnitData()
    {
        TextAsset ta = Resources.Load<TextAsset>("Data/UnitData/UnitData");

        dicUnitData = JsonConvert.DeserializeObject<UnitData[]>(ta.text).ToDictionary(d => d.name);
    }

    public UnitData GetUnitData(string key)
    {
        bool inHashtable = dicUnitData.ContainsKey(key);

        if (!inHashtable)
        {
            Debug.LogError("잘못된 키 입니다.");
        }

        return dicUnitData[key];
    }

    public bool HasUnitData(string key)
    {
        bool inHashtable = dicUnitData.ContainsKey(key);

        return inHashtable;
    }
}
