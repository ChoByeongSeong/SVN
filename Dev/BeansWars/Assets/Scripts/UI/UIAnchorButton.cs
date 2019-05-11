using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class UIAnchorButton : MonoBehaviour
{
    [Serializable]
    public class AnchorObj
    {
        public RectTransform moveValue;
        public float waitTime;
        public float dulation;
        public RectTransform target;
    }

    public AnchorObj[] arrAnchorObjs;

    private void Awake()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            for (int i = 0; i < arrAnchorObjs.Length; i++)
            {
                StartCoroutine(AnchorPos(arrAnchorObjs[i]));
            }
        });
    }

    IEnumerator AnchorPos(AnchorObj obj)
    {
        yield return new WaitForSeconds(obj.waitTime);

        Vector2 moveValue = obj.moveValue.anchoredPosition;
        obj.target.DOAnchorPos(-moveValue, obj.dulation);
    }
}