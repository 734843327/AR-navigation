using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Mr_newbiebt;
using Utils;
using LitJson;
using System;
//本脚本用来与百度地图Web API通讯，拿到导航所需要的信息（地点检索，路径规划等）
//注意：百度有专门的坐标体系，从设备中读取的坐标不能直接用，要使用算法进行一次变换，算法见gps2bd脚本
public class BaiduMap_main : MonoBehaviour {
    // static string queryName;
    public string region;//用户所在城市
    public static string ak = "************************";  //【请将这里的******替换成您的百度地图开发者ak，引号不要去掉】
    string urlPlace;
    string urlDirectin;
    string lng;
    string lat;
    public Vector2d targetBM;
    public static List<PlaceInfo> placeInfo;
    public static List<RoutePointInfo> routePointInfo;
    public static bool isMapReady = false;
    string status;
    public int year, month, day, hour, min, sec;
    public string timeURL = "http://cgi.im.qq.com/cgi-bin/cgi_svrtime";
    void Start()
    {
        //StartCoroutine(Geocoding());
        //Vector2d start = new Vector2d(39.113835, 117.185733);
        //Vector2d end = new Vector2d(39.109359, 117.175215);
        //StartCoroutine(queryDirection(start,end));
        StartCoroutine(searchCity());//打开应用后先确认用户所在城市，只在这个城市中检址。
       // StartCoroutine(GetTime());
    }
    //IEnumerator GetTime()
    //{
     //   WWW www = new WWW(timeURL);
    //    while (!www.isDone)
   //     {
   //         yield return www;
    //    }
    //    SplitTime(www.text);
    //    Debug.Log(year + " " + month + " " + day + " " + hour + " " + min + " " + sec);
    //    if(year>=2019||month>=4||day>=25){//给三天体验时间
    //        Application.Quit();
    //    }
    //}
    //void SplitTime(string dateTime)//处理拿到的网络时间，分割成年、月、日、时、分、秒
    //{
    //    dateTime = dateTime.Replace("-", "|");
     //   dateTime = dateTime.Replace(" ", "|");
     //   dateTime = dateTime.Replace(":", "|");
     //   string[] Times = dateTime.Split('|');
     //   year = int.Parse(Times[0]);
     //   month = int.Parse(Times[1]);
     //   day = int.Parse(Times[2]);
     //   hour = int.Parse(Times[3]);
     //   min = int.Parse(Times[4]);
     //   sec = int.Parse(Times[5]);
    //}
    IEnumerator searchCity(){//输入坐标，输出坐标所在城市
        yield return new WaitUntil(()=>Input.location.status == LocationServiceStatus.Running);
        Vector2d coordinate = gps2bd.wgs2bd(new Vector2d(Input.location.lastData.latitude, Input.location.lastData.longitude));
        string urlSearchCity = "http://api.map.baidu.com/geocoder/v2/?location="+coordinate.x.ToString()+","+coordinate.y.ToString()+"&output=json&ak=" + ak;
        WWW cityInfo = new WWW(urlSearchCity);
        yield return cityInfo;
        if(cityInfo.error==null){
            ReadJSON_city(cityInfo);
        }

    }


