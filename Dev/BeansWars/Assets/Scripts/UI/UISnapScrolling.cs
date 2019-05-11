using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UISnapScrolling : MonoBehaviour
{
    [Header("Controllers")]
    [Range(0, 500)]
    public int panOffset;
    [Range(0f, 20f)]
    public float snapSpeed;
    [Range(0f, 5f)]
    public float scaleOffset;
    [Range(1f, 20f)]
    public float scaleSpeed;
    [Header("Other Obejcts")]
    public GameObject panPrefab;
    public ScrollRect scrollRect;
    public Image backGround;
    public Text txtTitle;
    public Text txtCost;
    public Button btnBack;
    public Button btnNext;
    public Button btnRight;
    public Button btnLeft;
    public int[] userMapNum = new int[9];

    private int panCount;
    private int mapId;
    private GameObject[] instPans;
    private Vector2[] pansPos;
    private Vector2[] panScale;
    private List<string> mapPrefabName = new List<string>();
    private List<Sprite> mapSprite = new List<Sprite>();
    

    private RectTransform contentRect;
    private Vector2 contentVector;

    private int selectedPanId;
    private bool isScrolling;
  
    // Use this for initialization
    private void Start()
    {        
        this.btnLeft.onClick.AddListener(this.MoveLeft);
        this.btnRight.onClick.AddListener(this.MoveRight);
        this.panCount = this.userMapNum.Length;  
        this.SpriteInit();
        this.btnNext.onClick.AddListener(this.NextScene);
        
        contentRect = GetComponent<RectTransform>();
        instPans = new GameObject[panCount];
        pansPos = new Vector2[panCount];
        panScale = new Vector2[panCount];

        for (int i = 0; i < panCount; i++)
        {
            panPrefab.GetComponent<Image>().sprite = this.mapSprite[i];
            instPans[i] = Instantiate(panPrefab, transform, false);
            if (i == 0) continue;
            var x = instPans[i - 1].transform.localPosition.x + panPrefab.GetComponent<RectTransform>().sizeDelta.x + panOffset;
            var y = instPans[i].transform.localPosition.y;
            instPans[i].transform.localPosition = new Vector2(x, y);
            pansPos[i] = -instPans[i].transform.localPosition;            
        }
    }

    private void LateUpdate()
    {
        this.checkButton();
        if ((contentRect.anchoredPosition.x >= pansPos[0].x && !isScrolling) || (contentRect.anchoredPosition.x <= pansPos[pansPos.Length - 1].x && !isScrolling))
        {
            scrollRect.inertia = false;
        }
        float nearesPos = float.MaxValue;

        //가장 가까운 틀을 찾음
        for (int i = 0; i < panCount; i++)
        {
            float distance = Mathf.Abs(contentRect.anchoredPosition.x - pansPos[i].x);
            if (distance < nearesPos)
            {
                nearesPos = distance;
                selectedPanId = i;
                mapId = this.userMapNum[selectedPanId];
                this.txtTitle.text = this.mapPrefabName[i];
                this.txtCost.text = DataManager.GetInstance().dicMapCost[mapId].cost.ToString();
                this.backGround.GetComponent<Image>().sprite = this.mapSprite[i];            
            }        
            float scale = Mathf.Clamp(1 / (distance / panOffset) * scaleOffset, 0.5f, 1f);
            panScale[i].x = Mathf.SmoothStep(instPans[i].transform.localScale.x, scale + 0.3f, scaleSpeed * Time.deltaTime);
            panScale[i].y = Mathf.SmoothStep(instPans[i].transform.localScale.y, scale + 0.3f, scaleSpeed * Time.deltaTime);
            instPans[i].transform.localScale = panScale[i];           
        }
        float scrollVelocity = Mathf.Abs(scrollRect.velocity.x);      
        if (scrollVelocity < 400 && !isScrolling) scrollRect.inertia = false;
        if (isScrolling || scrollVelocity > 400) return;
        if (isScrolling) return;
       
        contentVector.x = Mathf.SmoothStep(contentRect.anchoredPosition.x, pansPos[selectedPanId].x, snapSpeed * Time.deltaTime);
        contentRect.anchoredPosition = contentVector;     
    }  

    private void SpriteInit()
    {
        for (int i = 0; i < panCount; i++)
        {
            this.mapPrefabName.Add(DataManager.GetInstance().dicMapData[this.userMapNum[i]].groundName);
            var groundPrefab = PrefabsManager.GetInstance().dicGroundPrefabs[mapPrefabName[i]];
            var sprite = groundPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
            this.mapSprite.Add(sprite);
        }
    }

    public void Scrolling(bool scroll)
    {
        isScrolling = scroll;
        if (scroll) scrollRect.inertia = true;
    }

    private void MoveLeft()
    {      
        this.contentRect.anchoredPosition += new Vector2(478f, 0);
    }

    private void MoveRight()
    {      
        if(this.contentRect.localPosition.x < -3700f)
        {
            return;
        }      
        this.contentRect.anchoredPosition -= new Vector2(478f, 0);              
    }

    private void checkButton()
    { 
        if(selectedPanId==0)
        {
            this.btnRight.gameObject.SetActive(true);
            this.btnLeft.gameObject.SetActive(false);
        }else if(selectedPanId==panCount-1)
        {
            this.btnRight.gameObject.SetActive(false);
            this.btnLeft.gameObject.SetActive(true);
        }else
        {
            this.btnLeft.gameObject.SetActive(true);
            this.btnRight.gameObject.SetActive(true);
        }
    }

    private void NextScene()
    {
        PlayData.id = 0;
        PlayData.map_id = mapId;
        PlayData.cost = DataManager.GetInstance().dicMapCost[PlayData.map_id].cost;
        PlayData.Mode = PlayData.ePlayMode.Editor;      

        App.Instance.LoadScene("5. Play");
    }
}
