using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeldScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private FeldScript next_Feld;

    private TokenControl token = null;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public FeldScript GetNext()
    {
        return next_Feld;
    }

    public TokenControl GetToken()
    {
        return token;
    }

    public void SetToken(TokenControl token)
    {
        this.token = token;
    }
}
