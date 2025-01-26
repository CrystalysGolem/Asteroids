using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerTeleport : MonoBehaviour
{
    [Header("Teleport Boundaries (Standard Mode)")]
    public float xBoundary = 10f;
    public float yBoundary = 10f;

    [Header("Screen Edge Mode")]
    public bool screenEdges = false;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        CheckPosition().Forget();
    }

    private async UniTaskVoid CheckPosition()
    {
        while (this != null && gameObject.activeSelf)
        {
            Vector3 screenTopRight = mainCamera.WorldToScreenPoint(new Vector3(Screen.width, Screen.height, 0));
            Vector3 screenBottomLeft = mainCamera.WorldToScreenPoint(Vector3.zero);

            xBoundary = screenTopRight.x;  
            yBoundary = screenTopRight.y;  

            Vector3 position = transform.position;

            if (screenEdges)
            {
                Vector3 screenPosition = mainCamera.WorldToScreenPoint(position);
                if (screenPosition.x < 0 || screenPosition.x > Screen.width || screenPosition.y < 0 || screenPosition.y > Screen.height)
                {
                    Vector3 newPosition = position;

                    if (screenPosition.x < 0)
                    {
                        newPosition.x = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, screenPosition.y, screenPosition.z)).x;
                    }
                    else if (screenPosition.x > Screen.width)
                    {
                        newPosition.x = mainCamera.ScreenToWorldPoint(new Vector3(0, screenPosition.y, screenPosition.z)).x;
                    }

                    if (screenPosition.y < 0)
                    {
                        newPosition.y = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, Screen.height, screenPosition.z)).y;
                    }
                    else if (screenPosition.y > Screen.height)
                    {
                        newPosition.y = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, 0, screenPosition.z)).y;
                    }

                    transform.position = newPosition;
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
