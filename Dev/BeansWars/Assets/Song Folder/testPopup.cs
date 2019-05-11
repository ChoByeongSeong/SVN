using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testPopup : MonoBehaviour {
    public GameObject popup;

    private Button btn;
    private Button btnX;
	// Use this for initialization
	void Start () {
        btn = this.gameObject.GetComponent<Button>();
        popup.SetActive(false);
        btn.onClick.AddListener(() => {
            popup.SetActive(true);
        });
        btnX = popup.transform.Find("popupWindow").transform.Find("btnX").gameObject.GetComponent<Button>();
        btnX.onClick.AddListener(() =>
        {
            popup.SetActive(false);
        });
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
