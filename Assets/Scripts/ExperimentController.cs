using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentController : MonoBehaviour
{
    [SerializeField]
    private int experimentNo = 1;

    // Start is called before the first frame update
    void Start()
    {
        if (experimentNo == 1)
        {
            List<GameObject> exp2Groups = GetComponent<ExpModelController>().GetGroups();
            for (int i = 0; i < exp2Groups.Count; i++)
            {
                List<GameObject> models = exp2Groups[i].GetComponent<GroupItems>().GetModels();
                for (int j = 0; j < models.Count; j++)
                {
                    models[j].GetComponent<ExpModelGazeRecorder>().enabled = false;
                }
            }
        }
        else if (experimentNo == 2) 
        {
            List<GameObject> exp1Groups = GetComponent<ModelController>().GetGroups();
            for (int i = 0; i < exp1Groups.Count; i++)
            {
                List<GameObject> models = exp1Groups[i].GetComponent<GroupItems>().GetModels();
                for (int j = 0; j < models.Count; j++)
                {
                    models[j].GetComponent<ModelGazeRecorder>().enabled = false;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
