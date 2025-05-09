using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System.Threading;

public class PlayerMove : MonoBehaviour
{
    [Inject] private ScoreProvider scoreManager;
    [Inject] private OptionsProvider optionsManager;
    [Inject] private PlayerConfigLoader configLoader;

    public JoyStick MoveStick;
    public PlayerMovementLogic movementLogic;

    private float maxSpeed;
    private float acceleration;
    private float deceleration;
    private int TeleportCheckDelay = 100;
    private bool IsMobile = false;
    private bool isTeleporting = false;
    private Camera mainCamera;
    private CancellationTokenSource cancellationTokenSource;

    private void Start()
    {
        cancellationTokenSource = new CancellationTokenSource();

        IsMobile = optionsManager.IsMobile;
        var config = configLoader.LoadConfig();
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

        HandleMovement(cancellationTokenSource.Token).Forget();
    }

    private void OnDestroy()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
    }

    public void ReduceSpeedAndAcceleration()
    {
        movementLogic.ReduceSpeedAndAcceleration();
    }

    private async UniTaskVoid HandleMovement(CancellationToken token)
    {
        Vector3 lastMousePosition = transform.position;

        while (!token.IsCancellationRequested && this != null && gameObject.activeSelf)
        {
            Vector2 input;
            Vector3 mousePosition;

            if (IsMobile && MoveStick != null && MoveStick.gameObject.activeSelf)
            {
                input = MoveStick.Direction * MoveStick.Speed;
                if (input.sqrMagnitude > 0.1f)
                {
                    mousePosition = transform.position + new Vector3(input.x, input.y, 0f);
                    lastMousePosition = mousePosition;
                }
                else
                {
                    mousePosition = lastMousePosition;
                }
            }
            else
            {
                input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                mousePosition = Input.mousePosition;

                if (mousePosition.x < 0 || mousePosition.y < 0 || mousePosition.x > Screen.width || mousePosition.y > Screen.height)
                {
                    mousePosition = lastMousePosition;
                }
                else
                {
                    mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -mainCamera.transform.position.z));
                    lastMousePosition = mousePosition;
                }
            }

            movementLogic.UpdateMovement(input, mousePosition, Time.deltaTime);
            transform.position = movementLogic.Position;
            transform.rotation = movementLogic.Rotation;
            scoreManager.SetMaxSpeed((int)movementLogic.MaxAchievedSpeed);
            scoreManager.SetTravelled((int)movementLogic.TravelledDistance);

            CheckTeleport(token).Forget();

            await UniTask.Yield(token);
        }
    }

    private async UniTaskVoid CheckTeleport(CancellationToken token)
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

            await UniTask.Delay(TeleportCheckDelay, cancellationToken: token);

            isTeleporting = false;
        }
    }
}
