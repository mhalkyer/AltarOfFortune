using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeathAxeActions : MonoBehaviour {

    //--------------------------------------------------------------------------------------//
    // PUBLIC FIELDS         ---------------------------------------------------------------//
    //--------------------------------------------------------------------------------------//

    public AudioClip    WhooshSfx,
                        GruntSfx;
    public Camera       mainCamera;
    public int          shakeBounces = 3;
    public float        shakeZRotMin = -2.5f,
                        shakeZRotMax = 2.5f;

    //--------------------------------------------------------------------------------------//
    // PRIVATE FIELDS         --------------------------------------------------------------//
    //--------------------------------------------------------------------------------------//

    private bool                shakeCamera = false;
    private int                 currectShakeBounces = 0;
    private enum ShakeDirection { LEFT, RIGHT };
    private ShakeDirection      currentShakeDirection = ShakeDirection.LEFT;
    private float               rotationStep = 0.5f;

    //--------------------------------------------------------------------------------------//
    // PUBLIC FUNCTIONS      ---------------------------------------------------------------//
    //--------------------------------------------------------------------------------------//

    public void Woosh()
    {
        //Play woosh
        GetComponent<AudioSource>().PlayOneShot(WhooshSfx);
    }

    public void GruntAndShake()
    {
        //Play grunt
        GetComponent<AudioSource>().PlayOneShot(GruntSfx);

        //Shake Camera
        currectShakeBounces = 0;
        //shakeCamera = true;

        //Take Damage
        GameObject sessionObj = FindObjectsOfType<GameObject>().Where(g => g.GetComponent<SessionDetails>() != null).FirstOrDefault();
        SessionDetails session = sessionObj.GetComponent<SessionDetails>();
        session.TakeDamage(1);
    }

    public void Hide()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public void Show()
    {
        GetComponent<SpriteRenderer>().enabled = true;
    }

    //--------------------------------------------------------------------------------------//
    // PRIVATE FUNCTIONS      --------------------------------------------------------------//
    //--------------------------------------------------------------------------------------//

    private void Start()
    {

    }

    private void Update()
    {
        if (shakeCamera)
        {

            //Get current rotation values
            //float XRot = mainCamera.transform.rotation.x;
            //float YRot = mainCamera.transform.rotation.y;
            float ZRot = mainCamera.transform.rotation.z;
            //float WRot = mainCamera.transform.rotation.w;

            //Shake main camera
            //Order of Camera "Shake" operations (aka applying Z Rotation left and right)
            //1. Move Camera Z Rot to shakeZRotMin
            //2. Move Camera Z Rot to shakeZRotMax
            //3. At zero, increment currectShakeBounces
            //4. Repeat until currectShakeBounces == shakeBounces

            float RotStep;
            if (currentShakeDirection == ShakeDirection.RIGHT)
                RotStep = rotationStep * -1;
            else
                RotStep = rotationStep;

            mainCamera.transform.Rotate(new Vector3(0f, 0f, RotStep));
            //mainCamera.transform.rotation.SetFromToRotation(mainCamera.transform.rotation,);

            ZRot = mainCamera.transform.rotation.z;
            if (ZRot <= -5)
            {
                currentShakeDirection = ShakeDirection.LEFT;
            }

            if (ZRot >= 5)
            {
                currentShakeDirection = ShakeDirection.RIGHT;
            }

            if (ZRot == 0.0f)
                currectShakeBounces++;

            if (currectShakeBounces == shakeBounces)
                shakeCamera = false;
        }
    }

}
