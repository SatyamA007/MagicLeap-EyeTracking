using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestoreOriginRotation : MonoBehaviour
{
    public GameObject bodyObj;
    private bool set = false;

    // Start is called before the first frame update
    void Start()
    {        
        
    }

    // Update is called once per frame
    void Update()
    {

        if(!set&&bodyObj.transform.rotation.x!=0){
            Debug.Log(transform.parent.rotation);
            transform.localRotation = Quaternion.Inverse(bodyObj.transform.rotation);
            set = true;
        }
    }
}
