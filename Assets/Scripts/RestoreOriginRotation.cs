using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestoreOriginRotation : MonoBehaviour
{
    public GameObject bodyObj;
    private Quaternion lastParentRotation;
    private Quaternion lastBodyRot;
    private bool set = true;

    // Start is called before the first frame update
    void Start()
    {
        lastParentRotation = transform.parent.localRotation;
        lastBodyRot = bodyObj.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Inverse(transform.parent.localRotation)*
            lastParentRotation*
            bodyObj.transform.localRotation*
            Quaternion.Inverse(lastBodyRot)*
            transform.localRotation;

            
        lastBodyRot = bodyObj.transform.localRotation;
        lastParentRotation = transform.parent.localRotation;
        if(set){
            //before first frame bodyRot is always 0,0,0
            transform.localRotation *= Quaternion.Inverse(lastBodyRot);
            set = false;
        }
            
    }
}
