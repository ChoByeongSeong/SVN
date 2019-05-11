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
    private void DoFavoriteStage()
    {
        // req_getAllStage 구조체를 채운다.
        var req_doFavoriteStage = new Protocol.req_doFavoriteStage();
        req_doFavoriteStage.cmd = 440;
        req_doFavoriteStage.stage_id = PlayData.id;

        // 시리얼 라이즈
        var json = JsonConvert.SerializeObject(req_doFavoriteStage);

        //StartCoroutine(this.Get("api/getAllStages", json, data => GetAllStageResponse(data), "authorization", token));
        if (App.Instance != null)
        {
            App.Instance.reqCoroutine =
            StartCoroutine(Protocol.Post("api/doFavoriteStage", json, data => DoFavoriteStageResponse(data)));
        }
    }

    void DoFavoriteStageResponse(string data)
    {
        // 디시리얼라이즈 한다.
        var res_doFavoriteStage = JsonConvert.DeserializeObject<Protocol.res_doFavoriteStage>(data);

        if (res_doFavoriteStage == null)
        {
            Protocol.ResponseError(Protocol.eErrorType.Null);
            return;
        }

        if (res_doFavoriteStage.cmd != 440)
        {
            Protocol.ResponseError(Protocol.eErrorType.doFavoriteStage);
            return;
        }

        // 보내기에 성공하면.
        // 좋아요 버튼 활성화를 취소한다.
        // 하트 모양을 끄고.
        // 하트를 날린다.
        btnLike.enabled = false;
        goHeart.SetActive(false);
        goLike.SetActive(true);

        //if (App.Instance != null)
        //{
        //    App.Instance.ShowMessage("좋아요!");
        //}
    }

    void DoSave()
    {
        // req_getAllStage 구조체를 채운다.
        var doSaveStage = new Protocol.req_doSaveStage();
        doSaveStage.cmd = 500;

        // 스테이지 정보를 채운다.
        // 나중에 입력을 통해 채우는 것으로 변경한다.
        doSaveStage.stageInfo = new Protocol.req_stage_info();
        doSaveStage.stageInfo.title = ifTitle.text;
        doSaveStage.stageInfo.user_id = Social.localUser.id;
        doSaveStage.stageInfo.map_id = PlayData.map_id;

        // 유닛 정보를 채운다.
        int unitCnt = listUnit.Count;
        doSaveStage.arrUnitInfo = new unit_info[unitCnt];

        for (int i = 0; i < listUnit.Count; i++)
        {
            
            Unit unit = listUnit[i];
            if (!unit.alive) continue;

            doSaveStage.arrUnitInfo[i] = new unit_info();

            doSaveStage.arrUnitInfo[i].unit_name = unit.status.name;
            doSaveStage.arrUnitInfo[i].x = unit.transform.position.x;
            doSaveStage.arrUnitInfo[i].y = unit.transform.position.y;
            doSaveStage.arrUnitInfo[i].z = unit.transform.position.z;
            doSaveStage.arrUnitInfo[i].color = 1;
        }

        // 시리얼 라이즈
        var json = JsonConvert.SerializeObject(doSaveStage);

        //StartCoroutine(this.Get("api/getAllStages", json, data => GetAllStageResponse(data), "authorization", token));
        if (App.Instance != null)
        {
            App.Instance.reqCoroutine =
            StartCoroutine(Protocol.Post("api/doSaveStage", json, data => DoSaveResponse(data)));
        }
    }

    void DoSaveResponse(string data)
    {
        // 시리얼 라이즈
        var res_doSaveState = JsonConvert.DeserializeObject<Protocol.res_doSaveStage>(data);

        if (res_doSaveState == null)
        {
            Protocol.ResponseError(Protocol.eErrorType.Null);

            if (App.Instance != null)
            {
                App.Instance.LoadScene("4. Mode Select");
            }

            return;
        }

        if (res_doSaveState.cmd != 500)
        {
            Protocol.ResponseError(Protocol.eErrorType.doSaveStage);

            if (App.Instance != null)
            {
                App.Instance.LoadScene("4. Mode Select");
            }

            return;
        }

        if(App.Instance !=null)
        {
            App.Instance.LoadScene(modeSelectSceneName);
        }
    }

    private void ClearStage()
    {
        var req_clearStage = new Protocol.req_clearStage();
        req_clearStage.cmd = 420;
        req_clearStage.stage_id = PlayData.id;
        req_clearStage.user_id = Social.localUser.id;
        req_clearStage.stage_elapsed_score = PlayData.score;

        // 시리얼 라이즈
        var json = JsonConvert.SerializeObject(req_clearStage);

        //StartCoroutine(this.Get("api/getAllStages", json, data => GetAllStageResponse(data), "authorization", token));
        if (App.Instance != null)
        {
            App.Instance.reqCoroutine =
            StartCoroutine(Protocol.Post("api/clearStage", json, data => ClearStageResponse(data)));
        }
    }

    void ClearStageResponse(string data)
    {
        // 디시리얼라이즈 한다.
        var res_clearStage = JsonConvert.DeserializeObject<Protocol.res_clearStage>(data);

        if (res_clearStage == null)
        {
            Protocol.ResponseError(Protocol.eErrorType.Null);
            return;
        }

        if (res_clearStage.cmd != 420)
        {
            Protocol.ResponseError(Protocol.eErrorType.clearStage);
            return;
        }

        // 점수
        txtScore.text = "0";
        txtScore.gameObject.SetActive(true);
        StartCoroutine(FadeScore(PlayData.score));
    }

    private void FailedStage()
    {
        var req_failedStage = new Protocol.req_failedStage();
        req_failedStage.cmd = 430;
        req_failedStage.stage_id = PlayData.id;

        // 시리얼 라이즈
        var json = JsonConvert.SerializeObject(req_failedStage);

        //StartCoroutine(this.Get("api/getAllStages", json, data => GetAllStageResponse(data), "authorization", token));
        if (App.Instance != null)
        {
            App.Instance.reqCoroutine =
            StartCoroutine(Protocol.Post("api/failedStage", json, data => FailedStageResponse(data)));
        }
    }

    void FailedStageResponse(string data)
    {
        // 디시리얼라이즈 한다.
        var res_failedStage = JsonConvert.DeserializeObject<Protocol.res_failedStage>(data);

        if (res_failedStage == null)
        {
            Protocol.ResponseError(Protocol.eErrorType.Null);
            return;
        }

        if (res_failedStage.cmd != 430)
        {
            Protocol.ResponseError(Protocol.eErrorType.failedStage);
            return;
        }
    }

    IEnumerator FadeScore(int score)
    {
        int currentScore = 0;
        var anim = txtCost.GetComponent<Animator>();

        while (currentScore <= score)
        {
            txtScore.text = string.Format("+{0}", currentScore);
            currentScore++;

            yield return new WaitForSeconds(0.015f);
        }

        anim.Play("anim_UIScore");
    }
}
