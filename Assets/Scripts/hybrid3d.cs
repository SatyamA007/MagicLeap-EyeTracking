using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap_EyeTracking {
public class hybrid3d : MonoBehaviour
{
    public GameObject allPoints;
    public GameObject frame;
    public bool doorOpen = false;
    string participantID;
    int idx = 0;
    int[] nextPos = {
                    25,20, 1,24,21,
                    10, 1, 1, 1,22,
                     1, 1, 1, 1, 1,
                     4, 1, 1, 1,16,
                     2, 3, 1, 6, 5};// nextPos is 1+ the idx, subtract when using
    float constTime = 5f;
    Logger.TrialLogger trialLogger;
    int TOTAL_PATHS = 12;
    private MLInput.Controller _controller;
    private bool recording = false; 

        void Start()
    {
        participantID = SceneManager.GetActiveScene().name+"_"+System.DateTime.Now.ToString("MMdd_HHmmss_tt");
        List<string> columnList = new List<string> ();
        
        // initialise trial logger
        trialLogger = GetComponent<Logger.TrialLogger>();
        trialLogger.Initialize(participantID, columnList);
        if(SceneManager.GetActiveScene().name.Equals("w2")){
            constTime = 14;
        }

        transform.position = getPositionNext(idx)[0];
    }

    // Update is called once per frame
    void Update()
    {
        if(doorOpen) {
            doorOpen = false;
            StartCoroutine(MoveToNext(idx));
        }
        if (!recording) {
            //-1 for when recorded data is not of interest
            logEyeTrackingData(99);
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            StartCoroutine(GetComponentInChildren<CountdownController>().CountdownToStart(-3));
            frame.SetActive(!frame.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.F)) {
            frame.SetActive(!frame.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.Z)) {
            QuitGame();
        }
        if (Input.GetKeyDown(KeyCode.X)) {
            trialLogger.gotReset = true;
            QuitGame();
        }
    }

    IEnumerator MoveToNext(int currentPath)
    {
        float timeElapsed = 0;
        Vector3 startPosition = transform.position;
        var step =  0.07f * Time.deltaTime; // 7cms max motion
        
        // record at current position
        while (timeElapsed < 2f)
        {
            recording = true;
            logEyeTrackingData(-1-currentPath);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        timeElapsed=0;
        recording = false;

        while (timeElapsed < constTime)
        {
            recording = true;
            logEyeTrackingData(currentPath+1);
            transform.position = Vector3.Lerp(getPositionNext(currentPath)[0], getPositionNext(currentPath)[1], timeElapsed / constTime);//(float) duration[idx-1]);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        recording = false;
        transform.position = getPositionNext(currentPath)[1];

        idx = nextPos[idx]-1;
        
        if(nextPos[idx]-1!=0){
            transform.position = getPositionNext(idx)[0]; 
            StartCoroutine(GetComponentInChildren<CountdownController>().MoveAfterSeconds(1.5f));
        }
        else
            QuitGame();
    }

    private void logEyeTrackingData(int currentPath)
    {
        // 99 - not interesting, -1 - static, 1 - moving target
        trialLogger.StartTrial(currentPath);
        trialLogger.EndTrial();
    }

    public void QuitGame()
    {
        trialLogger.flushDatatoFile();
        if(welcome.sceneIdx<welcome.sceneOrder.Count)
            SceneManager.LoadSceneAsync(welcome.sceneOrder[welcome.sceneIdx++]);
        else {
            #if UNITY_EDITOR
                // Application.Quit() does not work in the editor so
                // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

    }

    Vector3[] getPositionNext(int i){
        return new Vector3[]{allPoints.transform.GetChild(i).gameObject.transform.position, allPoints.transform.GetChild(nextPos[i]-1).gameObject.transform.position};
    }
}
}