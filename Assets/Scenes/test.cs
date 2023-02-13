using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class test : MonoBehaviour
{
    public string sceneName;
    public Light light;

    private void Awake()
    {
        if (light) light.gameObject.SetActive(false);
    }
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
