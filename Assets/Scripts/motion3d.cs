using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    List<int> randomizedPaths = new List<int>();
    Logger.TrialLogger trialLogger;
    void Start()
    {
        participantID = System.DateTime.Now.ToString("MMdd_HHmmss_tt");
        List<string> columnList = new List<string> { "gaze_confidence","gaze_x", "gaze_y", "gaze_z", "sphere_x", "sphere_y", "sphere_z" };

        // // initialise trial logger
        trialLogger = GetComponent<Logger.TrialLogger>();
        trialLogger.Initialize(participantID, columnList);

        for(int i=0;i<24;i++)
            randomizedPaths.Add(i);

        for (int i = 0; i < randomizedPaths.Count; i++) {
            int temp = randomizedPaths[i];
            int randomIndex = Random.Range(i, randomizedPaths.Count);
            randomizedPaths[i] = randomizedPaths[randomIndex];
            randomizedPaths[randomIndex] = temp;
        }

        transform.position = getPositionNext(randomizedPaths[idx])[0];
        StartCoroutine(GetComponentInChildren<CountdownController>().CountdownToStart());
    }

    // Update is called once per frame
    void Update()
    {
        if(doorOpen&&idx<24){
            doorOpen = false;
            StartCoroutine(MoveToEnd(randomizedPaths[idx++]));
        }
    }

    IEnumerator MoveToEnd(int nextPath)
    {
        // transform.position = getPositionNext(nextPath)[0]; 
        float timeElapsed = 0;
        Vector3 startPosition = transform.position;
        
        while (timeElapsed < (float)duration[idx-1])
        {
            logEyeTrackingData(nextPath);
            transform.position = Vector3.Lerp(startPosition, getPositionNext(nextPath)[1], timeElapsed / (float) duration[idx-1]);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = getPositionNext(nextPath)[1];
        
        if(idx<24){
            yield return new WaitForSeconds(1f);
            transform.position = getPositionNext(randomizedPaths[idx])[0]; 
            StartCoroutine(GetComponentInChildren<CountdownController>().CountdownToStart());
        }
        else
            QuitGame();
    }

    private void logEyeTrackingData(int nextPath)
    {
        trialLogger.StartTrial();
        trialLogger.trial["PathIDX"] = nextPath.ToString();
        trialLogger.trial["gaze_confidence"] = MLEyes.FixationConfidence.ToString();
        trialLogger.trial["gaze_x"] = MLEyes.FixationPoint.x.ToString();
        trialLogger.trial["gaze_y"] = MLEyes.FixationPoint.y.ToString();
        trialLogger.trial["gaze_z"] = MLEyes.FixationPoint.z.ToString();
        trialLogger.trial["sphere_x"] = transform.position.x.ToString();
        trialLogger.trial["sphere_y"] = transform.position.y.ToString();
        trialLogger.trial["sphere_z"] = transform.position.z.ToString();
        trialLogger.EndTrial();
    }

    public void QuitGame()
    {
        // save any game data here
        #if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    Vector3[] getPositionNext(int i){
        if(i<12)
            return new Vector3[]{allPoints.transform.GetChild(i).gameObject.transform.position, allPoints.transform.GetChild(i+12).gameObject.transform.position};
        else
            return new Vector3[]{allPoints.transform.GetChild(i).gameObject.transform.position, allPoints.transform.GetChild(i-12).gameObject.transform.position};
    }
}
}