using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToggle_ChangeImg : MonoBehaviour
{
    public Toggle toggle;
    public Image imgToggle;
    public Image imgCheckMark;

    void Awake()
    {
        if (toggle.name.CompareTo("music") == 0)
        {
            if (YellowBean.SoundManager.Instance.bgmMute)
            {
                toggle.isOn = false;

                imgToggle.gameObject.SetActive(true);
                imgCheckMark.gameObject.SetActive(true);
            }

            else
            {
                toggle.isOn = true;

                imgToggle.gameObject.SetActive(true);
                imgCheckMark.gameObject.SetActive(false);
            }
        }

        if (toggle.name.CompareTo("fx") == 0)
        {
            if (YellowBean.SoundManager.Instance.sfxMute)
            {
                toggle.isOn = false;

                imgToggle.gameObject.SetActive(true);
                imgCheckMark.gameObject.SetActive(true);
            }

            else
            {
                toggle.isOn = true;

                imgToggle.gameObject.SetActive(true);
                imgCheckMark.gameObject.SetActive(false);
            }
        }

        toggle.onValueChanged.AddListener((on) =>
        {
            if(toggle.name.CompareTo("music")==0)
            {
                // 토글이 켜지면
                if (on)
                {
                    imgToggle.enabled = true;
                    imgCheckMark.enabled = false;
                    YellowBean.SoundManager.Instance.BgmMute(false);
                }

                // 토글이 꺼지면
                else
                {                   
                    imgCheckMark.enabled = true;
                    YellowBean.SoundManager.Instance.BgmMute(true);
                }
            }else if(toggle.name.CompareTo("fx") == 0)
            {
                // 토글이 켜지면
                if (on)
                {
                    imgToggle.enabled = true;
                    imgCheckMark.enabled = false;
                    YellowBean.SoundManager.Instance.SfxMute(false);
                }

                // 토글이 꺼지면
                else
                {                   
                    imgCheckMark.enabled = true;
                    YellowBean.SoundManager.Instance.SfxMute(true);
                }
            }           
        });
    }
}
