using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class UIPopup_Option : MonoBehaviour
{
    public bool stopTime = true;
    public Button btnBack;
    public string backSceneName;
    
    public Button btnRestart;
    public string reStartSceneName;

    public Button btnClose;

    public Button btnNewGame;
    public GameObject goNewGame;
    public Button btnNewGameOK;
    public Button btnNewGameNo;

    public Button btnCredit;
    public GameObject goCredit;
    public Button btnCrediExit;

    GPGSManager gpgsManager;

    public void Awake()
    {
        gpgsManager = FindObjectOfType<GPGSManager>();

        if (btnClose != null)
            btnClose.onClick.AddListener(() =>
            {
                if(this.goCredit!=null)
                this.goCredit.SetActive(false);

                if (this.goNewGame != null)
                    this.goNewGame.SetActive(false);

                this.gameObject.SetActive(false);

                Time.timeScale = 1f;
            });

        if (btnRestart != null)
            btnRestart.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;

            if (App.Instance != null)
            {
                App.Instance.LoadScene(reStartSceneName);
            }
        });

        if (btnBack != null)
            btnBack.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;

            if (App.Instance != null)
            {
                App.Instance.LoadScene(backSceneName);
            }
        });
        

        if (btnNewGame)
        {
            btnNewGame.onClick.AddListener(() =>
            {
                goNewGame.SetActive(true);
            });

            btnNewGameOK.onClick.AddListener(() =>
            {
                DataManager.GetInstance().CreateUserData();
                Protocol.token = null;
                this.gpgsManager.SignOut();
                PlayData.Init();
                App.Instance.LoadScene("4. Mode Select");
            });

            btnNewGameNo.onClick.AddListener(() =>
            {
                goNewGame.SetActive(false);
            });
        }

        if(btnCredit)
        {
            btnCredit.onClick.AddListener(() => {
                goCredit.SetActive(true);
            });

            btnCrediExit.onClick.AddListener(() => {
                goCredit.SetActive(false);
            });

        }


    }

    void OnComplete()
    {
        if (stopTime)
            Time.timeScale = 0f;
    }
}
