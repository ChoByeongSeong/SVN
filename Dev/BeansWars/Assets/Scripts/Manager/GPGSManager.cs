using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames.BasicApi;
using GooglePlayGames;

public class GPGSManager : MonoBehaviour
{
    public System.Action OnLoadedImage;

    public void Init()
    {
        //GPGS 초기화
        PlayGamesClientConfiguration conf = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(conf);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        Debug.LogFormat("GPGSManager::Init");

        //FIREBASE 초기화
    }

    public void SignIn(System.Action<bool> onComplete)
    {
        Social.localUser.Authenticate((result) =>
        {
            onComplete(result);
        });
    }

    public void SignOut()
    {
        if(PlayGamesPlatform.Instance.IsAuthenticated()==true)
        {
            PlayGamesPlatform.Instance.SignOut();
        }
    }
}
