using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using UnityEngine.EventSystems;
using UnityEditor;
using System;

public class ThumbEditor : MonoBehaviour
{
    public Color walkableColor;
    public Color unwalkableColor;

    // 그리드
    YellowBean.Grid grid;

    // 유닛 리스트
    List<Unit> listUnit = new List<Unit>();

    public InputField ifMapId;
    public Button btnLoad;
    MapData mapData;

    public Button btnSaveThumb;

    public Transform trThumbPivot;

    private void Awake()
    {
        grid = GetComponent<YellowBean.Grid>();
    }

    private void Start()
    {
        btnLoad.onClick.AddListener(() =>
        {
            int id = 0;
            if (!int.TryParse(ifMapId.text, out id))
            {
                Debug.LogError("맵 아이디를 확인하세요");
            }

            string filePath = Application.dataPath + "/Resources/Data/MapData/";
            string fileName = string.Format("map_{0}.json", id);

            if (!File.Exists(filePath + fileName))
            {
                Debug.LogError("없는 위치입니다.");
                return;
            }

            // 맵을 만든다.
            string data = File.ReadAllText(filePath + fileName);
            mapData = JsonConvert.DeserializeObject<MapData>(data);

            CreateMap();
        });

        btnSaveThumb.onClick.AddListener(() =>
        {
            /* 세이버 버튼을 누르면.
             * 그리드에서 가져와서, 불값으로 저장한다.
             * 불값을 확장하고 스무싱 한다.
             */

            // 그리드의 개수를 받아온다.
            int gridX = grid.grid.GetLength(0);
            int gridY = grid.grid.GetLength(1);

            // 새 그리드를 만든다.
            // 타입을 bool 이차원 배열.
            bool[,] newGrid = new bool[gridX, gridY];

            // 반복문을 돌면서 복사한다.
            for (int y = 0; y < gridY; y++)
            {
                for (int x = 0; x < gridX; x++)
                {
                    newGrid[x, y] = this.grid.grid[x, y].walkable;
                }
            }

            for (int i = 0; i < 2; i++)
            {
                // 확장한다.
                newGrid = ExpandGrid(newGrid);

                // 부드럽게 한다.
                SmoothMap(newGrid);
            }

            Texture2D tex = new Texture2D(newGrid.GetLength(0), newGrid.GetLength(1), TextureFormat.RGBA32, false);

            for (int y = 0; y < newGrid.GetLength(1); y++)
            {
                for (int x = 0; x < newGrid.GetLength(0); x++)
                {
                    // 갈수 있는 지역
                    if(newGrid[x,y])
                    {                     
                        tex.SetPixel(x, y, walkableColor);
                    }

                    // 갈 수 없는 지역
                    else
                    {                    
                        tex.SetPixel(x, y, unwalkableColor);
                    }
                }
            }
            tex.Apply();

            // Encode texture into PNG
            byte[] bytes = tex.EncodeToPNG();

            // For testing purposes, also write to a file in the project folder
            string fileName = string.Format("thumb_{0}.png", ifMapId.text);
            File.WriteAllBytes(Application.dataPath + "/Resources/Sprites/Thumbs/" + fileName, bytes);

            // 텍스쳐를 이미지로 만든다.
            GameObject go = new GameObject();
            var img = go.AddComponent<Image>();

            go.transform.SetParent(trThumbPivot);
            go.transform.localScale = Vector3.one;
            go.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
           
            Rect rect = new Rect(0, 0, newGrid.GetLength(0), newGrid.GetLength(1));
            img.sprite = Sprite.Create(tex, rect, Vector2.zero);

            go.name = string.Format("thumb_{0}", ifMapId.text);
        });
    }

    void CreateMap()
    {
        GameObject newGo = new GameObject();
        newGo.name = this.mapData.map_id.ToString();

        // 그라운드를 만든다.
        GameObject groundPrefab = PrefabsManager.GetInstance().dicGroundPrefabs[mapData.groundName];
        Instantiate(groundPrefab).transform.parent = newGo.transform;

        // 환경을 배치한다.
        for (int i = 0; i < mapData.listEnvirnment.Count; i++)
        {
            GameObject envirGo = PrefabsManager.GetInstance().dicEnvironmentPrefabs[mapData.listEnvirnment[i].name];
            Vector3 pos = new Vector3(mapData.listEnvirnment[i].x, mapData.listEnvirnment[i].y, 0);

            Instantiate(envirGo, pos, Quaternion.identity).transform.parent = newGo.transform;
        }


        // 그리드를 생성한다.
        grid.gridWorldSize = new Vector2(mapData.grid_x, mapData.grid_y);
        grid.nodeRadius = mapData.node_radius;

        grid.CreateGrid();
    }


    bool[,] ExpandGrid(bool[,] grid)
    {
        // 크게 생성
        int gridX = grid.GetLength(0) * 2;
        int gridY = grid.GetLength(1) * 2;

        bool[,] newGrid = new bool[gridX, gridY];

        // 복사
        for (int y = 0; y < gridY; y++)
        {
            for (int x = 0; x < gridX; x++)
            {
                newGrid[x, y] = grid[x / 2, y / 2];
            }
        }

        return newGrid;
    }

    void SmoothMap(bool[,] grid)
    {
        int gridX = grid.GetLength(0);
        int gridY = grid.GetLength(1);

        for (int y = 0; y < gridY; y++)
        {
            for (int x = 0; x < gridX; x++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(grid, x, y);

                if (neighbourWallTiles > 4)
                    grid[x, y] = false;

                else if (neighbourWallTiles < 4)
                    grid[x, y] = true;
            }
        }
    }

    int GetSurroundingWallCount(bool[,] grid, int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (IsInMapRange(grid, neighbourX, neighbourY))
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        if(!grid[neighbourX, neighbourY])
                        {
                            wallCount++;
                        }
                    }
                }

                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    bool IsInMapRange(bool[,] grid, int x, int y)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        return x >= 0 && x < width && y >= 0 && y < height;
    }
}
