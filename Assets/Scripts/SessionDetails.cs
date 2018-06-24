using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SessionDetails : MonoBehaviour
{
    public static int Row = 1,
                      Score = 0,
                      Lives = 3,
                      CurrentCardLayer = 0,
                      TotalDrawnCards = 0;

    public DeckBehavior Deck;

    public List<GameObject> Hearts = new List<GameObject>() { };
    
    public Text rowText = null; 
    public Text scoreText = null;
    public Text bannerText = null;

    private string rowTextPrefix    = "Row: ";
    private string scoreTextPrefix  = "Score: ";
    private string bannerTextPrefix = "Next frame index: ";

    public bool LockCameraToActiveRow = true;
    private float newCameraY = 6.41f;
    private float cameraPosStep = 0.1f;

    // Use this for initialization
    void Start ()
    {
        if (Hearts.Count == 0)
            print("SessionDetails has no hearts game objs set! \r\n");

        if (rowText == null)
            print("UI rowText has no text to set! \r\n");

        if (scoreText == null)
            print("UI scoreText has no text to set! \r\n");

        if (bannerText == null)
            print("UI bannerText has no text to set! \r\n");
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Update the row based on the next draw target
        int nextIndex = Deck.nextFrameIndex;
                        
        if      ( nextIndex <= 1)  { Row = 1; }  //Zero *shouldn't* happen
        else if ( nextIndex <= 3)  { Row = 2; }
        else if ( nextIndex <= 6)  { Row = 3; }
        else if ( nextIndex <= 10) { Row = 4; }
        else if ( nextIndex <= 15) { Row = 5; }
        else                       { Row = 1; }  //This also *shouldn't* happen

        //Move the camera based on the row
        Camera[] cameras = new Camera[1];
        Camera.GetAllCameras(cameras);
        Camera cam = cameras[0];
        float x = cam.transform.position.x;
        float z = cam.transform.position.z;

        switch (Row)
        {
            case 1:  newCameraY = 6.41f;  break;
            case 2:  newCameraY = 6.41f;  break;
            case 3:  newCameraY = 1.6f;   break;
            case 4:  newCameraY = -6.17f; break;
            case 5:  newCameraY = -9.11f; break;
            default: newCameraY = 6.41f;  break;
        }

        //Move the camera towards the new Y position
        float moveY = cam.transform.position.y < newCameraY ? cameraPosStep : -cameraPosStep;
        Vector3 newPos = new Vector3(x, newCameraY, z);

        //Check if camera's Y position is within 1 'step' of Y distance
        if (Mathf.Abs(cam.transform.position.y - newPos.y) >= cameraPosStep)
        {
            cam.transform.position = new Vector3(cam.transform.position.x,
                                                 cam.transform.position.y + moveY,
                                                 cam.transform.position.z);
        }
        
        //Update row text
        if (rowText.text != rowTextPrefix + Row)
            rowText.text  = rowTextPrefix + Row;

        //Update score text
        if (scoreText.text != scoreTextPrefix + Score)
            scoreText.text  = scoreTextPrefix + Score;

        //Update banner text
        if (bannerText.text != bannerTextPrefix + Deck.nextFrameIndex)
            bannerText.text  = bannerTextPrefix + Deck.nextFrameIndex;
    }
}
