using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoButtonCtrl : MonoBehaviour {
    RectTransform tr2;//左下角按钮
    RectTransform tr_tbox;//弹出的文本框
    float length;
    float scrnHeight;
    float scrnWidth;
    public GameObject infoTextbox;
    bool isShowing = false;
    // Use this for initialization
    void Start () {
        length = Screen.width / 4;
        scrnHeight = Screen.height;
        scrnWidth = Screen.width;
        tr2 = GetComponent<RectTransform>();
        tr2.anchoredPosition = new Vector2(0.028f*scrnWidth, -0.98f*scrnHeight);
        tr2.sizeDelta = new Vector2(0.05f*scrnHeight, 0.05f * scrnHeight);
        setInfo();
    }
	
	// Update is called once per frame
	void Update () {
        if(isShowing==true&&Input.touchCount>0&&Input.touches[0].phase==TouchPhase.Began){
            closeInfo();
        }
	}
    public void showInfo()
    {
        infoTextbox.SetActive(true);
        isShowing = true;
    }
    void setInfo()
    {
        tr_tbox = GameObject.Find("messagebox_Info").GetComponent<RectTransform>();
        tr_tbox.anchoredPosition = new Vector2(Screen.width / 2, -Screen.height / 2);
        tr_tbox.sizeDelta = new Vector2(5 * length, 6.0f*length);
        closeInfo();
        
    }
    public void closeInfo()
    {
        infoTextbox.SetActive(false);
        isShowing = false;
    }
}
