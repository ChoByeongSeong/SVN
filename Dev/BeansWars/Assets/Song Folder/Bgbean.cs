using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bgbean : MonoBehaviour
{
    public float speed = 0.2f;
    private Material mat;
	// Use this for initialization
	void Start () {
        mat = this.gameObject.GetComponent<Renderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
        float newX = mat.mainTextureOffset.x - speed * Time.deltaTime;
        Vector2 newOffset = new Vector2(newX, 0);
        mat.mainTextureOffset = newOffset;
    }
}
