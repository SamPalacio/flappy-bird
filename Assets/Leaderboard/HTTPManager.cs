using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using TMPro;

public class HTTPManager : MonoBehaviour

{

    public string urlApi;
    
    public static Action<ScoresData> OnScoresGet;

    [Header("Username Fields")]
    [SerializeField] TMP_InputField InputFieldUsername;
    [SerializeField] TMP_InputField InputFieldPassword;
    [SerializeField] GameObject userPanel;
    [SerializeField] TMP_Text playerInfo;
    private string userName;
    private string token;
    private int bestScore;
    private void Start()
    {
        userPanel.SetActive(false);
        if (PlayerPrefs.HasKey("username") && PlayerPrefs.HasKey("token"))
        {
            userName = PlayerPrefs.GetString("username");
            token = PlayerPrefs.GetString("token");
            StartCoroutine(GetProfile());

        }
        else
        {
            userPanel.SetActive(true);

        }
    }
    private void OnEnable()
    {
        FlyBird.OnBirdDead += ChangeScore;
    }
    private void OnDisable()
    {
        FlyBird.OnBirdDead -= ChangeScore;

    }
    public void SignUp()
    {
        AuthData data = new AuthData(InputFieldUsername.text,InputFieldPassword.text);
        string postData = JsonUtility.ToJson(data);
        StartCoroutine(SignUp(postData));
    }
    public void LogIn()
    {
        AuthData data = new AuthData(InputFieldUsername.text, InputFieldPassword.text);
        string postData = JsonUtility.ToJson(data);
        StartCoroutine(LogIn(postData));
    }
    public void ChangeScore(int score)
    {

        if (score > bestScore)
        {
            UserData data = new UserData();
            data.username = PlayerPrefs.GetString("username");
            data.score = score;
            string patchData = JsonUtility.ToJson(data);
            StartCoroutine(UpdateScore(patchData));
        }

    }
    public void GetScoresGame()
    {
        StartCoroutine(GetScores());

    }


    IEnumerator UpdateScore(string patchData)
    {
        string url = urlApi + "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url, patchData);
        www.method = "PATCH";
        www.SetRequestHeader("content-type", "application/json");
        www.SetRequestHeader("x-token", token);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);
        }
       
    }

    IEnumerator GetScores()
    {
        string path = urlApi + "/api/usuarios?limit=10&sort=true";
        UnityWebRequest www = UnityWebRequest.Get(path);
        www.SetRequestHeader("x-token", token);

        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.LogError(www.error);
        }
        else if (www.responseCode == 200)
        {
            ScoresData resData = JsonUtility.FromJson<ScoresData>(www.downloadHandler.text);
            OnScoresGet?.Invoke(resData);
            
        }
    }

    IEnumerator SignUp(string postData)
    {
        string url = urlApi + "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url, postData);
        www.method = "POST";
        www.SetRequestHeader("content-type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);

            StartCoroutine(LogIn(postData));
        }
       
    }

    IEnumerator LogIn(string postData)
    {
        string url = urlApi + "/api/auth/login";
        UnityWebRequest www = UnityWebRequest.Put(url, postData);
        www.method = "POST";
        www.SetRequestHeader("content-type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);

            PlayerPrefs.SetString("token", resData.token);
            PlayerPrefs.SetString("username", resData.usuario.username);
            StartCoroutine(GetProfile());

        }
        
    }
    IEnumerator GetProfile()
    {
        Debug.Log("Getting the profile....");
        userName = PlayerPrefs.GetString("username");
        token = PlayerPrefs.GetString("token");
        string url = urlApi + "/api/usuarios/"+userName;
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("x-token",token);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);
            bestScore = resData.usuario.score;
            playerInfo.text = userName + " " + "Best Score: " + bestScore;


            userPanel.SetActive(false);

        }
        else
        {

            userPanel.SetActive(true);

        }
    }

    public void SignOut()
    {
        PlayerPrefs.DeleteAll();
        GameManager.instance.Replay();
    }
}









[System.Serializable]
public class ScoresData
{
    public UserData[] usuarios;

}

[System.Serializable]
public class AuthData
{

    public string username;
    public string password;
    public UserData usuario;
    public string token;

    public AuthData(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}

[System.Serializable]
public class UserData
{
    public string _id;
    public string username;
    public bool estado;
    public int score;
}