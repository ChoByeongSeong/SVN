using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour {

    public void InitUI(Transform parTf,string path)
    {
        var uiGo = Instantiate(Resources.Load(path) as GameObject);
        uiGo.transform.SetParent(parTf);
        uiGo.transform.localPosition = Vector3.zero;
        uiGo.transform.localScale = Vector3.one;
    }

    public void ShowUI(GameObject uiObject)
    {
        uiObject.SetActive(true);
    }

    public void HideUI(GameObject uiObject)
    {
        uiObject.SetActive(false);
    }

    public void DestroyUI()
    {
        Destroy(this.gameObject);
    }
}
