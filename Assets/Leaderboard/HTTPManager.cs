using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
public class HTTPManager : MonoBehaviour

{

    public string url;
    public static Action<ScoresData> OnScoresGet;

    public void GetScoresGame()
    {
        StartCoroutine(GetScores());

    }

    IEnumerator GetScores()
    {
        string path = url + "/leaders";

        UnityWebRequest www = UnityWebRequest.Get(path);
        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.LogError(www.error);
        }
        else if (www.responseCode == 200)
        {
            ScoresData resData = JsonUtility.FromJson<ScoresData>(www.downloadHandler.text);
            Debug.Log(www.downloadHandler.text);
            OnScoresGet?.Invoke(resData);
        }
    }
}
[System.Serializable]
public class ScoreItem
{
    public int userId;
    public int value;
    public string userName;

}
[System.Serializable]
public class ScoresData
{
    public ScoreItem[] scores;

}