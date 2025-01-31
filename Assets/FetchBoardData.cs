using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class FetchBoardData : MonoBehaviour
{
    private string apiUrl = "http://127.0.0.1:5000/api/boards/1098"; // Update with your URL

    // Start is called before the first frame update
    void Start()
    {
        // Start the data fetch process
        StartCoroutine(FetchDataFromAPI());
    }

    // Coroutine to fetch data from API
    IEnumerator FetchDataFromAPI()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);

        // Wait for the response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Successfully received data
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Data fetched: " + jsonResponse);
            
            // Now parse and use the data
            ProcessBoardData(jsonResponse);
        }
        else
        {
            // Handle errors
            Debug.LogError("Request failed: " + request.error);
        }
    }

    // Process and apply the fetched board data
    void ProcessBoardData(string jsonResponse)
    {
        // Use a simple JSON parsing method (if using Unity's built-in JSONUtility or a third-party library like JSON.NET)
        BoardData boardData = JsonUtility.FromJson<BoardData>(jsonResponse);

        // Example of using the data to print ladders and snakes
        Debug.Log("Board ID: " + boardData.board_id);
        foreach (var ladder in boardData.ladders)
        {
            Debug.Log($"Ladder {ladder.name} from {ladder.start} to {ladder.end}");
        }

        foreach (var snake in boardData.snakes)
        {
            Debug.Log($"Snake {snake.name} from {snake.start} to {snake.end}");
        }

        // Apply this data to your board game (e.g., update ladder and snake positions)
    }

    // Data structure to hold the board information
    [System.Serializable]
    public class BoardData
    {
        public int board_id;
        public Ladder[] ladders;
        public Snake[] snakes;
    }

    [System.Serializable]
    public class Ladder
    {
        public int start;
        public int end;
        public string name;
    }

    [System.Serializable]
    public class Snake
    {
        public int start;
        public int end;
        public string name;
    }
}
