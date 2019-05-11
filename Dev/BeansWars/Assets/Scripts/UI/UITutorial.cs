using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UITutorial : MonoBehaviour
{
    int animNum = 0;
    Animation anim;

    public Button btnNext;
    public Button btnBack;
    public Button btnSkip;

    public Action onComplete;

    public void CheckBtn()
    {
        if (animNum == 1 || animNum == 0)
        {
            btnBack.gameObject.SetActive(false);
        }

        if(animNum >= 2 && animNum <= 4)
        {
            btnBack.gameObject.SetActive(true);
            btnNext.GetComponentInChildren<Text>().text = "Next>";
        }

        if (animNum == 5)
        {
            btnNext.GetComponentInChildren<Text>().text = "End>";
        }
    }

    private void Awake()
    {
        anim = GetComponent<Animation>();

        if (btnNext != null)
        {
            btnNext.onClick.AddListener(() =>
            {
                animNum++;
                if(animNum >= 6)
                {
                    if (onComplete != null) onComplete();
                    return;
                }

                animNum = Mathf.Clamp(animNum, 1, 5);
                CheckBtn();

                anim.Play(Stap(animNum));
            });

            btnSkip.onClick.AddListener(() =>
            {
                if (onComplete != null) onComplete();
            });
        }

        if (btnBack != null)
        {
            btnBack.onClick.AddListener(() =>
            {
                animNum--;

                animNum = Mathf.Clamp(animNum, 1, 5);
                CheckBtn();

                anim.Play(Stap(animNum));
            });

            btnSkip.onClick.AddListener(() =>
            {
                if (onComplete != null) onComplete();
            });
        }
    }

    string Stap(int num)
    {
        num = Mathf.Clamp(num, 1, 5);

        return string.Format("stap0{0}", num);
    }
}