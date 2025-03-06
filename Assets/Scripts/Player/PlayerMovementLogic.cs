using UnityEngine;

public class PlayerMovementLogic
{
    [Header("For another classes")]
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }
    public Vector3 CurrentVelocity { get; private set; }

    // Move logic
    private float maxSpeed;
    private float acceleration;
    private float deceleration;
    private int speedReductionCount = 0;
    private const int maxSpeedReduction = 2;

    [Header("For Score counter")]
    public float MaxAchievedSpeed { get; private set; }
    public float TravelledDistance { get; private set; }


    public PlayerMovementLogic(float maxSpeed, float acceleration, float deceleration)
    {
        this.maxSpeed = maxSpeed;
        this.acceleration = acceleration;
        this.deceleration = deceleration;
        CurrentVelocity = Vector3.zero;
        Position = Vector3.zero;
    }

    public void UpdateMovement(Vector2 input, Vector3 mousePosition, float deltaTime)
    {
        Vector3 previousPosition = Position;
        Vector3 targetVelocity = new Vector3(input.x, input.y, 0f).normalized * maxSpeed;

        if (input.magnitude > 0)
        {
            CurrentVelocity = Vector3.MoveTowards(CurrentVelocity, targetVelocity, acceleration * deltaTime);
        }
        else
        {
            CurrentVelocity = Vector3.MoveTowards(CurrentVelocity, Vector3.zero, deceleration * deltaTime);
        }

        CurrentVelocity = Vector3.ClampMagnitude(CurrentVelocity, maxSpeed);
        Position += CurrentVelocity * deltaTime;

        float currentSpeed = CurrentVelocity.magnitude;
        if (currentSpeed > MaxAchievedSpeed)
        {
            MaxAchievedSpeed = currentSpeed;
        }

        TravelledDistance += Vector3.Distance(previousPosition, Position);

        Vector3 direction = (mousePosition - Position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    public void ReduceSpeedAndAcceleration()
    {
        if (speedReductionCount < maxSpeedReduction)
        {
            maxSpeed *= 0.75f;
            acceleration *= 0.75f;
            deceleration *= 0.75f;
            speedReductionCount++;
        }
    }

    public void ResetPosition(Vector3 newPosition)
    {
        Position = newPosition;
    }
}
