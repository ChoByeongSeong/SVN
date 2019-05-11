using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

    [SerializeField] private Canvas canvas;
    [SerializeField] private Transform targetTf;

    private RectTransform canvasRectTf;
    private RectTransform myRectTf;
    private Vector3 offset = new Vector3(0, 1.5f, 0);

    private void Start()
    {
        this.canvasRectTf = this.canvas.GetComponent<RectTransform>();
        this.myRectTf = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector2 pos;

        switch (this.canvas.renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
                {
                    this.myRectTf.position = RectTransformUtility.WorldToScreenPoint(Camera.main, this.targetTf.position + this.offset);

                }
                break;
            case RenderMode.ScreenSpaceCamera:
                {
                    Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, this.targetTf.position + this.offset);
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(this.canvasRectTf, screenPos, Camera.main, out pos);
                    this.myRectTf.localPosition = pos;
                }
                break;
            case RenderMode.WorldSpace:
                {
                    this.myRectTf.LookAt(Camera.main.transform);
                }
                break;
        }
    }
}
