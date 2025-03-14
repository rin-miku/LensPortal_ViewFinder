using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Camera cameraA;
    public Camera cameraB;

    void Start()
    {
        SceneManager.LoadScene("SceneA", LoadSceneMode.Additive);
        SceneManager.LoadScene("SceneB", LoadSceneMode.Additive);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            cameraA.enabled = false;
            cameraB.targetTexture = null;
            cameraB.enabled = true; 
        }
    }
}
