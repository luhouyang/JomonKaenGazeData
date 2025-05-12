using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupItems : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> models = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<GameObject> GetModels()
    {
        return models;
    }
}
