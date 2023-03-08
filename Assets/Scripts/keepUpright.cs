using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keepUpright : MonoBehaviour
{
    private bool set = false;
    public GameObject camera;
    // Start is called before the first frame update
    void Update()
    {        
        makeUpright();
    }


    void makeUpright(){
        Quaternion q = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
        // Quaternion q2 = Quaternion.FromToRotation(transform.forward, camera.transform.forward) * q;
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 5);

    }
}
