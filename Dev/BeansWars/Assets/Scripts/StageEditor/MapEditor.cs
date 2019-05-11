using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;

public class MapEditor : MonoBehaviour
{
    public InputField ifMapID;
    public InputField ifMapName;

    public Button btnSave;

    private void Awake()
    {
        btnSave.onClick.AddListener(() => SaveMap());
    }

    void SaveMap()
    {
        /*
         * 맵 데이터를 만든다.
         */
        MapData mapData = new MapData();

        // 아이디가 숫자가 아니거나.
        // 입력 값이 없다면, 오류.
        if (!int.TryParse(ifMapID.text, out mapData.map_id) || ifMapID.text.Length == 0)
        {
            Debug.LogError("맵 아이디를 확인하세요.");
            return;
        }
        
        // 맵 이름을 입력한다.
        mapData.map_name = ifMapName.text;

        // 맵 데이터에 그라운드 이름을 저장한다.
        mapData.groundName = GameObject.FindGameObjectWithTag("Ground").name;

        // 씬에서 환경 요소들을 가져와서.
        // 맵 데이터에 저장한다.
        GameObject[] arrEnvironmentGo = GameObject.FindGameObjectsWithTag("Environment");
        for (int i = 0; i < arrEnvironmentGo.Length; i++)
        {
            string envirName = arrEnvironmentGo[i].name;
            envirName = envirName.Split(' ')[0];    // 띄어쓰기 전 부분만 가져온다.

            EnvironmentData envir = new EnvironmentData
            {
                name = envirName,
                x = arrEnvironmentGo[i].transform.position.x,
                y = arrEnvironmentGo[i].transform.position.y
            };

            mapData.listEnvirnment.Add(envir);
        }

        // 그리드 정보를 받아온다.
        var grid = FindObjectOfType<YellowBean.Grid>();
        mapData.grid_x = grid.gridWorldSize.x;
        mapData.grid_y = grid.gridWorldSize.y;
        mapData.node_radius = grid.nodeRadius;


        // 베이스
        var yb = GameObject.FindGameObjectWithTag("YellowBase");
        if (yb != null)
        {
            mapData.yellowBaseName = yb.name;
        }

        var gb = GameObject.FindGameObjectWithTag("GreenBase");
        if (gb != null)
        {
            mapData.greenBaseName = gb.name;
        }

        // 맵데이터를 저장한다.
        string json = JsonConvert.SerializeObject(mapData);

        string path = Application.dataPath + "/Resources/Data/MapData/";
        string fileName = string.Format("map_{0}.json", mapData.map_id);
        string filePath = path + fileName;

        if (File.Exists(filePath))
        {
            Debug.LogError("중복된 파일 명 입니다.");
            return;
        }

        File.WriteAllText(filePath, json);
    }
}
