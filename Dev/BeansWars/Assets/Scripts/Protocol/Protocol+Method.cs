using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;

public partial class Protocol
{
    const string serverPath = "http://test00.cafe24app.com";

    #region Post
    public static IEnumerator Post(string uri, string data, System.Action<string> onResponse)
    {
        if (App.Instance != null)
        {
            App.Instance.ShowLoading();
        }

        // URL을 설정한다.
        var url = string.Format("{0}/{1}", serverPath, uri);

        //POST방식으로 http서버에 요청을 보내겠습니다.
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.timeout = 15;

        // 아스키 코드로 변환한다.
        byte[] bodyRaw = Encoding.UTF8.GetBytes(data);

        Debug.LogFormat("[Req]");
        Debug.LogFormat("[URL] : <color=yellow>{0}</color>", url);
        Debug.LogFormat("[Body({0})] : {1}", bodyRaw.Length, data);

        // request 구조채를 체운다.
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);          // 원격 서버에 업로드 할 본문 데이터를 관리하는 UploadHandler 객체에 대한 참조를 포함합니다.
        request.downloadHandler = new DownloadHandlerBuffer();          // downloadHandler : 이 UnityWebRequest에 의해 원격 서버에서받은 본문 데이터를 관리하는 DownloadHandler 객체에 대한 참조를 포함합니다.
                                                                        // DownloadHandlerBuffer : 수신 한 데이터를 네이티브 byte 버퍼에 저장하는 범용 DownloadHandler 구현입니다.

        Debug.LogFormat("********* Protocol.token: {0}", Protocol.token);

        if (Protocol.token != null)
        {
            request.SetRequestHeader("authorization", string.Format("Bearer {0}", token));
        }

        request.SetRequestHeader("Content-Type", "application/json");

        // 응답을 보낸다.
        yield return request.SendWebRequest();


        Debug.LogFormat("isNetworkError: {0}", request.isNetworkError);
        Debug.LogFormat("isHttpError: {0}", request.isHttpError);
        Debug.LogFormat("responseCode: {0}", request.responseCode);


        //************************************************************* request.error: Unknown Error ???

        if (request.isNetworkError || request.isHttpError)
        {
            if (App.Instance != null)
            {
                App.Instance.goLoading.SetActive(false);
                App.Instance.ShowMessage(request.error);
            }

            yield break;
        }

        if (App.Instance != null)
        {
            App.Instance.goLoading.SetActive(false);
        }

        // 응답을 받으면 출력한다.
        Debug.Log("[Res]");
        Debug.LogFormat("[Body({0})] : {1}", request.downloadHandler.text.Length, request.downloadHandler.text);

        // 응답을 받았습니다.
        onResponse(request.downloadHandler.text);
    }
    #endregion


    #region Get
    public static IEnumerator Get(string uri, string data, System.Action<string> onResponse, string headerName = null, string headerVal = null)
    {
        if (App.Instance != null)
        {
            App.Instance.ShowLoading();
        }

        // URL을 설정한다.
        var url = string.Format("{0}/{1}", serverPath, uri);

        Debug.LogFormat("[Req]");
        Debug.LogFormat("[URL] : <color=yellow>{0}</color>/<color=yellow>{1}</color>", serverPath, uri);

        // UnityWebRequest 를 받아온다.
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.timeout = 15;

        if (!string.IsNullOrEmpty(headerName) && !string.IsNullOrEmpty(headerVal))
        {
            www.SetRequestHeader(headerName, string.Format("Bearer {0}", headerVal));
        }

        // 응답을 보낸다.
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            if (App.Instance != null)
            {
                App.Instance.goLoading.SetActive(false);
                App.Instance.ShowMessage(www.error);
            }

            yield break;
        }

        if (App.Instance != null)
        {
            App.Instance.goLoading.SetActive(false);
        }

        // 응답을 받으면 출력한다.
        Debug.Log("[Res]");
        Debug.LogFormat("[Body({0})] : {1}", www.downloadHandler.text.Length, www.downloadHandler.text);

        onResponse(www.downloadHandler.text);
    }
    #endregion

    static public void ResponseError(eErrorType type)
    {
        string message = null;

        switch (type)
        {
            case eErrorType.None:
                message = "Error";
                break;

            case eErrorType.Login:
                message = "Error";
                break;

            case eErrorType.Null:
                message = "Error";
                break;

            case eErrorType.getAllStages:
                message = "Error";
                break;

            case eErrorType.doSaveStage:
                message = "Error";
                break;

            case eErrorType.doPlay:
                message = "Error";
                break;

            case eErrorType.getRank:
                message = "Error";
                break;

            case eErrorType.doFavoriteStage:
                message = "Error";
                break;

            case eErrorType.clearStage:
                message = "Error";
                break;

            case eErrorType.failedStage:
                message = "Error";
                break;

            default:
                message = "Error";
                break;
        }

        if (App.Instance != null)
        {
            App.Instance.ShowMessage(message);
        }
    }

    public enum eErrorType
    {
        None = -1,
        Null,
        Login,
        getAllStages,
        doSaveStage,
        doPlay,
        getRank,
        getMyRank,
        doFavoriteStage,
        clearStage,
        failedStage
    }
}