using UnityEngine;
using System.Collections;

public class SliderBehaviour : MonoBehaviour {

    public counterTime cTime;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnValueChanged(float newValue) {
        cTime.setTimeMultiplier((int)(newValue));
    }
}
