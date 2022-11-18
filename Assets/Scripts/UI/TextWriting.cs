using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextWriting : MonoBehaviour
{
    private string Textstr;
    private string TextstrCut;
    /*
    [SerializeField] private AudioSource keysound1;
    [SerializeField] private AudioSource keysound2;
    [SerializeField] private AudioSource keysound3;
    [SerializeField] private AudioSource keysound4;
    [SerializeField] private AudioSource keysound5;
    [SerializeField] private AudioSource keysound6;
    */
    [SerializeField] AudioClip[] keyboardsounds;
    private AudioSource soundSource;


    private void Start()
    {
        Textstr = GetComponent<Text>().text;
        TextstrCut = Textstr.Remove(Textstr.Length - 1);
        GetComponent<Text>().text = "";
        StartCoroutine(TextWritingCoroutine());
        StartCoroutine(SoundCoroutine());
        soundSource = GetComponent<AudioSource>();

    }

    IEnumerator TextWritingCoroutine()
    {
        foreach(char abc in Textstr)
        {
            yield return new WaitForSeconds(0.07f);
            GetComponent<Text>().text += abc;
        }
        
    }

    IEnumerator SoundCoroutine()
    {
        foreach (char abc in TextstrCut)
        {
            yield return new WaitForSeconds(0.07f);
            PlayRandomPrintKey();
        }

    }

    private void PlayRandomPrintKey()
    {
        AudioClip clip = keyboardsounds[Random.Range(0, keyboardsounds.Length)];
        soundSource.PlayOneShot(clip);
    }

}
