using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MagicLeap_EyeTracking{
public class CountdownController : MonoBehaviour
{
    public Text countdownDisplay;
    int countdownTime = 3;
    
    private void Start()
    {
        
    }

    public IEnumerator CountdownToStart()
    {
       
        countdownDisplay.gameObject.SetActive(true);

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
        GetComponentInChildren<motion3d>().doorOpen = true;
    }
}
}