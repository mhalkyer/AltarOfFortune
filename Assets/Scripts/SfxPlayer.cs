using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxPlayer : MonoBehaviour {

    public List<AudioClip> EffectsToPickFrom;
    private int currentIndex = 0;
    public enum PlayStyle { Random, Sequential };
    public PlayStyle SfxPlayStyle;

    // Use this for initialization
    void Start ()
    {
        if (GetComponent<AudioSource>() == null)
            print("No Audio Source defined!! \r\n");
	}
	
	// Update is called once per frame
	void Update ()
    {
    }

    /// <summary>
    /// Selects a new clip to play next
    /// </summary>
    public void SelectNewClip()
    {
		if(SfxPlayStyle == PlayStyle.Random)
        {
            System.Random rand = new System.Random();
            currentIndex = rand.Next(EffectsToPickFrom.Count - 1);
        }
        else if(SfxPlayStyle == PlayStyle.Sequential)
        {
            if (EffectsToPickFrom.Count >= currentIndex + 1)
                currentIndex++;
            else
                currentIndex = 0;
        }

        AudioSource myAudioSource = GetComponent<AudioSource>();
        myAudioSource.clip = EffectsToPickFrom[currentIndex];
    }

    public void Play()
    {
        AudioSource myAudioSource = GetComponent<AudioSource>();
        myAudioSource.Play();
    }
}
