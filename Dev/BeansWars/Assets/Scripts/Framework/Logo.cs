using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Logo : MonoBehaviour
{
    public Image imgLogo;

    public string titleSceneName;
    public Image dimSprite;
    public float timeToShow;

    public float waitTime;  // 처음 화면이 어두운 시간.
    public float duration;  // 페이드 인, 아웃 유지 시간.

    void Start ()
    {
        imgLogo.GetComponent<RectTransform>().DOShakeScale(1.5f, 0.5f);
        /*
         * 2초를 기다린다.
         * 페이드 아웃을 통해, 로고를 보여준다.
         * 페이드 인을 통해, 로고를 사라지게 한다.
         */
        App app = App.Instance;
        StartCoroutine(app.WaitForSeconds(waitTime, () =>
        {
            app.FadeOut(this.dimSprite, duration, () =>
            {
                app.FadeIn(this.dimSprite, duration, () =>
                {
                    app.LoadScene(titleSceneName);
                }); // fade in end
            }); // fade out end
        }));
	}
}
