using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamemodeSwinchingButtons : MonoBehaviour
{
    
    IEnumerator ButtonScalingCoroutine()
    {
        transform.LeanScale(new Vector2(0.8f, 0.8f), 0.1f).setEaseOutBack();
        yield return new WaitForSeconds(0.1f);
        transform.LeanScale(new Vector2(1f, 1f), 0.1f).setEaseOutBack();

    }

    public void ButtonScaling()
    {
        StartCoroutine(ButtonScalingCoroutine());
    }
}
