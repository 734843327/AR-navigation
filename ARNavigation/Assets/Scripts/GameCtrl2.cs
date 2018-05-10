using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.UI;
//识别平面生成精灵；跟随模式；
public class GameCtrl2 : MonoBehaviour {
    //Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
    public GameObject obj;
    public float damptrace = 2;
    public static bool isARReady = false;//是否成功召唤精灵

    public static GameObject currentObj;
     private Vector3 targetPos;
     private Ray ray;
    private RaycastHit hit;
     private Vector3 RayStart;
     public GameObject testObj;
     public GameObject testobjround;
    // private Vector3 initCameraPos;
    // private Vector3 initCameraRot;
    //public Text textA;
    //public Text textB;
    public static GameObject MainChatBox;
    public static bool isNav = false;
    public static bool isFollow = false;
    public static bool isFirstFollow = false;
    public static bool FirstFollowDone = false;
    public string planeIdentifier;
    public GameObject detectedPlaneFather;
    public GameObject detectedPlane;//最新的平面
    //public GameObject detectedPlaneTest;
    public string _string;
    public Myplane lastPlane;//没用
    public static Bounds bound;
    float len;
    public GameObject _generatePlane;
    //public GameObject Centerpos;
    //public GameObject Apos;
   // public GameObject Bpos;
   // public GameObject Cpos;
    //public GameObject Dpos;
    public static float height_y;

    void OnEnable()
    {
        UnityARSessionNativeInterface.ARAnchorAddedEvent += MyARAnchorAdded;
        UnityARSessionNativeInterface.ARAnchorUpdatedEvent += MyARAnchorUpdated;
        //UnityARSessionNativeInterface.ARAnchorRemovedEvent += MyARAnchorRemoved;
    }
     void OnDisable()
    {
        UnityARSessionNativeInterface.ARAnchorAddedEvent -= MyARAnchorAdded;
        UnityARSessionNativeInterface.ARAnchorUpdatedEvent -= MyARAnchorUpdated;
        //UnityARSessionNativeInterface.ARAnchorRemovedEvent -= MyARAnchorRemoved;
    }
    void MyARAnchorAdded(ARPlaneAnchor anchorData){//若成功识别出新平面，则：
        _generatePlane.SendMessage("AdjustPlane", anchorData);
        height_y = UnityARMatrixOps.GetPosition(anchorData.transform).y;
        //height_y = FindPlaneHeight(anchorData);
        // Debug.Log("add");
        //GameObject test = Instantiate(testObj, UnityARMatrixOps.GetPosition(anchorData.transform), new Quaternion(0f, 0f, 0f, 0f));
        //planeIdentifier = anchorData.identifier;
        //_string = planeIdentifier + "/Plane";
        //StartCoroutine(Test(anchorData));
        //ShowPlane();
        //Debug.Log("add   "+UnityARMatrixOps.GetPosition(anchorData.transform));
        if(!isARReady){
            //音效
            currentObj = GameObject.Instantiate(obj) as GameObject;
            currentObj.transform.position = UnityARMatrixOps.GetPosition(anchorData.transform);
            //Debug.Log(currentObj.transform.position);
            currentObj.transform.LookAt(new Vector3(Camera.main.transform.position.x, currentObj.transform.position.y, Camera.main.transform.position.z));
            //MainChatBox.SetActive(true);
            MainChatBox.GetComponentInChildren<Text>().text = "你成功召唤了我！请告诉我，你想去哪里呀？";
            GameObject.Find("UserInput").SendMessage("ShowInputField");
            //currentObj.transform.parent = GameObject.FindWithTag("YCtrl").transform;
            isARReady = true;
        }
        if(isARReady){//跟随模式要用
            RayStart.x = Screen.width / 2;
            RayStart.y = Screen.height / 2;
            RayStart.z = 0;
            ray = Camera.main.ScreenPointToRay(RayStart);
            if(Physics.Raycast(ray,out hit)){
                targetPos = hit.point;
            }
            else{targetPos = UnityARMatrixOps.GetPosition(anchorData.transform);}
        }
    }
    void MyARAnchorUpdated(ARPlaneAnchor anchorData){
        height_y = UnityARMatrixOps.GetPosition(anchorData.transform).y;
        //_generatePlane.SendMessage("AdjustPlane", anchorData);
       // MyDebug(anchorData);
        //Debug.Log("update");
        //float xhalf = detectedPlane.transform.localScale.x / 2;
        //float zhalf = detectedPlane.transform.localScale.z / 2;
        //Vector3 center = detectedPlane.GetComponent<MeshRenderer>().bounds.center;
        //bound = detectedPlane.GetComponent<MeshRenderer>().bounds;//todo：要不要在added里也写？【核心】
        //lastPlane = new Myplane(center, xhalf, zhalf);
        //height_y = FindPlaneHeight(anchorData);
        //ShowPlane();
    }
   
