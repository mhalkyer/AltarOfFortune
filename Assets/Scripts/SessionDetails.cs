using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SessionDetails : MonoBehaviour
{
    public int CurrentRow = 1,
               Score = 0,
               Lives = 3,
               CurrentCardLayer = 0,
               TotalDrawnCards = 0;

    public enum GameMode { DefaultTowerRules, KilledByRoyalty }

    public GameMode myGameMode = GameMode.DefaultTowerRules;

    public Sprite fullHeartSprite,
                  emptyHeartSprite;

    public List<GameObject> Hearts = new List<GameObject>() { };

    public GameObject particleObject,
                      deathAxeEffect,
                      CurrentCard;

    public Text rowText          = null,
                rowTextShadow    = null,
                scoreText        = null,
                scoreTextShadow  = null,
                bannerText       = null,
                bannerTextShadow = null;

    private readonly string rowTextPrefix    = "Row: ",
                            scoreTextPrefix  = "Score: ",
                            NL               = "\r\n";

    private string banner = string.Empty;

    public bool LockCameraToActiveRow = true;

    private float newCameraY = 6.41f,
                  cameraPosStep = 0.2f;

    // Use this for initialization
    void Start ()
    {
        if (Hearts.Count == 0)
            print("SessionDetails has no hearts game objs set!" + NL);

        if (rowText == null)
            print("UI rowText has no text to set!" + NL);

        if (scoreText == null)
            print("UI scoreText has no text to set!" + NL);

        if (bannerText == null)
            print("UI bannerText has no text to set!" + NL);

        if (particleObject == null)
            print("No particle effect defined!" + NL);
    }
	
	// Update is called once per frame
	void Update ()
    {
        DeckBehavior Deck = GetComponent<DeckBehavior>();

        //Update the row based on the next draw target
        int nextIndex = Deck.nextFrameIndex;

        if      (nextIndex <= 0)  { CurrentRow = 1; }
        else if (nextIndex <= 3)  { CurrentRow = 2; }
        else if (nextIndex <= 6)  { CurrentRow = 3; }
        else if (nextIndex <= 10) { CurrentRow = 4; }
        else if (nextIndex <= 15) { CurrentRow = 5; }
        else                      { CurrentRow = 1; }                           //This *shouldn't* happen

        //Move the camera based on the row
        Camera cam = getMainCamera();
        float x = cam.transform.position.x;
        float z = cam.transform.position.z;

        switch (CurrentRow)
        {
            case 1:  newCameraY = 6.41f;  break;
            case 2:  newCameraY = 6.41f;  break;
            case 3:  newCameraY = 1.6f;   break;
            case 4:  newCameraY = -6.17f; break;
            case 5:  newCameraY = -9.11f; break;
            default: newCameraY = 6.41f;  break;
        }

        //Move the camera towards the new Y position
        //Check if camera's Y position is within 1 'step' of the target Y position
        //if not move another step towards the target Y
        Vector3 newPos = new Vector3(x, newCameraY, z);
        if (cameraPosStep <= Mathf.Abs(cam.transform.position.y - newPos.y))
        {
            float moveY = cam.transform.position.y < newCameraY ? cameraPosStep : -cameraPosStep;
            cam.transform.position = new Vector3(cam.transform.position.x,
                                                 cam.transform.position.y + moveY,
                                                 cam.transform.position.z);
        }

        //Update row text
        string newText = rowTextPrefix + CurrentRow;
        if (rowText.text != newText)
        {
            rowText.text = newText;
            rowTextShadow.text = newText;
        }

        //Update score text
        newText = scoreTextPrefix + Score;
        if (scoreText.text != newText)
        {
            scoreText.text = newText;
            scoreTextShadow.text = newText;
        }

        //Update banner text
        if (banner != string.Empty && bannerText.text != banner)
        {
            bannerText.text = banner;
            bannerTextShadow.text = banner;
        }      

        //Check lives for endgame
        if (Lives <= 0)
        {
            GameOver();
            bannerText.color = Color.red;
        }
    }

    private static Camera getMainCamera()
    {
        Camera[] cameras = new Camera[1];
        Camera.GetAllCameras(cameras);
        Camera cam = cameras[0];
        return cam;
    }

    // Check the state of all cards and trigger any needed events
    public void CheckTheRules()
    {
        switch(myGameMode)
        {
            case GameMode.DefaultTowerRules:

                GameObject[] gObjs = FindObjectsOfType<GameObject>();

                //Get all card objs
                var allCards = from GameObject g in gObjs
                              where   g.GetComponent<CardBehavior>() != null
                              orderby g.GetComponent<CardBehavior>().Index
                              select  g;

                //Build list of adjacent cards from adjacent indexes
                List<int> adjacentCardIndexes = TowerRules.getAdjacentIndexes(CurrentCard.GetComponent<CardBehavior>().Index);
                List<GameObject> adjacentCards = new List<GameObject>();
                foreach (int i in adjacentCardIndexes)
                {
                    GameObject adjCard = allCards.FirstOrDefault(g => g.GetComponent<CardBehavior>().Index == i);

                    if (adjCard != null)
                        adjacentCards.Add(adjCard);

                }

                //Check if the adjacent cards match the current card
                List<GameObject> matchingCards = adjacentCards.Where(g => g.name == CurrentCard.name).ToList();
                foreach (GameObject gObj in matchingCards)
                {
                    SpawnAnimatedWeaponOnTarget(CurrentCard);

                    if (Score < 300)
                        Score = 0;
                    else
                        Score -= 300;
                }

                //If there's no matching cards, play coin effect and add points
                if (matchingCards.Count == 0)
                {
                    Score += 150;
                    PlayParticleEffect(CurrentCard);
                }

                break;



            case GameMode.KilledByRoyalty:

                if (CurrentCard.name.ToLower().Contains("king" ) ||
                    CurrentCard.name.ToLower().Contains("queen") ||
                    CurrentCard.name.ToLower().Contains("jack" ) )
                {
                    SpawnAnimatedWeaponOnTarget(CurrentCard);

                    //Subtract from score
                    int axeScoreDamage = 300;
                    if (Score >= axeScoreDamage)
                        Score -= axeScoreDamage;
                    else
                        Score = 0;
                }
                else
                {
                    Score += 100;
                    PlayParticleEffect(CurrentCard);
                }
                break;


        }
        return;
    }

    public void TakeDamage(int damage)
    {
        Lives -= damage;

        switch(Lives)
        {
            case 3:
                break;

            case 2:
                Hearts[2].GetComponent<SpriteRenderer>().sprite = emptyHeartSprite;
                break;

            case 1:
                Hearts[1].GetComponent<SpriteRenderer>().sprite = emptyHeartSprite;
                break;

            case 0:
                Hearts[0].GetComponent<SpriteRenderer>().sprite = emptyHeartSprite;
                break;
        }
    }

    private void GameOver()
    {
        updateBanner("GAME OVER!");
        
        DeckBehavior Deck = GetComponent<DeckBehavior>();
        Deck.SetNextCardClickToShuffle();

        Score = 0;
    }

    public void ResetLives()
    {
        Lives = 3;

        foreach (SpriteRenderer renderer in Hearts.Select(e => e.GetComponent<SpriteRenderer>()))
            renderer.sprite = fullHeartSprite;
    }

    private void SpawnAnimatedWeaponOnTarget(GameObject targetObj)
    {
        Vector3 newPos = new Vector3(targetObj.transform.position.x - 0.7f,  //Animation start is off-screen from target
                                     targetObj.transform.position.y - 12.0f,
                                     deathAxeEffect.transform.position.z);

        GameObject newDeathAxeEffect = Instantiate(deathAxeEffect, newPos, new Quaternion());

        //Assign the main camera to the death axe (for shake)
        newDeathAxeEffect.GetComponentInChildren<DeathAxeActions>().mainCamera = getMainCamera();

        //Move death axe to target card
        newDeathAxeEffect.transform.position = newPos;

        //Animate death axe!
        newDeathAxeEffect.GetComponentInChildren<Animator>().Play("axeFlyIn", -1, 0f);     //Using the extra 2 parameters helps it reset (?)
    }

    private void PlayParticleEffect(GameObject targetCard)
    {
        //Create and play particle effect
        GameObject goldCoinParticleEffect = Instantiate(particleObject, targetCard.transform.position, new Quaternion());
        goldCoinParticleEffect.GetComponent<ParticleSystem>().Play();

        //Destroy the effect after it finishes playing
        Destroy(goldCoinParticleEffect, goldCoinParticleEffect.GetComponent<ParticleSystem>().main.duration);
    }

    public void updateBanner(string newText)
    {
        banner = newText;
    }
}
