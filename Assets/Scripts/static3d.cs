using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.MagicLeap;
using static UnityEngine.XR.MagicLeap.MLHeadTracking;

namespace MagicLeap_EyeTracking {
public class static3d : MonoBehaviour
{
    public GameObject allPoints;
    public bool doorOpen = false;
    string participantID;
    int idx = 0;
    float constTime = 3f;
    List<int> randomizedPositions = new List<int>();
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

        for(int i=0;i<TOTAL_PATHS;i++){
            randomizedPositions.Add(i);
        }

        // Simple randomization
        for (int i = 0; i < randomizedPositions.Count; i++) {
            int temp = randomizedPositions[i];
            int randomIndex = Random.Range(i, randomizedPositions.Count);
            randomizedPositions[i] = randomizedPositions[randomIndex];
            randomizedPositions[randomIndex] = temp;
        }

        transform.position = getPositionNext(randomizedPositions[idx])[0];
    }


    // Update is called once per frame
    void Update()
    {
        if(doorOpen) {
            doorOpen = false;
            StartCoroutine(MoveToNext(randomizedPositions[idx++]));
        }
        if (!recording) {
            //-1 for when recorded data is not of interest
            logEyeTrackingData(99);
        }
        if ( Input.GetKeyDown(KeyCode.Q)&&idx<TOTAL_PATHS) {
            StartCoroutine(GetComponentInChildren<CountdownController>().CountdownToStart(2));
        }
        if ( Input.GetKeyDown(KeyCode.X)) {
            // trialLogger.gotReset = true;
            QuitGame();
        }
    }

    IEnumerator MoveToNext(int currentPath)
    {
        float timeElapsed = 0;
        // Vector3 startPosition = transform.position;
        
        while (timeElapsed < constTime)
        {
            recording = true;
            logEyeTrackingData(currentPath+1);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        recording = false;
        transform.position = getPositionNext(currentPath)[1];
        
        if(idx<TOTAL_PATHS){
            transform.position = getPositionNext(randomizedPositions[idx])[0]; 
            StartCoroutine(GetComponentInChildren<CountdownController>().MoveAfterSeconds(2f));
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
        return new Vector3[]{allPoints.transform.GetChild(i).gameObject.transform.position, allPoints.transform.GetChild(i).gameObject.transform.position};
    }
}
}