using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using TMPro;
using System.IO;
using System;

//Data being recorded in each frame
[System.Serializable]
public class ScreenFrames
{
    public long timestamp;
    public Vector3 upperFramePos;
    public Quaternion upperFrameRot;
    public Vector3 upperFrameRotEuler;
    public Vector3 lowerFramePos;
    public Quaternion lowerFrameRot;
    public Vector3 lowerFrameRotEuler;
    public Vector3 leftFramePos;
    public Quaternion leftFrameRot;
    public Vector3 leftFrameRotEuler;
    public Vector3 rightFramePos;
    public Quaternion rightFrameRot;
    public Vector3 rightFrameRotEuler;
    public float horizontalDistance;
    public float verticalDistance;
}

[System.Serializable]
public class WriteList
{
    public List<ScreenFrames> writeframes;
}
public class ScreenSize : MonoBehaviour
{

    public GameObject[] frames;
    public TMP_Text resultText;
    public TMP_Text stateText;
    public int current;
    public bool record;
    public bool horizontal;
    public bool vertical;
    public WriteList recordings;
    private string[] names = { "top", "right", "bottom", "left" };
    private long startTime;
    MLInput.Controller _controller;

    // Start is called before the first frame update
    void Start()
    {
        // MLInput.Start();
        _controller = MLInput.GetController(MLInput.Hand.Left);
        MLInput.OnControllerButtonDown += OnButtonDown;
        
        resultText.GetComponent<TextMeshProUGUI>().text = "";
        startTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
        
        //Initialize class to write data
        recordings = new WriteList();
        recordings.writeframes = new List<ScreenFrames>();

        record = false;
        vertical = true;
        horizontal = false;
        current = 0;
    }
    void OnButtonDown(byte controllerId, MLInput.Controller.Button button) {
        //Switch between recording and not recording
        if (button == MLInput.Controller.Button.Bumper) {
            record = !record;
            //If end of recording, save data to file
            if(!record)
            {
                print(recordings.writeframes.Count);
                string jsonData = JsonUtility.ToJson(recordings);
                File.WriteAllText(Path.Combine(Application.persistentDataPath, DateTime.Now.ToString("MMdd_HHmmss_tt") + ".json"), jsonData);
            }
        }
    }

    void updateTouchpadInput() 
    {
        string gestureType = _controller.CurrentTouchpadGesture.Type.ToString();
        string gestureState = _controller.TouchpadGestureState.ToString();
        string gestureDirection = _controller.CurrentTouchpadGesture.Direction.ToString();

        stateText.text = "Type: " + gestureType+" |State: " + gestureState+" |Direction: " + gestureDirection+" |"+record.ToString();

        float x = _controller.Touch1PosAndForce.x;
        float y = _controller.Touch1PosAndForce.y;
        float force = _controller.Touch1PosAndForce.z;

        if(force>0){

            //Select top frame to adjust        
            if(y>0.5&&Math.Abs(x)<0.5){
                current = 0;
                horizontal = false;
                vertical = true;
            }
            //Select right frame to adjust
            if(x>0.5&&Math.Abs(y)<0.5){
                current = 1;
                horizontal = true;
                vertical = false;
            }
            //Select lower frame to adjust
            if(y<-0.5&&Math.Abs(x)<0.5){
                current = 2;
                horizontal = false;
                vertical = true;
            }
            //Select left frame to adjust
            if(x<-0.5&&Math.Abs(y)<0.5){
                current = 3;
                horizontal = true;
                vertical = false;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        updateTouchpadInput();
        bool force = _controller.Touch1PosAndForce.z>0;
        bool directionReverse = (_controller.CurrentTouchpadGesture.Type.ToString().Contains("Force")||_controller.CurrentTouchpadGesture.Type.ToString().Contains("Radial"))&&force;
        bool directionFrwrd = (_controller.CurrentTouchpadGesture.Type.ToString().Contains("Tap")||_controller.CurrentTouchpadGesture.Type.ToString().Contains("Swipe"))&&force;
        
        if(current==2||current==3){
            bool temp = directionFrwrd;
            directionFrwrd = directionReverse;
            directionReverse = temp;
        }

        //Move frame down by 0.1cm
        if (directionReverse && vertical)
        {
            frames[current].transform.position = frames[current].transform.position - new Vector3(0.0f, 0.001f, 0.0f);
        }
        //Move frame up by 0.1cm
        if (directionFrwrd && vertical)
        {
            frames[current].transform.position = frames[current].transform.position + new Vector3(0.0f, 0.001f, 0.0f);
        }
        //Move frame left by 0.1cm
        if (directionReverse && horizontal)
        {
            frames[current].transform.position = frames[current].transform.position - new Vector3(0.001f, 0.0f, 0.0f);
        }
        //Move frame right by 0.1cm
        if (directionFrwrd && horizontal)
        {
            frames[current].transform.position = frames[current].transform.position + new Vector3(0.001f, 0.0f, 0.0f);
        }
        
        if (record)
        {
            //Save all data for each frame
            ScreenFrames temp = new ScreenFrames();
            temp.timestamp = System.DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
            temp.upperFramePos = frames[0].transform.position;
            temp.upperFrameRot = frames[0].transform.rotation;
            temp.upperFrameRotEuler = frames[0].transform.eulerAngles;
            temp.lowerFramePos = frames[2].transform.position;
            temp.lowerFrameRot = frames[2].transform.rotation;
            temp.lowerFrameRotEuler = frames[2].transform.eulerAngles;
            temp.leftFramePos = frames[3].transform.position;
            temp.leftFrameRot = frames[3].transform.rotation;
            temp.leftFrameRotEuler = frames[3].transform.eulerAngles;
            temp.rightFramePos = frames[1].transform.position;
            temp.rightFrameRot = frames[1].transform.rotation;
            temp.rightFrameRotEuler = frames[1].transform.eulerAngles;
            temp.verticalDistance = Vector3.Distance(frames[0].transform.position, frames[2].transform.position);
            temp.horizontalDistance = Vector3.Distance(frames[1].transform.position, frames[3].transform.position);

            recordings.writeframes.Add(temp);
        }
        
        //Write info to screen
        resultText.GetComponent<TextMeshProUGUI>().text = "selected frame: " + names[current] + "\ndimensions :" + (Vector3.Distance(frames[1].transform.position, frames[3].transform.position) * 100.0f).ToString("F2") + " x " + (Vector3.Distance(frames[0].transform.position, frames[2].transform.position) * 100.0f).ToString("F2") + "\nrecording: " + record.ToString();
    }
}
