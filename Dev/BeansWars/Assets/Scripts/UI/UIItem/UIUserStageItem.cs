using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUserStageItem : MonoBehaviour
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
            if (value)
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

    public Text txtId;
    public Text txtTitle;
    public Image imgLike;
    public Image imgClearCnt;
    public Image imgScore;
    public Image imgDate;
    public int mapId;

    public void SetUserStage(int id, string title)
    {
        string strId = id.ToString();

        if (strId.Length > 2)
        {
            int num = 0;
            string newStrId = null;

            foreach (var c in strId)
            {
                if (num == 2)
                    break;

                newStrId += c;

                num++;
            }

            newStrId += "...";
            strId = newStrId;
        }

        txtId.text = strId;


        if (title.Length > 7)
        {
            int num = 0;
            string newTitle = null;

            foreach (var c in title)
            {
                if (num == 7)
                    break;

                newTitle += c;

                num++;
            }

            newTitle += "...";
            title = newTitle;
        }

        txtTitle.text = title;
    }
}
