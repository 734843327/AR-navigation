using UnityEngine;
using System.Collections;
using Mr_newbiebt;
using Utils;
public class GetGPS : MonoBehaviour
{
    public static float north;
    Vector2d pos;

    // Use this for initialization  
    void Awake()
    {
       // StartCoroutine(StartGPS());

        Input.location.Start(5.0f, 5.0f);
        Input.compass.enabled = true;
    }
    void Start()
    {
        
        north = Input.compass.trueHeading;
    }
    

    void OnGUI()
    {
        //pos = gps2bd.wgs2bd(new Vector2d(Input.location.lastData.latitude, Input.location.lastData.longitude));
       // GUI.skin.label.fontSize = 28;
       // GUI.Label(new Rect(100, 200, 600, 100), "当前GPS:  "+pos.x.ToString()+"   "+ pos.y.ToString());
        //GUI.Label(new Rect(100, 100, 600, 48), "copmass:"+Input.compass.trueHeading.ToString());


    }

    // Input.location = LocationService  
    // LocationService.lastData = LocationInfo   

    //IEnumerator StartGPS()
    //{
    //    // Input.location 用于访问设备的位置属性（手持设备）, 静态的LocationService位置  
    //    // LocationService.isEnabledByUser 用户设置里的定位服务是否启用  
    //    if (!Input.location.isEnabledByUser)
    //    {
    //        this.gps_info = "isEnabledByUser value is:" + Input.location.isEnabledByUser.ToString() + " Please turn on the GPS";
            
    //    }

    //    // LocationService.Start() 启动位置服务的更新,最后一个位置坐标会被使用  
    //    Input.location.Start(5.0f, 1.0f);

    //    int maxWait = 20;
    //    while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
    //    {
    //        // 暂停协同程序的执行(1秒)  
    //        yield return new WaitForSeconds(1);
    //        maxWait--;
    //    }

    //    if (maxWait < 1)
    //    {
    //        this.gps_info = "Init GPS service time out";
            
    //    }

    //    if (Input.location.status == LocationServiceStatus.Failed)
    //    {
    //        this.gps_info = "Unable to determine device location";
            
    //    }
    //    else
    //    {
    //        this.gps_info = "N:" + Input.location.lastData.latitude + " E:" + Input.location.lastData.longitude;
    //        this.gps_info = this.gps_info + " Time:" + Input.location.lastData.timestamp;
    //        yield return new WaitForSeconds(100);
    //    }
    //}
}