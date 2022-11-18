using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeSwitching : MonoBehaviour
{
    public Text GamemodeName;
    public Button NextModeButton;
    public Button PrevModeButton;
    public AudioSource WawSwitching;

    private int GamemodeNumInList = 0;

    public List<string> GamemodeList = new List<string>() { "<Random Game>", "Guess the theme", "Animal analyzing", "Suspicious inspectors", "Cooking Time" };


    public void Start()
    {
        GamemodeName.text = GamemodeList[GamemodeNumInList];

        NextModeButton.onClick.AddListener(() =>
        {
            if (GamemodeNumInList >= GamemodeList.Count-1)
            {
                GamemodeNumInList = 0;
            }
            else
            {
                GamemodeNumInList++;
            }

            WawSwitching.Play();
            GamemodeName.text = GamemodeList[GamemodeNumInList];
        });

        PrevModeButton.onClick.AddListener(() =>
        {
            if (GamemodeNumInList <= 0)
            {
                GamemodeNumInList= GamemodeList.Count-1;
            }
            else
            {
                GamemodeNumInList--;
            }

            WawSwitching.Play();
            GamemodeName.text = GamemodeList[GamemodeNumInList];
        });

    }

}
