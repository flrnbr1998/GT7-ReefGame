using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{

    [SerializeField]
    private Finish nextField;

    private TokenControl token = null; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetToken(TokenControl token)
    {
        this.token = token;
    }

    public void RemoveToken()
    {
        this.token = null;
    }

    public Finish GetNext() { 
        return nextField;
    }
}