    // Use this for initialization
    void Start () {
        //initCameraPos = Camera.main.transform.position;
        //initCameraRot = Camera.main.transform.eulerAngles;
        //GameObject.Find("CameraParent").transform.eulerAngles = new Vector3(0, Input.compass.trueHeading, 0);
        MainChatBox = GameObject.Find("Image_MainChatBox");
        //MainChatBox.SetActive(false);
        len = Screen.width / 4;
        SetMianChatBox();
        MainChatBox.GetComponentInChildren<Text>().text = "请用手机扫描地面来召唤你的导航小精灵";
	}
    void SetMianChatBox(){
        RectTransform main_tr;
        main_tr = MainChatBox.GetComponent<RectTransform>();
        main_tr.anchoredPosition = new Vector2(Screen.width *0.183f, -Screen.height*0.227f);
        main_tr.sizeDelta = new Vector2(Screen.width * 0.75f, Screen.height * 0.09f);
    }
    // Update is called once per frame
    void Update()//跟随模式
    {
        
       
        if (isFirstFollow||isFollow)
        {
            RayStart.x = Screen.width / 2;
            RayStart.y = Screen.height / 2;
            RayStart.z = 0;
           ray = Camera.main.ScreenPointToRay(RayStart);
            if (Physics.Raycast(ray, out hit))
            {
                targetPos = hit.point;
            }



            if(Vector3.Distance(currentObj.transform.position,targetPos)>0.15)
            { 
                currentObj.transform.LookAt(new Vector3(targetPos.x, currentObj.transform.position.y, targetPos.z));
                currentObj.transform.position = Vector3.Lerp(currentObj.transform.position, targetPos, Time.deltaTime * damptrace);
                GameTest.IsMove = true; 
            }
            else { GameTest.IsMove = false; }
        }
    }
    void MyDebug(ARPlaneAnchor anchorData){//没用
        float x = anchorData.extent.x / 2;
        float z = anchorData.extent.z / 2;
        //Debug.Log("update");
        GameObject test = Instantiate(testObj, UnityARMatrixOps.GetPosition(anchorData.transform), new Quaternion(0f, 0f, 0f, 0f));
       // GameObject test2 = Instantiate(testobjround, UnityARMatrixOps.GetPosition(anchorData.transform) +anchorData.center+ new Vector3(x, 0, z), new Quaternion(0f, 0f, 0f, 0f));
        //GameObject test3 = Instantiate(testobjround, UnityARMatrixOps.GetPosition(anchorData.transform) + anchorData.center+ new Vector3(x, 0, -z), new Quaternion(0f, 0f, 0f, 0f));
       // GameObject test4 = Instantiate(testobjround, UnityARMatrixOps.GetPosition(anchorData.transform) + anchorData.center+ new Vector3(-x, 0, z), new Quaternion(0f, 0f, 0f, 0f));
       // GameObject test5 = Instantiate(testobjround, UnityARMatrixOps.GetPosition(anchorData.transform) + anchorData.center+ new Vector3(-x, 0, -z), new Quaternion(0f, 0f, 0f, 0f));

    }
    void OnGUI()
    {
        //GUI.skin.label.fontSize = 24;
        //if (!isARReady) {
          //  GUI.Label(new Rect(Screen.width/4,Screen.height*0.75f,Screen.width*0.75f,Screen.height*0.15f),"请用手机扫描地面来召唤你的导航小精灵");//【移动到mainchatbox】
        //}
        if(isFirstFollow){
            if(GUI.Button(new Rect(new Rect(Screen.width * 0.386f, Screen.height * 0.927f, Screen.width * 0.57f, Screen.height * 0.05f)),"我已走到户外大路上了，请你带路吧",GameTest.buttonStyle)){
                isFirstFollow = false;
                isNav = true;
                FirstFollowDone = true;
                MainChatBox.SetActive(false);

            }
        }
      //  textA.GetComponent<Text>().text = "init position:"+initCameraPos.ToString()
      //                                  +"\ninit rotation:"+initCameraRot.ToString()
      //                                  +"\nCamera position:" + Camera.main.transform.position.ToString()
      //                                  +"\nCamera rotation:" + Camera.main.transform.eulerAngles.ToString()
      //                                  +"\nCompass:" + Input.compass.trueHeading.ToString();
      //  
     //   textA.GetComponent<Text>().color = Color.black;
     //   if(IsOnShow){
     //       textB.GetComponent<Text>().text = "Chan position:" + GameObject.Find("unitychan(Clone)").transform.position.ToString();
     //       textB.GetComponent<Text>().color = Color.black;
     //   }
    }

