using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class UITitle : UIBase
{
    public Button btn_panel;
    public Button btn_start;
    public Text txt_touchToScreen;
    public GameObject loadingScreen;
    public Slider loadingSlider;
    public Action onComplete;
    public Text txt_appVersion;
    
    //로딩바를 실행해주는 매쏘드(매개변수로 AsyncOperation타입을 받음)
    public void LoadingProgress(AsyncOperation op)
    {
        //코루틴 LoadAsynchronously를 실행
        StartCoroutine(this.LoadAsynchronously(op, this.loadingSlider, () =>
        {
            this.txt_appVersion.text = Application.version;
            this.loadingSlider.gameObject.SetActive(false);
            this.txt_touchToScreen.gameObject.SetActive(true);
            this.btn_panel.onClick.AddListener(() =>
            {
                StopCoroutine("LoadAsynchronously");
                this.txt_touchToScreen.gameObject.SetActive(false);
                this.btn_start.gameObject.SetActive(true);
                this.btn_start.onClick.AddListener(() =>
                {
                    this.onComplete();
                });
            });
        }));
    }

    //로딩바의 value값을 주는 코루틴
    public IEnumerator LoadAsynchronously(AsyncOperation op,Slider slider, Action onComplete)
    {
        this.loadingScreen.SetActive(true);
        //op.isDone 씬로드가 완료되었나를 알려준다.
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            slider.value = progress;

            yield return null;
        }
        onComplete();
    }

    public void ShowButton()
    {
        this.btn_start.gameObject.SetActive(true);
        this.btn_start.onClick.AddListener(() =>
        {
            this.onComplete();
        });
    }
}
