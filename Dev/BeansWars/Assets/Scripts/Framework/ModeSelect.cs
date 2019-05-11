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
    [Header("Mode Select")]
    public GameObject userModeLock;
    int starLimitNum = 6;
    public Text txtStarLimit;

    public Button btnUserMode;
    public GameObject goUserMode;
    public GameObject option;

    public Button btnStageMode;
    public GameObject goStageMode;
    public GameObject goBalloon;

    Coroutine animControl;
    private Animation animBall;
   

    private void Awake()
    {
        // 필요한 컴포넌트를 가져온다.
        this.animBall = this.goBalloon.GetComponent<Animation>();

        // 모드 셀렉트 부분을 초기화 한다.
        ModeSelectInit();

        // 기존에 모드가 있을때,
        if (PlayData.Mode != PlayData.ePlayMode.None)
        {
            // 모드 중에서, 스토리일 때.
            if (PlayData.Mode == PlayData.ePlayMode.Story)
            {
                YellowBean.SoundManager.Instance.bgm.enabled = true;
                YellowBean.SoundManager.Instance.PlayBgm("Whistle Fairy");
                // 화면을 이동한다.
                // 스토리 모드 쪽으로
                Vector2 moveValue = rtStoryMode.anchoredPosition;
                rtUI.DOAnchorPos(-moveValue, duration);
            }

            // 스토리 모드가 아닐때.
            else
            {
                // 유저 모드를 초기화 한다.
                UserModeInit();
            }
        }

        // 스테이지 목록을 초기화 한다.
        StoryModeInit();

        // 첫번 째 스테이지를 활성화 한다.
        StoryModeSort();

        if (!animBall.isPlaying)
        {
            userModeLock.GetComponent<Button>().onClick.AddListener(() =>
            {
                YellowBean.SoundManager.Instance.PlaySFX("Button Click 03");
                ShowBalloon("Not Enough Star");
            });
        }

        // 유저 모드 버튼에 초기화를 추가한다.
        // 유저 모드는 단계가 필요하기 때문에 버튼에 추가한다.
        btnUserMode.onClick.AddListener(() =>
        {
            goBalloon.SetActive(false);
            UserModeInit();
        });

        btnStageMode.onClick.AddListener(() =>
        {
            goBalloon.SetActive(false);
        });
    }

    void ShowBalloon(string txt)
    {
        animBall.transform.GetComponentInChildren<Text>().text = txt;

        animBall.Rewind();
        goBalloon.SetActive(true);

        if (animControl != null)
        {
            StopCoroutine(animControl);
            animControl = null;
        }
        animControl = StartCoroutine(this.WaitForTime(animBall.clip.length, () =>
        {
            goBalloon.SetActive(false);
        }));
    }

    IEnumerator WaitForTime(float t, Action onComplete)
    {
        yield return new WaitForSeconds(t);
        onComplete();
    }
   
    void ModeSelectInit()
    {
        // 유저의 별 개수.
        int startCnt = DataManager.GetInstance().GetStarCnt();

        if (startCnt >= starLimitNum)
        {
            userModeLock.SetActive(false);
        }

        else
        {
            txtStarLimit.text = string.Format("{0} / {1}", startCnt, starLimitNum);
        }
    }
}
