using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class SessionDetails : MonoBehaviour
{
    public int CurrentRow = 1,
               TempScore = 0,
               BankedScore = 0,
               Lives = 3,
               CurrentCardLayer = 0,
               TotalDrawnCards = 0;

    public bool CheckRulesNextUpdate = false;

    public DeckBehavior Deck;

    public enum GameMode { DefaultTowerRules, KilledByRoyalty }

    public GameMode myGameMode = GameMode.DefaultTowerRules;

    public Sprite fullHeartSprite,
                  emptyHeartSprite;

    public List<GameObject> Hearts = new List<GameObject>() { };

    public GameObject pointsEffect,
                      deathAxeEffect,
                      CurrentCard;
                     
    public EnvelopeBehavior Envelope;

    public Text TopRightText          = null,
                TopRightTextShadow    = null,
                BottomRightText       = null,
                BottomRightTextShadow = null,
                TopLeftText           = null,
                TopLeftTextShadow     = null;

    private string banner = string.Empty;
    private readonly string NL = "\r\n";

    public bool LockCameraToActiveRow = true;

    private float newCameraY = 6.41f,
                  cameraPosStep = 0.2f;

    void Start ()
    {
        if (Hearts.Count == 0)
            print("SessionDetails has no hearts game objs set!" + NL);

        if (TopRightText == null)
            print("UI rowText has no text to set!" + NL);

        if (BottomRightText == null)
            print("UI scoreText has no text to set!" + NL);

        if (TopLeftText == null)
            print("UI bannerText has no text to set!" + NL);

        if (pointsEffect == null)
            print("No particle effect defined!" + NL);

        if (Envelope == null)
            print("No Envelope defined!" + NL);
    }
	
	void Update ()
    {
        //Check lives for endgame
        if (Lives <= 0)
            GameOver();

        //Update the row based on the next draw target
        int nextIndex = Deck.nextFrameIndex;

        if (nextIndex <= 0) { CurrentRow = 1; }
        else if (nextIndex <= 3) { CurrentRow = 2; }
        else if (nextIndex <= 6) { CurrentRow = 3; }
        else if (nextIndex <= 10) { CurrentRow = 4; }
        else if (nextIndex <= 15) { CurrentRow = 5; }
        else { CurrentRow = 1; }       //This *shouldn't* happen

        MoveCameraToCurrentRow();
        UpdateTopLeftText();
        UpdateTopRightText();
        UpdateBottomRightText();

        //Check if condition is met to show the envelope
        if (TempScore > 500 && Envelope.VisibleInTheScene == false)
            Envelope.EnterScene();

        if (CheckRulesNextUpdate)
            CheckTheRules();
    }

    private void UpdateTopLeftText()
    {
        if (banner != string.Empty && TopLeftText.text != banner)
        {
            TopLeftText.text = banner;
            TopLeftTextShadow.text = banner;
        }
    }

    private void UpdateTopRightText()
    {
        const string rowTextPrefix = "Row: ";
        string OnscreenRowText = rowTextPrefix + CurrentRow;
        if (TopRightText.text != OnscreenRowText)
        {
            TopRightText.text = OnscreenRowText;
            TopRightTextShadow.text = OnscreenRowText;
        }
    }

    private void UpdateBottomRightText()
    {
        const string tempTextPrefix = "Score: ";
        const string bankTextPrefix = "Bank: ";

        var OnscreenScoreText = tempTextPrefix + String.Format("{0:D5}", TempScore) + NL +
                                bankTextPrefix + String.Format("{0:D5}", BankedScore);

        if (BottomRightText.text != OnscreenScoreText)
        {
            BottomRightText.text       = OnscreenScoreText;
            BottomRightTextShadow.text = OnscreenScoreText;
        }
    }

    private void MoveCameraToCurrentRow()
    {
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
    }

    private static Camera getMainCamera()
    {
        Camera[] cameras = new Camera[1];
        Camera.GetAllCameras(cameras);
        Camera cam = cameras[0];
        return cam;
    }

    public void TriggerRulesCheck()
    {
        CheckRulesNextUpdate = true;
    }

    /// <summary>
    /// Check the state of all cards and trigger any needed events
    /// </summary>
    private void CheckTheRules()
    {
        CheckRulesNextUpdate = false;

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

                    if (TempScore < 300)
                        TempScore = 0;
                    else
                        TempScore -= 300;
                }

                //If there's no matching cards, play coin effect and add points
                if (matchingCards.Count == 0)
                {
                    TempScore += 150;
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
                    if (TempScore >= axeScoreDamage)
                        TempScore -= axeScoreDamage;
                    else
                        TempScore = 0;
                }
                else
                {
                    TempScore += 100;
                    PlayParticleEffect(CurrentCard);
                }
                break;

        }
        return;
    }

    public void BankScore()
    {
        BankedScore += TempScore;
        TempScore = 0;
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
        TempScore = 0;
        TopLeftText.color = Color.red;
        UpdateBanner("GAME OVER!");
        
        Deck.SetNextCardClickToShuffle();
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
        GameObject effect = Instantiate(pointsEffect, 
                                        targetCard.transform.position, 
                                        pointsEffect.transform.rotation);

        ParticleSystem particles = effect.GetComponent<ParticleSystem>();
        particles.Play();

        //Destroy the effect after it finishes playing
        Destroy(effect, particles.main.startLifetime.constant);
    }

    public void UpdateBanner(string newText)
    {
        banner = newText;
    }
}
