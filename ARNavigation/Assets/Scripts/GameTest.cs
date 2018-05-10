using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using System;
using Mr_newbiebt;
using UnityEngine.XR.iOS;
using LitJson;

public class GameTest : MonoBehaviour {
    Vector2d nowPos;
    string s_nowPos;
    Vector2d targetPos;
    Vector2d direction;
    public static bool IsMove = false;
    //private bool isNav2 = false;
    public static int ith=0;//现在走到了第几个目标点
    public float speed = 1;
    public float TargetChangeRange = 0.0001f;//当当前的目标点距离小于该值时，视为到达，将目标点切换到list中的下一个//0.0001f约为10米
    public float ElfWaitRange = 10;//当elf与人的距离小于该值时，再移动
    public GameObject obj_Elf;
    private bool DoOnce = true;
    float distance2;//人距精灵的距离
    Vector3 tarposUpdate;//下一帧精灵要去的点
    Vector3 tarposUpdate_temp;
    public LayerMask collisionLayer = 1 << 10;
    //Ray ray;
    //RaycastHit hit;
    public Texture2D topInfoImg;
    public Texture2D button_hover_Img;
    public Texture2D button_normal_Img;
    public Texture2D button_pressed_Img;
    float len;
    public static GUIStyle topInfoStyle;
    public static GUIStyle buttonStyle;
    GUIStyle msgStyle;
    //string str1;
    string roadMsg= "unknown";
    string roadURL;
    public GameObject BaiduMapTest2;
    //public static Bounds bound;
    // Use this for initialization
     
    void Start () {
        len = Screen.width / 4;
        StartCoroutine(GetRoadMsg());
	}
	
	// Update is called once per frame
	void Update () {
//        Debug.Log(DirectionsFactory.isMapReady.ToString());
        //bound=UnityARGeneratePlane.arpags[UnityARGeneratePlane.arpags.Count - 1].gameObject.GetComponent<MeshRenderer>().bounds;
        if(GameCtrl2.isARReady&&DoOnce){
            obj_Elf = GameObject.Find("unitychan(Clone)");
            DoOnce = false;
        }
        
        if (GameCtrl2.isNav)//导航模式
        {
            nowPos= gps2bd.wgs2bd(new Vector2d(Input.location.lastData.latitude, Input.location.lastData.longitude));
            targetPos = BaiduMap_main.routePointInfo[ith].coordinate;
            //direction = changeCoordinateSystem(VectorDirection(nowPos, targetPos), GetGPS.north);
            direction = VectorDirectionNew(nowPos, targetPos);
            //【核心移动代码，需修改！！！！】
            distance2 = (obj_Elf.transform.position - Camera.main.transform.position).magnitude;
            tarposUpdate = obj_Elf.transform.position + new Vector3((float)direction.x, 0, (float)direction.y)*Time.deltaTime;
            tarposUpdate.y = GameCtrl2.height_y;
            if ( distance2< ElfWaitRange)
            {
                //if (distance2 < 2) { speed = 2.0f; }

                //obj_Elf.transform.LookAt(new Vector3((float)direction.x + obj_Elf.transform.position.x, obj_Elf.transform.position.y, (float)direction.y + obj_Elf.transform.position.z));
                //obj_Elf.transform.position += new Vector3((float)direction.x, 0, (float)direction.y) * speed * Time.deltaTime;
                IsMove = true;
                //ray = new Ray(tarposUpdate + new Vector3(0, 30, 0), new Vector3(0, -1, 0));
                //if(Physics.Raycast(ray,out hit,Mathf.Infinity,collisionLayer)){//前方点是已识别平面，直接上
                if(UnityARAnchorManager.bound.Contains(tarposUpdate)){
                    obj_Elf.transform.LookAt(new Vector3(tarposUpdate.x, obj_Elf.transform.position.y,tarposUpdate.z));
                    if(Vector3.Distance(obj_Elf.transform.position,tarposUpdate)>=2.0f){
                        obj_Elf.transform.position = Vector3.MoveTowards(obj_Elf.transform.position, tarposUpdate, Time.deltaTime * 1.0f);
                    }
                    else{
                        obj_Elf.transform.position = tarposUpdate;
                    }
                    //str1 = "在平面内";
                }
                if (!UnityARAnchorManager.bound.Contains(tarposUpdate)){//前方点不是已识别平面，绕一绕
                    tarposUpdate_temp= UnityARAnchorManager.bound.ClosestPoint(tarposUpdate);
                    obj_Elf.transform.LookAt(new Vector3(tarposUpdate_temp.x, obj_Elf.transform.position.y, tarposUpdate_temp.z));
                    obj_Elf.transform.position = Vector3.MoveTowards(obj_Elf.transform.position, tarposUpdate_temp, Time.deltaTime * 1.0f);
                    //str1 = "在平面外";
                }
            }
            else { IsMove = false; }
            if (distance(nowPos, targetPos) < TargetChangeRange) { 
                ith++;
                StartCoroutine(GetRoadMsg());

            }//todo：显示道路名
            if (ith>=( BaiduMap_main.routePointInfo.Count-1)){//【完成导航】
                
                GameCtrl2.isNav = false;
                GameCtrl2.MainChatBox.SetActive(true);
                GameCtrl2.MainChatBox.GetComponentInChildren<Text>().text = "你已经到终点啦，导航结束～";
            }
        }
        //nowPos = new Vector2d(39.10638, 117.17005);
        //targetPos = DirectionsFactory.datVector2d[0];
        //direction = changeCoordinateSystem(VectorDirection(nowPos, targetPos),30);
        //Debug.Log(nowPos.ToString() + "   " + targetPos.ToString());
        //Debug.Log(direction.x.ToString() + "   " + direction.y.ToString());

    }
    IEnumerator GetRoadMsg(){
        s_nowPos = nowPos.x + "," + nowPos.y;
        roadURL = "http://api.map.baidu.com/geocoder/v2/?location="+s_nowPos+"&output=json&extensions_road=true&ak="+BaiduMap_main.ak;
        WWW roadInfo = new WWW(roadURL);
        //定义www为WWW类型并且等于所下载下来的WWW中内容。
        yield return roadInfo;
        //返回所下载的www的值
        if (roadInfo.error == null)
        {
            //saveData(addressInfo,"天津大学");
            string my_Json = roadInfo.text;
            JSONNode jsonData = JSON.Parse(my_Json);
            roadMsg = jsonData["result"]["roads"][0]["name"];
        }
    }
   
