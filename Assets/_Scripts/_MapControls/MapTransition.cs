using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class MapTransition : MonoBehaviour
{
    [SerializeField] BoxCollider2D mapBoundry;
    [SerializeField] Direction direction;
    [SerializeField] float distance = 2;
    [SerializeField] float transitionDuration = 0.25f;

    [Header("Trigger Chaining")]
    [SerializeField] GameObject triggerToEnable;
    [SerializeField] float triggerEnableDelay = 2f;

    CinemachineConfiner2D confiner;
    Coroutine activeTransition;
    Coroutine activeBoundaryWatch;
    Coroutine enableDelayRoutine;
    enum Direction {Up, Down, Left, Right}
    private void Awake()
    {
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();
    }

    private void Update()
    {
        if (triggerToEnable == null)
            return;

        bool shouldBeActive = confiner.BoundingShape2D == mapBoundry;

        if (shouldBeActive)
        {
            if (!triggerToEnable.activeSelf && enableDelayRoutine == null)
                enableDelayRoutine = StartCoroutine(EnableAfterDelay());
        }
        else
        {
            if (enableDelayRoutine != null)
            {
                StopCoroutine(enableDelayRoutine);
                enableDelayRoutine = null;
            }
            triggerToEnable.SetActive(false);
        }
    }

    private IEnumerator EnableAfterDelay()
    {
        yield return new WaitForSeconds(triggerEnableDelay);
        triggerToEnable.SetActive(true);
        enableDelayRoutine = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.attachedRigidbody;
            Vector2 oldPlayerPos = playerRb.position;
            Vector2 newPlayerPos = oldPlayerPos;

            switch (direction)
            {
                case Direction.Up:
                    newPlayerPos.y += distance;
                    break;
                case Direction.Down:
                    newPlayerPos.y -= distance;
                    break;
                case Direction.Left:
                    newPlayerPos.x -= distance;
                    break;
                case Direction.Right:
                    newPlayerPos.x += distance;
                    break;
            }

            if (activeTransition != null)
                StopCoroutine(activeTransition);
            activeTransition = StartCoroutine(SmoothMove(playerRb, oldPlayerPos, newPlayerPos));

            if (activeBoundaryWatch != null)
                StopCoroutine(activeBoundaryWatch);
            activeBoundaryWatch = StartCoroutine(SwitchBoundaryOnceInside(playerRb));
        }
    }

    private IEnumerator SmoothMove(Rigidbody2D playerRb, Vector2 from, Vector2 to)
    {
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            playerRb.MovePosition(Vector2.Lerp(from, to, elapsed / transitionDuration));
            yield return null;
        }
        playerRb.MovePosition(to);
        activeTransition = null;
    }

    private IEnumerator SwitchBoundaryOnceInside(Rigidbody2D playerRb)
    {
        while (!mapBoundry.OverlapPoint(playerRb.position))
            yield return null;

        confiner.BoundingShape2D = mapBoundry;
        confiner.InvalidateBoundingShapeCache();

        activeBoundaryWatch = null;
    }

    /*private void UpdatePlayerPos(GameObject player)
    {
        Vector3 newPos = player.transform.position;

        switch(direction)
        {
            case Direction.Up:
                newPos.y += distance;
                break;
            case Direction.Down:
                newPos.y -= distance;
                break;
            case Direction.Left:
                newPos.x -= distance;
                break;
            case Direction.Right:
                newPos.x += distance;
                break;
        }

        player.transform.position = newPos;
    }*/
}
