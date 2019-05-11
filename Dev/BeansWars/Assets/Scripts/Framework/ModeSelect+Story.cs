using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public partial class ModeSelect : MonoBehaviour
{
    [Header("Story Mode")]
    public string playScneName;
    public Transform trStroyContent;
    public GameObject itemStoryStage;
    public Button btnStoryPlay;
    public Button btnBack;
    public Button btnOption;
    public Text txtTotalStarts;
    public Text txtStoryStageDesc;
    public Image imgStoryThumb;

    private void StoryModeInit()
    {  
        /* 스테이지 목록을 쭉 불러온다.
         */
        for (int i = 0; i < 20; i++)
        {
            // 스테이지 데이터가 없으면.
            // 넘어간다.
            if (!DataManager.GetInstance().dicStoryStageData.ContainsKey(i + 1)) continue;

            // 1번 스테이지 부터 20번 스테이지 까지.
            var stageData = DataManager.GetInstance().dicStoryStageData[i + 1];

            // 아이템을 생성한다.
            GameObject go = Instantiate(itemStoryStage);
            go.transform.SetParent(trStroyContent);
            go.transform.localRotation = Quaternion.Euler(Vector3.zero);
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition3D = Vector3.zero;

            // 아이템 정보를 세팅한다.
            var item = go.GetComponent<UIStoryStageItem>();
            item.IsSelect = false;

            // 타이틀을 설정한다.
            string stageTitle = string.Format("STAGE {0}", stageData.id);
            item.txtTitle.text = stageTitle;

            // 별을 기본 -1로 설정한다.
            // 잠금 이미지.
            item.StarCnt = -1;

            // 유저 정보를 기반으로 다시 생성한다.
            if (DataManager.GetInstance().userInfo.arrStageInfo.Length > i)
            {
                var stageInfo = DataManager.GetInstance().userInfo.arrStageInfo[i];

                if (stageInfo.opened)
                {
                    item.StarCnt = stageInfo.starCnt;
                }
            }

            // 버튼을 추가한다.
            var btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                // 나머지 버튼을 초기화 한다.
                {
                    for (int ii = 0; ii < trStroyContent.childCount; ii++)
                    {
                        var child = trStroyContent.GetChild(ii);
                        child.GetComponent<UIStoryStageItem>().IsSelect = false;
                    }
                }
                btn.GetComponent<UIStoryStageItem>().IsSelect = true;

                // 플레이 데이터를 설정한다.
                PlayData.id = stageData.id;
                PlayData.map_id = stageData.map_id;
                PlayData.cost = stageData.cost;
                PlayData.Mode = PlayData.ePlayMode.Story;
                PlayData.listUnitInfos = stageData.listUnitInfos;

                // Desc를 설정한다.
                txtStoryStageDesc.text = DataManager.GetInstance().dicStoryStageData[PlayData.id].desc;

                // 썸네일
                var mapData = DataManager.GetInstance().dicMapData[PlayData.map_id];
                Sprite mapSprite = PrefabsManager.GetInstance().dicGroundPrefabs[mapData.groundName].GetComponentInChildren<SpriteRenderer>().sprite;
                imgStoryThumb.sprite = mapSprite;
                imgStoryThumb.color = Color.white;
            });

        }

        // 플레이 버튼을 초기화 한다.
        btnStoryPlay.onClick.AddListener(() =>
        {
            YellowBean.SoundManager.Instance.PlayBgm("Castle Attack",true,0.2f);
            UnityAnalyticsManager.GetInstance().OnClickButton();
            if (PlayData.id == -1)
                return;

            App.Instance.LoadScene(playScneName);
        });

        btnOption.onClick.AddListener(() =>
        {
            this.option.SetActive(true);
        });

        // 유저 총 별 개수를 표시한다.
        int startCnt = DataManager.GetInstance().GetStarCnt();
        txtTotalStarts.text = startCnt.ToString();
    }

    void StoryModeSort()
    {
        // 나머지 버튼을 초기화 한다.
        {
            for (int ii = 0; ii < trStroyContent.childCount; ii++)
            {
                var child = trStroyContent.GetChild(ii);
                child.GetComponent<UIStoryStageItem>().IsSelect = false;
            }
        }

        trStroyContent.GetChild(0).GetComponent<UIStoryStageItem>().IsSelect = true;

        // 1번 스테이지
        var stageData = DataManager.GetInstance().dicStoryStageData[1];

        // 플레이 데이터를 설정한다.
        PlayData.id = stageData.id;
        PlayData.map_id = stageData.map_id;
        PlayData.cost = stageData.cost;
        PlayData.Mode = PlayData.ePlayMode.Story;
        PlayData.listUnitInfos = stageData.listUnitInfos;

        // Desc를 설정한다.
        txtStoryStageDesc.text = DataManager.GetInstance().dicStoryStageData[PlayData.id].desc;

        // 썸네일
        var mapData = DataManager.GetInstance().dicMapData[PlayData.map_id];
        Sprite mapSprite = PrefabsManager.GetInstance().dicGroundPrefabs[mapData.groundName].GetComponentInChildren<SpriteRenderer>().sprite;
        imgStoryThumb.sprite = mapSprite;
        imgStoryThumb.color = Color.white;

        // 리스트 맨 위로
        trStroyContent.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
    }
}
