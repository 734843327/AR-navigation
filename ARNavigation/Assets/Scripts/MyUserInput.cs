using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using System;
//监控SearchInput，用户输入地点后，传递给Geocoding函数
public class MyUserInput : MonoBehaviour {
    InputField _inputField;
    RectTransform input_tr;
    bool ButtonOnShow = true;
    public GameObject SearchInput;
    bool ButtonDone = false;
    bool isResultNull = false;
    List<BaiduMap_main.PlaceInfo> placeInfoForGUI;
    float len = Screen.width / 4;
    public static Vector2d MyCoordinate;
    GUIStyle optionStyle;
    public Texture2D optionImg;
    void Awake()
    {
        _inputField = GetComponent<InputField>();
        _inputField.onEndEdit.AddListener(HandleUserInput);
    }

	// Use this for initialization
	void Start () {
       // SearchInput = GameObject.FindWithTag("SearchInput");
        //RectTransform input_tr;
        input_tr = SearchInput.GetComponent<RectTransform>();
        input_tr.anchoredPosition=new Vector2(Screen.width *0.219f, -Screen.height*0.645f);
        input_tr.sizeDelta = new Vector2(Screen.width * 0.31f,  Screen.height * 0.04f);
        HideInputField();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void ShowInputField()
    {
        input_tr.anchoredPosition += new Vector2(Screen.width, 0);
    }
    void HideInputField()
    {
        input_tr.GetComponent<RectTransform>().anchoredPosition -= new Vector2(Screen.width, 0);
    }
    void HandleUserInput(string searchString)//输入地名后发消息
    {
        
        if (!string.IsNullOrEmpty(searchString))//要搜素的地名
        {
            // string[] country = { "cn" };
            // _resource.Country = country;
            //string _queryName= searchString;
            //
            GameObject.Find("BaiduMapTest2").SendMessage("Geocoding", searchString);
            HideInputField();
            GameCtrl2.MainChatBox.SetActive(false);
            //Debug.Log("!!!");
        }
    }
    void GeocoderResponse(List<BaiduMap_main.PlaceInfo> placeInfo )//接受消息 
    {
        ButtonOnShow = true;
        placeInfoForGUI = placeInfo;
        if (null == placeInfo)
        {
            isResultNull = true;
        }
    }
    void OnGUI()
    {
        GUI.skin.button.wordWrap = true;
        GUI.skin.button.clipping = TextClipping.Clip;
        GUI.skin.button.fontSize = 24;
        optionStyle = new GUIStyle();
        optionStyle.fontSize = 24;
        optionStyle.wordWrap = true;
        optionStyle.normal.background = optionImg;
        optionStyle.alignment = TextAnchor.MiddleCenter;
        if (null != placeInfoForGUI && placeInfoForGUI.Count > 0 && ButtonOnShow)
        {
            //var center = res.Features[0].Center;
            //_inputField.text = string.Format("{0},{1}", center.x, center.y);
            //_coordinate = res.Features[0].Center;
            for (int i = 0; i < placeInfoForGUI.Count; i++)
            {
                
                //float k = 0;
                //if (i > 2) { k = 2 * len; }
                string buttonMsg = placeInfoForGUI[i].address.ToString();

                if (GUI.Button(new Rect(Screen.width / 2 - 0.75f * len + len * (float)Math.Pow(-1, i + 1), 0.6f * (i + 1) * len, 1.5f * len, 0.6f * len), buttonMsg,optionStyle))
                {
                    MyCoordinate = placeInfoForGUI[i].coordinate;
                    ButtonDone = true;
                    GameObject.Find("GameCtrl").SendMessage("FirstFollow");
                    //Debug.Log(MyCoordinate);
                    ButtonOnShow = false;
                }
            }
            if (GUI.Button(new Rect(Screen.width * 0.386f, Screen.height * 0.927f, Screen.width * 0.57f, Screen.height * 0.05f), "\"这些都不是我想去的地方，我要重新搜索\"", GameTest.buttonStyle))
            {
                ShowInputField();
                ButtonOnShow = false;
            }


        }
        if (isResultNull && ButtonOnShow)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 1.75f, 0.6f * len, 1.5f * len, 0.6f * len), "抱歉我不知道这个地方呢，请换一个地名吧～",optionStyle))
            {
                ShowInputField();
                ButtonOnShow = false;
                isResultNull = false;
            }

        }
        if (ButtonDone && GameCtrl2.FirstFollowDone)
        {
            GameObject.Find("BaiduMapTest2").SendMessage("PreQueryDirection",MyCoordinate);//todo
            ButtonDone = false;
            //Debug.Log("ok");
        }

    }
}
