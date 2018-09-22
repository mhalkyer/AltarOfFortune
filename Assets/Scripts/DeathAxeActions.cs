using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAxeActions : MonoBehaviour {

    public AudioClip WhooshSfx,
                     GruntSfx;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    { 
		
	}

    public void Woosh()
    {
        //Play woosh
        GetComponent<AudioSource>().PlayOneShot(WhooshSfx);
    }

    public void Grunt()
    {
        //Play grunt
        GetComponent<AudioSource>().PlayOneShot(GruntSfx);
    }

    public void Hide()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public void Show()
    {
        GetComponent<SpriteRenderer>().enabled = true;
    }
}
