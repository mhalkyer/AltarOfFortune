using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardBehavior : MonoBehaviour
{
    private const string GRABtag = "beingGrabbed";
    private const string FREEtag = "Untagged";
    private bool beingClicked = false;
    private Vector3 screenPoint, offset;
    private float scalingStep = 0.05f;
    private int topOfCards = 1;
    private List<GameObject> adjacentCards = new List<GameObject>();

    public int Row = -1;
    public int Index = -1;
    public bool ScaleOnClick = true;
    public bool ClickToDrag = false;
    public bool untagOnClick = true;
    public bool bringObjToFront = true;            //Done by changing the sprite renderer's sorting order
    public bool gridlikeMovement = false;
    public float gridMoveFactor = 0.3175f;
    public Vector3 scaleToIncreaseOnClick = new Vector3(1.25f, 1.25f, 1.25f);

    // Use this for initialization
    void Start ()
    { }

    void Update()
    {
        if (ScaleOnClick)
        {
            //Scale the card while clicked/unclicked
            if (beingClicked && transform.localScale.x < scaleToIncreaseOnClick.x)
            {
                transform.localScale = new Vector3(transform.localScale.x + scalingStep,
                                                   transform.localScale.y + scalingStep,
                                                   transform.localScale.z + scalingStep);

            }
            if (!beingClicked && transform.localScale.x > 1f)
            {
                transform.localScale = new Vector3(transform.localScale.x - scalingStep,
                                                   transform.localScale.y - scalingStep,
                                                   transform.localScale.z - scalingStep);

            }
        }
    }
    
    void OnMouseDown()
    {
        beingClicked = true;

        if (ClickToDrag)
        {
            if (untagOnClick)
                gameObject.tag = FREEtag;

            //Calculate offset of mouseclick on the sprite
            //Elegant click move w/ offset solution from https://stackoverflow.com/questions/40706412/drag-object-with-mouse-and-snap-it-to-value
            screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

            //Bring the object to the front of the sorting order
            if (bringObjToFront)
            {
                topOfCards++;
                GameObject[] cardSprites = GameObject.FindObjectsOfType<GameObject>(); //= GetComponents<SpriteRenderer>();

                //Reset all cards sorting order to zero
                foreach (GameObject card in cardSprites)
                {
                    SpriteRenderer sRenderer = card.GetComponent<SpriteRenderer>();
                    if (sRenderer && sRenderer.sortingLayerName.Equals("CardLayer"))
                        sRenderer.sortingOrder = 0;
                }

                //Set the current sprint sorting order to 1
                gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
        }
    }

    void OnMouseDrag()
    {
        if (ClickToDrag)
        {
            Vector3 curScreenPt = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 curPos = Camera.main.ScreenToWorldPoint(curScreenPt) + offset;

            //If not being "Grabbed" by the snapAttach script (See snapAttach.cs for more details)
            if ((string)gameObject.tag != GRABtag)
            {
                if (gridlikeMovement)
                    transform.position = new Vector3(Mathf.Round(curPos.x / gridMoveFactor) * gridMoveFactor, 
                                                     Mathf.Round(curPos.y / gridMoveFactor) * gridMoveFactor);
                else
                    transform.position = new Vector3(curPos.x, curPos.y);
            }
        }
    }

    void OnMouseUp()
    {
        beingClicked = false;
    }

    #region My old dumb way  ¯\_(ツ)_/¯
    //public float xOffset = 0.0f;
    //public float yOffset = 0.0f;
    //private bool beingClicked = false;
    // Update is called once per frame
    //void Update ()
    //   {
    //       Vector3 mousePos = Input.mousePosition;

    //       //Check if left-mouse button is clicked
    //       if (!beingClicked && Input.GetMouseButtonDown(0))
    //       {
    //           //Create ray from camera through mouse position
    //           RaycastHit hit;
    //           Ray ray = Camera.main.ScreenPointToRay(mousePos);

    //           if (Physics.Raycast(ray, out hit))
    //           {
    //               //Test if hit object is the gameobject's name
    //               if (hit.transform.name == gameObject.name)
    //               {
    //                   //print("My gameObject is clicked by mouse");
    //                   beingClicked = true;
    //               }
    //           }
    //       }

    //       //Check if left-mouse button is NOT clicked
    //       if (Input.GetMouseButtonUp(0))
    //       {
    //           beingClicked = false;
    //       }

    //       if (beingClicked)
    //           moveToMousePosition(mousePos);
    //   }

    //private void moveToMousePosition(Vector3 mousePos)
    //{
    //    //Convert the Screen Point mouse coordinates to World Point coordinates
    //    Vector2 localPosition = Camera.main.ScreenToWorldPoint(mousePos);

    //    //Move gameobject to the mouse coordinates
    //    float newX = localPosition.x,
    //          newY = localPosition.y;

    //    print(string.Format("Move {0} to ({1:F2},{2:F2})", gameObject.name, newX, newY));
    //    gameObject.transform.position = new Vector2(newX, newY);
    //}
    #endregion
}
