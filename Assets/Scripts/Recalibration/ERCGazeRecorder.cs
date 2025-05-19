using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SampleGazeData;
using Microsoft.MixedReality.Toolkit.Utilities;
using TMPro;
using static UnityEngine.Random;

public class ERCGazeRecorder : MonoBehaviour
{
    [System.Serializable]
    public class GazeData
    {
        public double timestamp;
        public Vector3 headPosition;
        public Vector3 headForward;
        public Vector3 eyeOrigin;
        public Vector3 eyeDirection;
        public Vector3 hitPosition;
        public string targetName;
        public Vector3 localHitPosition;
    }

    [System.Serializable]
    public class SessionData
    {
        public string selectedObjectName;
        public List<GazeData> gazeData = new List<GazeData>();
    }

    [SerializeField]
    private int numTarget = 80;

    [SerializeField]
    private List<GameObject> targetList = new List<GameObject>();

    [SerializeField]
    private GameObject currentModel;

    private string sessionPath;
    private int numTargetAppeared;
    private double timeInterval;
    private GameObject currentTarget;

    public bool isRecording;
    public SessionData currentSession = new SessionData();

    private string saveDir;
    private double startingTime;
    private StringBuilder gaze_csv = new StringBuilder();
    private Renderer targetRenderer;
    private Bounds localBounds;
    private StringBuilder pc_sb = new StringBuilder();

    void Start()
    {
        // deactivate all points
        for (int i = 0; i < targetList.Count; i++)
        {
            targetList[i].SetActive(false);
        }
    }

    void Update()
    {
        if (!isRecording || currentModel == null) return;

        var eyeTarget = EyeTrackingTarget.LookedAtEyeTarget;
        var gazedObject = eyeTarget != null ? eyeTarget.gameObject : null;

        RecordGazeData(gazedObject);

        if (timeInterval < 0)
        {
            if (numTargetAppeared == numTarget)
            {
                currentTarget.SetActive(false);
                SetIsRecording(false);
                SaveAllData();
            } else
            {
                timeInterval = Range(100, 151) / 100.0;
                currentTarget.SetActive(false);
                currentTarget = targetList[Range(1, targetList.Count)];
                currentTarget.SetActive(true);
                numTargetAppeared++;
            }
        } else
        {
            timeInterval -= Time.deltaTime;
        }
    }

    public void SetIsRecording(bool val)
    {
        isRecording = val;
        startingTime = Time.unscaledTimeAsDouble;

        if (val && currentModel != null)
        {
            sessionPath = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + "_recalibration";
            numTargetAppeared = 1;
            timeInterval = Range(100, 151) / 100.0;

            currentSession.selectedObjectName = currentModel.name;
            saveDir = Path.Combine(Application.persistentDataPath, sessionPath, currentSession.selectedObjectName);

            gaze_csv = new StringBuilder();
            gaze_csv.AppendLine("Timestamp,HeadX,HeadY,HeadZ,HeadFwdX,HeadFwdY,HeadFwdZ,EyeOriginX,EyeOriginY,EyeOriginZ,EyeDirX,EyeDirY,EyeDirZ,HitX,HitY,HitZ,TargetName");

            targetRenderer = currentModel.GetComponent<Renderer>();
            localBounds = targetRenderer.localBounds;
            pc_sb = new StringBuilder();
            pc_sb.AppendLine("x,y,z,timestamp");

            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }

            currentTarget = targetList[Range(1, targetList.Count)];
            currentTarget.SetActive(true);
        }
    }



    private void RecordGazeData(GameObject target)
    {
        var eyeProvider = CoreServices.InputSystem?.EyeGazeProvider;
        if (eyeProvider == null) return;

        var gaze = new GazeData
        {
            timestamp = Time.unscaledTimeAsDouble - startingTime,
            headPosition = CameraCache.Main.transform.position,
            headForward = CameraCache.Main.transform.forward,
            eyeOrigin = eyeProvider.GazeOrigin,
            eyeDirection = eyeProvider.GazeDirection,
            hitPosition = eyeProvider.IsEyeTrackingEnabledAndValid ? eyeProvider.HitPosition : Vector3.zero,
            targetName = target != null ? target.name : "null"
        };

        if (target != null && target.name == currentSession.selectedObjectName)
        {
            gaze.localHitPosition = target.transform.InverseTransformPoint(gaze.hitPosition);
            Vector3 pos = gaze.localHitPosition;
            if (localBounds.Contains(pos) && gaze.targetName == target.name && gaze.targetName != "null")
            {
                pos = UnapplyUnityTransforms(pos, target.transform.eulerAngles);
                pc_sb.AppendLine($"{pos.x:F6},{pos.y:F6},{pos.z:F6},{gaze.timestamp:F6}");
            }
        }
        else
        {
            gaze.localHitPosition = Vector3.zero;
        }

        currentSession.gazeData.Add(gaze);

        gaze_csv.AppendLine($"{gaze.timestamp:F6}," +
                           $"{gaze.headPosition.x:F4},{gaze.headPosition.y:F4},{gaze.headPosition.z:F4}," +
                           $"{gaze.headForward.x:F4},{gaze.headForward.y:F4},{gaze.headForward.z:F4}," +
                           $"{gaze.eyeOrigin.x:F4},{gaze.eyeOrigin.y:F4},{gaze.eyeOrigin.z:F4}," +
                           $"{gaze.eyeDirection.x:F4},{gaze.eyeDirection.y:F4},{gaze.eyeDirection.z:F4}," +
                           $"{gaze.hitPosition.x:F4},{gaze.hitPosition.y:F4},{gaze.hitPosition.z:F4}," +
                           $"{gaze.targetName}");
    }

    private Vector3 UnapplyUnityTransforms(Vector3 originalVector, Vector3 anglesInDegrees)
    {
        Quaternion xRotation = Quaternion.AngleAxis(anglesInDegrees.x, Vector3.right);
        Quaternion yRotation = Quaternion.AngleAxis(anglesInDegrees.y, Vector3.up);
        Quaternion zRotation = Quaternion.AngleAxis(anglesInDegrees.z, Vector3.forward);

        Vector3 rotatedVector = xRotation * originalVector;
        rotatedVector = yRotation * rotatedVector;
        rotatedVector = zRotation * rotatedVector;

        return new Vector3(-rotatedVector.x, rotatedVector.y, rotatedVector.z);
    }

    public void ResetAll()
    {
        if (isRecording)
        {
            SetIsRecording(false);
        }
        currentSession = new SessionData();
        StopAllCoroutines(); // Ensure any ongoing audio recording coroutines are stopped
    }

    public void SaveAllData()
    {
        SaveSession("session.json");
        SaveGazeData("gaze_data.csv");
        ExportPointCloud(currentModel);
        Debug.Log("SAVED DATA AT: " + saveDir);
    }

    public void SaveSession(string fileName)
    {
        string json = JsonUtility.ToJson(currentSession);
        File.WriteAllText(Path.Combine(saveDir, fileName), json);
    }

    public void SaveGazeData(string fileName)
    {
        File.WriteAllText(Path.Combine(saveDir, fileName), gaze_csv.ToString());
    }

    public void ExportPointCloud(GameObject target)
    {
        File.WriteAllText(Path.Combine(saveDir, "pointcloud.csv"), pc_sb.ToString());
    }
}