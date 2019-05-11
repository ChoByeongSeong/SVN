using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class PlayData
{
    public enum ePlayMode
    {
        None = -1,
        Editor,
        Story,
        User
    }
    
    public static int id = -1;
    public static int cost;
    public static int map_id;

    static ePlayMode mode = ePlayMode.None;
    public static ePlayMode Mode
    {
        get
        {
            return mode;
        }

        set
        {
            mode = value;
        }
    }

    public static int score;

    public static string base_name;

    public static List<unit_info> listUnitInfos = new List<unit_info>();

    public static void Init()
    {
        id = -1;
        cost = -1;
        map_id = -1;
        Mode = ePlayMode.None;
        listUnitInfos = null;
    }
}
