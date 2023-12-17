using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReadyAnimation : MonoBehaviour
{
    public GameObject readyText;
    private TMPro.TextMeshPro animText;
    private Color startColor;
    private Color endColor;
    private TMP_Text textComp;
    private float charWidth = 4;

    void Awake()
    {
        animText = readyText.GetComponent<TMPro.TextMeshPro>();
        startColor = animText.color;
        endColor = animText.color;
        endColor.a = 0.1f;
        StartCoroutine(PlayTextAnim());
    }

    void Start()
    {
        animText = readyText.GetComponent<TMPro.TextMeshPro>();
        startColor = animText.color;
        endColor = animText.color;
        endColor.a = 0.1f;
        StartCoroutine(PlayTextAnim());
    }

    public void StartAnim()
    {
        readyText.SetActive(true);
        textComp = readyText.GetComponent<TMP_Text>();
        StartCoroutine(PlayTextAnim());
    }

    IEnumerator PlayAnim()
    {
        //textComp.ForceMeshUpdate();
        var textInfo = textComp.textInfo;

        float CurrentTime = 0;
        float AnimLength = 1.5f;
        float LerpDistance = charWidth * 0.01f;///AnimLength;

        while (CurrentTime < AnimLength)
        {
            CurrentTime += Time.deltaTime;
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                verts[charInfo.vertexIndex + 0] = verts[charInfo.vertexIndex + 0] + new Vector3(Mathf.Lerp(0, i*LerpDistance, CurrentTime / AnimLength), 0, 0);
                verts[charInfo.vertexIndex + 1] = verts[charInfo.vertexIndex + 1] + new Vector3(Mathf.Lerp(0, i*LerpDistance, CurrentTime / AnimLength), 0, 0);
                verts[charInfo.vertexIndex + 2] = verts[charInfo.vertexIndex + 2] + new Vector3(Mathf.Lerp(0, i*LerpDistance, CurrentTime / AnimLength), 0, 0);
                verts[charInfo.vertexIndex + 3] = verts[charInfo.vertexIndex + 3] + new Vector3(Mathf.Lerp(0, i*LerpDistance, CurrentTime / AnimLength), 0, 0);
            }

            for(int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                var m = textInfo.meshInfo[i];
                m.mesh.vertices = m.vertices;
                textComp.UpdateGeometry(m.mesh, i);
            }
            yield return null;
        }
        EndAnim();
        yield break;
    }

    IEnumerator PlayTextAnim()
    {
        float currentTime = 0;
        float animLength = 1.5f;
        float animSpeedInSec = animLength;

        float oldSpace = -1;
        float newSpace = 0;

        while (currentTime < animSpeedInSec)
        {
            currentTime += Time.deltaTime;
            animText.color = Color.Lerp(startColor, endColor, currentTime / animSpeedInSec);
            newSpace = Mathf.Lerp(-1, 1.5f, currentTime / animSpeedInSec);
            animText.text = ((string)animText.text).Replace($"<cspace={oldSpace}em>", $"<cspace={newSpace}em>");
            oldSpace = newSpace;
            yield return null;
        }
        EndAnim();
        yield break;
    }

    public void EndAnim()
    {
        readyText.SetActive(false);
        Destroy(gameObject);
    }
}
