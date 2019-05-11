using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    /*
     * 필요한 데이터를 로드한다.
     * 필요한 프리펩을 로드한다.
     */

    private void Awake()
    {
        // 데이터 매니저를 생성해 놓는다.
        DataManager.GetInstance();
        PrefabsManager.GetInstance();

        // 사운드 로딩.
        if(YellowBean.SoundManager.Instance !=null)
        {
            YellowBean.SoundManager.Instance.Init();
        }
    }
}
