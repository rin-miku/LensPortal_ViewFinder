using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Teleporter : MonoBehaviour
{
    [Header("Scene")]
    public string loadSceneName;
    private string unloadSceneName;

    [Header("Player")]
    private Camera playerCamera;
    private PlayerController playerController;

    [Header("Teleporter")]
    private Camera teleporterCamera;
    private Canvas teleporterCanvas;
    private RawImage teleporterRawImage;
    private RenderTexture renderTexture;
    private bool needLookAt = true;
    private bool needSwitchScene = false;

    private void Start()
    {
        unloadSceneName = gameObject.scene.name;

        playerCamera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
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
            teleporterCamera.transform.position = playerCamera.transform.position;
            teleporterCamera.transform.LookAt(teleporterCanvas.transform);
        }
    }

    private IEnumerator LoadScene()
    {
        teleporterCamera.cullingMask = 1 << LayerMask.NameToLayer(loadSceneName);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer(loadSceneName), true);

        yield return SceneManager.LoadSceneAsync(loadSceneName, LoadSceneMode.Additive);

        renderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        renderTexture.Create();
        teleporterCamera.targetTexture = renderTexture;
        teleporterRawImage.texture = teleporterCamera.targetTexture;

        // 场景和targetTexture均准备好之后再开启相机 防止主相机切换导致的闪屏
        teleporterCamera.enabled = true;
    }

    public void MovePlayerCamera()
    {
        foreach (Teleporter teleporter in FindObjectsOfType<Teleporter>(true))
        {
            teleporter.enabled = true;
        }

        playerController.enabled = false;
        needSwitchScene = true;

        Vector3 canvasPos = teleporterCanvas.transform.position;
        playerController.transform.DOMove(canvasPos + teleporterCanvas.transform.forward * 1f, 2f);
        playerController.transform.DODynamicLookAt(canvasPos, 2f);
        playerCamera.transform.DODynamicLookAt(canvasPos, 2f).OnComplete(OnMovePlayerCameraCompleted);

        void OnMovePlayerCameraCompleted()
        {
            needLookAt = false;

            teleporterCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            teleporterCanvas.worldCamera = playerCamera;
            RectTransform rectTransform = teleporterCanvas.GetComponent<RectTransform>();
            rectTransform.position = Vector3.zero;
            teleporterRawImage.transform.localScale = Vector3.one;

            RectTransform maskRectTransform = teleporterCanvas.transform.GetChild(0).GetComponent<RectTransform>();
            maskRectTransform.DOSizeDelta(new Vector2(3000f, 3000f), 2f)
                .OnComplete(() =>
                {
                    maskRectTransform.sizeDelta = Vector2.zero;
                    StartCoroutine(SwitchScene());
                });
        }

        IEnumerator SwitchScene()
        {
            if (!unloadSceneName.Equals("_Main"))
            {
                playerCamera.cullingMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer(loadSceneName));
                
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer(loadSceneName), false);
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer(unloadSceneName), true);

                playerController.enabled = true;

                yield return SceneManager.UnloadSceneAsync(unloadSceneName);
            }
        }
    }

    private void OnDestroy()
    {
        renderTexture?.Release();
        if (!needSwitchScene)
        {
            SceneManager.UnloadSceneAsync(loadSceneName);
        }
    }
}
