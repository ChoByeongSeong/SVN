using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.EventSystems;

public class UIInGame : UIBase
{
    public Button btn_openStatus;
    public Button btn_closeTab;
    public Button btn_clear;
    public Button btn_back;
    public Button btn_delete;
    public Button btn_gameStart;
    public Button btn_nextStage;
    public Button btn_beforeStage;
    public GameObject tabPane;
    public GameObject statusPane;
    public GameObject stageData;
    public GameObject uiPopup_pause;
    public GameObject uiPopup_result;
    public Action onCompleteBack;
    // public StageInfo stageInfo;
    private Animator anim_status;
    private Animator anim_tab;

    

    private void Start()
    {
        if(this.statusPane!=null)
        {
            this.anim_status = this.statusPane.GetComponent<Animator>();
            this.btn_openStatus.onClick.AddListener(() =>
            {
                if (this.anim_status != null)
                {
                    bool open = this.anim_status.GetBool("isOpen");
                    this.anim_status.SetBool("isOpen", !open);
                }
            });
        }

        if(this.tabPane!=null)
        {
            this.anim_tab = this.tabPane.GetComponent<Animator>();
            this.btn_closeTab.onClick.AddListener(() =>
            {
                if(this.anim_tab!=null)
                {
                    bool close = this.anim_tab.GetBool("isClose");
                    this.anim_tab.SetBool("isClose", !close);
                }
            });
        }
        
        this.btn_gameStart.onClick.AddListener(() =>
        {
            this.StartGame();    
        });  
    }

    private void StartGame()
    {
        this.tabPane.SetActive(false);
        this.statusPane.SetActive(false);
        this.stageData.SetActive(false);
        this.btn_gameStart.gameObject.SetActive(false);

    }
}
