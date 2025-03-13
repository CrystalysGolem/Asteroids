using Firebase;
using Firebase.Analytics;
using Firebase.Database;
using UnityEngine;

public class FirebaseInitializer : MonoBehaviour
{
    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                LogEvent("level_start", new Parameter("level", 1));
                Debug.Log("Firebase успешно инициализирован!");
                WriteTestData();
            }
            else
            {
                Debug.LogError($"Firebase не удалось инициализировать: {task.Result}");
            }
        });
    }

    public static void LogEvent(string eventName, params Parameter[] parameters)
    {
        FirebaseAnalytics.LogEvent(eventName, parameters);
    }


    private void WriteTestData()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        TestData testData = new TestData("Hello Firebase!", 123);
        string json = JsonUtility.ToJson(testData);

        reference.Child("test").SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Ошибка записи данных в Realtime Database: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
            }
        });
    }

    public class TestData
    {
        public string message;
        public int number;

        public TestData(string message, int number)
        {
            this.message = message;
            this.number = number;
        }
    }
}
