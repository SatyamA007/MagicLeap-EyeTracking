using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class welcome : MonoBehaviour
{

    public static List<string> sceneOrder = new List<string> {
        "w1", "w2", "w3", "s4", "b5", "callibrate"
    };
    public static int sceneIdx = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Simple randomization
        for (int i = 0; i < sceneOrder.Count; i++) {
            string temp = sceneOrder[i];
            int randomIndex = Random.Range(i, sceneOrder.Count);
            sceneOrder[i] = sceneOrder[randomIndex];
            sceneOrder[randomIndex] = temp;
        }

        
        SceneManager.LoadSceneAsync(sceneOrder[sceneIdx++]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
