using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using DG.Tweening;

public class Title : MonoBehaviour
{
    /*
     * 타이틀 화면을 보여준다.
     * 로딩씬을 로드한다.
     * 로딩씬은 데이터와 프리펩을 로드한다.
     */

    public string loadingSceneName;
    public string stageSelectSceneName;

    bool loadComplete = false;

    // UI
    public float duration;
    public Image dimSprite;

    public GameObject loadingGo;
    public GameObject loadingCompleteGo;

    public Button btnStart;
    public Text txtTouchToStart;

    private void Awake()
    {
        // 화면을 어둡게 한다.
        dimSprite.gameObject.SetActive(true);

        StartCoroutine("LoadLoadingScene");

        btnStart.onClick.AddListener(() =>
        {
            txtTouchToStart.GetComponent<RectTransform>().DOShakeScale(.15f, .5f);
            btnStart.enabled = false;

            App.Instance.FadeIn(this.dimSprite, duration, () =>
            {
                App.Instance.LoadScene(stageSelectSceneName);
            });
        });
    }

    IEnumerator LoadLoadingScene()
    {
        // 로딩씬을 로드한다.
        App.Instance.LoadScene(loadingSceneName, LoadSceneMode.Additive, () => {
            loadComplete = true;
        });

        // 로딩 이미지를 보여준다.
        yield return new WaitForSeconds(0.5f);

        App.Instance.FadeOut(dimSprite, 1f);

        // 로딩 이미지를 보여준다.
        yield return new WaitForSeconds(0.7f);

        yield return new WaitUntil(() => loadComplete);

        // 배경음악을 재생한다.
        YellowBean.SoundManager.Instance.PlayBgm("Whistle Fairy", true,0.5f);

        // 로딩 텍스트를 보여준다.
        txtTouchToStart.enabled = true;
        loadingGo.SetActive(false);

        yield return new WaitForSeconds(.3f);

        loadingCompleteGo.SetActive(true);
    }
}