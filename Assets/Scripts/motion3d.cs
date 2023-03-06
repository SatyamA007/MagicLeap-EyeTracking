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
    float[] duration = {1, 0.5f, 1.5f, 0.75f, 1.25f, 0.3f, 1.7f, 2f, 
                        1, 0.5f, 1.5f, 0.75f, 1.25f, 0.3f, 1.7f, 2f,
                        1, 0.5f, 1.5f, 0.75f, 1.25f, 0.3f, 1.7f, 2f};
    float constTime = 3f;
    List<int> randomizedPaths = new List<int>();
    Logger.TrialLogger trialLogger;
    int TOTAL_PATHS = 12;
    private MLInput.Controller _controller;
    private bool gameStarted = false;
    void Start()
    {
        participantID = SceneManager.GetActiveScene().name+"_"+System.DateTime.Now.ToString("MMdd_HHmmss_tt");
        List<string> columnList = new List<string> ();
        
        // initialise trial logger
        trialLogger = GetComponent<Logger.TrialLogger>();
        trialLogger.Initialize(participantID, columnList);

        for(int i=0;i<TOTAL_PATHS;i++){
            randomizedPaths.Add(i);
            duration[i]*=4;
        }

        transform.position = getPositionNext(randomizedPaths[idx])[0];
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W)&&doorOpen&&idx<TOTAL_PATHS){
            doorOpen = false;
            StartCoroutine(MoveToEnd(randomizedPaths[idx++]));
        }
        if ( Input.GetKeyDown(KeyCode.Q)) {
            StartCoroutine(GetComponentInChildren<CountdownController>().CountdownToStart());
            gameStarted = true;
        }
        if ( Input.GetKeyDown(KeyCode.X)) {
            QuitGame();
        }
    }

    IEnumerator MoveToEnd(int nextPath)
    {
        float timeElapsed = 0;
        Vector3 startPosition = transform.position;
        
        while (timeElapsed < constTime)
        {
            logEyeTrackingData(nextPath);
            transform.position = Vector3.Lerp(startPosition, getPositionNext(nextPath)[1], timeElapsed / constTime);//(float) duration[idx-1]);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = getPositionNext(nextPath)[1];
        
        if(idx<TOTAL_PATHS){
            yield return new WaitForSeconds(1f);
            transform.position = getPositionNext(randomizedPaths[idx])[0]; 
            StartCoroutine(GetComponentInChildren<CountdownController>().CountdownToStart());
        }
        else
            QuitGame();
    }

    private void logEyeTrackingData(int nextPath)
    {
        trialLogger.StartTrial(nextPath);
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
        if(i<12)
            return new Vector3[]{allPoints.transform.GetChild(i).gameObject.transform.position, allPoints.transform.GetChild(i+12).gameObject.transform.position};
        else
            return new Vector3[]{allPoints.transform.GetChild(i).gameObject.transform.position, allPoints.transform.GetChild(i-12).gameObject.transform.position};
    }
}
}