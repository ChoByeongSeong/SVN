using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class TouchCamera : MonoBehaviour
{
    Vector2?[] oldTouchPositions = {
        null,
        null
    };

    Vector2 oldTouchVector;
    float oldTouchDistance;

    Vector2 max = new Vector2(20f, 12.5f);
    Vector2 min = new Vector2(-20f, -16.0f);
    public Camera mainCamera;

    float scaleSpeedDefault = 1.2f;
    float scaleSpeed = 0f;
    bool scaling = false;
    Tweener cameraTweener;

    void Update()
    {
        // 터치가 없을 때
        if (Input.touchCount == 0)
        {
            oldTouchPositions[0] = null;
            oldTouchPositions[1] = null;
        }

        // 터치가 둘 일 때
        else if (Input.touchCount == 2)
        {
            if (!IsPointerOverUIObject())
            {
                if (oldTouchPositions[0] == null || oldTouchPositions[1] == null)
                {
                    oldTouchPositions[0] = Input.GetTouch(0).position;
                    oldTouchPositions[1] = Input.GetTouch(1).position;

                    Vector2 oldTouchVector = Input.GetTouch(0).position - Input.GetTouch(1).position;
                    oldTouchDistance = oldTouchVector.magnitude;
                }

                else
                {
                    Vector2 screen = new Vector2(mainCamera.pixelWidth, mainCamera.pixelHeight);
                    Vector2[] newTouchPositions = {
                        Input.GetTouch(0).position,
                        Input.GetTouch(1).position
                        };

                    // 스크롤
                    Vector2 newTouchVector = newTouchPositions[0] - newTouchPositions[1];
                    float newTouchDistance = newTouchVector.magnitude;
                    transform.position += transform.TransformDirection((Vector3)((oldTouchPositions[0] + oldTouchPositions[1] - screen) * mainCamera.orthographicSize / screen.y));

                    // 확대, 축소
                    float dst = newTouchDistance - oldTouchDistance;
                    mainCamera.orthographicSize -= dst * scaleSpeed * Time.deltaTime;

                    transform.position -= transform.TransformDirection((newTouchPositions[0] + newTouchPositions[1] - screen) * mainCamera.orthographicSize / screen.y);

                    oldTouchPositions[0] = newTouchPositions[0];
                    oldTouchPositions[1] = newTouchPositions[1];
                    oldTouchVector = newTouchVector;
                    oldTouchDistance = newTouchDistance;
                }
            }
        }

        mainCamera.orthographicSize = (mainCamera.orthographicSize > 15f) ? 15f : mainCamera.orthographicSize;
        mainCamera.orthographicSize = (mainCamera.orthographicSize < 5.2f) ? 5.2f : mainCamera.orthographicSize;

        Vector2 cameraMax = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
        Vector2 cameraMin = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));

        Vector2 maxDst = cameraMax - max;
        Vector2 minDst = min - cameraMin;

        float ratio = (cameraMax.x - cameraMin.x) / (max.x - min.x);

        // 더 클때
        if (ratio > 1)
        {
            scaleSpeed = scaleSpeedDefault / (ratio * ratio * ratio);

            Vector3 pos = mainCamera.transform.position;
            pos.y = Mathf.Clamp(pos.y, min.y, max.y);

            mainCamera.transform.position = new Vector3(0, pos.y, pos.z);

            if (Input.touchCount == 0)
            {
                if (!scaling)
                {
                    scaling = true;

                    cameraTweener = mainCamera.DOOrthoSize(13f, 0.15f);
                    cameraTweener.onComplete = () =>
                    {
                        scaling = false;
                    };
                }
            }

            else
            {
                scaling = false;

                if (cameraTweener != null)
                {
                    cameraTweener.Kill();
                    cameraTweener = null;
                }
            }
        }

        // 더 작을 때
        else
        {
            scaleSpeed = scaleSpeedDefault;

            if (maxDst.x > 0)
            {
                mainCamera.transform.Translate(new Vector2(-maxDst.x, 0));
            }

            if (maxDst.y > 0)
            {
                mainCamera.transform.Translate(new Vector2(0, -maxDst.y));
            }

            if (minDst.x > 0)
            {
                mainCamera.transform.Translate(new Vector2(minDst.x, 0));
            }

            if (minDst.y > 0)
            {
                mainCamera.transform.Translate(new Vector2(0, minDst.y));
            }
        }
    }

    //IEnumerator ScaleCamera()
    //{
    //    float scaleSpeed = 100;
    //    scaling = true;

    //    while (mainCamera.orthographicSize > 11f)
    //    {
    //        mainCamera.orthographicSize -= scaleSpeed * Time.deltaTime;

    //        yield return null;
    //    }

    //    scaling = false;
    //}

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
