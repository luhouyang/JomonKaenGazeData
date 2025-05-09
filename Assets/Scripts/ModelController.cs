using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModelController : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> models = new List<GameObject>();

    private int currentModelIndex = 0;
    private Vector3 previousModelPosition = Vector3.zero;
    public static GameObject currentModel;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < models.Count(); i++)
        {
            models[i].SetActive(false);
        }
        LoadModel();
    }

    // Update is called once per frame 
    void Update()
    {
        
    }

    public void StartRecording()
    {
        currentModel.GetComponent<ModelGazeRecorder>().SetIsRecording(true);
    }

    public void StopRecording()
    {
        if (currentModel.GetComponent<ModelGazeRecorder>().isRecording)
        {
            currentModel.GetComponent<ModelGazeRecorder>().SetIsRecording(false);
            currentModel.GetComponent<ModelGazeRecorder>().SaveAllData(); 
        }
    }

    private void LoadModel()
    {
        // Reset previous model position and rotation if there was a previous model
        if (previousModelPosition != Vector3.zero)
        {
            currentModel.transform.parent.SetPositionAndRotation(previousModelPosition, new Quaternion());
            StopRecording();
            currentModel.SetActive(false);
        }

        // Select the next model
        currentModel = models[currentModelIndex];
        currentModel.SetActive(true);
        
        // Record the original transform
        previousModelPosition = currentModel.transform.parent.position;

        // Move the model to the viewing area
        currentModel.transform.parent.position = transform.position;
    }

    public void LoadPrevious() 
    {
        if (currentModelIndex == 0)
        {
            currentModelIndex = models.Count - 1;
        }
        else
        {
            currentModelIndex--;
        }

        LoadModel();

        Debug.Log("Loading " + models[currentModelIndex].name);
    }

    public void LoadNext()
    {
        if (currentModelIndex == models.Count - 1)
        {
            currentModelIndex = 0;
        }
        else
        {
            currentModelIndex++;
        }

        LoadModel();

        Debug.Log("Loading " + models[currentModelIndex].name);
    }
}