    void OnGUI()
    {
        topInfoStyle = new GUIStyle();
        topInfoStyle.fontSize = 24;
        topInfoStyle.wordWrap = true;
        topInfoStyle.normal.background = topInfoImg;
        topInfoStyle.alignment = TextAnchor.MiddleCenter;
        buttonStyle = new GUIStyle();
        buttonStyle.fontSize = 24;
        buttonStyle.wordWrap = true;
        buttonStyle.normal.background = button_normal_Img;
        buttonStyle.hover.background = button_hover_Img;
        buttonStyle.active.background = button_pressed_Img;
        buttonStyle.alignment = TextAnchor.MiddleCenter;
        msgStyle = new GUIStyle();
        msgStyle.fontSize = 38;
        msgStyle.normal.textColor = Color.red;
        msgStyle.wordWrap = true;
        //GUI.skin.label.fontSize = 24;
        //GUI.skin.button.fontSize = 24;
        //if (GUI.Button(new Rect(100, 100, 200, 100), "开始导航"))
        //{
        //    isNav2 = true;
        //}
        //GUI.Label(new Rect(100, 500, 600, 48), "相机旋转：" + Camera.main.transform.localEulerAngles.ToString());
        //GUI.Label(new Rect(100, 600, 600, 48), "相机位置：" + Camera.main.transform.position.ToString());
        //GUI.Label(new Rect(len / 2, len * 1.5f, 2.7f * len, len / 2), "此版本为仅供测试。作者qq：734843327", msgStyle);
        //GUI.Label(new Rect(len / 2, len, 2.7f * len, len / 2), "当前道路：" + roadMsg);
        if (BaiduMap_main.isMapReady)
        {
            
            //GUI.Label(new Rect(100, 300, 1200, 48), "第" + (ith + 1) + "/" + BaiduMap_main.routePointInfo.Count + "个目标点坐标为：" + BaiduMap_main .routePointInfo[ith].coordinate.x.ToString() + " , " + BaiduMap_main.routePointInfo[ith].coordinate.y.ToString());
            GUI.Label(new Rect(Screen.width*0.061f,Screen.height*0.035f,Screen.width*0.65f,Screen.height*0.09f), BaiduMap_main.routePointInfo[ith].instructionInfo,topInfoStyle);//todo:显示道路名
        }
       // if (GameCtrl2.isARReady)
       // {
       //     GUI.Label(new Rect(100, 400, 400, 48), "精灵坐标：" + obj_Elf.transform.position.ToString());
       // }
        if (GameCtrl2.isNav)
        {
            if (GUI.Button(new Rect(Screen.width*0.386f,Screen.height*0.927f,Screen.width*0.57f,Screen.height*0.05f), "诶你带的路不太对呀，先跟我走吧",buttonStyle))
            {
                GameCtrl2.isFollow = true;
                GameCtrl2.isNav = false;
            }
        }
        if (GameCtrl2.isFollow)
        {
            if (GUI.Button(new Rect(Screen.width * 0.386f, Screen.height * 0.927f, Screen.width * 0.57f, Screen.height * 0.05f),"现在由你来带路吧！", buttonStyle))
            {
                BaiduMapTest2.SendMessage("PreQueryDirection",MyUserInput.MyCoordinate);
                GameCtrl2.isFollow = false;
                GameCtrl2.isNav = true;
            }
        }
       // GUI.Label(new Rect(100, 100, 500, 100), str1);
        //if (BaiduMap_main.routePointInfo[ith].coordinate != null)
        //{
        //    GUI.Label(new Rect(100, 300, 500, 100), "目标GPS:" + BaiduMap_main.routePointInfo[ith].coordinate.x + " " + BaiduMap_main.routePointInfo[ith].coordinate.y);
       // }
    }
    //先把终点和起点的GPS坐标做差,求方向矢量,x/y要反一下
    public Vector2d VectorDirection(Vector2d start, Vector2d end)
    {
        Vector2d direction1;
        direction1.y= end.x - start.x;
        direction1.x = end.y - start.y;
       // Debug.Log(direction1.x.ToString() + "   " + direction1.y.ToString());
        return direction1;
    }
    public Vector2d VectorDirectionNew(Vector2d start, Vector2d end){
        Vector2d result;
        double deltaX;
        double deltaY;
        deltaX = end.y - start.y;
        deltaY = end.x - start.x;
        result.x = deltaX / Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
        result.y = deltaY / Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
        return result;
      
    }
    //再把求得的方向矢量转成Unity坐标系下坐标,并归一化
    //【x、y坐标反了一下，要注意，因为vector2d.x代表纬度， .y代表经度.与Unity坐标系相反】
    public Vector2d changeCoordinateSystem(Vector2d point, float angle)
    {
        Vector2d result;
        double deltaX;
        double deltaY;
        angle = angle * 3.14159265f / 180;//角度化弧度
        deltaX = point.y * Mathd.Cos(angle) + point.x * Mathd.Sin(angle);
        deltaY = point.x * Mathd.Cos(angle) - point.y * Mathd.Sin(angle);
        //Debug.Log("delta:" + deltaX.ToString() + "   " + deltaY.ToString());
        result.y = deltaX / Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
        result.x = deltaY / Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
        return result;
    }
    //public void OnValueChanged_TargetChangeRange()
    //{
    //    string input= GameObject.Find("InputField_TargetChangeRange").GetComponent<InputField>().text;
    //    TargetChangeRange = Convert.ToSingle(input);
    //}
   // public void OnValueChanged_ElfWaitRange()
   // {
    //    string input = GameObject.Find("InputField_ElfWaitRange").GetComponent<InputField>().text;
    //    ElfWaitRange = Convert.ToSingle(input);
    //}
    public double distance(Vector2d start,Vector2d end)
    {
        Vector2d direction;
        double dis;
        direction.x = end.x - start.x;
        direction.y = end.y - start.y;
        dis= Mathd.Sqrt(direction.x * direction.x + direction.y * direction.y);
        return dis;
    }
    /*void test()
    {
        for(int i = 0; i < DirectionsFactory.datVector2d.Count - 1; i++)
        {
            direction = changeCoordinateSystem(VectorDirection(DirectionsFactory.datVector2d[i], DirectionsFactory.datVector2d[i+1]), 30);
            Debug.Log((i + 1).ToString() + "th direction is:" + direction.x.ToString()+","+direction.y.ToString());
        }
    }*/
}
