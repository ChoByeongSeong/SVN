using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UISettingButton : MonoBehaviour
{
    public GameObject goSetting;
    public Button btnSetting;

    private void Awake()
    {
        btnSetting.onClick.AddListener(()=> {
            goSetting.SetActive(true);
        });
    }
}