using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton_ChangeImg : MonoBehaviour
{
    public Button btnBgm;
    public Image imgBgmBackGround;
    public Image imgBgmCheckMark;

    public Button btnSfx;
    public Image imgSfxBackGround;
    public Image imgSfxCheckMark;

    void Awake()
    {
        CheckBgm();
        CheckSfx();

        btnBgm.onClick.AddListener(() =>
        {
            Debug.Log("btnBgm");

            var mute = YellowBean.SoundManager.Instance.bgmMute;

            YellowBean.SoundManager.Instance.BgmMute(!mute);

            CheckBgm();
        });

        btnSfx.onClick.AddListener(() =>
        {
            Debug.Log("btnSfx");

            var mute = YellowBean.SoundManager.Instance.sfxMute;


            YellowBean.SoundManager.Instance.SfxMute(!mute);

            CheckSfx();
        });
    }

    public void CheckBgm()
    {
        imgBgmCheckMark.gameObject.SetActive(
            YellowBean.SoundManager.Instance.bgmMute
            );
    }

    public void CheckSfx()
    {
        imgSfxCheckMark.gameObject.SetActive(
            YellowBean.SoundManager.Instance.sfxMute
            );
    }
}
