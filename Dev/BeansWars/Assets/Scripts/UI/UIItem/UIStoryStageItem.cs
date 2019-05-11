using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStoryStageItem : MonoBehaviour
{
    public Image imgDefault;
    public Image imgSelected;

    bool isSelect;
    public bool IsSelect
    {
        get
        {
            return isSelect;
        }

        set
        {
            if(value)
            {
                isSelect = value;
                imgSelected.gameObject.SetActive(true);
                imgDefault.gameObject.SetActive(false);
            }

            else
            {
                isSelect = value;
                imgSelected.gameObject.SetActive(false);
                imgDefault.gameObject.SetActive(true);
            }
        }
    }
    Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();    
    }

    int starCnt;

    public int StarCnt
    {
        get { return starCnt; }
        set
        {
            starCnt = value;
            SetStarImg();
        }
    }

    public Text txtTitle;

    public Image imgBoard;
    public Image imgLock;
    public Image imgStarLeft;
    public Image imgStarRight;
    public Image imgStarCenter;

    void SetStarImg()
    {
        switch (starCnt)
        {
            case -1:
                {
                    btn.enabled = false;

                    imgBoard.enabled = false;
                    imgLock.enabled = true;
                    imgStarLeft.enabled = false;
                    imgStarRight.enabled = false;
                    imgStarCenter.enabled = false;
                } break;

            case 0:
                {
                    btn.enabled = true;

                    imgBoard.enabled = true;
                    imgLock.enabled = false;
                    imgStarLeft.enabled = false;
                    imgStarRight.enabled = false;
                    imgStarCenter.enabled = false;
                }
                break;

            case 1:
                {
                    btn.enabled = true;

                    imgBoard.enabled = true;
                    imgLock.enabled = false;
                    imgStarLeft.enabled = true;
                    imgStarRight.enabled = false;
                    imgStarCenter.enabled = false;
                }
                break;

            case 2:
                {
                    btn.enabled = true;

                    imgBoard.enabled = true;
                    imgLock.enabled = false;
                    imgStarLeft.enabled = true;
                    imgStarRight.enabled = false;
                    imgStarCenter.enabled = true;
                }
                break;

            case 3:
                {
                    btn.enabled = true;

                    imgBoard.enabled = true;
                    imgLock.enabled = false;
                    imgStarLeft.enabled = true;
                    imgStarRight.enabled = true;
                    imgStarCenter.enabled = true;
                }
                break;

        }
    }
}
