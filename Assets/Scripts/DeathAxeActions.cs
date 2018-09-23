using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAxeActions : MonoBehaviour {

    public AudioClip WhooshSfx,
                     GruntSfx;

    public Camera mainCamera;
    public int shakeBounces = 3;

    public float shakeZRotMin = -2.5f,
                 shakeZRotMax = 2.5f;

    private bool shakeCamera = false;
    private int currectShakeBounces = 0;
    private enum ShakeDirection { LEFT, RIGHT };
    private ShakeDirection currentShakeDirection = ShakeDirection.LEFT;
    private float rotationStep = 0.5f;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (shakeCamera)
        {
            float XRot = 0f, YRot = 0f, ZRot = 0f, WRot = 0f, RotStep = 0f;

            //Get current rotation values
            XRot = mainCamera.transform.rotation.x;
            YRot = mainCamera.transform.rotation.y;
            ZRot = mainCamera.transform.rotation.z;
            WRot = mainCamera.transform.rotation.w;

            //Shake main camera
            //Order of Camera "Shake" operations (aka applying Z Rotation left and right)
            //1. Move Camera Z Rot to shakeZRotMin
            //2. Move Camera Z Rot to shakeZRotMax
            //3. At zero, increment currectShakeBounces
            //4. Repeat until currectShakeBounces == shakeBounces

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
