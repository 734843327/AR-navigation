using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//测试用，可以忽略
public class DirectionTest : MonoBehaviour {
    float init = Screen.height / 2;
    bool isTest = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
     void OnGUI()
    {
        if((GameCtrl2.currentObj!=null)&&isTest)
        {
            if (GUI.RepeatButton(new Rect(100, init, 100, 100), "北"))
            {
                GameCtrl2.currentObj.transform.position += new Vector3(0, 0, 1) * Time.deltaTime;
            }
            if (GUI.RepeatButton(new Rect(100, init + 200, 100, 100), "南"))
            {
                GameCtrl2.currentObj.transform.position += new Vector3(0, 0, -1) * Time.deltaTime;
            }
            if (GUI.RepeatButton(new Rect(0, init + 100, 100, 100), "西"))
            {
                GameCtrl2.currentObj.transform.position += new Vector3(-1, 0, 0) * Time.deltaTime;
            }
            if (GUI.RepeatButton(new Rect(200, init + 100, 100, 100), "东"))
            {
                GameCtrl2.currentObj.transform.position += new Vector3(1, 0, 0) * Time.deltaTime;
            }
        }

    }
}
