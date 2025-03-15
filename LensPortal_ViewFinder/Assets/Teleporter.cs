using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Teleporter : MonoBehaviour
{
    [Header("Scene")]
    public string loadSceneName;
    private string unloadSceneName;

    [Header("Player")]
    private Transform playerCameraTransform;
    private PlayerController playerController;

    [Header("Teleporter")]
    private Camera teleporterCamera;
    private Canvas teleporterCanvas;
    private RawImage teleporterRawImage;
    private RenderTexture renderTexture;
    private bool needLookAt = true;

    private void OnEnable()
    {
        Debug.Log("OnEnable");
    }

    private void Start()
    {
        Debug.Log("Start");
        unloadSceneName = SceneManager.GetActiveScene().name;

        playerCameraTransform = GameObject.Find("PlayerCamera").transform;
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        teleporterCamera = transform.GetComponentInChildren<Camera>();
        teleporterCanvas = transform.GetComponentInChildren<Canvas>();
        teleporterRawImage = teleporterCanvas.transform.GetChild(0).GetChild(0).GetComponent<RawImage>();

        StartCoroutine(LoadScene());
    }

    private void Update()
    {
        if (needLookAt)
        {
            teleporterCamera.transform.position = playerCameraTransform.position;
            teleporterCamera.transform.LookAt(teleporterCanvas.transform);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            MovePlayerCamera();
        }
    }

    private IEnumerator LoadScene()
    {
        teleporterCamera.cullingMask = 1 << LayerMask.NameToLayer(loadSceneName);

        yield return SceneManager.LoadSceneAsync(loadSceneName, LoadSceneMode.Additive);

        renderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        renderTexture.Create();
        teleporterCamera.targetTexture = renderTexture;
        teleporterRawImage.texture = teleporterCamera.targetTexture;
    }

    public void MovePlayerCamera()
    {
        playerController.enabled = false;

        Vector3 canvasPos = teleporterCanvas.transform.position;
        Vector3 pos = new Vector3(canvasPos.x, canvasPos.y, canvasPos.z + 1f);
        playerCameraTransform.DOMove(pos, 2f);
        playerCameraTransform.DODynamicLookAt(teleporterCanvas.transform.position, 2f)
            .OnComplete(() =>
            {
                needLookAt = false;

                teleporterCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                teleporterCanvas.worldCamera = playerCameraTransform.GetComponent<Camera>();
                RectTransform rectTransform = teleporterCanvas.GetComponent<RectTransform>();
                rectTransform.position = Vector3.zero;
                teleporterCanvas.transform.GetChild(0).GetComponent<RectTransform>().DOSizeDelta(new Vector2(3000f, 3000f), 2f);
                teleporterRawImage.transform.localScale = Vector3.one;
            });
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
    }

    private void OnDestroy()
    {
        Debug.Log("OnDestroy");
        renderTexture.Release();
    }
}
