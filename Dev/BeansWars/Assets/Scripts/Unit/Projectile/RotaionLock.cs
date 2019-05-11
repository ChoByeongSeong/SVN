using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 골렘 공격 이펙트 회전을 고정한다.
 */
public class RotaionLock : MonoBehaviour
{
    public GameObject go;

    private void Update()
    {
        go.transform.eulerAngles = Vector3.zero;
    }
}
