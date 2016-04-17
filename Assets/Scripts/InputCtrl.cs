using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputCtrl : MonoBehaviour {

    [System.Serializable]
    public struct InputParams 
    {
        public Vector2 leftAxis;
        public bool[] button;
        public bool[] buttonDown;
        public bool[] buttonUp;
        
        public InputParams(Vector2 axis)
        {
            leftAxis = axis;
            button = new bool[5];
            buttonDown = new bool[5];
            buttonUp = new bool[5];
        }
        
        
        public void Reset()
        {
            leftAxis = Vector2.zero;
            for(int i=0; i<5;++i)
            {
                button[i] = false;
                buttonDown[i] = false;
                buttonUp[i] = false;
            }
        }
        
        public void Clone(InputParams other)
        {
            leftAxis = other.leftAxis;
            for(int i=0; i<5;++i)
            {
                button[i] = other.button[i];
                buttonDown[i] = other.buttonDown[i];
                buttonUp[i] = other.buttonUp[i];
            }
        }
    }
    
    User user;


    InputParams inputParams = new InputParams(Vector2.zero);
    InputParams lastInputParams = new InputParams(Vector2.zero);
    
    void Awake()
    {
        user = GetComponent<User>();
    }

    void Update () 
    {
        float deltaTime = Time.deltaTime;
        lastInputParams.Clone(inputParams);
        inputParams.Reset();
        /*if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
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
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            inputParams.leftAxis.y = 1f;
        }*/
        if (Input.GetMouseButton(0))
        {
            inputParams.button[0] = true;
        }
        if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.Space))
        {
            inputParams.button[1] = true;
        }
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Q))
        {
            inputParams.button[2] = true;
        }
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.W))
        {
            inputParams.button[3] = true;
        }
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.E))
        {
            inputParams.button[4] = true;
        }
        for(int i=0; i<5; ++i)
        {
            if( !lastInputParams.button[i] && inputParams.button[i] )
            {
                inputParams.buttonDown[i] = true;
            }
            if( lastInputParams.button[i] && !inputParams.button[i] )
            {
                inputParams.buttonUp[i] = true;
            }
        }
        
         user.ProcessInput(deltaTime, inputParams);   
        
	}
}
