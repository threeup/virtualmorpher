using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputCtrl : MonoBehaviour {

    [System.Serializable]
    public struct InputParams 
    {
        public Vector2 leftAxis;
        public bool primaryButton;
        public bool secondaryButton;
        public bool tertiaryButton;
    }
    
    User user;


    [SerializeField]
    InputParams inputParams = new InputParams();
    InputParams lastInputParams = new InputParams();
    
    void Awake()
    {
        user = GetComponent<User>();
    }

    void Update () 
    {
        float deltaTime = Time.deltaTime;
        lastInputParams = inputParams;
        inputParams.leftAxis = Vector2.zero;
        inputParams.primaryButton = false;
        inputParams.secondaryButton = false;
        inputParams.tertiaryButton = false;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            inputParams.leftAxis.y = 1f;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            inputParams.leftAxis.y = -1f;
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            inputParams.leftAxis.x = 1f;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            inputParams.leftAxis.x = -1f;
        }
        if (Input.GetMouseButton(0))
        {
            inputParams.primaryButton = true;
        }
        if (Input.GetMouseButton(1))
        {
            inputParams.secondaryButton = true;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            inputParams.tertiaryButton = true;
        }
        if( user == null || user.ProcessInput == null )
        {
            Debug.Log(user);
            Debug.Log(user.ProcessInput);
            Debug.Log(inputParams);
        }
        else
        {
            user.ProcessInput(deltaTime, inputParams);    
        }
        
	}
}
