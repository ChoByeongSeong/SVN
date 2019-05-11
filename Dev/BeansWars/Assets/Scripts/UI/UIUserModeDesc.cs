using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIUserModeDesc : MonoBehaviour
{
    public GameObject goLike;
    public Text txtLike;

    public GameObject goClear;
    public Text txtClear;

    public GameObject goScore;
    public Text txtScore;

    public GameObject goDate;
    public Text txtDate;

    public Text txtName;

    public void Init(int sortType, string like, string clear, string score, string data, string name)
    {
        if (name != null)
        {           
            if (name.Length > 12)
            {               
                name = name.Substring(0, 12);
                name += "...";
            }
        }
      
        txtName.text = string.Format("by. {0}", name);

        goLike.SetActive(false);
        goClear.SetActive(false);
        goScore.SetActive(false);
        goDate.SetActive(false);

        switch (sortType)
        {
            case -1:
            case 0:
                {
                    goLike.SetActive(true);
                    txtLike.text = like;
                }
                break;

            case 1:
                {
                    goClear.SetActive(true);
                    txtClear.text = clear;
                }
                break;

            case 2:
                {
                    goScore.SetActive(true);
                    txtScore.text = score;
                }
                break;

            case 3:
                {
                    goDate.SetActive(true);
                    txtDate.text = data;
                }
                break;

            default:
                {
                    txtName.text = "";
                }
                break;
        }
    }

}
