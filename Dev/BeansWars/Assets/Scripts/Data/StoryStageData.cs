using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class StoryStageData
{
    public int id;
    public string desc;
    public int cost;
    public int map_id;

    public List<unit_info> listUnitInfos = new List<unit_info>();
}