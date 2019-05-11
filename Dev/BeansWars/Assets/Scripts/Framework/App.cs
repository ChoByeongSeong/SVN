using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class App : MonoBehaviour
{
    // 싱글톤
    public static App Instance;
    public string startSceneName;

    public GameObject goLoading;
    public Image imgLoading;

    public Coroutine reqCoroutine;

    public GameObject goMessage;
    Coroutine messageCoroutine;
    Animator animError;
    Text txtError;

    // 로딩 화면 잠금 트위너
    Coroutine showLoading;

    private void Awake()
    {
        App.Instance = this;
        DontDestroyOnLoad(this);

        Application.targetFrameRate = 60;
        Screen.SetResolution(1280, 720, true);

        animError = goMessage.GetComponent<Animator>();
        txtError = goMessage.GetComponentInChildren<Text>();

    }

    // 다음 씬으로 넘기는 매소드에 매개변수를 넣고 호출
    void Start()
    {
        this.LoadScene(this.startSceneName, LoadSceneMode.Single);
    }

    // 다음씬으로 넘기는 매소드 
    // string과 로드씬 모드 매개변수로 받는다.
    public void LoadScene(string sceneName, LoadSceneMode sceneMode = LoadSceneMode.Single, Action onComplete = null)
    {
        System.GC.Collect();

        StartCoroutine(LoadSceneImpl(sceneName, sceneMode, onComplete));
    }

    IEnumerator LoadSceneImpl(string sceneName, LoadSceneMode sceneMode, Action onComplete = null)
    {
        ShowLoading();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, sceneMode);

        // 비동기씬 로드가 끝날때 까지 기다린다.
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (onComplete != null) onComplete();

        goLoading.SetActive(false);
    }

    public void ShowLoading()
    {
        goLoading.SetActive(true);

        imgLoading.color = new Color(0, 0, 0, 0);
        
        if(showLoading!=null)
        {
            StopCoroutine(showLoading);
            showLoading = null;
        }
        showLoading = StartCoroutine(ShowLoadingImpl());
    }

    IEnumerator ShowLoadingImpl()
    {
        float speed = 0.5f;

        while (imgLoading.color.a <= 0.4f)
        {
            imgLoading.color += new Color(0, 0, 0, speed * Time.deltaTime);

            yield return null;
        }
    }


    // 초를 재주는 코루틴
    // t : 카운트 시간 
    // onComplete : 경과시간 완료를 알려주는 대리자
    public IEnumerator WaitForSeconds(float t, System.Action onComplete)
    {
        yield return new WaitForSeconds(t);

        onComplete();
    }

    public void ShowMessage(string message)
    {
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
        }

        messageCoroutine = StartCoroutine(ShowErrorImpl(message));
    }

    IEnumerator ShowErrorImpl(string message)
    {
        goMessage.SetActive(true);
        animError.Play("error");
        txtError.text = "";

        yield return new WaitForSeconds(0.5f);
        txtError.text = message;

        yield return new WaitForSeconds(1.5f);

        goMessage.SetActive(false);
    }

    /// <summary>
    /// 페이드인 점점 밝게
    /// dimsprite : 화면을 가릴때 쓰는 이미지
    /// onComplete : 페이드인의 완료를 알려주는 대리자
    /// DOTween.ToAlpha는 닷트윈에서 지원해주는 함수 닥스 참조
    /// </summary>
    public void FadeIn(Image dimSprite, float duration, System.Action onComplete = null)
    {
        dimSprite.gameObject.SetActive(true);
        dimSprite.color = new Color(0, 0, 0, 0);

        DOTween.ToAlpha(() => dimSprite.color, x => dimSprite.color = x, 1, duration).onComplete = () =>
        {
            if (onComplete != null) onComplete();
        };
    }

    /// <summary>
    /// dimsprite : 화면을 가릴때 쓰는 이미지
    /// onComplete : 페이드인의 완료를 알려주는 대리자
    /// DOTween.ToAlpha는 닷트윈에서 지원해주는 함수 닥스 참조
    /// </summary>
    public void FadeOut(Image dimSprite, float duration, System.Action onComplete = null)
    {
        dimSprite.gameObject.SetActive(true);
        dimSprite.color = new Color(0, 0, 0, 1);

        DOTween.ToAlpha(() => dimSprite.color, x => dimSprite.color = x, 0, duration).onComplete = () =>
        {
            dimSprite.gameObject.SetActive(false);
            if (onComplete != null) onComplete();
        };
    }
}
