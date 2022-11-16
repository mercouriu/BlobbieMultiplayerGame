using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISwitching : MonoBehaviour
{
    public Transform LoginB;
    public Transform SelectionB;


    private void OnEnable()
    {
        LoginB.LeanMoveLocalY(0, 0.5f).setEaseOutExpo().delay = 0.1f;

    }

    public void CloseDialog()
    {

    }
}
