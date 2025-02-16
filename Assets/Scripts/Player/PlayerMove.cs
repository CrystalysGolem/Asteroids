using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 25f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 15f;

    private Camera mainCamera;
    public PlayerMovementLogic movementLogic;

    [Inject] private ScoreManager scoreManager;

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            enabled = false;
            return;
        }

        movementLogic = new PlayerMovementLogic(maxSpeed, acceleration, deceleration);
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

            await UniTask.Yield();
        }
    }

    public void ReduceSpeedAndAcceleration()
    {
        movementLogic.ReduceSpeedAndAcceleration();
    }
}
