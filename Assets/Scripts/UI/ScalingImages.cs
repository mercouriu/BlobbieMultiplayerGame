using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalingImages : ScalingButtons
{

    protected override IEnumerator ButtonScalingCoroutine()
    {   
        //transform.LeanRotate(new Vector3(0, 0, 5f), ScaleTime).setEaseOutBack();
        yield return StartCoroutine(base.ButtonScalingCoroutine());
        /*
        transform.LeanRotate(new Vector3(0, 0, 0f), ScaleTime).setEaseOutBack();
        yield return new WaitForSeconds(1f);
        transform.LeanRotate(new Vector3(0, 0, -10f), ScaleTime).setEaseOutBack();
        yield return StartCoroutine(base.ButtonScalingCoroutine());
        transform.LeanRotate(new Vector3(0, 0, 0f), ScaleTime).setEaseOutBack();*/
        yield return new WaitForSeconds(3f);
        StartCoroutine(ButtonScalingCoroutine());
    }

    void Start()
    {
        ScaleTime = 0.2f;
        StartCoroutine(ButtonScalingCoroutine());
        
    }
}
