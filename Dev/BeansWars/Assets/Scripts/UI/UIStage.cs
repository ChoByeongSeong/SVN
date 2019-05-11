using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UIStage : UIBase
{
    public Button btn_back;
    public Button btn_setting;
    public Button btn_ready;
    public Text txt_stageNo;
    public List<Button> btn_stages;
    public Action onCompleteBack;
    public Action onCompleteReady;
    public GameObject uiPopup_setting;
    public GameObject uiPopup_message;
     
    private void Start()
    {
        for (int i = 0; i < this.btn_stages.Count; i++)
        {
            var spr = this.btn_stages[i].GetComponent<Image>().sprite;
            
            this.btn_stages[i].onClick.AddListener(() =>
            {
                if(spr !=null)
                {
                    this.uiPopup_message.SetActive(true);
                }
                else
                {
                    this.txt_stageNo.text = "스테이지 넘버";
                }              
            });         
        }

        this.btn_back.onClick.AddListener(() =>
        {
            this.onCompleteBack();
        });

        this.btn_ready.onClick.AddListener(() =>
        {
            this.onCompleteReady();
        });

        this.btn_setting.onClick.AddListener(() =>
        {
            this.uiPopup_setting.SetActive(true);
        });
    }   
}
