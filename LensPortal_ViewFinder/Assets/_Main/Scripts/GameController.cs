using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LoadLevel0());
    }

    private IEnumerator LoadLevel0()
    {
        yield return SceneManager.LoadSceneAsync("Level0", LoadSceneMode.Additive);

        Camera playerCamera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        playerCamera.cullingMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Level0"));

        foreach (Teleporter teleporter in FindObjectsOfType<Teleporter>(true))
        {
            teleporter.enabled = true;
        }
    }
}
