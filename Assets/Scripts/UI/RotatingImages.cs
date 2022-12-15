using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingImages : MonoBehaviour
{
    public int rotationmode;

    IEnumerator RotatingWithDelay()
    {
        yield return new WaitForSeconds(3f);
        transform.LeanRotate(new Vector3(0, 0, 40f), 0.1f).setEaseInBack();
        yield return new WaitForSeconds(0.1f);
        transform.LeanRotate(new Vector3(0, 0, -30f), 0.1f).setEaseInBack();
        yield return new WaitForSeconds(0.1f);
        transform.LeanRotate(new Vector3(0, 0, 30f), 0.1f).setEaseInBack();
        yield return new WaitForSeconds(0.1f);
        transform.LeanRotate(new Vector3(0, 0, -15f), 0.1f).setEaseInBack();
        yield return new WaitForSeconds(0.1f);
        transform.LeanRotate(new Vector3(0, 0, 20f), 0.1f).setEaseInBack();
        yield return new WaitForSeconds(0.1f);
        transform.LeanRotate(new Vector3(0, 0, 5f), 0.1f).setEaseInBack();
        yield return new WaitForSeconds(0.1f);
        transform.LeanRotate(new Vector3(0, 0, 13f), 0.1f).setEaseInBack();

        yield return new WaitForSeconds(3f);
        StartCoroutine(RotatingWithDelay());
    }

    IEnumerator FullRotating()
    {
        transform.LeanRotate(new Vector3(0, 0, 0f), 1f).setEaseOutCubic();
        yield return new WaitForSeconds(1f);
        transform.LeanRotate(new Vector3(0, 0, -90f), 1f).setEaseOutCubic();
        yield return new WaitForSeconds(1f);
        transform.LeanRotate(new Vector3(0, 0, -180f), 1f).setEaseOutCubic();
        yield return new WaitForSeconds(1f);
        transform.LeanRotate(new Vector3(0, 0, -270f), 1f).setEaseOutCubic();
        yield return new WaitForSeconds(1f);
        

        StartCoroutine(FullRotating());
    }


    void Start()
    {
        if (rotationmode == 0)
        {
            StartCoroutine(RotatingWithDelay());
        }
        else if(rotationmode == 1)
        {
            StartCoroutine(FullRotating());
        }
        
    }

    
}
