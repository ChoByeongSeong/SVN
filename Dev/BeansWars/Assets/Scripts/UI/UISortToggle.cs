using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISortToggle : MonoBehaviour
{
    public Button btnLike;
    public Button btnClear;
    public Button btnScore;
    public Button btnDate;

    GameObject goLike;
    GameObject goLikeColor;

    GameObject goClear;
    GameObject goClearColor;

    GameObject goScore;
    GameObject goScoreColor;

    GameObject goDate;
    GameObject goDateColor;

    private void Awake()
    {
        goLike = btnLike.transform.Find("idle").gameObject;
        goLikeColor = btnLike.transform.Find("color").gameObject;

        goClear = btnClear.transform.Find("idle").gameObject;
        goClearColor = btnClear.transform.Find("color").gameObject;

        goScore = btnScore.transform.Find("idle").gameObject;
        goScoreColor = btnScore.transform.Find("color").gameObject;

        goDate = btnDate.transform.Find("idle").gameObject;
        goDateColor = btnDate.transform.Find("color").gameObject;
    }


    public void Toggle(int sortType)
    {
        goLike.SetActive(true);
        goClear.SetActive(true);
        goScore.SetActive(true);
        goDate.SetActive(true);

        goLikeColor.SetActive(false);
        goClearColor.SetActive(false);
        goScoreColor.SetActive(false);
        goDateColor.SetActive(false);

        switch (sortType)
        {
            case -1:
            case 0:
                {
                    goLike.SetActive(false);
                    goLikeColor.SetActive(true);
                } break;

            case 1:
                {
                    goClear.SetActive(false);
                    goClearColor.SetActive(true);
                }
                break;

            case 2:
                {
                    goScore.SetActive(false);
                    goScoreColor.SetActive(true);
                }
                break;

            case 3:
                {
                    goDate.SetActive(false);
                    goDateColor.SetActive(true);
                }
                break;
        }
    }
}
