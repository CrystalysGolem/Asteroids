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

    [Inject] private ScoreManager scoreManager;

    private void Start()
    {
        mainCamera = Camera.main;
        savedPosition = transform.position;
        HandleMovement().Forget();
    }

    private async UniTaskVoid HandleMovement()
    {
        Vector3 previousPosition = transform.position;

        while (true)
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

            currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
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
            mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -mainCamera.transform.position.z));
            Vector3 direction = (mousePosition - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

            savedPosition = transform.position;

            await UniTask.Yield();
        }
    }
}
