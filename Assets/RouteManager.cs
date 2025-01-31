using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class RouteData
{
    public int start;
    public int end;
    public string name;
}

[System.Serializable]
public class BoardData
{
    public int board_id;
    public List<RouteData> snakes;
    public List<RouteData> ladders;
}

public class RouteManager : MonoBehaviour
{
    public Route route;

    public GameObject[] snakeObjects; // Assign existing snake objects in the scene
    public GameObject[] ladderObjects; // Assign existing ladder objects in the scene

    private string apiUrl = "https://4e9b-124-43-79-210.ngrok-free.app/api/boards/1098";

    void Start()
    {
        int boardId = 1096; // Example board ID
        StartCoroutine(FetchRouteData(boardId));
    }

    IEnumerator FetchRouteData(int boardId)
    {
        string urlWithId = $"{apiUrl}?board_id={boardId}";
        UnityWebRequest request = UnityWebRequest.Get(urlWithId);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log("Received JSON from server: " + json);

            try
            {
                BoardData boardData = JsonUtility.FromJson<BoardData>(json);
                UpdateRoutes(boardData);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error deserializing JSON: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("Failed to fetch route data: " + apiUrl + request.error);
        }
    }

    void UpdateRoutes(BoardData data)
    {
        foreach (var node in route.nodeList)
        {
            node.GetComponent<Node>().connectionNode = null;
        }

        if (data.snakes.Count > snakeObjects.Length)
        {
            Debug.LogError($"Not enough snake objects! Needed: {data.snakes.Count}, Assigned: {snakeObjects.Length}");
            return;
        }

        if (data.ladders.Count > ladderObjects.Length)
        {
            Debug.LogError($"Not enough ladder objects! Needed: {data.ladders.Count}, Assigned: {ladderObjects.Length}");
            return;
        }

        // Move Snakes to their respective start positions and set route connections
        for (int i = 0; i < data.snakes.Count; i++)
        {
            RouteData snake = data.snakes[i];
            PositionAtSquareStart(snakeObjects[i], snake.start, true);
            AssignRouteConnection(snake.start, snake.end);
        }
        Debug.Log("Snakes are placed correctly! ✅");

        // Move Ladders to their respective start positions and set route connections
        for (int i = 0; i < data.ladders.Count; i++)
        {
            RouteData ladder = data.ladders[i];
            PositionAtSquareStart(ladderObjects[i], ladder.start, false);
            AssignRouteConnection(ladder.start, ladder.end);
        }
        Debug.Log("Ladders are placed correctly! ✅");
    }

    void PositionAtSquareStart(GameObject obj, int squareIndex, bool isSnake)
    {
        if (squareIndex < route.nodeList.Count)
        {
            Vector3 basePosition = route.nodeList[squareIndex].transform.position;

            // Ensure snakes align their head at the start field
            obj.transform.position = new Vector3(
                basePosition.x,
                basePosition.y,
                basePosition.z
            );

            Debug.Log($"{obj.name} moved to square {squareIndex} at {obj.transform.position}");
        }
        else
        {
            Debug.LogError($"Invalid square index: {squareIndex}");
        }
    }

    void AssignRouteConnection(int start, int end)
    {
        if (start < route.nodeList.Count && end < route.nodeList.Count)
        {
            Node startNode = route.nodeList[start].GetComponent<Node>();
            Node endNode = route.nodeList[end].GetComponent<Node>();
            startNode.connectionNode = endNode; // This ensures correct movement of the stone.

            Debug.Log($"Route connection assigned: {start} → {end}");
        }
        else
        {
            Debug.LogError($"Invalid node indices for route connection: {start} → {end}");
        }
    }
}
