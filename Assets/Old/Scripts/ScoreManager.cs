using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class ScoreManager : MonoBehaviour
{

    public int Score;

    int _currentScore;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ScoreUpdate()
    {
        if (PhotonNetwork.LocalPlayer != null && Score != _currentScore)
        {
            _currentScore = Score;
            PhotonNetwork.LocalPlayer.SetScore(Score);
        }
    }


}
