using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour
{
    // 싱글
    public Button btnSingle;

    // 로그인
    public Button btnLogin;

    public string editorSceneName;
    public string playSceneName;

    // 스테이지
    public GameObject stageGo;
    public RectTransform content;
    public GameObject stageItemPrefab;
    const int listItemCnt = 10;
    List<UIOnlineStageItem> listStageItem = new List<UIOnlineStageItem>();
    int stageId;

    // 미니맵
    public Image imgMiniMap;

    // 솔트 버튼
    public GameObject sortBtnGo;
    Dictionary<int, Button> dicSortBtn = new Dictionary<int, Button>();

    // 플레이 버튼
    public Button btnPaly;

    // 세이브 버튼
    public Button btnCreate;

    // 랭킹 버튼
    public Button btnRank;
    public Button btnMyRank;


    private void Start()
    {
        // 로그인 버튼의 기능을 연결한다.
        btnLogin.onClick.AddListener(() => Login());

        // 정렬 버튼을 가져온다.
        dicSortBtn = sortBtnGo.GetComponentsInChildren<Button>().ToDictionary(btn =>
        {
            string[] str = btn.name.Split('_');
            return int.Parse(str[0]);
        });

        foreach (var kvBtn in dicSortBtn)
        {
            kvBtn.Value.onClick.AddListener(() => GetAllStages(kvBtn.Key));
        }

        // 스테이지리스트 10개를 만든다.
        for (int i = 0; i < listItemCnt; i++)
        {
            var listItemgGo = Instantiate<GameObject>(this.stageItemPrefab);
            listItemgGo.transform.SetParent(this.content.transform);

            UIOnlineStageItem stageItem = listItemgGo.GetComponent<UIOnlineStageItem>();
            listStageItem.Add(stageItem);

            stageItem.btnStage = listItemgGo.GetComponentInChildren<Button>();
            stageItem.btnStage.onClick.AddListener(() => {

                int stageId = int.Parse(stageItem.txtStageId.text.ToString());
                this.stageId = stageId;
                
            });
        }

        btnPaly.onClick.AddListener(() => {         
            DoPlay(this.stageId);
        });

        btnCreate.onClick.AddListener(() => {      
            App.Instance.LoadScene(editorSceneName);
        });

        btnRank.onClick.AddListener(() => {      
            GetRank();
        });

        btnMyRank.onClick.AddListener(() => {      
            GetMyRank();
        });
    }

    public void Login()
    {       
        // req_login 구조체에 채운다.
        var req_login = new Protocol.req_login();
        req_login.cmd = 900;
        req_login.userName = null;

        // 테스트
        req_login.id = "hong1@gmail.com";

        // 시리얼 라이즈
        var json = JsonConvert.SerializeObject(req_login);

        // 포스트로 보낸다.
        if (App.Instance != null)
        {
            App.Instance.reqCoroutine =
            StartCoroutine(Protocol.Post("api/doLogin", json, data => LoginResponse(data)));
        }
    }

    public void LoginResponse(string data)
    {
        // 응답 받은 데이터를 디시리얼라이즈 한다.
        Protocol.res_login res_login = JsonConvert.DeserializeObject<Protocol.res_login>(data);

        
        if(res_login==null)
        {
            Protocol.ResponseError(Protocol.eErrorType.Null);
            return;
        }

        // 로그인 명령어
        if (res_login.cmd != 300)
        {
            Protocol.ResponseError(Protocol.eErrorType.Login);
            return;
        }

        /* 로그인이 성공하면.
        * 다음 UI를 표시한다.
        */
        Protocol.token = res_login.token;

        stageGo.SetActive(true);

        // 로그인에 성곡하면 기본 목록을 보여준다.
        GetAllStages(-1);

        btnLogin.enabled = false;
    }

    void GetAllStages(int sortType)
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
    StartCoroutine(Protocol.Get(strSortType, json, data => GetAllStageResponse(data), "authorization", Protocol.token));
        }

        // 솔트타입으로 들어온 항목만 보여준다.
        ShowListItemSortType(sortType);
    }

    void ShowListItemSortType(int sortType)
    {
        for (int i = 0; i < listStageItem.Count; i++)
        {
            listStageItem[i].txtFavoriteCount.gameObject.SetActive(false);
            listStageItem[i].txtClearCount.gameObject.SetActive(false);
            listStageItem[i].txtElapsedScore.gameObject.SetActive(false);
            listStageItem[i].txtCreateAt.gameObject.SetActive(false);

            switch (sortType)
            {
                case 0:
                    listStageItem[i].txtFavoriteCount.gameObject.SetActive(true);
                    break;

                case 1:
                    listStageItem[i].txtClearCount.gameObject.SetActive(true);
                    break;

                case 2:
                    listStageItem[i].txtElapsedScore.gameObject.SetActive(true);
                    break;

                case 3:
                    listStageItem[i].txtCreateAt.gameObject.SetActive(true);
                    break;

                default:
                    listStageItem[i].txtCreateAt.gameObject.SetActive(true);
                    break;
            }
        }
    }

    void GetAllStageResponse(string data)
    {
        // 디시리얼라이즈 한다.
        var res_getAllStages = JsonConvert.DeserializeObject<Protocol.res_getStages>(data);

        if(res_getAllStages == null)
        {
            Protocol.ResponseError(Protocol.eErrorType.Null);
            return;
        }

        if(res_getAllStages.cmd != 400)
        {
            Protocol.ResponseError(Protocol.eErrorType.getAllStages);
            return;
        }

        for (int i = 0; i < listStageItem.Count; i++)
        {
            if (res_getAllStages.arrStageInfo.Count() <= i)
                continue;

            Protocol.res_stage_info stageInfo = res_getAllStages.arrStageInfo[i];

            listStageItem[i].mapId = stageInfo.map_id;

            listStageItem[i].txtStageId.text = stageInfo.stage_id.ToString();
            listStageItem[i].txtTitle.text = stageInfo.title;
            listStageItem[i].txtUserName.text = stageInfo.user_name;

            listStageItem[i].txtCreateAt.text = stageInfo.created_at;
            listStageItem[i].txtClearCount.text = stageInfo.clear_count.ToString();
            listStageItem[i].txtFavoriteCount.text = stageInfo.favorite_count.ToString();
            listStageItem[i].txtElapsedScore.text = stageInfo.elapsed_score.ToString();
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

        //UnitInfos.arrUnitInfo = new List<unit_info>();
        //for (int i = 0; i < res_doPlay.arrUnitInfo.Length; i++)
        //{
        //    unit_info unit_Info = new unit_info(res_doPlay.arrUnitInfo[i]);
        //    UnitInfos.arrUnitInfo.Add(unit_Info);
        //}

        App.Instance.LoadScene(playSceneName);
    }

    void GetRank()
    {
        // req_getAllStage 구조체를 채운다.
        var req_getRank = new Protocol.req_getRank();
        req_getRank.cmd = 700;

        // 시리얼 라이즈
        var json = JsonConvert.SerializeObject(req_getRank);

        string api = string.Format("api/getRank/{0}", "hong1@gmail.com");

        if (App.Instance != null)
        {
            App.Instance.reqCoroutine =
    StartCoroutine(Protocol.Get(api,
            json, data => GetAllStageResponse(data), "authorization", Protocol.token));
        }
    }

    void GetRankResponse(string data)
    {
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

        Protocol.res_user_info[] arrUserInfo = res_getRank.arrUserInfo;

        GetMyRank();
    }

    void GetMyRank()
    {
        // req_getAllStage 구조체를 채운다.
        var req_getMyRank = new Protocol.req_getMyRank();
        req_getMyRank.cmd = 800;
        req_getMyRank.user_id = "hong2@gmail.com";

        // 시리얼 라이즈
        var json = JsonConvert.SerializeObject(req_getMyRank);


        string api = string.Format("api/getMyRank/{0}", "hong1@gmail.com");

        if (App.Instance != null)
        {
            App.Instance.reqCoroutine =
    StartCoroutine(Protocol.Get(api,
            json, data => GetAllStageResponse(data), "authorization", Protocol.token));
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
    }
}
