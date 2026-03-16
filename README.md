# Magic Leap Eye Tracking Test Suite (EyeTTS)
Eye tracking is an increasingly popular method for interacting with augmented reality (AR) and measuring user attention. 
Evaluating eye tracking across multiple platforms and use cases is challenging for the lack of standardized metrics.

We developed EyeTTS, an eye tracking test suite specifically designed for scenarios involving head movement and locomotion in AR. 

Please find the repositories for our eye tracking test suites for respective headsets below. Though we created separate projects for these devices, primarily due the different SDKs for each device. The same test suite has been implemented in these three projects.  

- Magic Leap 2 - https://github.com/SatyamA007/MagicLeap-EyeTracking
- Meta Quest Pro - https://github.com/sydneylim/QuestPro_EyeTracking
- Hololens 2 - https://github.com/vivianross06/HoloLens-Eye-Tracking

## Overview
We conducted a user study on Magic Leap using EyeTTS to elucidate differences in eye tracking performance in AR due to device types, calibration methods, and stimulus movement. 

We outline the tasks presented in the two studies below:

<br/><b>R</b> Users rest their head on a chin rest and watch a tracking stimulus on the screen, which shifts to random positions after a brief delay. 
<p align="center"><img src="ReadMe/r.png" alt="R" width="250"></p> 
<br/><br/><b>HC</b> Users rest their head on a chin rest and watch a tracking stimulus, which moves along a path on the screen inside their field of view (FOV).
<p align="center"><img src="ReadMe/hc.png" alt="HC" width="250"></p>
<br/><br/><b>BC</b> Users sit and turn their heads to watch a tracking stimulus, which moves along a path in the world FOR and wider than their FOV.
<p align="center"><img src="ReadMe/bc.png" alt="BC" width="250"></p>
<br/><br/><b>SSW</b> Users walk in circles around a table while watching a tracking stimulus, which moves along a path on the screen inside their FOV.
<p align="center"><img src="ReadMe/ssw.png" alt="SSW" width="250"></p>
<br/><br/><b>WSW</b> Users walk in circles around a table while watching a tracking stimulus, which moves along a path in the world FOR above the table.
<p align="center"><img src="ReadMe/wsw.png" alt="WSW" width="250"></p>
<br/><br/><b>BSW</b> Users walk in circles around a table while watching a tracking stimulus, which moves along a path constrained to a controller strapped to their body.
<p align="center"><img src="ReadMe/bsw.png" alt="BSW" width="250"></p>
<br/><br/><b>H</b> Users walk down a hallway while watching a tracking stimulus, which moves down the hallway in front of them.
<p align="center"><img src="ReadMe/h.png" alt="H" width="250"></p>


## Development
There are two main scripts in the project: <b>welcome.cs</b> and <b>Logger/TrialLogger.cs</b>.

[**welcome.cs**](https://github.com/SatyamA007/MagicLeap-EyeTracking/blob/main/Assets/Scripts/welcome.cs) is responsible for the flow of the experiment, including the order of scenes and trials. It contains the logic for randomizing the order of scenes across participants using a latin square design.

[**Logger/TrialLogger.cs**](https://github.com/SatyamA007/MagicLeap-EyeTracking/blob/main/Assets/Scripts/Logger/TrialLogger.cs) is responsible for logging the eye tracking data and other relevant information during the experiment. It creates a CSV file for each participant and writes the data to it in real time. 
 - Moreover, it provides controls to fine-tune scene elements such as the size and position of the stimulus, and scene controls to reset the scene. 
 - These controls are useful for debugging and testing the scenes during development.

## Compatibility
This project was developed and tested on the following software and hardware:
- Magic Leap 2
- Unity Editor 2020.3
- Magic Leap Unity SDK 0.26.0