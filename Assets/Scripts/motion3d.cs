using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class motion3d : MonoBehaviour
{
    public GameObject allPoints;
    float[] duration = {1, 0.5f, 1.5f, 0.75f, 1.25f, 0.3f, 1.7f, 2f, 
                        1, 0.5f, 1.5f, 0.75f, 1.25f, 0.3f, 1.7f, 2f,
                        1, 0.5f, 1.5f, 0.75f, 1.25f, 0.3f, 1.7f, 2f};
    public bool doorOpen = false;
    int idx = 0;
    List<int> randomizedPaths = new List<int>();
    // Start is called before the first frame update
    void Start()
    {
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
    }


    Vector3[] getPositionNext(int i){
        if(i<12)
            return new Vector3[]{allPoints.transform.GetChild(i).gameObject.transform.position, allPoints.transform.GetChild(i+12).gameObject.transform.position};
        else
            return new Vector3[]{allPoints.transform.GetChild(i).gameObject.transform.position, allPoints.transform.GetChild(i-12).gameObject.transform.position};
    }
}
