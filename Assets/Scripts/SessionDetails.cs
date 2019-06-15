using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class SessionDetails : MonoBehaviour
{
    //--------------------------------------------------------------------------------------//
    // PUBLIC FIELDS         ---------------------------------------------------------------//
    //--------------------------------------------------------------------------------------//

    public int              CurrentRow = 1;
    public int              TempScore = 0;
    public int              BankedScore = 0;
    public int              Lives = 3;
    public int              CurrentCardLayer = 0;
    public int              TotalDrawnCards = 0;
    public bool             CheckRulesNextUpdate = false;
    public DeckBehavior     Deck;
    public enum GameMode    { DefaultTowerRules, KilledByRoyalty }
    public GameMode         myGameMode = GameMode.DefaultTowerRules;
    public Sprite           fullHeartSprite;
    public Sprite           emptyHeartSprite;
    public List<GameObject> Hearts = new List<GameObject>() { };
    public GameObject       pointsEffect;
    public GameObject       deathAxeEffect;
    public GameObject       EndGamePanel;
    public EnvelopeBehavior Envelope;
    public Text             TopRightText          = null;
    public Text             TopRightTextShadow    = null;
    public Text             BottomRightText       = null;
    public Text             BottomRightTextShadow = null;
    public Text             TopLeftText           = null;
    public Text             TopLeftTextShadow     = null;
    public GameObject       CurrentCard { get; set; }
    public bool             LockCameraToActiveRow = true;
    public SfxPlayer        CoinSoundEffectPlayer = null;

    //--------------------------------------------------------------------------------------//
    // PRIVATE FIELDS        ---------------------------------------------------------------//
    //--------------------------------------------------------------------------------------//

    private string                  banner { get; set; }
    private float                   cameraPosStep = 0.2f;
    private float                   newCameraY = 6.41f;
    private readonly string         DEFENDER = "Defender";
    private readonly string         NL = "\r\n";
    private Dictionary<string, int> PlayerPerks = new Dictionary<string, int>();

    //--------------------------------------------------------------------------------------//
    // PUBLIC FUNCTIONS      ---------------------------------------------------------------//
    //--------------------------------------------------------------------------------------//

    public void TriggerRulesCheck()
    {
        CheckRulesNextUpdate = true;
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

    public void ResetLives()
    {
        Lives = 3;

        foreach (SpriteRenderer renderer in Hearts.Select(e => e.GetComponent<SpriteRenderer>()))
            renderer.sprite = fullHeartSprite;
    }

    public void UpdateBanner(string newText, Color newColor)
    {
        banner = newText;
        TopLeftText.color = newColor;
    }

    //--------------------------------------------------------------------------------------//
    // PRIVATE FUNCTIONS      --------------------------------------------------------------//
    //--------------------------------------------------------------------------------------//

    private void Start ()
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

    private void Update ()
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
        if (banner           != string.Empty    && 
            TopLeftText.text != banner)
        {
            TopLeftText.text        = banner;
            TopLeftTextShadow.text  = banner;
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
    
    /// <summary>
    /// Check the state of all cards and trigger any needed events
    /// </summary>
    private void CheckTheRules()
    {
        CheckRulesNextUpdate = false;

        switch(myGameMode)
        {
            case GameMode.DefaultTowerRules:        // Inspired by one of my favorite card games, Fortunes Tower from Fable 2 - <3

                GameObject[] gObjs = FindObjectsOfType<GameObject>();

                //Get all card objs
                var allCards = from GameObject g in gObjs
                              where   g.GetComponent<CardBehavior>() != null
                              orderby g.GetComponent<CardBehavior>().Index
                              select  g;

                //-------------------------------------------------------------------------------------------------------------------------//
                // DA RULES                             -----------------------------------------------------------------------------------//
                //-------------------------------------------------------------------------------------------------------------------------//

                // ------------------
                // RULE 1 : DEFENDERS
                // ------------------
                
                // Check for Trigger Card to add Player "Defender" (blocks attacks)            
                if (CurrentCard.name.ToLower().Contains("king"))
                {
                    if (PlayerPerks.ContainsKey(DEFENDER))      //Increment the defender count
                    {
                        int newValue = PlayerPerks[DEFENDER];
                        newValue++;
                        PlayerPerks[DEFENDER] = newValue;
                        UpdateBanner("You now have " + PlayerPerks[DEFENDER] + " Defenders!", Color.yellow);

                    }
                    else
                    {
                        PlayerPerks.Add(DEFENDER, 1);           //Set the defender count to one
                        UpdateBanner("You now have a Defender!", Color.yellow);
                    }
                }

                // ---------------------
                // RULE 2 : NUMBER MATCH
                // ---------------------

                // Build list of adjacent cards from adjacent indexes
                List<int> adjacentCardIndexes = TowerRules.getAdjacentIndexes(CurrentCard.GetComponent<CardBehavior>().Index);
                List<GameObject> adjacentCards = new List<GameObject>();
                foreach (int i in adjacentCardIndexes)
                {
                    GameObject adjCard = allCards.FirstOrDefault(g => g.GetComponent<CardBehavior>().Index == i);

                    if (adjCard != null)
                        adjacentCards.Add(adjCard);
                }

                // Check if the adjacent card number matches the current card number
                int CurrentCardNum = GetNumberFromCardName(CurrentCard.name);

                List<GameObject> matchingCards = adjacentCards.Where(g => GetNumberFromCardName(g.name) == CurrentCardNum).ToList();
                bool cardMatch = matchingCards.Any();

                if(cardMatch)
                {
                    // If the player has a defender, block the attack
                    int defenders = 0;
                    PlayerPerks.TryGetValue(DEFENDER, out defenders);
                    if (PlayerPerks.ContainsKey(DEFENDER) && defenders > 0)      
                    {
                        PlayerPerks[DEFENDER] = defenders - 1;
                        UpdateBanner("Defender Blocked You!" + NL + 
                                     PlayerPerks[DEFENDER] + " Defenders left.", Color.white);

                    }
                    else // Otherwise take damage
                    {
                        SpawnAnimatedWeaponOnTarget(CurrentCard);
                        DecrementScore(300);
                        UpdateBanner("Ouch!", Color.red);
                    }
                }

                // ------------------------
                // RULE 3 : STOLE TREASURE!
                // ------------------------
                
                //If there were no matching cards above, play coin effect and add points
                if (!cardMatch)
                {
                    CoinSoundEffectPlayer.SelectNewClip();
                    CoinSoundEffectPlayer.Play();
                    UpdateBanner("You got 150 coins!", Color.yellow);
                    IncrementScore(150);
                    PlayParticleEffect(CurrentCard);
                }

                //-------------------------------------------------------------------------------------------------------------------------//
                // END OF DA RULES <3                   -----------------------------------------------------------------------------------//
                //-------------------------------------------------------------------------------------------------------------------------//

                break;

            case GameMode.KilledByRoyalty:

                if (CurrentCard.name.ToLower().Contains("king" ) ||
                    CurrentCard.name.ToLower().Contains("queen") ||
                    CurrentCard.name.ToLower().Contains("jack" ) )
                {
                    SpawnAnimatedWeaponOnTarget(CurrentCard);

                    //Subtract from score
                    int axeScoreDamage = 300;
                    DecrementScore(axeScoreDamage);
                }
                else
                {
                    IncrementScore(100);
                    PlayParticleEffect(CurrentCard);
                }
                break;

        }
        return;
    }

    /// <summary>
    /// Returns the number found in a card name. Returns -1 if nothing is found.
    /// </summary>
    /// <param name="cardName">String to inspect for a number inside</param>
    /// <returns></returns>
    private int GetNumberFromCardName(string cardName)
    {
        int[] numbers = { 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        foreach (int num in numbers)
            if(cardName.Contains(num.ToString()))
                return num;

        return -1;
    }

    private void IncrementScore(int amount)
    {
        amount = Math.Abs(amount);  //Make sure the amount is postive
        TempScore += amount;
    }

    private void DecrementScore(int amount)
    {
        amount = Math.Abs(amount);  //Make sure the amount is postive
        if (TempScore < amount)
            TempScore = 0;
        else
            TempScore -= amount;
    }

    private void GameOver()
    {
        EndGamePanel.SetActive(true);
        TempScore = 0;
        UpdateBanner("GAME OVER!", Color.red);
        
        Deck.SetNextCardClickToShuffle();
    }

    private void SpawnAnimatedWeaponOnTarget(GameObject targetObj)
    {
        float AnimXOffset =  -0.7f;
        float AnimYOffset = -12.0f;

        Vector3 newPos = new Vector3(targetObj.transform.position.x + AnimXOffset,  // Animation start is off-screen from target
                                     targetObj.transform.position.y + AnimYOffset,
                                     deathAxeEffect.transform.position.z);          // Leave Z unchanged

        GameObject newDeathAxeEffect = Instantiate(deathAxeEffect, newPos, new Quaternion());

        // Update the main camera of the death axe (for shake effect)
        newDeathAxeEffect.GetComponentInChildren<DeathAxeActions>().mainCamera = getMainCamera();

        // Move death axe to right position to hit target card
        newDeathAxeEffect.transform.position = newPos;

        // Animate death axe! 
        // Using the extra 2 parameters helps it reset (?)                       //Layer  //Normalized Time Offset (between 0-1)
        newDeathAxeEffect.GetComponentInChildren<Animator>().Play("axeFlyIn",   -1,       0f);
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
}
