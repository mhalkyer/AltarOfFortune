using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class SnapAttach : MonoBehaviour
{
    public bool useAutoFree = false;            //With this enabled the script will "let go" of the card while still being held by the player.
    public int secsToWaitUntilAutoFree = 2;
    public int maxCapacity = 1;

    private const string GRABtag = "beingGrabbed";
    private const string FREEtag = "Untagged";
    private string NL = "\r\n";
    private List<GameObject> grabbedObjs = new List<GameObject>();

    private bool atCapacity
    {
        get
        {
            if (grabbedObjs.Count >= maxCapacity)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
        set
        {
            atCapacity = value;
        }
    }

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update()
    {
        List<GameObject> cardsToFree = new List<GameObject>();

        //Snap colliding obj to match my position
        foreach (GameObject g in grabbedObjs)
        {
            //If 'free' mark for removal, otherwise move it to the right position
            if (g.tag == FREEtag)
                cardsToFree.Add(g);
            else
                g.transform.position = transform.position;   
        }

        //Remove any freed cards
        foreach (GameObject g in cardsToFree)
            freeCard(g);
    }

    void OnMouseDown()
    {
        //If any object is being grabbed at my position and I'm clicked... Release them!
        //print("FREED all grabbed objs" + NL);
        //foreach (GameObject card in grabbedObjs)
        //    freeCard(card);

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        print(other.gameObject.name + " entered " + gameObject.name + "'s collider" + NL);

        if (!atCapacity && useAutoFree)
        {
            //Set colliding card/gameObject's tag to Grabbed
            grabCard(other.gameObject);

            //Wait X seconds until freeing
            StartCoroutine(delayedFreeCard(secsToWaitUntilAutoFree, other.gameObject));

            //Disable collider for snap logic for cooldown period
            print("Waiting " + 3 + "secs before snap grabbin' the next card" + NL);
            StartCoroutine(delayedToggleCollider(0, false));
            StartCoroutine(delayedToggleCollider(3, true));
        }
    }

    void grabCard(GameObject gObj)
    {
        StartCoroutine(delayedToggleCollider(0, false));
        
        //Set colliding gameObject's tag to Grabbed
        //print("GRABBED " + other.gameObject.name + NL);
        gObj.tag = GRABtag;

        //Add the colliding gameObject to the grabbed obj collection for tracking
        if (!grabbedObjs.Contains(gObj))
            grabbedObjs.Add(gObj);
    }

    void freeCard(GameObject gObj)
    {
        //print("FREED " + gObj.name + NL);

        //Remove the passed obj from the list of grabbed obj's
        grabbedObjs.Remove(gObj);
        
        //Set colliding gameObject's tag to Free/Untagged
        gObj.tag = FREEtag;

        StartCoroutine(delayedToggleCollider(3, true));
    }

    void OnTriggerStay2D(Collider2D other)
    {
    }

    void OnTriggerExit2D(Collider2D other)
    {
        freeCard(other.gameObject);
    }
    
    // Coroutines

    IEnumerator delayedToggleCollider(int secondsToWaitFor, bool enableValue)
    {
        yield return new WaitForSeconds(secondsToWaitFor);

        Collider2D myCollider = gameObject.GetComponent<Collider2D>();
        myCollider.enabled = enableValue;
    }

    IEnumerator delayedFreeCard(int secondsToWaitFor, GameObject gObj)
    {
        yield return new WaitForSeconds(secondsToWaitFor);
        freeCard(gObj);
    }

}
