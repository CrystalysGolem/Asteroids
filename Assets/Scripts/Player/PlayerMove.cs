using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

public class PlayerMove : MonoBehaviour
{
    private float maxSpeed;
    private float acceleration;
    private float deceleration;

    private Camera mainCamera;
    public PlayerMovementLogic movementLogic;
    private bool isTeleporting = false;

    [Inject] private ScoreManager scoreManager;

    private void Start()
    {
        var config = PlayerConfigLoader.LoadConfig();
        maxSpeed = config.maxSpeed;
        acceleration = config.acceleration;
        deceleration = config.deceleration;

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            enabled = false;
            return;
        }

        movementLogic = new PlayerMovementLogic(maxSpeed, acceleration, deceleration);
        movementLogic.ResetPosition(transform.position);
        HandleMovement().Forget();
    }

    private async UniTaskVoid HandleMovement()
    {
        while (this != null && gameObject.activeSelf)
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector3 mousePosition = Input.mousePosition;
            if (mousePosition.x < 0 || mousePosition.y < 0 || mousePosition.x > Screen.width || mousePosition.y > Screen.height)
            {
                mousePosition = transform.position + movementLogic.CurrentVelocity.normalized;
            }
            mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -mainCamera.transform.position.z));
            movementLogic.UpdateMovement(input, mousePosition, Time.deltaTime);
            transform.position = movementLogic.Position;
            transform.rotation = movementLogic.Rotation;
            scoreManager.SetMaxSpeed((int)movementLogic.MaxAchievedSpeed);
            scoreManager.SetTravelled((int)movementLogic.TravelledDistance);
            CheckTeleport().Forget();

            await UniTask.Yield();
        }
    }

    private async UniTaskVoid CheckTeleport()
    {
        if (isTeleporting)
            return;

        Vector3 position = transform.position;
        float distance = Mathf.Abs(mainCamera.transform.position.z - position.z);
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(position);
        const float margin = 10f; 

        bool outside = screenPosition.x < 0 || screenPosition.x > Screen.width ||
                       screenPosition.y < 0 || screenPosition.y > Screen.height;

        if (outside)
        {
            isTeleporting = true; 
            float newScreenX = screenPosition.x;
            float newScreenY = screenPosition.y;

            if (screenPosition.x < 0)
                newScreenX = Screen.width - margin;
            else if (screenPosition.x > Screen.width)
                newScreenX = margin;

            if (screenPosition.y < 0)
                newScreenY = Screen.height - margin;
            else if (screenPosition.y > Screen.height)
                newScreenY = margin;

            Vector3 newWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(newScreenX, newScreenY, distance));
            transform.position = newWorldPosition;
            movementLogic.ResetPosition(newWorldPosition);
            await UniTask.Delay(100);
            isTeleporting = false;
        }
    }


    public void ReduceSpeedAndAcceleration()
    {
        movementLogic.ReduceSpeedAndAcceleration();
    }
}
