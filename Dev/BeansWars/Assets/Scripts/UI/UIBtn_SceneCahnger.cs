using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIBtn_SceneCahnger : MonoBehaviour {

    public string nextSceneName;

    private void Awake()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            if (App.Instance != null)
            {
                App.Instance.LoadScene(nextSceneName);
            }
        });
    }
}
