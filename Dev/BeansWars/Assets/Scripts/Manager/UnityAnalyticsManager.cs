using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;


public class UnityAnalyticsManager : MonoBehaviour
{
    public static UnityAnalyticsManager instance;
    private bool quitFlag = false;
    private float userPlayTime;

    private UnityAnalyticsManager()
    {
        UnityAnalyticsManager.instance = this;
    }

    public static UnityAnalyticsManager GetInstance()
    {
        if (UnityAnalyticsManager.instance == null)
        {
            UnityAnalyticsManager.instance = new UnityAnalyticsManager();
        }

        return UnityAnalyticsManager.instance;
    }

    private void Update()
    {
        this.userPlayTime = Time.realtimeSinceStartup;       
    }


    private void OnApplicationQuit()
    {
        this.quitFlag = true;
        this.HowLongPlay(userPlayTime);
        Analytics.CustomEvent("종료 분석", new Dictionary<string, object>
            {
                {"종료",quitFlag },              
            });
    }

    public void OnClickButton()
    {
        int count = 1;
        Analytics.CustomEvent("ClickButton", new Dictionary<string, object>
            {
                {"플레이 버튼 클릭",count }
            });
    }

    public void OnClickCreateMap()
    {
        int count = 1;
        Analytics.CustomEvent("ClickButtonCreateMap", new Dictionary<string, object>
            {
                {"맵 만들기 버튼 클릭",count }
            });
    }

    public void HowLongPlay(float playTime)
    {
        Analytics.CustomEvent("HowLongPlayGame", new Dictionary<string, object>
        {
            {"유저 게임 플레이 시간",playTime/60}
        });
    }

    public void UseUnit(string name)
    {
        switch (name)
        {
            case "Archer":
                {
                    int count = 1;
                    Analytics.CustomEvent("UseArcher", new Dictionary<string, object>
                    {

                        {"궁수콩 사용",count },
                        {"종료?",quitFlag}
                    });
                }
                break;
            case "Golem":
                {
                    int count = 1;
                    Analytics.CustomEvent("UseGolem", new Dictionary<string, object>
                    {
                        {"골렘콩 사용",count },
                        {"종료?",quitFlag}
                    });
                }
                break;
            case "Warrior":
                {
                    int count = 1;
                    Analytics.CustomEvent("UseWarrior", new Dictionary<string, object>
                    {
                        {"전사콩 사용",count },
                        {"종료?",quitFlag}
                    });
                }
                break;
            case "Defender":
                {
                    int count = 1;
                    Analytics.CustomEvent("UseDefender", new Dictionary<string, object>
                    {
                        {"방패콩 사용",count },
                        {"종료?",quitFlag}
                    });
                }
                break;
            case "Catapult":
                {
                    int count = 1;
                    Analytics.CustomEvent("UseCatapult", new Dictionary<string, object>
                    {
                        {"투석콩 사용",count },
                        {"종료?",quitFlag}
                    });
                }
                break;
            case "Warlock":
                {
                    int count = 1;
                    Analytics.CustomEvent("UseWarlock", new Dictionary<string, object>
                    {
                        {"법사콩 사용",count },
                        {"종료?",quitFlag}
                    });
                }
                break;
        }
    }

    public void UserLoginCount()
    {
        int count = 1;
        Analytics.CustomEvent("UserLogin", new Dictionary<string, object>
            {
                {"유저 로그인",count },
                {"종료?",quitFlag}
            });
    }
}
