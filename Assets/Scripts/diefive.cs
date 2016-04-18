using UnityEngine;
using System.Collections;

public class diefive : MonoBehaviour {

	float dieTimer = 2f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		dieTimer -= Time.deltaTime;
		if( dieTimer < 0f)
		{
			Destroy(this.gameObject);
		}
	}
}
