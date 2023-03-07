using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MagicLeap_EyeTracking{
public class CountdownController : MonoBehaviour
{
    public Text countdownDisplay;
    public GameObject clickToContinue;
    int countdownTime = 3;
    
    private void Start()
    {
        countdownDisplay.gameObject.SetActive(false);
    }

    public IEnumerator MoveAfterSeconds(float s=1f){
        yield return new WaitForSeconds(s);

        if(s==2f){
            GetComponentInChildren<static3d>().doorOpen = true;
        }
        else
            GetComponentInChildren<motion3d>().doorOpen = true;
    }
    public IEnumerator CountdownToStart(int count = 3)
    {
        countdownTime = count;
        countdownDisplay.gameObject.SetActive(true);
        clickToContinue.gameObject.SetActive(false);

        while(countdownTime > 0)
        {
            countdownDisplay.text = countdownTime.ToString();
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }

        countdownDisplay.text = "GO!";
		countdownTime = 3;
		/* Call the code to "begin" your game here.
		 * For example, mine allows the player to start
		 * moving and starts the in game timer.
         */
        
        yield return new WaitForSeconds(1f);
        countdownDisplay.gameObject.SetActive(false);
        
        if(count==2){
            GetComponentInChildren<static3d>().doorOpen = true;
        }
        else
            GetComponentInChildren<motion3d>().doorOpen = true;
    }
}
}