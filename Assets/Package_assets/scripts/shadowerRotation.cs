using UnityEngine;
using System.Collections;

public class shadowerRotation : MonoBehaviour {

	private float x,y,z;
    public counterTime c_Time;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		transform.localRotation = Quaternion.AngleAxis( (-1)*(c_Time.getTick()/3.9322f) , new Vector3(1f,-90f,90f));


	}
}
