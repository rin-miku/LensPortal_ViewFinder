using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Camera cameraA;
    public Camera cameraB;
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene("SceneA", LoadSceneMode.Additive);
        SceneManager.LoadScene("SceneB", LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
