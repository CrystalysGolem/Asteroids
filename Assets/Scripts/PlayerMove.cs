using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed = 25f;
    public float acceleration = 20f;
    public float deceleration = 15f;
    private Camera mainCamera;
    private Vector3 currentVelocity = Vector3.zero;
    public Vector3 savedPosition;
    public float maxAchievedSpeed = 0f;
    public float travelledDistance = 0f;

    private int speedReductionCount = 0;
    private const int maxSpeedReduction = 2;

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

        savedPosition = transform.position;
        HandleMovement().Forget();
    }

    private async UniTaskVoid HandleMovement()
    {
        Vector3 previousPosition = transform.position;

        while (this != null && gameObject.activeSelf)
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector3 targetVelocity = new Vector3(input.x, input.y, 0f).normalized * maxSpeed;

            if (input.magnitude > 0)
            {
                currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
            }
            else
            {
                currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
            }

            if (currentVelocity.magnitude > maxSpeed)
            {
                currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
            }

            transform.position += currentVelocity * Time.deltaTime;

            float currentSpeed = currentVelocity.magnitude;
            if (currentSpeed > maxAchievedSpeed)
            {
                maxAchievedSpeed = currentSpeed;
                scoreManager.SetMaxSpeed((int)currentSpeed);
            }

            travelledDistance += Vector3.Distance(previousPosition, transform.position);
            scoreManager.SetTravelled((int)travelledDistance);

            previousPosition = transform.position;

            Vector3 mousePosition = Input.mousePosition;
            if (mousePosition.x < 0 || mousePosition.y < 0 || mousePosition.x > Screen.width || mousePosition.y > Screen.height)
            {
                mousePosition = transform.position + currentVelocity.normalized;
            }

            mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -mainCamera.transform.position.z));
            Vector3 direction = (mousePosition - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

            savedPosition = transform.position;

            await UniTask.Yield();
        }
    }

    public void ReduceSpeedAndAcceleration()
    {
        if (speedReductionCount < maxSpeedReduction)
        {
            maxSpeed *= 0.75f;
            acceleration *= 0.75f;
            deceleration *= 0.75f;
            speedReductionCount++;
            Debug.Log($"Speed and acceleration reduced. Current maxSpeed: {maxSpeed}, acceleration: {acceleration}");
        }
        else
        {
            Debug.Log("Maximum speed reduction reached.");
        }
    }
}
