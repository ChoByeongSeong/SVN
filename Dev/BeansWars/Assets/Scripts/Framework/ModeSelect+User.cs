using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using System.Linq;

public partial class ModeSelect : MonoBehaviour
{
    int userStageId;

    [Header("User Mode")]
    public string mapSelectSceneName;

    public RectTransform rtUI;
    public RectTransform rtStoryMode;
    public RectTransform rtUserMode;
    public RectTransform rtRanking;
    public float duration;

    public UIRankingItem itemMyRank;
    public Transform trUserContent;
    public UIUserStageItem itemUserStage;
    public Button btnUserPlay;

    public UISortToggle sortToggle;
    public Button btnLike;
    public Button btnClearCnt;
    public Button btnScore;
    public Button btnDate;
    public Button btnOnlineModeBack;

    public UIUserModeDesc userModeDesc;
    public Image imgUserThumb;

    public Button btnRanking;
    public Button btnCreateStage;
    public Button btnUserModePlay;
    public Button btnOption2;

    public Transform trRankContent;
    public GameObject goRankItemPrefab;
    public GPGSManager gpgsManager;

    private string country;
    private List<GameObject> listUserRankingItem = new List<GameObject>();

    void UserModeInit()
    {
        YellowBean.SoundManager.Instance.PlayBgm("Heroes of Legend (no loop)",true,0.2f);
        userModeDesc.Init(-99, null, null, null, null, null);
        sortToggle.Toggle(-99);

        // 모드 선택 창 => 유저 모드 선택.
        // 로그인을 한다.
        Login(() =>
        {
            Vector2 moveValue = rtUserMode.anchoredPosition;
            rtUI.DOAnchorPos(-moveValue, duration);
        });

        btnOnlineModeBack.onClick.AddListener(() =>
        {
            YellowBean.SoundManager.Instance.PlayBgm("Whistle Fairy",true,0.5f);
        });

        // 정렬 버튼
        btnLike.onClick.AddListener(() =>
        {        
            GetStages((int)YBEnum.eSortType.Like);
        });
        btnClearCnt.onClick.AddListener(() =>
        {       
            GetStages((int)YBEnum.eSortType.ClearCnt);
        });
        btnScore.onClick.AddListener(() =>
        {        
            GetStages((int)YBEnum.eSortType.Score);
        });
        btnDate.onClick.AddListener(() =>
        {        
            GetStages((int)YBEnum.eSortType.Date);
        });

        btnOption2.onClick.AddListener(() =>
        {        
            option.SetActive(true);
        });


        // 맵 만들기
        btnCreateStage.onClick.AddListener(() =>
        {
            UnityAnalyticsManager.GetInstance().OnClickCreateMap();
            PlayData.Init();
            PlayData.Mode = PlayData.ePlayMode.Editor;

            App.Instance.LoadScene(mapSelectSceneName);
        });

        btnUserModePlay.onClick.AddListener(() =>
        {
            UnityAnalyticsManager.GetInstance().OnClickButton();
            if (PlayData.id != -1)
            {
                DoPlay(PlayData.id);
            }
        });

        btnRanking.onClick.AddListener(() =>
        {           
            GetRank();
            // GetMyRank();

            Vector2 moveValue = rtRanking.anchoredPosition;
            rtUI.DOAnchorPos(-moveValue, duration);
        });
    }

