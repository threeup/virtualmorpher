using UnityEngine;
using System.Collections;

public class Gyro : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		
		this.transform.rotation = Quaternion.Euler(0, this.transform.eulerAngles.y, 0);
	}
}
