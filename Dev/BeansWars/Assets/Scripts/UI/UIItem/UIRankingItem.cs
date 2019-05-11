using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRankingItem : MonoBehaviour
{
    Text txtRank;
    public string TxTRank
    {
        get
        {
            return txtRank.text;
        }

        set
        {
            txtRank.text = value;

            int rank = int.Parse(value);
            if (rank == 1)
            {
                imgG.gameObject.SetActive(true);
                imgS.gameObject.SetActive(false);
                imgC.gameObject.SetActive(false);
                imgW.gameObject.SetActive(false);
            }

            else if (rank == 2)
            {
                imgG.gameObject.SetActive(false);
                imgS.gameObject.SetActive(true);
                imgC.gameObject.SetActive(false);
                imgW.gameObject.SetActive(false);
            }

            else if (rank == 3)
            {
                imgG.gameObject.SetActive(false);
                imgS.gameObject.SetActive(false);
                imgC.gameObject.SetActive(true);
                imgW.gameObject.SetActive(false);
            }

            else
            {
                imgG.gameObject.SetActive(false);
                imgS.gameObject.SetActive(false);
                imgC.gameObject.SetActive(false);
                imgW.gameObject.SetActive(true);
            }
        }
    }

    public Text txtUserName;
    public Text txtScore;
    public Image countryFlag;
    
    public Image imgG;
    public Image imgS;
    public Image imgC;
    public Image imgW;

    private void Awake()
    {
        txtRank = transform.Find("rank").GetComponent<Text>();
    }
}