    public void Login(Action onComplete)
    {
        // 토큰이 있으면.
        // 구글 + 서버 접속 완료.
        if (Protocol.token != null)
        {
            onComplete();

            // 로그인에 성곡하면 기본 목록을 보여준다.
            GetStages(-1);

            return;
        }

        // 토큰이 없으면.
        // 서버 접속 실패.
        else
        {
            // GPGS 로그인이 안되어 있다면.
            if (Social.localUser.authenticated == false)
            {
                if (App.Instance != null)
                {
                    App.Instance.ShowLoading();
                }

                this.gpgsManager.Init();

                this.gpgsManager.SignIn((result) =>
                {
                    App.Instance.goLoading.SetActive(false);

                    UnityAnalyticsManager.GetInstance().UserLoginCount();
                    // GPGS 로그인에 성공 했다면.                 
                    if (result)
                    {
                        this.convertCountry();
                                           
                        // req_login 구조체에 채운다.
                        var req_login = new Protocol.req_login();
                        req_login.cmd = 900;
                        req_login.id = Social.localUser.id;
                        req_login.userName = Social.localUser.userName;
                        req_login.country = this.country;                       
                        req_login.deviceId = SystemInfo.deviceName;                        

                        // 시리얼 라이즈
                        var json = JsonConvert.SerializeObject(req_login);

                        // 포스트로 보낸다.
                        if (App.Instance != null)
                        {
                            App.Instance.reqCoroutine =
                            StartCoroutine(Protocol.Post("api/doLogin", json, data => LoginResponse(data, onComplete)));
                        }
                    }

                    // GPGS 로그인에 실패 했다면.
                    else
                    {
                        if (App.Instance != null) App.Instance.ShowMessage("GPGS SignIn Error");
                        return;
                    }
                });
            }

            // 로그인이 되어있었다면.
            else
            {
                this.convertCountry();

                // req_login 구조체에 채운다.
                var req_login = new Protocol.req_login();
                req_login.cmd = 900;
                req_login.id = Social.localUser.id;
                req_login.userName = Social.localUser.userName;
                req_login.country = this.country;               
                req_login.deviceId = SystemInfo.deviceName;             

                // 시리얼 라이즈
                var json = JsonConvert.SerializeObject(req_login);

                // 포스트로 보낸다.
                if (App.Instance != null)
                {
                    App.Instance.reqCoroutine =
                    StartCoroutine(Protocol.Post("api/doLogin", json, data => LoginResponse(data, onComplete)));
                }
            }
        }
    }

    public void convertCountry()
    {
        for (int i = 0; i < DataManager.GetInstance().dicCountryData.Count; i++)
        {
            if (Application.systemLanguage.ToString() == DataManager.GetInstance().dicCountryData[i].language)
            {
                this.country = DataManager.GetInstance().dicCountryData[i].convert_language;
            }
        }
    }

    public void LoginResponse(string data, Action onComplete)
    {
        // 응답 받은 데이터를 디시리얼라이즈 한다.
        Protocol.res_login res_login = JsonConvert.DeserializeObject<Protocol.res_login>(data);

        if (res_login == null)
        {
            // Protocol.ResponseError(Protocol.eErrorType.Null);
            return;
        }

        // 로그인 명령어
        if (res_login.cmd != 300)
        {
            // Protocol.ResponseError(Protocol.eErrorType.Login);
            return;
        }

        /* 로그인이 성공하면.
        * 다음 UI를 표시한다.
        */

        Protocol.token = res_login.token;

        onComplete();

        // 로그인에 성곡하면 기본 목록을 보여준다.
        GetStages(-1);
    }

    void GetStages(int sortType)
    {
        // req_getAllStage 구조체를 채운다.
        var req_getAllStage = new Protocol.req_getStages();
        req_getAllStage.cmd = 400;
        req_getAllStage.sort_type = sortType;

        // 시리얼 라이즈
        var json = JsonConvert.SerializeObject(req_getAllStage);

        // 190114 겟타입으로 변경.
        string strSortType = string.Format("api/stages/{0}", sortType);
        if (App.Instance != null)
        {
            App.Instance.reqCoroutine =
            StartCoroutine(Protocol.Get(strSortType, json, data => GetStagesResponse(data, sortType), "authorization", Protocol.token));
        }
    }

