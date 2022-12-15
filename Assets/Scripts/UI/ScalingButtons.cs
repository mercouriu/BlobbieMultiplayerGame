using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalingButtons : MonoBehaviour
{
    protected float ScaleTime = 0.1f;

    protected virtual IEnumerator ButtonScalingCoroutine()
    {
        transform.LeanScale(new Vector2(0.8f, 0.8f), ScaleTime).setEaseOutBack();
        yield return new WaitForSeconds(ScaleTime);
        transform.LeanScale(new Vector2(1f, 1f), ScaleTime+0.05f).setEaseOutBack();
    }


    public virtual void ButtonScaling()
    {
        StartCoroutine(ButtonScalingCoroutine());
    }

}
