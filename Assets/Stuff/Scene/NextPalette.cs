using UnityEngine;
using System.Collections;

public class NextPalette : MonoBehaviour {


    void Awake() {
        DontDestroyOnLoad(transform.gameObject);
    }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void getNextScene() {
        if (Application.loadedLevel + 1 > 3) {
            Application.LoadLevel(0);
        } else {
            Application.LoadLevel(Application.loadedLevel + 1);
        }
    }

}