    void GetStagesResponse(string data, int sortType)
    {
        // 디시리얼라이즈 한다.
        var res_getAllStages = JsonConvert.DeserializeObject<Protocol.res_getStages>(data);

        if (res_getAllStages == null)
        {
            Protocol.ResponseError(Protocol.eErrorType.Null);
            return;
        }

        if (res_getAllStages.cmd != 400)
        {
            Protocol.ResponseError(Protocol.eErrorType.getAllStages);
            return;
        }

        for (int i = 0; i < trUserContent.childCount; i++)
        {
            Destroy(trUserContent.GetChild(i).gameObject);
        }
        userModeDesc.Init(-99, null, null, null, null, null);

        sortToggle.Toggle(sortType);

        int cnt = res_getAllStages.arrStageInfo.Length;
        for (int i = 0; i < cnt; i++)
        {
            var stageInfo = res_getAllStages.arrStageInfo[i];

            // 위치를 설정한다.
            var item = Instantiate<UIUserStageItem>(itemUserStage);
            item.transform.SetParent(trUserContent);
            item.transform.localRotation = Quaternion.Euler(Vector3.zero);

            if(i == 0) item.IsSelect = true;
            else
                item.IsSelect = false;

            RectTransform rt = item.GetComponent<RectTransform>();
            rt.anchoredPosition3D = Vector3.zero;

            // 정보를 설정한다.
            item.SetUserStage(stageInfo.stage_id, stageInfo.title);

            // 버튼 기능을 추가한다.
            var btn = item.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                // 나머지 버튼을 초기화 한다.
                {
                    for (int ii = 0; ii < trUserContent.childCount; ii++)
                    {
                        var child = trUserContent.GetChild(ii);
                        child.GetComponent<UIUserStageItem>().IsSelect = false;
                    }
                }
                btn.GetComponent<UIUserStageItem>().IsSelect = true;

                // 플레이 데이터를 채운다.
                PlayData.id = stageInfo.stage_id;
                PlayData.map_id = stageInfo.map_id;
                PlayData.Mode = PlayData.ePlayMode.User;
                PlayData.cost = DataManager.GetInstance().dicMapCost[PlayData.map_id].cost;
                PlayData.score = stageInfo.elapsed_score;

                string desc = string.Format("[{0}][{1}]", PlayData.id, stageInfo.title);

                string name = stageInfo.user_name;
                userModeDesc.Init(
                    sortType,
                    stageInfo.favorite_count.ToString(),
                    stageInfo.clear_count.ToString(),
                    stageInfo.elapsed_score.ToString(),
                    stageInfo.created_at,
                    name);

                // 썸네일
                var mapData =  DataManager.GetInstance().dicMapData[PlayData.map_id];
                Sprite mapSprite =  
                    PrefabsManager.GetInstance().dicGroundPrefabs[mapData.groundName].GetComponentInChildren<SpriteRenderer>().sprite;
                imgUserThumb.sprite = mapSprite;
                imgUserThumb.color = Color.white;
            });
        }

        // 첫번째 스테이지 활성화
        {
            var stageInfo = res_getAllStages.arrStageInfo[0];

            // 플레이 데이터를 채운다.
            PlayData.id = stageInfo.stage_id;
            PlayData.map_id = stageInfo.map_id;
            PlayData.Mode = PlayData.ePlayMode.User;
            PlayData.cost = DataManager.GetInstance().dicMapCost[PlayData.map_id].cost;
            PlayData.score = stageInfo.elapsed_score;

            string desc = string.Format("[{0}][{1}]", PlayData.id, stageInfo.title);

            userModeDesc.Init(
                sortType,
                stageInfo.favorite_count.ToString(),
                stageInfo.clear_count.ToString(),
                stageInfo.elapsed_score.ToString(),
                stageInfo.created_at,
                stageInfo.user_name);

            // 썸네일
            var mapData = DataManager.GetInstance().dicMapData[PlayData.map_id];
            Sprite mapSprite =
                PrefabsManager.GetInstance().dicGroundPrefabs[mapData.groundName].GetComponentInChildren<SpriteRenderer>().sprite;
            imgUserThumb.sprite = mapSprite;
            imgUserThumb.color = Color.white;

            // 리스트 맨 위로
            trUserContent.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        }
    }

    private void DoPlay(int num)
    {
        // req_getAllStage 구조체를 채운다.
        var req_doPlay = new Protocol.req_doPlay();
        req_doPlay.cmd = 600;
        req_doPlay.stage_id = num;

        // 시리얼 라이즈
        var json = JsonConvert.SerializeObject(req_doPlay);

        //StartCoroutine(this.Get("api/getAllStages", json, data => GetAllStageResponse(data), "authorization", token));
        if (App.Instance != null)
        {
            App.Instance.reqCoroutine =
            StartCoroutine(Protocol.Post("api/doPlay", json, data => DoPlayResponse(data)));
        }
    }

    void DoPlayResponse(string data)
    {
        // 디시리얼라이즈 한다.
        var res_doPlay = JsonConvert.DeserializeObject<Protocol.res_doPlay>(data);

        if (res_doPlay == null)
        {
            Protocol.ResponseError(Protocol.eErrorType.Null);
            return;
        }

        if (res_doPlay.cmd != 600)
        {
            Protocol.ResponseError(Protocol.eErrorType.doPlay);
            return;
        }

        // 플레이 데이터에 유닛 데이터를 채운다.
        PlayData.listUnitInfos = res_doPlay.arrUnitInfo.ToList();

        App.Instance.LoadScene(playScneName);
    }


    void GetRank()
    {
        // req_getAllStage 구조체를 채운다.
        var req_getRank = new Protocol.req_getRank();
        req_getRank.cmd = 700;

        // 시리얼 라이즈
        var json = JsonConvert.SerializeObject(req_getRank);

        string api = string.Format("api/getRank/{0}", Social.localUser.id);

        if (App.Instance != null)
        {
            App.Instance.reqCoroutine =
    StartCoroutine(Protocol.Get(api,
            json, data => GetRankResponse(data), "authorization", Protocol.token));
        }
    }

    void GetRankResponse(string data)
    {
        GetMyRank();

        for (int i = 0; i < listUserRankingItem.Count; i++)
        {
            Destroy(listUserRankingItem[i]);
        }
        listUserRankingItem.Clear();

        // 디시리얼라이즈 한다.
        var res_getRank = JsonConvert.DeserializeObject<Protocol.res_getRank>(data);

        if (res_getRank == null)
        {
            Protocol.ResponseError(Protocol.eErrorType.Null);
            return;
        }

        if (res_getRank.cmd != 700)
        {
            Protocol.ResponseError(Protocol.eErrorType.getRank);
            return;
        }

        // 유저 랭크를 받아와서 보여준다.
        Protocol.res_user_info[] arrUserInfo = res_getRank.arrUserInfo;

        for (int i = 0; i < arrUserInfo.Length; i++)
        {
            var go = Instantiate(goRankItemPrefab);
            go.transform.SetParent(trRankContent);
            go.transform.localRotation = Quaternion.Euler(Vector3.zero);

            var tr = go.GetComponent<RectTransform>();
            tr.anchoredPosition3D = new Vector3(419.6f, -58.55f, 6.7f);
            tr.localScale = Vector3.one;

            var rankItem = go.GetComponent<UIRankingItem>();
            rankItem.TxTRank = arrUserInfo[i].rank.ToString();
            rankItem.txtScore.text = arrUserInfo[i].score.ToString();
            rankItem.txtUserName.text = arrUserInfo[i].user_name;
            var path = string.Format("Textures/Flags/{0}", arrUserInfo[i].country.ToLower());
            var countryFlag = Resources.Load<Sprite>(path);
            rankItem.countryFlag.sprite = countryFlag;

            listUserRankingItem.Add(go);
        }
    }

    void GetMyRank()
    {
        // req_getAllStage 구조체를 채운다.
        var req_getMyRank = new Protocol.req_getMyRank();
        req_getMyRank.cmd = 800;
        req_getMyRank.user_id = Social.localUser.id;

        // 시리얼 라이즈
        var json = JsonConvert.SerializeObject(req_getMyRank);

        string api = string.Format("api/getMyRank/{0}", req_getMyRank.user_id);

        if (App.Instance != null)
        {
            App.Instance.reqCoroutine =
    StartCoroutine(Protocol.Get(api,
            json, data => GetMyRankResponse(data), "authorization", Protocol.token));
        }
    }

    void GetMyRankResponse(string data)
    {
        // 디시리얼라이즈 한다.
        var res_getMyRank = JsonConvert.DeserializeObject<Protocol.res_getMyRank>(data);

        if (res_getMyRank == null)
        {
            Protocol.ResponseError(Protocol.eErrorType.Null);
            return;
        }

        if (res_getMyRank.cmd != 800)
        {
            Protocol.ResponseError(Protocol.eErrorType.getMyRank);
            return;
        }

        Protocol.res_user_info[] arrUserInfo = res_getMyRank.userInfo;

        itemMyRank.TxTRank = arrUserInfo[0].rank.ToString(); 
        itemMyRank.txtScore.text = arrUserInfo[0].score.ToString();
        itemMyRank.txtUserName.text = arrUserInfo[0].user_name;
        var path = string.Format("Textures/Flags/{0}", arrUserInfo[0].country.ToLower());
        var countryFlag = Resources.Load<Sprite>(path);
        itemMyRank.countryFlag.sprite = countryFlag;
    }
}