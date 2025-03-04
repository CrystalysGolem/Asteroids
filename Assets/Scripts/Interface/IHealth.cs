public interface IHealth
{
    int CurrentHealth { get; set; }
}
public static class IHealthExtensions
{
    public static void IncreaseHealth(this IHealth health, int amount)
    {
        if (health != null)
        {
            health.SetHealth(health.CurrentHealth + amount);
        }
    }

    public static void SetHealth(this IHealth health, int amount)
    {
        health.CurrentHealth = amount;
    }

    public static void DecreaseHealth(this IHealth health, int amount)
    {
        SetHealth(health, health.CurrentHealth - amount);
    }

    public static void ApplyDifficulty(this IHealth health, int EasyHP, int MediumHP, int HardHP, DifficultyManager.Difficulty difficulty)
    {
        switch (difficulty)
        {
            case DifficultyManager.Difficulty.Easy:
                health.SetHealth(EasyHP);
                break;
            case DifficultyManager.Difficulty.Medium:
                health.SetHealth(MediumHP);
                break;
            case DifficultyManager.Difficulty.Hard:
                health.SetHealth(HardHP);
                break;
        }
    }

}
