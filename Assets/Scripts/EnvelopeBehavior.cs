using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvelopeBehavior : MonoBehaviour {
    
    public bool VisibleInTheScene = false;
    public SessionDetails Session = null;
    public AudioSource OnClickSound = null;
    public Animator MyAnimator = null;

    private readonly string NL = "\r\n";

    // Use this for initialization
    void Start ()
    {
        if(Session==null)
            print("SessionDetails is not set!" + NL);

        if (OnClickSound == null)
            print("OnClickSound is not set!" + NL);

        if (MyAnimator == null)
            print("MyAnimator is not set!" + NL);
    }
	
	// Update is called once per frame
	void Update ()
    {
	}

    void OnMouseDown()
    {
        if (VisibleInTheScene)
        {
            OnClickSound.Play();
            Session.BankScore();
            LeaveScene();
        }
    }

    public void LeaveScene()
    {
        MyAnimator.SetBool("EnteringScene", false);
        MyAnimator.SetBool("LeavingScene",  true);
    }

    public void EnterScene()
    {
        MyAnimator.SetBool("LeavingScene",  false);
        MyAnimator.SetBool("EnteringScene", true);
    }

    public void NowVisibleInScene()
    {
        VisibleInTheScene = true;
    }

    public void NotVisibleInScene()
    {
        VisibleInTheScene = false;
    }
}
