using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportPoint : MonoBehaviour
{
    [Header("Teleport Settings")]
    [SerializeField] private Transform destination;
    [SerializeField] private float teleportDelay = 3f;

    [Header("Camera Boundary")]
    [SerializeField] private BoxCollider2D cameraBoundary;

    [Header("Transition Image")]
    [SerializeField] private GameObject transitionImage;
    [SerializeField] private float transitionShowDelay = 2f;
    [SerializeField] private float transitionHideDelay = 1f;
    [SerializeField] private float fadeDuration = 0.5f;

    private CinemachineConfiner2D confiner;
    private CinemachineCamera vCam;
    private CanvasGroup transitionCanvasGroup;
    private Rigidbody2D playerInRange;
    private Coroutine pendingTeleport;
    private bool awaitingTeleport;

    private void Awake()
    {
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();
        vCam = FindFirstObjectByType<CinemachineCamera>();
        transitionCanvasGroup = transitionImage != null ? transitionImage.GetComponent<CanvasGroup>() : null;
    }

    private void Update()
    {
        if (playerInRange == null)
            return;

        if (pendingTeleport == null && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            pendingTeleport = StartCoroutine(DelayedTeleport());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            playerInRange = collision.attachedRigidbody;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (awaitingTeleport && pendingTeleport != null)
        {
            StopCoroutine(pendingTeleport);
            pendingTeleport = null;
            awaitingTeleport = false;

            if (transitionImage != null)
                transitionImage.SetActive(false);
            if (transitionCanvasGroup != null)
                transitionCanvasGroup.alpha = 0f;
        }

        playerInRange = null;
    }

    private IEnumerator DelayedTeleport()
    {
        awaitingTeleport = true;

        yield return new WaitForSeconds(transitionShowDelay);

        if (transitionImage != null)
        {
            if (transitionCanvasGroup != null)
                transitionCanvasGroup.alpha = 0f;
            transitionImage.SetActive(true);
            yield return FadeCanvasGroup(1f);
        }

        float remainingDelay = teleportDelay - transitionShowDelay;
        if (remainingDelay > 0f)
            yield return new WaitForSeconds(remainingDelay);

        awaitingTeleport = false;
        Teleport();

        yield return new WaitForSeconds(transitionHideDelay);

        if (transitionImage != null)
        {
            yield return FadeCanvasGroup(0f);
            transitionImage.SetActive(false);
        }

        pendingTeleport = null;
    }

    private IEnumerator FadeCanvasGroup(float targetAlpha)
    {
        if (transitionCanvasGroup == null)
            yield break;

        float startAlpha = transitionCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            transitionCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        transitionCanvasGroup.alpha = targetAlpha;
    }

    private void Teleport()
    {
        if (destination == null)
        {
            Debug.LogWarning($"{name}: no teleport destination assigned.", this);
            return;
        }

        Vector3 oldPosition = playerInRange.transform.position;
        playerInRange.position = destination.position;

        if (cameraBoundary != null && confiner != null)
        {
            confiner.BoundingShape2D = cameraBoundary;
            confiner.InvalidateBoundingShapeCache();
        }

        if (vCam != null)
            vCam.OnTargetObjectWarped(playerInRange.transform, playerInRange.transform.position - oldPosition);

        playerInRange = null;
    }
}
