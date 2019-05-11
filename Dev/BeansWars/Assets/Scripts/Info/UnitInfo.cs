using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class unit_info
{
    public string unit_name;    // 유닛이름
    public float x;             // x좌표
    public float y;             // y좌표
    public float z;             // z좌표
    public int color;           // 0 : Y   1 : G

    public unit_info()
    {
    }

    public unit_info(unit_info other)
    {
        this.unit_name = other.unit_name;
        this.x = other.x;
        this.y = other.y;
        this.z = other.z;
        this.color = other.color;
    }
}