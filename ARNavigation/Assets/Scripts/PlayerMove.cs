using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Animator animator;
    public static bool IsMove = false;
    public float height = 0.3f;
    public float dist = 0.0f;
    public float damptrace = 20.0f;
    public static Transform tr2;
    //private Vector3 distPos;
    // Use this for initialization
    void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();


    }

    // Update is called once per frame
    void Update()
    {
        
        //tr.LookAt(new Vector3(Camera.main.transform.position.x, tr.position.y, Camera.main.transform.position.z));
        if (GameCtrl2.isARReady)
        {
            
           
            if (GameTest.IsMove == true) { animator.SetBool("IsMove2", true); }
            else { animator.SetBool("IsMove2", false); }
        }
    }
    /*void OnGUI()
    {
        if (GUI.Button(new Rect(150, 50, 50, 50), "run"))
        {


            Debug.Log("new" + tr2.position.ToString());
        }
        if (GUI.Button(new Rect(100, 50, 50, 50), "stop"))
        {
            IsMove = false;
            animator.SetBool("IsMove2", false);
        }
    }*/

}