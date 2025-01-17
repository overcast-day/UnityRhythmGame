using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CalLane : MonoBehaviour
{
    public AudioSource hitSFX;
    public AudioSource missSFX;
    public AudioSource beatSFX;
    public AudioSource perfectSFX;

    public GameObject hitAnimation;
    private HitAnimation hitScript;
    public GameObject visualGuide;
    SpriteRenderer visualSprite;

    private Vector3 visualScale;
    public Vector3 changeScale = new Vector3(0.15f,0.15f,0.15f);

    private Color visualColor;
    public Color missColor = new Color(1, 0.2f, 0.2f);
    public Color hitColor = new Color(0.4f, 0.7f, 0.9f);  //nice light blue 0.1f, 0.8f, 1
    public Color perfectHitColor = new Color(0, 1, 0); //darker green 0.1f, 0.7f, 0.3f

    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    public KeyCode input;
    public GameObject notePrefab;
    List<CalNote> notes = new List<CalNote>();
    public List<double> timeStamps = new List<double>(); // each tap needs a timestamp

    int spawnIndex = 0;
    int inputIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        hitScript = hitAnimation.GetComponent<HitAnimation>();
        visualSprite = visualGuide.GetComponent<SpriteRenderer>();
        visualColor = visualSprite.color;
        visualScale = visualSprite.transform.localScale;
    }

    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] array)
    {
        foreach (var note in array)
        {
            if (note.NoteName == noteRestriction)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, CalConductor.SongMidi.GetTempoMap());
                timeStamps.Add((double)(metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f));   
            }
        }
        InternalGameLog.LogMessage("In this lane there are " + timeStamps.Count + " notes");
        //SharedData.maxScore += timeStamps.Count*2;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnIndex < timeStamps.Count)
        {
            if (CalConductor.GetMusicSourceTime() >= timeStamps[spawnIndex] - CalConductor.Instance.noteTime)
            {
                var note = Instantiate(notePrefab, transform);
                notes.Add(note.GetComponent<CalNote>());
                note.GetComponent<CalNote>().assignedTime = (float)timeStamps[spawnIndex];
                spawnIndex++;
            }
        }

        if (inputIndex < timeStamps.Count)
        {
            double timeStamp = timeStamps[inputIndex];
            double GoodHitThreshold = CalConductor.Instance.GoodHitThreshold;
            double PerfectHitThreshold = CalConductor.Instance.PerfectHitThreshold;
            double audioTime = CalConductor.GetMusicSourceTime() - (SharedData.inputDelay / 1000.0); //(CalConductor.Instance.InputDelayInMilliseconds / 1000.0);

            //for audio sync testing
            if ((Math.Abs(audioTime - timeStamp) < 0.025) && SharedData.debugMode)
            {
                //Beat();
                PerfectHit();
                Destroy(notes[inputIndex].gameObject);
                inputIndex++;
            }
            //

            if (Input.GetKeyDown(input))
            {
                //StartCoroutine(AnimateKeyPress(new Color(0.4f, 0.7f, 0.9f))); //for testing
                if (Math.Abs(audioTime - timeStamp) < PerfectHitThreshold)
                {
                    PerfectHit();
                    InternalGameLog.LogMessage($"Perfect hit on {inputIndex} note");
                    Destroy(notes[inputIndex].gameObject);
                    inputIndex++;
                }
                else if (Math.Abs(audioTime - timeStamp) < GoodHitThreshold)
                {
                    Hit();
                    InternalGameLog.LogMessage($"Hit on {inputIndex} note");
                    Destroy(notes[inputIndex].gameObject);
                    inputIndex++;
                }
                else
                {
                    InternalGameLog.LogMessage($"Hit inaccurate on {inputIndex} note with {Math.Abs(audioTime - timeStamp)} delay");
                    StartCoroutine(AnimateKeyPress(missColor));
                    hitScript.StartAnim("Miss");
                    Miss();
                }
            }

            if (timeStamp + GoodHitThreshold <= audioTime)
            {
                InternalGameLog.LogMessage($"Missed {inputIndex} note");
                inputIndex++;
            }
        }

    }
    private void Hit()
    {
        StartCoroutine(AnimateKeyPress(hitColor));
        hitScript.StartAnim("Nice");
        hitSFX.Play();
    }
    private void PerfectHit()
    {
        StartCoroutine(AnimateKeyPress(perfectHitColor));
        hitScript.StartAnim("Perfect");
        perfectSFX.Play();
    }
    private void Miss()
    {
        missSFX.Play();
    }
    private void Beat()
    {
        beatSFX.Play();
    }
    private IEnumerator AnimateKeyPress(Color cr)
    {
        Color pressedColor = cr; // new Color(1, 0, 0);
        pressedColor.a = visualColor.a; //Set Alpha to match original colors
        visualSprite.color = pressedColor; 

        visualSprite.transform.localScale += changeScale;
        Vector3 pressedScale = visualSprite.transform.localScale;
        float animSpeedInSec = 0.2f;
        float counter = 0;

        //Fade Back
        while (counter < animSpeedInSec)
        {
            counter += Time.deltaTime;
            visualSprite.color = Color.Lerp(pressedColor, visualColor, counter / animSpeedInSec);
            visualSprite.transform.localScale = Vector3.Lerp(pressedScale, visualScale, counter / animSpeedInSec);
            yield return null;
        }

        yield break;
    }
}
