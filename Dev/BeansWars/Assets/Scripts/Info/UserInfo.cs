using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class UserInfo : ScriptableObject
{
    public int score;
    public StoryStageInfo[] arrStageInfo = new StoryStageInfo[20];
}