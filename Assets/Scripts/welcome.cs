using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class welcome : MonoBehaviour
{

    public static List<string> sceneOrder = new List<string> {};

    int[,] latinSquareOrder = new int [1,6]{
        // {1,	2,	4,	0,	3,	5},//A
        // {2,	3,	5,	1,	4,	0},//B
        {0,	1,	3,	5,	2,	4},//C
        // {5,	0,	2,	4,	1,	3},//D
        // {4,	5,	1,	3,	0,	2},//E
        // {3,	4,	0,	2,	5,	1} //F
    };
    public static int sceneIdx = 0;

    // Start is called before the first frame update
    void Start()
    {

        string [] temp = {"callibrate","w1", "w2", "w3", "s4", "b5"};
        int r = Random.Range(0, latinSquareOrder.GetLength(0));

        for(int i=0;i<temp.Length;i++){
            sceneOrder.Add(temp[latinSquareOrder[r,i]]);
        }

        SceneManager.LoadSceneAsync(sceneOrder[sceneIdx++]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
