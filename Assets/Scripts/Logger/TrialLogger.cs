using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.XR.MagicLeap;
using UnityEngine.SceneManagement;

namespace MagicLeap_EyeTracking.Logger
{
    public class TrialLogger : MonoBehaviour {

        public Camera camera;
        public bool gotReset = false;
        public int currentTrialNumber = 0;    
        private GameObject target;    
        List<string> header;
        [HideInInspector]
        public Dictionary<string, string> trial;
        [HideInInspector]
        public string outputFolder;
        bool trialStarted = false;
        string ppid;
        string dataOutputPath;
        List<string> output;
        bool isPaused;
        
        // Use this for initialization
        void Awake () {
            outputFolder = Path.Combine( Application.persistentDataPath,"output");
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
        }
        

        // Update is called once per frame
        void Update () {
            if (Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.P))
            {
                isPaused = !isPaused;
                Time.timeScale = isPaused ? 0 : 1;
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                gotReset = true;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                
            }
        }

        public void Initialize(string participantID, List<string> customHeader)
        {
            ppid = participantID;
            header = customHeader;
            InitHeader();
            InitDict();
            output = new List<string>();
            output.Add(string.Join(",", header.ToArray()));
            dataOutputPath = outputFolder + "/" + participantID + ".csv";
        }

        private void InitHeader()
        {
            header.Insert(0, "number");
            header.Insert(1, "PathIDX");
            header.Insert(2, "timestamp");
            header.Insert(3, "end_time");
            header.Insert(4, "head_pos");
            header.Insert(5, "head_euler");
            header.Insert(6, "gaze_confidence");
            header.Insert(7, "gaze_pos");
            header.Insert(8, "target_pos");
            header.Insert(9, "gaze_vector");
            header.Insert(10, "target_vector");
            header.Insert(11, "gaze_vis_x");
            header.Insert(12, "gaze_vis_y");
            header.Insert(13, "target_vis_x");
            header.Insert(14, "target_vis_y");
            header.Insert(15, "local_x_axis");
            header.Insert(16, "local_y_axis");
            header.Insert(17, "local_z_axis");
            header.Insert(18, "left_right_eye_center");
            header.Insert(19, "left_right_eye_center_confidence");
            header.Insert(20, "left_right_eye_gaze");
            header.Insert(21, "left_right_eye_forward_gaze");
            header.Insert(22, "left_right_eye_is_blinking");
            header.Insert(23, "calibration_status");
        }

        private void InitDict()
        {
            trial = new Dictionary<string, string>();
            foreach (string value in header)
            {
                trial.Add(value, "");
            }
        }
        public void StartTrial(int nextPath)
        {
            trialStarted = true;
            currentTrialNumber += 1;
            InitDict();
            trial["number"] = currentTrialNumber.ToString();
            trial["PathIDX"] = nextPath.ToString();
            trial["timestamp"] = System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.ffff tt").Replace(',', '_');
            trial["head_pos"] = camera.transform.position.ToString("f7").Replace(',', '_');
            trial["head_euler"] = camera.transform.eulerAngles.ToString().Replace(',', '_');
            trial["gaze_confidence"] = MLEyes.FixationConfidence.ToString();
            trial["gaze_pos"] = MLEyes.FixationPoint.ToString("f7").Replace(',', '_');
            trial["target_pos"] = transform.position.ToString("f7").Replace(',', '_');
            visualAngles();
            trial["left_right_eye_center"] = MLEyes.LeftEye.Center.ToString("f7").Replace(',', '_')+":"+MLEyes.RightEye.Center.ToString("f7").Replace(',', '_');
            trial["left_right_eye_center_confidence"] = MLEyes.LeftEye.CenterConfidence.ToString()+":"+MLEyes.RightEye.CenterConfidence.ToString();
            trial["left_right_eye_gaze"] = MLEyes.LeftEye.Gaze.ToString("f7").Replace(',', '_')+":"+MLEyes.RightEye.Gaze.ToString("f7").Replace(',', '_');
            trial["left_right_eye_forward_gaze"] = MLEyes.LeftEye.ForwardGaze.ToString("f7").Replace(',', '_')+":"+MLEyes.RightEye.ForwardGaze.ToString("f7").Replace(',', '_');
            trial["left_right_eye_is_blinking"] = MLEyes.LeftEye.IsBlinking.ToString()+":"+MLEyes.RightEye.IsBlinking.ToString();
            trial["calibration_status"] = MLEyes.CalibrationStatus.ToString();
        }

        private void visualAngles(){
            
            Transform t = camera.transform; 
            Vector3 gazeVector = t.InverseTransformPoint(MLEyes.FixationPoint);
            Vector3 targetVector = t.InverseTransformPoint(transform.position);

            trial["gaze_vector"] = gazeVector.ToString("f7").Replace(',', '_');
            trial["target_vector"] = targetVector.ToString("f7").Replace(',', '_');

            //Noting the visual angles for the gaze
            trial["gaze_vis_x"] = (Mathf.Rad2Deg*Mathf.Atan(gazeVector.x/gazeVector.z)).ToString();
            trial["gaze_vis_y"] = (Mathf.Rad2Deg*Mathf.Atan(gazeVector.y/gazeVector.z)).ToString();

            //Noting the visual angles for the target           
            trial["target_vis_x"] = (Mathf.Rad2Deg*Mathf.Atan(targetVector.x/targetVector.z)).ToString();
            trial["target_vis_y"] = (Mathf.Rad2Deg*Mathf.Atan(targetVector.y/targetVector.z)).ToString();
            
            trial["local_x_axis"] = t.TransformVector(Vector3.right).ToString("f7").Replace(',', '_');
            trial["local_y_axis"] = t.TransformVector(Vector3.up).ToString("f7").Replace(',', '_');
            trial["local_z_axis"] = t.TransformVector(Vector3.forward).ToString("f7").Replace(',', '_');
        }
        public void EndTrial()
        {
            if (output != null && dataOutputPath != null)
            {
                if (trialStarted)
                {
                    trial["end_time"] = Time.time.ToString();
                    output.Add(FormatTrialData());
                    trialStarted = false;
                }
                else Debug.LogError("Error ending trial - Trial wasn't started properly");

            }
            else Debug.LogError("Error ending trial - TrialLogger was not initialsed properly");
        

        }

        private string FormatTrialData()
        {
            List<string> rowData = new List<string>();
            foreach (string value in header)
            {
                rowData.Add(trial[value]);
            }
            return string.Join(",", rowData.ToArray());
        }

        private void OnApplicationQuit()
        {
            flushDatatoFile();
        }

        public void flushDatatoFile()
        {
            if(gotReset) {
                Debug.LogError("Reset pressed, skipping file save");
                return;
            }

            if (output != null && dataOutputPath != null)
            {
                File.WriteAllLines(dataOutputPath, output.ToArray());
                Debug.Log(string.Format("Saved data to {0}.", dataOutputPath));
            }
            else Debug.LogError("Error saving data - TrialLogger was not initialsed properly");
        }
    }
}