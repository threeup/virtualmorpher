using UnityEngine;
using System.Collections;

public class Follower : MonoBehaviour {

	Vector3 offset;
	public Transform target;
	// Use this for initialization
	void Start () {
		offset = this.transform.position;
	}
	
	// Update is called once per frame
	public void Update()
    {
        if(target != null)
        {
			this.transform.position = offset + target.position;
		}
		
	}
}
