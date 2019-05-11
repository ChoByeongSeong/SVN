using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MapData
{
    public int map_id;
    public string map_name;

    public float grid_x;
    public float grid_y;
    public float node_radius;

    public string groundName;
    public string yellowBaseName;
    public string greenBaseName;

    public List<EnvironmentData> listEnvirnment = new List<EnvironmentData>();
}