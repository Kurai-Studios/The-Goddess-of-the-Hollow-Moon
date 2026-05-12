using Unity.Cinemachine;
using UnityEngine;

public class MapTransition : MonoBehaviour
{
    [SerializeField] BoxCollider2D mapBoundry;
    [SerializeField] Direction direction;
    [SerializeField] float distance = 2;

    CinemachineCamera vCamera;
    CinemachineConfiner2D confiner;
    enum Direction {Up, Down, Left, Right}
    private void Awake()
    {
        vCamera = FindFirstObjectByType<CinemachineCamera>();
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 oldplayerPos = collision.transform.position;
            Vector3 newPlayerPos = oldplayerPos;

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

            confiner.BoundingShape2D = mapBoundry;
            confiner.InvalidateBoundingShapeCache();

            collision.transform.position = newPlayerPos;

            Vector3 delta = newPlayerPos - oldplayerPos;

            vCamera.OnTargetObjectWarped(collision.transform, delta);
        }
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
