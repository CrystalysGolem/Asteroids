using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerTeleport : MonoBehaviour
{
    [Header("Teleport Boundaries (Standard Mode)")]
    public float xBoundary = 10f;
    public float yBoundary = 10f;

    [Header("Screen Edge Mode")]
    public bool screenEdges = false;

    [Header("Teleport Boundaries (Screen Mode)")]
    public float screenXBoundary = 10f;
    public float screenYBoundary = 10f;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        CheckPosition().Forget();
    }

    private async UniTaskVoid CheckPosition()
    {
        while (true)
        {
            Vector3 position = transform.position;

            if (screenEdges)
            {
                Vector3 screenPosition = mainCamera.WorldToScreenPoint(position);
                if (screenPosition.x < 0 || screenPosition.x > Screen.width || screenPosition.y < 0 || screenPosition.y > Screen.height)
                {
                    Vector3 clampedPosition = position;
                    if (screenPosition.x < 0)
                    {
                        clampedPosition.x = 0;
                        screenXBoundary = clampedPosition.x;
                    }
                    if (screenPosition.x > Screen.width)
                    {
                        clampedPosition.x = Screen.width;
                        screenXBoundary = clampedPosition.x;
                    }
                    if (screenPosition.y < 0)
                    {
                        clampedPosition.y = 0;
                        screenYBoundary = clampedPosition.y;
                    }
                    if (screenPosition.y > Screen.height)
                    {
                        clampedPosition.y = Screen.height;
                        screenYBoundary = clampedPosition.y;
                    }
                    clampedPosition.z = position.z;
                    transform.position = mainCamera.ScreenToWorldPoint(new Vector3(clampedPosition.x, clampedPosition.y, 0));
                }
            }
            else
            {
                if (Mathf.Abs(position.x) > xBoundary)
                {
                    position.x = -Mathf.Sign(position.x) * xBoundary;
                    transform.position = position;
                }

                if (Mathf.Abs(position.y) > yBoundary)
                {
                    position.y = -Mathf.Sign(position.y) * yBoundary;
                    transform.position = position;
                }
            }

            await UniTask.Yield();
        }
    }
}
