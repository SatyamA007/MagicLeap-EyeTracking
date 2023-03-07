using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap_EyeTracking {
public class motion3d : MonoBehaviour
{
    public GameObject allPoints;
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
    private bool gameStarted = false;
    private bool recording = false;

        void Start()
    {
        participantID = SceneManager.GetActiveScene().name+"_"+System.DateTime.Now.ToString("MMdd_HHmmss_tt");
        List<string> columnList = new List<string> ();
        
        // initialise trial logger
        trialLogger = GetComponent<Logger.TrialLogger>();
        trialLogger.Initialize(participantID, columnList);


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
            logEyeTrackingData(-1);
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            StartCoroutine(GetComponentInChildren<CountdownController>().CountdownToStart());
            gameStarted = true;
        }
        if (Input.GetKeyDown(KeyCode.X)) {
            // trialLogger.gotReset = true;
            QuitGame();
        }
    }

    IEnumerator MoveToNext(int currentPath)
    {
        float timeElapsed = 0;
        Vector3 startPosition = transform.position;
        
        while (timeElapsed < constTime)
        {
            recording = true;
            logEyeTrackingData(currentPath);
            transform.position = Vector3.Lerp(startPosition, getPositionNext(currentPath)[1], timeElapsed / constTime);//(float) duration[idx-1]);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        recording = false;
        transform.position = getPositionNext(currentPath)[1];
        idx = nextPos[idx]-1;
        
        if(nextPos[idx]-1!=0){
            transform.position = getPositionNext(idx)[0]; 
            StartCoroutine(GetComponentInChildren<CountdownController>().MoveAfterSeconds(0.1f));
        }
        else
            QuitGame();
    }

    private void logEyeTrackingData(int currentPath)
    {
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