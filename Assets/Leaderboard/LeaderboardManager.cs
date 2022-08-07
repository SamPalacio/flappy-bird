using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;



public class LeaderboardManager : MonoBehaviour
{
    public GameObject scoreItems;
    
    private void OnEnable()
    {
        HTTPManager.OnScoresGet += ShowScores;
    }

    private void OnDisable()
    {
        HTTPManager.OnScoresGet -= ShowScores;
    }

    void ShowScores(ScoresData data)
    {

        transform.GetChild(0).gameObject.SetActive(true);

        List<ScoreItem> myScores = data.scores.ToList();
        myScores.Sort((a, b) => b.value.CompareTo(a.value));

        for (int i = 0; i < scoreItems.transform.childCount; i++)
        {
            if(i< myScores.Count)
            {
                scoreItems.transform.GetChild(i).GetComponent<TMP_Text>().text = string.Format("{0} {1} {2}",(i+1),myScores[i].userName, myScores[i].value);
            }
            else
            {
                scoreItems.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
       
    }

  
}
