using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{

    public GameObject obj;
    // Start is called before the first frame update
    void Start()
    {
        
        if(this.obj == null){
            Debug.Log("Error, did not specify which obejec to follow!");
        }
         Invoke("followObjext", 2.5f);//wait .2 secs
    }

    public void followObjext(){
        Vector3 position = this.obj.transform.position;
        //transform.position = position;
        new Vector3( position.x +26f, position.y -4f, position.z +7f    );
        //26  -4 7
    }

    // Update is called once per frame
    void Update()
    {
        
       
    }
}
