using UnityEngine;
using UnityEditor;
using System.Collections;

public class FPSDisplayer : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private GUIStyle style;
    private int width;
    private int height;
    private long totalReservedMemory;
    private long totalAllocMemory;
    private long totalUnusedReservedMemory;
 
    void Awake()
    {
        this.width = Screen.width;
        this.height = Screen.height;
        this.style = new GUIStyle();
        Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        this.style.font = font;
        //UnityEngine.Profiling.Profiler.logFile = "mylog.log";
        //UnityEngine.Profiling.Profiler.enabled = true;
    }
    void Update()
    {
        this.deltaTime += (Time.deltaTime - this.deltaTime) * 0.1f;
 
        this.totalReservedMemory = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong();
        this.totalAllocMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();
        this.totalUnusedReservedMemory = UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong();
    }
 
    private void ShowFPS()
    {
        Rect rect = new Rect(0, 0, this.width, this.height * 2 / 100);
        this.style.alignment = TextAnchor.UpperLeft;
        this.style.fontSize = this.height * 2 / 100;
        this.style.normal.textColor = Color.red;
        float msec = this.deltaTime * 1000.0f;
        float fps = 1.0f / this.deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, this.style);
    }
 
    private void ShowReservedMemory()
    {
        Rect rect = new Rect(0, 20, this.width, this.height * 2 / 100);
        this.style.alignment = TextAnchor.UpperLeft;
        this.style.fontSize = this.height * 2 / 100;
        this.style.normal.textColor = Color.red;
        string text = string.Format("사용가능한 메모리: {0}MB", this.ConvertBytesToMegabytes(this.totalReservedMemory).ToString("N"));
        GUI.Label(rect, text, this.style);
    }
 
    private void ShowAllocMemory()
    {
        Rect rect = new Rect(0, 40, this.width, this.height * 2 / 100);
        this.style.alignment = TextAnchor.UpperLeft;
        this.style.fontSize = this.height * 2 / 100;
        this.style.normal.textColor = Color.red;
        string text = string.Format("사용하고있음: {0}MB", this.ConvertBytesToMegabytes(this.totalAllocMemory).ToString("N"));
        GUI.Label(rect, text, this.style);
    }
 
    private void ShowUnusedReservedMemory()
    {
        Rect rect = new Rect(0, 60, this.width, this.height * 2 / 100);
        this.style.alignment = TextAnchor.UpperLeft;
        this.style.fontSize = this.height * 2 / 100;
        this.style.normal.textColor = Color.red;
        string text = string.Format("남은량: {0}MB", this.ConvertBytesToMegabytes(this.totalUnusedReservedMemory).ToString("N"));
        GUI.Label(rect, text, this.style);
    }
 
    void OnGUI()
    {
        
        this.ShowFPS();
 
        this.ShowReservedMemory();
 
        this.ShowAllocMemory();
 
        this.ShowUnusedReservedMemory();
 
    }
 
    private double ConvertBytesToMegabytes(long bytes)
    {
        return (bytes / 1024f) / 1024f;
    }
}