    IEnumerator Geocoding(string queryName)//输入要搜索的地名，返回最相关的五个结果
    {
        //yield return new WaitUntil(() => queryName != null);
        //在C#中，需要用到yield的话，必须建立在IEnumerator类中执行。

        urlPlace = "http://api.map.baidu.com/place/v2/search?query=" + queryName +"&region=" + region +  "&output=json&ak=" + ak;//
       // urlPlace = "http://api.map.baidu.com/place/v2/search?query=" +queryName+ "&region=天津市"  + "&output=json&ak=" + ak;
        WWW addressInfo = new WWW(urlPlace);
        //定义www为WWW类型并且等于所下载下来的WWW中内容。
        yield return addressInfo;
        //返回所下载的www的值
        if (addressInfo.error == null)
        {
            //saveData(addressInfo,"天津大学");
            readJSON_Geocoding(addressInfo);
        }
    }
    public  void PreQueryDirection(Vector2d end)//变换一下坐标
    {
       
        //Debug.Log(end.ToString());
        Vector2d start=gps2bd.wgs2bd(new Vector2d(Input.location.lastData.latitude, Input.location.lastData.longitude));
        //Vector2d start = new Vector2d(39.115452, 117.192);
        StartCoroutine(QueryDirection(start, end));
    }
    IEnumerator QueryDirection(Vector2d start,Vector2d end)//输入起点终点坐标，输出百度给出的路径规划
    {
        

        string s_start = start.x.ToString() + "," + start.y.ToString();
        string s_end =end.x.ToString() + "," + end.y.ToString();
        urlDirectin = " http://api.map.baidu.com/direction/v1?mode=walking&origin="+s_start+"&destination="+s_end+"&region="+region+"&output=json&ak=" + ak;

        WWW directionInfo = new WWW(urlDirectin);
        yield return directionInfo;
        //返回所下载的www的值
        if (directionInfo.error == null)
        {
            //saveData(directionInfo,"路线");
            readJSON_Direction(directionInfo);
        }
    }
    public void ReadJSON_city(WWW www){
        string my_Json = www.text;
        JSONNode jsonData = JSON.Parse(my_Json);
        region = jsonData["result"]["addressComponent"]["city"];
    }
    public void readJSON_Geocoding(WWW www)
    {
        placeInfo = new List<PlaceInfo>();
        string _name;
        string address;
        string addressInfo;
        string my_Json = www.text;
        Vector2d coordinate;
        JSONNode jsonData = JSON.Parse(my_Json);
        int num = jsonData["results"].Count;
        if (num > 5) { num = 5; }//最多只显示5个结果
        for (int i = 0; i < num;i++){
            _name = jsonData["results"][i]["name"];
            address=jsonData["results"][i]["address"];
            addressInfo = _name + "," + address;
            coordinate = new Vector2d(Convert.ToDouble(jsonData["results"][i]["location"]["lat"]), Convert.ToDouble(jsonData["results"][i]["location"]["lng"]));
            placeInfo.Add(new PlaceInfo(coordinate,addressInfo,i));
        }
        GameObject.Find("UserInput").SendMessage("GeocoderResponse",placeInfo);
        status = jsonData["status"];
    }
    public void readJSON_Direction(WWW www)//处理路径规划结果：分解成若干个途经点。
    {
        string my_Json = www.text;
        JSONNode jsonData = JSON.Parse(my_Json);
        //Debug.Log(jsonData["result"]["routes"][0]["steps"].Count);
        //Debug.Log(jsonData["result"]["routes"][0]["steps"][1]["instructions"]);
        //Debug.Log(jsonData["result"]["routes"][0]["steps"][0]["path"]);
        //Debug.Log(jsonData["result"]["routes"][0]["steps"][1]["stepOriginLocation"]["lng"]);
        //results里包括routes和一些没用的东西，routes里包括steps和一些没用的东西
        //routes后面只能接[0]，steps表示每个路段（不一定是直线！多为折线），接[i];path表示每个路段折线中的每一个顶点
        //我要的：每个steps的instruction（需翻译成汉字）、path（需分割并翻译成vector2d）、
        //                   stepOriginLocation和stepDestinationLocation（翻译成vector2d）
        routePointInfo = new List<RoutePointInfo>();
        int stepCount=jsonData["result"]["routes"][0]["steps"].Count;
        int pathCount;


        for (int i = 0; i < stepCount; i++)//对于每一个路段：
        {
            //pathCount = jsonData["result"]["routes"][0]["steps"][i]["path"].Count;
            //for (int j = 0; j < pathCount; j++) { }//上句和这句不行，path是一个长的字符串，没做处理
            string _instructionInfo = jsonData["result"]["routes"][0]["steps"][i]["instructions"];
            string pathInfo=jsonData["result"]["routes"][0]["steps"][i]["path"];//拿到path和instruction，准备处理成结构体
            string[] WholeString = pathInfo.Split(';');
            pathCount = WholeString.Length;

            double y_start = Convert.ToDouble(jsonData["result"]["routes"][0]["steps"][i]["stepOriginLocation"]["lng"]);
            double x_start = Convert.ToDouble(jsonData["result"]["routes"][0]["steps"][i]["stepOriginLocation"]["lat"]);
            double y_end = Convert.ToDouble(jsonData["result"]["routes"][0]["steps"][i]["stepDestinationLocation"]["lng"]);
            double x_end = Convert.ToDouble(jsonData["result"]["routes"][0]["steps"][i]["stepDestinationLocation"]["lat"]);
            routePointInfo.Add(new RoutePointInfo(new Vector2d(x_start,y_start), _instructionInfo, "Origin",i,-1));
            for (int j = 0; j < pathCount; j++) {//对于路段中的每一个顶点：
                string[] wholeCoodinate = WholeString[j].Split(',');
                double y = Convert.ToDouble(wholeCoodinate[0]);
                double x = Convert.ToDouble(wholeCoodinate[1]);
                Vector2d _coordinate =new Vector2d (x, y);
                routePointInfo.Add(new RoutePointInfo(_coordinate, _instructionInfo,"path",i,j));
                //Debug.Log(_coordinate.x + "," + _coordinate.y);
            }
            routePointInfo.Add(new RoutePointInfo(new Vector2d(x_end, y_end), _instructionInfo, "Destination",i,-2));
        }
        for(int i = 0; i < routePointInfo.Count; i++)
        {
            Debug.Log(routePointInfo[i].coordinate+"   "+routePointInfo[i].instructionInfo+"   "+routePointInfo[i].typeInfo);
        }
        isMapReady = true;
        GameTest.ith = 0;
    }
    void saveData(WWW www, string name)//没用
    {

        //以下代码作用为：将数据保存到本地生成json文件
        string filename = name +"南开内"+UnityEngine.Random.Range(0.0f, 5.0f).ToString();
       
        byte[] stream = www.bytes;
        FileStream fs = new FileStream(filename, FileMode.CreateNew);
        // Create the writer for data.
        BinaryWriter w = new BinaryWriter(fs);
        // Write data to Test.data.
        w.Write(stream);
        w.Close();
        fs.Close();
    }
    public struct RoutePointInfo{
        public Vector2d coordinate;
        public string instructionInfo;
        public string typeInfo;
        public int step_th;//第几个路段
        public int path_th;//第几个顶点,-1表示路段起点，-2表示路段终点
        public RoutePointInfo(Vector2d coordinate, string instructionInfo,string typeInfo, int step_th,int path_th)
        {
            this.coordinate = coordinate;
            this.instructionInfo = instructionInfo;
            this.typeInfo = typeInfo;
            this.step_th = step_th;
            this.path_th = path_th;
        }
    }
    public struct PlaceInfo{
        public Vector2d coordinate;
        public string address;
        public int ranking;//搜索结果排名，0-4
        public PlaceInfo(Vector2d coordinate,string address,int ranking){
            this.coordinate = coordinate;
            this.address = address;
            this.ranking = ranking;
        }
    }
     void OnGUI()
    {
        //if(region!=null){
         //   GUI.Label(new Rect(100,700,600,48),region);
        //}
       // GUI.Label(new Rect(100, 800, 600, 48), status);
    }
}
