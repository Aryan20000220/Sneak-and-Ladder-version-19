using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameSettings : MonoBehaviour
{
    public Toggle redHuman, redCPU;
    public Toggle greenHuman, greenCPU;

    public static bool loadgame = false;
    public int Reddone;
    public int Greendone;

    private void Start()
    {
        // Validate toggles to prevent null reference issues
        ValidateToggles();
    }

    private void ValidateToggles()
    {
        if (redHuman == null || redCPU == null)
            Debug.LogError("Red toggles are not assigned in the inspector!");
        if (greenHuman == null || greenCPU == null)
            Debug.LogError("Green toggles are not assigned in the inspector!");
    }

    public void ReadToggles()
    {
        if (redHuman == null || redCPU == null || greenHuman == null || greenCPU == null || SaveSettings.players.Length < 2) return;

        SaveSettings.players[0] = redCPU.isOn ? "CPU" : "HUMAN";
        SaveSettings.players[1] = greenCPU.isOn ? "CPU" : "HUMAN";
    }

    public void StartGame(int scene)
    {
        ReadToggles();
        if (SceneManager.GetSceneByBuildIndex(scene) != null)
        {
            SceneManager.LoadScene(scene);

            if (loadgame)
            {
                loadPositions();
            }
        }
        else
        {
            Debug.LogError($"Scene with index {scene} does not exist or is not added to the build settings.");
        }
    }

    public void loadPositions()
    {
        if (loadgame)
        {
            Debug.Log("Loading saved positions...");
            StartCoroutine(getData());
        }
    }

    IEnumerator getData()
    {
        Debug.Log("Connecting to the database...");
        WWWForm form = new WWWForm();
        if (string.IsNullOrEmpty(DBManager.username))
        {
            Debug.LogError("DBManager.username is null or empty!");
            yield break;
        }

        form.AddField("Name", DBManager.username);

        using (UnityWebRequest webRequest = UnityWebRequest.Post("https://agrohub.ml/sqlconnect/webtest.php", form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error fetching data: " + webRequest.error);
            }
            else
            {
                Debug.Log("Data fetched successfully.");
                string responseText = webRequest.downloadHandler.text;

                string[] data = responseText.Split('\t');
                if (data.Length < 3)
                {
                    Debug.LogError("Invalid response format. Expected at least 3 fields.");
                    yield break;
                }

                // Parse positions
                if (int.TryParse(data[1], out Reddone) && int.TryParse(data[2], out Greendone))
                {
                    Debug.Log($"Positions Loaded: Red({Reddone}), Green({Greendone})");
                    SceneManager.LoadScene(2);
                }
                else
                {
                    Debug.LogError("Failed to parse positions from the response.");
                }
            }
        }

        loadgame = false;
    }
}

public static class SaveSettings
{
    public static string[] players = new string[2]; // Updated for only two players
}
