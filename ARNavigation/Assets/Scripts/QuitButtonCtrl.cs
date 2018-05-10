using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitButtonCtrl : MonoBehaviour {
    RectTransform tr;
    float length;
    public GameObject msgbox;
    public RectTransform msgboxTr;
    float scrnHeight;
    float scrnWidth;
	// Use this for initialization
	void Start () {
        scrnHeight = Screen.height;
        scrnWidth = Screen.width;
        length = Screen.width / 4;
        tr = GetComponent<RectTransform>();
        tr.anchoredPosition = new Vector2(0.972f * scrnWidth, -0.02f * scrnHeight);
        tr.sizeDelta = new Vector2(0.05f * scrnHeight, 0.05f * scrnHeight);
        setMsgbox();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void quit()
    {
        msgbox.SetActive(true);
       // Application.Quit();
    }
    public void setMsgbox()
    {
        msgbox = GameObject.Find("messagebox_quit");
        msgboxTr = msgbox.GetComponent<RectTransform>();
        msgboxTr.anchoredPosition = new Vector2(Screen.width / 2, -Screen. height / 2);
        msgboxTr.sizeDelta = new Vector2(3 * length, 3 * length / 1.33f);
        msgbox.SetActive(false);
    }
    public void sure()
    {
        Application.Quit();
    }
    public void cancel()
    {
        msgbox.SetActive(false);
    }
}
