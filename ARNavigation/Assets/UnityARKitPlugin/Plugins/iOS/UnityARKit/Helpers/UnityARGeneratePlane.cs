using System;
using System.Collections.Generic;

namespace UnityEngine.XR.iOS
{
	public class UnityARGeneratePlane : MonoBehaviour
	{
		public GameObject planePrefab;
        private UnityARAnchorManager unityARAnchorManager;
        public static List<ARPlaneAnchorGameObject> arpags;
        //public static float height_y;
        //public static Bounds bound;
        //public static Bounds bound2;//使用一个平面还是两个？如果两个的话，启用bound2
		// Use this for initialization
		void Start () {
            unityARAnchorManager = new UnityARAnchorManager();
			UnityARUtility.InitializePlanePrefab (planePrefab);
           // bound2 = new Bounds(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
		}

        void OnDestroy()
        {
            unityARAnchorManager.Destroy ();
        }

		 //void Update()//自己写的
		//{
         //   arpags = unityARAnchorManager.GetCurrentPlaneAnchors();
         //   if(arpags.Count>2){
                //Debug.Log("delete");
         //       GameObject.Destroy(arpags[0].gameObject);
         //       arpags.RemoveAt(0);

                //Debug.Log(arpags.Count);

         //   }
		//}
        public void AdjustPlane(ARPlaneAnchor anchorData){//自己写的
            
            //height_y = UnityARMatrixOps.GetPosition(anchorData.transform).y;
            //arpags = unityARAnchorManager.GetCurrentPlaneAnchors();
            //Debug.Log(arpags.Count);
            //if (arpags.Count > 1)//若多于3（或改成2）个平面，则删除之前的
           // {
                //Debug.Log("delete");
                //GameObject.Destroy(arpags[0].gameObject);
                //arpags.RemoveAt(0);
                //Debug.Log(arpags.Count);
                //unityARAnchorManager.RemoveAnchor(arpags[0].planeAnchor);

            //}
            //arpags = unityARAnchorManager.GetCurrentPlaneAnchors();
            //int t = arpags.Count - 1;
            //Debug.Log(arpags.Count);
            //bound =arpags[t].gameObject.GetComponentInChildren<MeshRenderer>().bounds;

           // bound=UnityARAnchorManager.nowPlane.GetComponentInChildren<MeshRenderer>().bounds;

            //Debug.Log(bound.extents.ToString());
            //if(arpags.Count>1){
             //   bound2= arpags[arpags.Count - 2].gameObject.GetComponent<MeshRenderer>().bounds;
           // }
        }
		void OnGUI()
        {
           // List<ARPlaneAnchorGameObject> arpags = unityARAnchorManager.GetCurrentPlaneAnchors ();
            //if (arpags.Count >= 1) {
                //ARPlaneAnchor ap = arpags [0].planeAnchor;
                //GUI.Box (new Rect (100, 100, 800, 60), string.Format ("Center: x:{0}, y:{1}, z:{2}", ap.center.x, ap.center.y, ap.center.z));
                //GUI.Box(new Rect(100, 200, 800, 60), string.Format ("Extent: x:{0}, y:{1}, z:{2}", ap.extent.x, ap.extent.y, ap.extent.z));
            //}
        }
	}
}

