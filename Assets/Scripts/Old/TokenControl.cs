using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TokenControl : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private GameObject basePos;
    [SerializeField] private FeldScript startField;
    [SerializeField] private FeldScript endField;
    [SerializeField] private TokenControl nextToken;
    [SerializeField] private FeldScript finishField;
    [SerializeField]
    private bool isChosen = false;

    private bool isOnRun = false;

    private FeldScript fieldPosition;

    [SerializeField]
    private Material material;

    private int num;

    // Start is called before the first frame update
    void Start()
    {
        initPosition();
        
    }

    float speed = 1.0f;
    float height = 0.15f;
    float pos = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (isChosen)
        {
            transform.position += new Vector3(0.0f, (height / 5.0f) * Mathf.Sin(pos), 0.0f);
            pos += speed / 10.0f;
            _particleSystem.Play();
            if (pos - 6.28f >= 0.001f)
            {
                pos = 0.0f;
            }
        }
        else
        {
            _particleSystem.Pause();
            _particleSystem.Clear();
        }
    }

    public bool einsatz()
    {

        if (this.startField.GetToken() != null)
        {
            if (!schlagen(this.startField.GetToken()))
            {
                return false;
            }
        }
        fieldPosition = startField;
        fieldPosition.SetToken(this);
        this.isOnRun = true;
        initPosition();
        return true;
    }

    public bool ziehen(int n)
    {
        bool val = true;
        FeldScript currFeld = fieldPosition;
        
        if (isOnRun)
        {

            for (int i = 0; i < n; i++)
            {
                if (this.fieldPosition == this.endField)
                {
                    fieldPosition = finishField;
                }
                else
                {
                    if (fieldPosition.GetNext() == null)
                    {

                        fieldPosition = currFeld;
                        return false;
                    }
                    fieldPosition = fieldPosition.GetNext();
                }
            }

            if (fieldPosition.GetToken() != null)
            {
                if (!schlagen(fieldPosition.GetToken()))
                {
                    this.fieldPosition = currFeld;
                    val = false;               }
            }
            currFeld.SetToken(null);
            fieldPosition.SetToken(this);
            
            initPosition();

        }

        return val;
    }
    
    private bool schlagen(TokenControl opp)
    {
        if (opp.GetStartFeld() == this.GetStartFeld())
        {
            return false;
        }
        else {
            opp.GetKicked();
        }
        return true;
    }

    public TokenControl GetNext()
    {
        return nextToken;
    }

    public void SetChosenTrue(){
        this.isChosen = true;
    }

    public void SetChosenFalse()
    {
        initPosition();
        this.isChosen = false;
    }

    private void initPosition()
    {
        this.pos = 0.0f;
        if (fieldPosition != null) {
            Vector3 einPos = fieldPosition.gameObject.GetComponent<Renderer>().transform.position;
            Vector3 pos = new Vector3(einPos.x,
                          einPos.y + 1.0f,
                          einPos.z);

            this.transform.position = pos;
        }

        if (isOnRun == false)
        {
            Vector3 einPos = basePos.gameObject.GetComponent<Renderer>().transform.position;
            Vector3 pos = new Vector3(einPos.x,
                          einPos.y + 1.0f,
                          einPos.z);

            this.transform.position = pos;
        }
    }

    public FeldScript GetStartFeld()
    {
        return this.startField;
    }

    public void GetKicked()
    {
        this.isOnRun = false;
        this.fieldPosition = null;
        initPosition();
    }

    public bool GetIsOnRun()
    {
        return this.isOnRun;
    }

    public bool isAbleToMove(int steps)
    {   
        //Wenn das Token nicht auf dem Feld ist, und die gewürfelte Zahl keine 6 ist, ist der Zug nicht Valide
        if (!this.isOnRun && steps != 6)
        {
            return false;
        }
        //Wenn das Token auf Spielfeld ist und es keinen validen Zug gibt
        if (isOnRun)
        {
             
            FeldScript currFeld = fieldPosition;
            for (int i = 0; i < steps; i++)
            {
                if (currFeld == this.endField)
                {
                    currFeld = finishField;
                }
                else
                {
                    if (currFeld.GetNext() == null)
                    {
                        return false;
                    }
                    currFeld = currFeld.GetNext();
                }
            }

            if (currFeld.GetToken() != null)
            {
                if(currFeld.GetToken().GetStartFeld() == this.startField)
                {
                    return false;
                }
            }
        }

        return true;
    }



}