    void FirstFollow(){
        isFirstFollow = true;
        MainChatBox.SetActive(true);
        MainChatBox.GetComponentInChildren<Text>().text = "嗯嗯我知道啦，我会先跟随你行走，请走到户外大路上再叫我，我就会开始导航";
    }
    //IEnumerator Test(ARPlaneAnchor anchorData){
        //yield return new WaitUntil(() => GameObject.Find(planeIdentifier) != null);
        //detectedPlaneFather = GameObject.Find(planeIdentifier);
        //detectedPlane = GameObject.Find(_string);
        //float xhalf = detectedPlane.GetComponent<MeshRenderer>().bounds.size.x * detectedPlane.transform.localScale.x/2;
        //float zhalf = detectedPlane.GetComponent<MeshRenderer>().bounds.size.z * detectedPlane.transform.localScale.z/2;
       // float xhalf = detectedPlane.transform.localScale.x / 2;
        //float zhalf = detectedPlane.transform.localScale.z/2;
        //Vector3 center = detectedPlane.GetComponent<MeshRenderer>().bounds.center;
       // bound = detectedPlane.GetComponent<MeshRenderer>().bounds;
        //lastPlane = new Myplane(center, xhalf, zhalf);
        //height_y = FindPlaneHeight(anchorData);
        //GameObject test = Instantiate(testObj, center, new Quaternion(0f, 0f, 0f, 0f));
        //GameObject test2 = Instantiate(testobjround, center + new Vector3(xhalf, 0, zhalf), new Quaternion(0f, 0f, 0f, 0f));
        //GameObject test3 = Instantiate(testobjround,center+ new Vector3(xhalf, 0, -zhalf), new Quaternion(0f, 0f, 0f, 0f));
        //GameObject test4 = Instantiate(testobjround, center + new Vector3(-xhalf, 0, zhalf), new Quaternion(0f, 0f, 0f, 0f));
        //GameObject test5 = Instantiate(testobjround, center+ new Vector3(-xhalf, 0, -zhalf), new Quaternion(0f, 0f, 0f, 0f));
    //}
   public struct Myplane{
        public Vector3 center;
        public float xhalf;
        public float zhalf;
        public Myplane(Vector3 center,float xhalf,float zhalf){
            this.center = center;
            this.xhalf = xhalf;
            this.zhalf = zhalf;
        }
    }
    //void ShowPlane(){
    //    Centerpos.transform.position=lastPlane.center;
     //   Apos.transform.position = lastPlane.center+new Vector3(lastPlane.xhalf, 0,lastPlane.zhalf);
     //   Bpos.transform.position = lastPlane.center+ new Vector3(lastPlane.xhalf, 0, -lastPlane.zhalf);
    //    Cpos.transform.position = lastPlane.center+ new Vector3(-lastPlane.xhalf, 0, lastPlane.zhalf);
    //    Dpos.transform.position = lastPlane.center+ new Vector3(-lastPlane.xhalf, 0, -lastPlane.zhalf);
   // }
    public float FindPlaneHeight(ARPlaneAnchor anchorData){
        float _y = UnityARMatrixOps.GetPosition(anchorData.transform).y;
        return _y;
    }
}
