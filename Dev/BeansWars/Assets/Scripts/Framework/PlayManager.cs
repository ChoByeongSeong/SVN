using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public partial class PlayManager : MonoBehaviour
{
    public string playSceneName;
    public string modeSelectSceneName;

    enum ePlayState
    {
        Tutorial,
        CameraZoom,
        Ready,
        Play,
        Win,
        Lose,
        End
    }

    ePlayState playState = ePlayState.Tutorial;    // 진행 상태.
    Camera mainCamera;          // 메인 카메라.
    bool cameraMode = false;
    TouchCamera touchCamera;
    public RectTransform rtContent;

    YellowBean.Grid grid;       // 길찾기 그리드.

    float touchTime = float.MaxValue;
    float touchTimeLimit = 0.05f;
    bool twoTouch;

    [Header("Play")]
    public Button btnPlay;
    public UIAnimationHandler animBottom;
    public UIAnimationHandler animTop;

    [Header("Save")]
    public Button btnSave;
    public Button btnSaveOK;

    public InputField ifTitle;
    public GameObject goSave;

    [Header("Pause")]
    public Button btnPause;
    public GameObject goPause;

    [Header("Undo/ Reset")]
    public Button btnUndo;
    public Button btnReset;

    [Header("Unit Toggle")]
    public Toggle[] arrUnitToggle;

    [Header("Time/ Cost")]
    public Text txtTime;
    public float limitTime;
    bool overTime;
    public float timerScale;
    float startTime;

    [Header("TuTorial")]
    public GameObject goTutorial;

    [Header("Win/ Lose")]
    public GameObject goEnd;
    public Animation animEnd;
    public AnimEventer eventer;
    public Image imgEndDim;

    [Header("Like")]
    public Button btnLike;
    public GameObject goHeart;
    public GameObject goLike;
    public Text txtScore;

    public Button btnNextStage;
    public Button btnRestart;
    public Button btnBackStage;

    // 코스트
    public Text txtCost;
    int maxCost;

    // 비용 말풍선
    public GameObject goBalloon;
    Coroutine animBallRoutine;
    private Animation animBall;

    // 슬라이더
    public Slider sliderY;
    public Image imgSliderY;
    float yMaxCost;
    float yOldCost;
    Coroutine yellowSlideCoroutine;
    Tweener ySliderTweener;

    public Slider sliderG;
    public Image imgSliderG;
    float gMaxCost;
    float gOldCost;
    Coroutine greeenSlideCoroutine;
    Tweener gSliderTweener;

    [Header(" ")]
    public Transform mapTr;
    // 유닛 위치
    public Transform unitsTr;

    // 베이스, 플레이를 누르면 사라진다.
    GameObject goBase;

    // 유닛 리스트
    List<Unit> listUnit = new List<Unit>();
    List<Unit> listEnemies = new List<Unit>();

    // 선택된 유닛 이름
    string selectedUnitName;

    private void Awake()
    {
        mainCamera = Camera.main;
        touchCamera = mainCamera.GetComponent<TouchCamera>();
        touchCamera.enabled = false;

        this.grid = FindObjectOfType<YellowBean.Grid>();

        this.animBall = this.goBalloon.GetComponent<Animation>();
    }

    void Start()
    {
        // 플레이 데이터가 있으면 초기화 한다.
        Initialize();

        // UI 관련 부분을 초기화한다.
        InitUI();

        // 튜토리얼 초기화.
        goTutorial.GetComponent<UITutorial>().onComplete = () =>
        {
            goTutorial.SetActive(false);
            playState = ePlayState.CameraZoom;
        };
    }

    void Update()
    {
        switch (playState)
        {
            case ePlayState.Tutorial:
                {
                    if (PlayData.Mode != PlayData.ePlayMode.Story || PlayData.id != 1)
                    {
                        playState = ePlayState.CameraZoom;
                        return;
                    }
                    goTutorial.SetActive(true);
                }
                break;

            case ePlayState.CameraZoom:
                {
                    if (!cameraMode)
                    {
                        cameraMode = true;

                        mainCamera.DOOrthoSize(11f, 0.8f).onComplete = () =>
                        {
                            mainCamera.DOOrthoSize(7f, 0.8f).onComplete = () =>
                            {
                                playState = ePlayState.Ready;
                                touchCamera.enabled = true;
                            };
                        };
                    }

                }
                break;

            case ePlayState.Ready:
                {
                    if(Input.touchCount == 2 )
                    {
                        twoTouch = true;
                    }

                    if(twoTouch)
                    {
                        if (Input.touchCount == 0) twoTouch = false;
                    }

                    touchTime += Time.deltaTime;

                    // 터치
                    if (Input.touchCount == 1 && touchTime>= touchTimeLimit && !IsPointerOverUIObject() && !twoTouch)
                    {
                        touchTime = 0f;
                        StartCoroutine(this.Touch());
                    }

                    else
                    {
                        //if (Input.GetMouseButtonDown(0))
                        //{
                        //    StartCoroutine(this.Touch());
                        //}
                    }
                }
                break;

            case ePlayState.Play:
                {
                    Play();
                }
                break;

            case ePlayState.Win:
                {
                    if (!imgEndDim.gameObject.activeSelf)
                    {
                        imgEndDim.gameObject.SetActive(true);
                        imgEndDim.color = new Color(0.11f, 0.025f, 0.025f, 0);

                        DOTween.ToAlpha(() => imgEndDim.color, x => imgEndDim.color = x, 0.89f, 0.8f).onComplete = () =>
                        {
                            imgEndDim.enabled = false;

                            if (PlayData.Mode == PlayData.ePlayMode.Story)
                                WinStoryMode();

                            else
                                WinUserMode();
                        };
                    }
                }
                break;

            case ePlayState.Lose:
                {
                    if (!imgEndDim.gameObject.activeSelf)
                    {
                        imgEndDim.gameObject.SetActive(true);
                        imgEndDim.color = new Color(0.11f, 0.025f, 0.025f, 0);

                        DOTween.ToAlpha(() => imgEndDim.color, x => imgEndDim.color = x, 0.89f, 0.8f).onComplete = () =>
                        {
                            imgEndDim.enabled = false;

                            if (PlayData.Mode == PlayData.ePlayMode.Story)
                                LoseStoryMode();

                            else
                                LoseUserMode();
                        };
                    }
                }
                break;

            case ePlayState.End:
                {

                }
                break;
        }
    }

    void Play()
    {
        // 시간을 센다.
        {
            float t = Time.time - startTime;

            if (!overTime)
            {
                if (t > limitTime)
                {
                    overTime = true;

                    //// 타이머
                    //txtTime.rectTransform.DOScale(new Vector3(1.5f,2,1), 1).onComplete = () => {
                    //    txtTime.rectTransform.DOScale(new Vector3(1, 1, 1), 1f);
                    //};
                    txtTime.DOColor(new Color(1, 0, 0, 1), 30);
                }
            }

            string sec = t.ToString("f2");

            txtTime.text = string.Format("{0}", sec);
        }

        // 코스트를 계산한다.
        {
            float yNewCost = GetCurrentCost(listUnit);
            float gNewCost = GetCurrentCost(listEnemies);

            // 만약 코스트가 바꼈다면.
            if (yNewCost != yOldCost)
            {
                if(ySliderTweener !=null)
                {
                    ySliderTweener.Kill();
                    ySliderTweener = null;
                }
                ySliderTweener = imgSliderY.rectTransform.DOShakeScale(.25f, .14f);
                ySliderTweener.onComplete = () =>
                {
                    ySliderTweener = imgSliderY.rectTransform.DOScale(Vector3.one, .1f);
                };


                // 코스트를 줄인다.
                if (yellowSlideCoroutine != null)
                {
                    StopCoroutine(yellowSlideCoroutine);
                    yellowSlideCoroutine = null;
                }

                yellowSlideCoroutine = StartCoroutine(FadeSlider(sliderY, yNewCost / yMaxCost));

                // 올드 코스트를 갱신한다.
                yOldCost = yNewCost;
            }

            // 만약 코스트가 바꼈다면.
            if (gNewCost != gOldCost)
            {
                if (gSliderTweener != null)
                {
                    gSliderTweener.Kill();
                    gSliderTweener = null;
                }
                gSliderTweener = imgSliderG.rectTransform.DOShakeScale(.25f, .14f);

                gSliderTweener.onComplete = () =>
                {
                    gSliderTweener = imgSliderG.rectTransform.DOScale(Vector3.one, .1f);
                };

                // 코스트를 줄인다.
                if (greeenSlideCoroutine != null)
                {
                    StopCoroutine(greeenSlideCoroutine);
                    greeenSlideCoroutine = null;
                }

                greeenSlideCoroutine = StartCoroutine(FadeSlider(sliderG, gNewCost / gMaxCost));

                // 올드 코스트를 갱신한다.
                gOldCost = gNewCost;
            }


            if (gNewCost <= 0)
            {
                playState = ePlayState.Win;

                return;
            }

            if (yNewCost <= 0)
            {
                playState = ePlayState.Lose;

                return;
            }
        }
    }

    void WinUserMode()
    {
        // 클리어 스테이지.
        ClearStage();

        // 좋아요, 리스타트, 스테이지.
        btnLike.gameObject.SetActive(true);
        btnRestart.gameObject.SetActive(true);
        btnBackStage.gameObject.SetActive(true);

        // 엔드 오브젝트
        goEnd.SetActive(true);
        animEnd.Play("UIstageEndPopupTrophy");
        if (YellowBean.SoundManager.Instance != null)
        {
            YellowBean.SoundManager.Instance.bgm.enabled = false;
            YellowBean.SoundManager.Instance.PlaySFX("victory");
        }

        playState = ePlayState.End;
    }

    void WinStoryMode()
    {
        // 승리 조건을 계산한다.
        int starCnt = 0;

        // 승리 했다면.
        {
            starCnt += 1;
        }

        // 1분 안으로 끝냈다면.
        {
            float t = Time.time - startTime;

            starCnt = (t < limitTime) ? starCnt += 1 : starCnt;
        }

        // 체력이 10프로 이상이라면.
        {
            float currentYellowHp = 0f;
            foreach (var unit in listUnit)
            {
                if (unit.alive) currentYellowHp += unit.status.cost;
            }

            starCnt = (currentYellowHp / yMaxCost > 0.1f) ? starCnt += 1 : starCnt;
        }

        // 이겼을 때.
        // 유저 정보를 갱신한다.
        var arr = DataManager.GetInstance().userInfo.arrStageInfo;

        // 별 개수가 현재 클리어한 별 개수보다 작다면.
        // 갱신한다.
        if (arr[PlayData.id - 1].starCnt < starCnt)
        {
            arr[PlayData.id - 1].starCnt = starCnt;
        }

        // 다음 스테이지를 오픈한다.
        if (PlayData.id < arr.Length)
        {
            if (!arr[PlayData.id].opened)
            {
                arr[PlayData.id].opened = true;
                arr[PlayData.id].starCnt = 0;
            }
        }

        // 리스타트, 스테이지.
        btnNextStage.gameObject.SetActive(true);
        btnRestart.gameObject.SetActive(true);
        btnBackStage.gameObject.SetActive(true);

        // 엔드 오브젝트
        goEnd.SetActive(true);
        animEnd.Play(string.Format("UIstageEndPopupStar{0}", starCnt));
        if (YellowBean.SoundManager.Instance != null)
        {
            YellowBean.SoundManager.Instance.bgm.enabled = false;
            YellowBean.SoundManager.Instance.PlaySFX("victory");
        }

        // 저장한다.
        DataManager.GetInstance().SaveUserInfo();

        playState = ePlayState.End;
    }

    void LoseUserMode()
    {
        // 클리어 실패.
        FailedStage();

        eventer.onComplete = () =>
        {

            // 좋아요, 리스타트, 스테이지.
            btnLike.gameObject.SetActive(true);
            btnRestart.gameObject.SetActive(true);
            btnBackStage.gameObject.SetActive(true);
        };

        // 엔드 오브젝트
        goEnd.SetActive(true);
        animEnd.Play("UIstageEndPopupLose");

        if (YellowBean.SoundManager.Instance != null)
        {
            YellowBean.SoundManager.Instance.bgm.enabled = false;
            YellowBean.SoundManager.Instance.PlaySFX("defeat");
        }

        playState = ePlayState.End;
    }

    void LoseStoryMode()
    {
        eventer.onComplete = () =>
        {

            // 리스타트, 스테이지.
            btnRestart.gameObject.SetActive(true);
            btnBackStage.gameObject.SetActive(true);
        };

        // 엔드 오브젝트
        goEnd.SetActive(true);
        animEnd.Play("UIstageEndPopupLose");
        if (YellowBean.SoundManager.Instance != null)
        {
            YellowBean.SoundManager.Instance.bgm.enabled = false;
            YellowBean.SoundManager.Instance.PlaySFX("defeat");
        }

        playState = ePlayState.End;
    }

    float GetCurrentCost(List<Unit> listUnit)
    {
        float totalCost = 0;

        foreach (var unit in listUnit)
        {
            if (unit.alive)
                totalCost += unit.status.cost;
        }

        return totalCost;
    }

    MapData GetMapData(int id)
    {
        return DataManager.GetInstance().dicMapData[id];
    }

    void CreateGround(MapData data)
    {
        // 그라운드 생성.
        GameObject groundPrefab = PrefabsManager.GetInstance().dicGroundPrefabs[data.groundName];
        var go = Instantiate(groundPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        go.transform.parent = mapTr;
    }

    void CreateEnvironment(MapData data)
    {
        // 환경 생성.
        for (int i = 0; i < data.listEnvirnment.Count; i++)
        {
            GameObject envirGo = PrefabsManager.GetInstance().dicEnvironmentPrefabs[data.listEnvirnment[i].name];
            Vector3 pos = new Vector3(data.listEnvirnment[i].x, data.listEnvirnment[i].y, 0);

            var go = Instantiate(envirGo, pos, Quaternion.identity);
            go.transform.parent = mapTr;
        }
    }

    void CreateBase(string baseName)
    {
        // 그라운드 생성.
        GameObject basePrefab = PrefabsManager.GetInstance().dicBasePrefabs[baseName];
        goBase = Instantiate(basePrefab);
    }

    void CreateGrid(MapData data)
    {
        // Grid 생성
        grid.gridWorldSize = new Vector2(data.grid_x, data.grid_y);
        grid.nodeRadius = data.node_radius;
        grid.CreateGrid();
    }

    void CreateUnit()
    {
        // 유닛 생성
        for (int i = 0; i < PlayData.listUnitInfos.Count; i++)
        {
            var info = PlayData.listUnitInfos[i];

            YBEnum.eUnitName eUnitName = UnitName.ParseToEnum(info.unit_name);
            YBEnum.eColorType colorType = (YBEnum.eColorType)info.color;
            Vector2 pos = new Vector2(info.x, info.y);

            Unit unit = UnitsPool.instance.CreateUnit(eUnitName, colorType, pos);
            unit.transform.parent = unitsTr;

            var trModel = unit.transform.Find("Model").transform;

            // 왼쪽이 0
            int dir = (0 - unit.transform.position.x < 0) ? 0 : 1;
            if (unit.status.name.CompareTo("Catapult") == 0)
            {
                dir = (0 - unit.transform.position.x < 0) ? 1 : 0;
            }

            trModel.rotation = Quaternion.Euler(new Vector3(0, 180 * dir, 0));

            listEnemies.Add(unit);
        }
    }

    void Initialize()
    {
        if (PlayData.id == -1)
            return;

        // 맵을 로드한다.
        MapData mapData = GetMapData(PlayData.map_id);

        switch (PlayData.Mode)
        {
            case PlayData.ePlayMode.None:
                break;

            case PlayData.ePlayMode.Editor:
                // 버튼 설정.
                btnPlay.gameObject.SetActive(false);

                // 맵 데이터 기반.
                CreateGround(mapData);
                CreateEnvironment(mapData);

                // 맵 앞에 있으면 터치가 씹힌다.
                mapTr.Translate(new Vector3(0, 0, 100));

                // 베이스 생성.
                CreateBase(mapData.greenBaseName);
                // CreateGrid(mapData);

                // 코스트 저장
                txtCost.text = PlayData.cost.ToString();
                maxCost = PlayData.cost;

                break;

            case PlayData.ePlayMode.Story:
            case PlayData.ePlayMode.User:
                {
                    // 버튼 설정.
                    btnSave.gameObject.SetActive(false);

                    // 맵 데이터 기반.
                    CreateGround(mapData);
                    CreateEnvironment(mapData);

                    // 맵 앞에 있으면 터치가 씹힌다.
                    mapTr.Translate(new Vector3(0, 0, 100));

                    // 베이스 생성.
                    CreateBase(mapData.yellowBaseName);
                    CreateGrid(mapData);

                    // 스테이지 데이터 기반.
                    // 유닛 생성
                    CreateUnit();

                    // 코스트 저장
                    txtCost.text = PlayData.cost.ToString();
                    maxCost = PlayData.cost;
                }
                break;

            default:
                break;
        }
    }

    void
        InitUI()
    {
        if (btnUndo != null)
            btnUndo.onClick.AddListener(() =>
            {
                int cnt = listUnit.Count;

                if (cnt > 0)
                {
                    var unit = listUnit[cnt - 1];

                    maxCost += DataManager.GetInstance().GetUnitData(unit.status.name).cost;
                    txtCost.text = maxCost.ToString();

                    listUnit.Remove(unit);
                    Destroy(unit.gameObject);
                }
            });

        if (btnReset != null)
            btnReset.onClick.AddListener(() =>
            {
                foreach (var unit in listUnit)
                {
                    Destroy(unit.gameObject);
                }

                listUnit.Clear();

                maxCost = PlayData.cost;
                txtCost.text = maxCost.ToString();
            });


        if (btnPause != null)
            btnPause.onClick.AddListener(() =>
            {
                goPause.SetActive(true);
            });


        if (btnPlay != null)
            btnPlay.onClick.AddListener(() =>
        {
            YellowBean.SoundManager.Instance.bgm.enabled = true;
            YellowBean.SoundManager.Instance.PlayBgm("Castle in the sky");
            // 현재 유닛 이름을 저장한다.
            for (int i = 0; i < listUnit.Count; i++)
            {
                string unitName = listUnit[i].status.name;
                UnityAnalyticsManager.GetInstance().UseUnit(unitName);
            }

            var arrSR = goBase.GetComponentsInChildren<SpriteRenderer>();

            for (int i = 0; i < arrSR.Length; i++)
            {
                arrSR[i].DOColor(new Color(0, 0, 0, 0), 1f);
            }

            animBottom.Play("move", () =>
            {
                animTop.Play("move", () =>
                {

                    foreach (var unit in listUnit)
                    {
                        yMaxCost += unit.status.cost;

                        StartCoroutine(StartAI(unit));
                    }

                    foreach (var enemy in listEnemies)
                    {
                        gMaxCost += enemy.status.cost;

                        StartCoroutine(StartAI(enemy));
                    }

                    yOldCost = yMaxCost;
                    gOldCost = gMaxCost;

                    playState = ePlayState.Play;
                    startTime = Time.time;
                });
            });
        });

        for (int i = 0; i < arrUnitToggle.LongLength; i++)
        {
            // 토글을 가져온다.
            var toggleUnit = arrUnitToggle[i];

            // 토글에 맞는 유닛의 데이터를 가져온다.
            var data = DataManager.GetInstance().GetUnitData(toggleUnit.name);

            var txtCost = toggleUnit.GetComponentInChildren<Text>();
            txtCost.text = data.cost.ToString();

            // 토글에 이벤트를 추가한다.
            toggleUnit.onValueChanged.AddListener((on) =>
            {
                if (on)
                {
                    selectedUnitName = toggleUnit.name;
                }
            });
        }

        btnLike.onClick.AddListener(() =>
        {
            DoFavoriteStage();
        });

        btnRestart.onClick.AddListener(() =>
        {
            if (App.Instance != null)
            {
                btnRestart.enabled = false;
                App.Instance.LoadScene(playSceneName);
            }
        });

        btnBackStage.onClick.AddListener(() =>
        {
            if (App.Instance != null)
            {
                btnBackStage.enabled = false;
                App.Instance.LoadScene(modeSelectSceneName);
            }
        });

        btnNextStage.onClick.AddListener(() =>
        {
            int stageID = PlayData.id;
            Debug.Log(stageID);

            if (DataManager.GetInstance().dicStoryStageData.ContainsKey(stageID++))
            {
                var stageData = DataManager.GetInstance().dicStoryStageData[stageID];
                PlayData.id = stageData.id;
                PlayData.map_id = stageData.map_id;
                PlayData.cost = stageData.cost;
                PlayData.Mode = PlayData.ePlayMode.Story;
                PlayData.listUnitInfos = stageData.listUnitInfos;

                if (App.Instance != null)
                {
                    App.Instance.LoadScene(playSceneName);
                }
            }
        });

        btnSave.onClick.AddListener(() =>
        {
            if (PlayData.cost - GetCurrentCost(listUnit) > 40f)
            {
                animBall.Rewind();
                var animClip = animBall.clip;

                goBalloon.SetActive(true);

                if (animBallRoutine != null)
                {
                    StopCoroutine(animBallRoutine);
                    animBallRoutine = null;
                }
                animBallRoutine = StartCoroutine(this.WaitForTime(animClip.length, () =>
                {
                    goBalloon.SetActive(false);
                }));

                return;
            }

            ifTitle.text = DataManager.GetInstance().dicMapData[PlayData.map_id].map_name;
            goSave.SetActive(true);
        });

        btnSaveOK.onClick.AddListener(() =>
        {
            DoSave();
        });

        // 콘텐츠 맨 앞.
        rtContent.anchoredPosition3D = Vector3.zero;

        // 애니메이션
        animBottom.Play("start", null);
    }


    IEnumerator WaitForTime(float t, Action onComplete)
    {
        yield return new WaitForSeconds(t);
        onComplete();
    }


    IEnumerator StartAI(Unit unit)
    {
        yield return new WaitForSeconds(0.3f);

        float randomTime = UnityEngine.Random.Range(0, 0.5f);
        yield return new WaitForSeconds(randomTime);

        unit.onAi = true;
    }

    IEnumerator Touch()
    {
        /* 터치가 한번 있었을 때
         * 움직이면 false;
         * 터치하나더 생기면 false;
         */
        float time = 0;
        // Vector2 oldPos = Input.GetTouch(0).position;

        while (time < 0.05f)
        {
            time += Time.deltaTime;

            //float dst = (oldPos - Input.GetTouch(0).position).magnitude;
            //if (dst > 1f)
            //{
            //    yield break;
            //}

            if (Input.touchCount == 2)
            {
                yield break;
            }

            yield return null;
        }

        if (this.selectedUnitName != null)
        {
            // 코스트를 계산한다.
            var data = DataManager.GetInstance().GetUnitData(selectedUnitName);
            var unitCost = data.cost;

            if (maxCost - unitCost < 0)
            {
                yield break;
            }

            else
            {
                Vector3 touchPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                Ray2D ray = new Ray2D(touchPos, Vector2.zero);
                RaycastHit2D hit;

                hit = Physics2D.Raycast(ray.origin, ray.direction);

                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("YellowBase") || hit.collider.CompareTag("GreenBase"))
                    {

                        // 코스트 계산한다.
                        maxCost -= unitCost;
                        txtCost.text = maxCost.ToString();

                        // 소환
                        YBEnum.eUnitName strToEnum = UnitName.ParseToEnum(this.selectedUnitName);

                        // 리스트에 추가한다.
                        var unit = UnitsPool.instance.CreateUnit(strToEnum, YBEnum.eColorType.Yellow, hit.point);
                        var trModel = unit.transform.Find("Model").transform;

                        // 왼쪽이 0
                        int dir = (0 - unit.transform.position.x < 0) ? 0 : 1;
                        if (unit.status.name.CompareTo("Catapult") == 0)
                        {
                            dir = (0 - unit.transform.position.x < 0) ? 1 : 0;
                        }

                        trModel.rotation = Quaternion.Euler(new Vector3(0, 180 * dir, 0));

                        listUnit.Add(unit);

                        if (hit.collider.CompareTag("YellowBase"))
                        {
                            YellowBean.SoundManager.Instance.PlaySFX(string.Format("y_idle{0}", UnityEngine.Random.Range(0, 10)));
                        }
                        else
                        {
                            YellowBean.SoundManager.Instance.PlaySFX(string.Format("g_idle{0}", UnityEngine.Random.Range(0, 12)));
                        }
                    }
                }
            }
        }
    }


    IEnumerator FadeSlider(Slider slider, float value)
    {
        while (slider.value - value > 0.01f)
        {
            slider.value -= 0.3f * Time.deltaTime;

            yield return null;
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }
}
