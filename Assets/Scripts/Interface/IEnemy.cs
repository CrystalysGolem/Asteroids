using UnityEngine;

public interface IEnemy
{
    void StartUP();
    GameObject gameObject { get; }
}

public static class IEnemyExtensions
{
    public static async void TakeDamage(this IEnemy enemy)
    {
        if (enemy is MonoBehaviour monoBehaviour)
        {
            var health = monoBehaviour.GetComponent<IHealth>();
            if (health != null)
            {
                health.DecreaseHealth(1);
                if (health.CurrentHealth <= 0)
                {
                    monoBehaviour.gameObject.SetActive(false);
                }
            }
        }
    }

    public static void SetInitialPosition(this IEnemy enemy, PlayerMovementLogic playerMovementLogic, out Vector3 targetPosition, out Vector3 currentDirection)
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Main camera not found.");
            targetPosition = Vector3.zero;
            currentDirection = Vector3.forward;
            return;
        }

        Vector3 screenTopRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.nearClipPlane));
        Vector3 screenBottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        float xMax = screenTopRight.x;
        float yMax = screenTopRight.y;
        float xMin = screenBottomLeft.x;
        float yMin = screenBottomLeft.y;

        int side = Random.Range(0, 4);
        Vector3 pos = enemy.gameObject.transform.position;
        switch (side)
        {
            case 0:
                pos = new Vector3(Random.Range(xMin, xMax), yMax, 0f);
                break;
            case 1:
                pos = new Vector3(Random.Range(xMin, xMax), yMin, 0f);
                break;
            case 2:
                pos = new Vector3(xMin, Random.Range(yMin, yMax), 0f);
                break;
            case 3:
                pos = new Vector3(xMax, Random.Range(yMin, yMax), 0f);
                break;
        }
        enemy.gameObject.transform.position = pos;
        Vector3 playerPos = playerMovementLogic.Position;
        Vector3 offset = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0f);
        targetPosition = playerPos + offset;
        currentDirection = (targetPosition - pos).normalized;
    }
}
