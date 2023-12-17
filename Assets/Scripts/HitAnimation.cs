using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HitAnimation : MonoBehaviour
{
    public GameObject hitTextObject;
    private TMPro.TextMeshPro hitText;

    private Color startColor;
    private Color currentColor;
    public static float showAlpha = 0.8f;
    public Color missColor = new Color(1, 0.2f, 0.2f);
    public Color hitColor = new Color(0.4f, 0.7f, 0.9f);
    public Color perfectHitColor = new Color(0, 1, 0); 

    void Start()
        {
            hitText = hitTextObject.GetComponent<TMPro.TextMeshPro>();
            startColor = hitText.color;
            //startColor.a = 0;
            currentColor = hitText.color;
            currentColor.a = showAlpha;
            startColor = hitText.color;
        }

    public void StartAnim(string hitString)
    {
        StopAllCoroutines();
        hitText.color = currentColor;
        hitText.text = hitString;
        StartCoroutine(PlayAnim());
    }

    public void StartAnim(string hitString, float animLength)
    {
        StopAllCoroutines();
        hitText.color = currentColor;
        hitText.text = hitString;
        StartCoroutine(PlayAnim(animLength));
    }

    IEnumerator PlayAnim(float animLength = 0.5f)
    {
        Color currentColor = hitText.color;

        float currentTime = 0;
        float animSpeedInSec = animLength;

        while (currentTime < animSpeedInSec)
        {
            currentTime += Time.deltaTime;
            hitText.color = Color.Lerp(currentColor, startColor, currentTime / animSpeedInSec);
            yield return null;
        }
        EndAnim();
        yield break;
    }

    public void EndAnim()
    {
        StopAllCoroutines();
        hitText.color = startColor;
    }

    //additional methods
    public void StartComboAnim(string comboString, float animLength)
    {
        StopAllCoroutines();
        hitText.color = currentColor;
        string newText = hitText.text;
        newText = newText.Substring(0, newText.IndexOf(">") + 1);
        hitText.text = newText + comboString;
        StartCoroutine(PlayComboAnim(animLength));
    }

    IEnumerator PlayComboAnim(float animLength = 0.5f)
    {
        string alp;
        string newTextAlpha = "";
        string oldTextAlpha = "<alpha=#99>";
        string newText = hitText.text;

        float currentTime = 0;
        float animSpeedInSec = animLength;

        while (currentTime < animSpeedInSec)
        {
            currentTime += Time.deltaTime;
            alp = Mathf.Lerp(99, 0, currentTime / animSpeedInSec).ToString("00");
            newTextAlpha = $"<alpha=#{alp}>";
            hitText.text = ((string)hitText.text).Replace(oldTextAlpha, newTextAlpha);
            oldTextAlpha = newTextAlpha;
            yield return null;
        }
        EndAnim();
        yield break;
    }
    
    public void SetAlpha(float newAlpha)
    {
        EndAnim();
        Color col = currentColor;
        col.a = newAlpha;
        hitText.color = col;
    }
}
