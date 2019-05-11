using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class StaticCoroutine : MonoBehaviour
{
    private static StaticCoroutine instacne = null;

    private static StaticCoroutine Instance
    {
        get
        {
            if (instacne == null)
            {
                instacne = GameObject.FindObjectOfType(typeof(StaticCoroutine)) as StaticCoroutine;

                if (instacne == null)
                {
                    instacne = new GameObject("StaticCoroutine").AddComponent<StaticCoroutine>();
                }
            }
            return instacne;
        }
    }

    void Awake()
    {
        if (instacne == null)
        {
            instacne = this as StaticCoroutine;
        }
    }

    // 다음씬으로 넘기는 매소드 
    // string과 로드씬 모드 매개변수로 받는다.
    public void LoadScene(string sceneName, LoadSceneMode sceneMode = LoadSceneMode.Single, Action onComplete = null)
    {
        StartCoroutine(LoadSceneImpl(sceneName, sceneMode, onComplete));
    }

    IEnumerator LoadSceneImpl(string sceneName, LoadSceneMode sceneMode, Action onComplete = null)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, sceneMode);

        // 비동기씬 로드가 끝날때 까지 기다린다.
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (onComplete != null) onComplete();
    }


    // 초를 재주는 코루틴
    // t : 카운트 시간 
    // onComplete : 경과시간 완료를 알려주는 대리자
    public IEnumerator WaitForSeconds(float t, System.Action onComplete)
    {
        yield return new WaitForSeconds(t);

        onComplete();
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
        dimSprite.color = new Color(1, 1, 1, 0);

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
        dimSprite.color = new Color(1, 1, 1, 1);

        DOTween.ToAlpha(() => dimSprite.color, x => dimSprite.color = x, 0, duration).onComplete = () =>
        {
            dimSprite.gameObject.SetActive(false);
            if (onComplete != null) onComplete();
        };
    }

    IEnumerator Perform(IEnumerator coroutine)
    {
        yield return StartCoroutine(coroutine);

        Die();
    }

    public static void DoCoroutine(IEnumerator coroutine)
    {
        //여기서 인스턴스에 있는 코루틴이 실행될 것이다.
        instacne.StartCoroutine(instacne.Perform(coroutine));
    }

    void Die()
    {
        instacne = null;

        Destroy(gameObject);
    }

    void OnApplicationQuit()
    {
        instacne = null;
    }
}
