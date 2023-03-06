using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    int TOTAL_PATHS = 25;
    private MLInput.Controller _controller;
    private bool gameStarted = false;

        void Start()
    {
        participantID = System.DateTime.Now.ToString("MMdd_HHmmss_tt");
        List<string> columnList = new List<string>();
        
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
        if(doorOpen&&idx<TOTAL_PATHS){
            doorOpen = false;
            StartCoroutine(MoveToEnd(randomizedPositions[idx++]));
        }
        if ( Input.GetKeyDown(KeyCode.Q)) {
            StartCoroutine(GetComponentInChildren<CountdownController>().CountdownToStart(1));
            gameStarted = true;
        }
    }

    IEnumerator MoveToEnd(int nextPath)
    {
        float timeElapsed = 0;
        Vector3 startPosition = transform.position;
        
        while (timeElapsed < constTime)
        {
            if(timeElapsed>1f)
                logEyeTrackingData(nextPath);
            transform.position = getPositionNext(nextPath)[1];//Vector3.Lerp(startPosition, getPositionNext(nextPath)[1], timeElapsed / constTime);//(float) duration[idx-1]);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = getPositionNext(nextPath)[1];
        
        if(idx<TOTAL_PATHS){
            yield return new WaitForSeconds(1f);
            transform.position = getPositionNext(randomizedPositions[idx])[0]; 
            StartCoroutine(GetComponentInChildren<CountdownController>().CountdownToStart(1));
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
    }

    Vector3[] getPositionNext(int i){
        return new Vector3[]{allPoints.transform.GetChild(i).gameObject.transform.position, allPoints.transform.GetChild(i).gameObject.transform.position};
    }
}
}