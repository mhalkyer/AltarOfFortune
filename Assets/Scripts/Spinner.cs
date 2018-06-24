using UnityEngine;
using System.Collections;

public class Spinner : MonoBehaviour
{
    public enum axisOption { X, Y, Z, XY, XZ, YZ, XYZ }
    public axisOption rotateAxis = axisOption.Y;
    public float rotateSpeed = 1.0f;
    public Sprite cardBack;
    
    private Sprite cardFront;

    private bool backIsShown
    {
        get
        {
            //int numOfAxesFlipped = 0;
            float xRot = transform.localEulerAngles.x, 
                  yRot = transform.localEulerAngles.y, 
                  zRot = transform.localEulerAngles.z;

            //Adjust the rotation value to be between -180 <----> 180
            if (xRot >= 270)
                xRot -= 360;
            if (yRot >= 270)
                yRot -= 360;
            if (zRot >= 270)
                zRot -= 360;

            if (rotateAxis == axisOption.X)
            {
                if (xRot > -90.0f && xRot < 90.0f)
                    return false;
                else
                    return true;
            }
            else if (rotateAxis == axisOption.Y)
            {
                if (yRot > -90.0f && yRot < 90.0f)
                    return false;
                else
                    return true;
            }
            return false;

            //if (numOfAxesFlipped % 2 == 0)
            //    return false;   //If 2 axises are flipped, it's back to it's original side!
            //else
            //    return true;   //If an odd number of axises are flipped, it's back is shown!
        }
    }

    private static float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }

    private static float UnwrapAngle(float angle)
    {
        if (angle >= 0)
            return angle;

        angle = -angle % 360;

        return 360 - angle;
    }

    // Use this for initialization
    void Start()
    {
        cardFront = gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    // Update is called once per frame
    void Update()
    {
        switch (rotateAxis)
        {
            case axisOption.X:
                gameObject.transform.Rotate(rotateSpeed, 0f, 0f);                       //Rotate X
                break;

            case axisOption.Y:
                gameObject.transform.Rotate(0f, rotateSpeed, 0f);                       //Rotate Y
                break;

            case axisOption.Z:
                gameObject.transform.Rotate(0f, 0f, rotateSpeed);                       //Rotate Z
                break;

            case axisOption.XY:
                gameObject.transform.Rotate(rotateSpeed, rotateSpeed, 0f);              //Rotate XY
                break;

            case axisOption.XZ:
                gameObject.transform.Rotate(rotateSpeed, 0f, rotateSpeed);              //Rotate XZ
                break;

            case axisOption.YZ:
                gameObject.transform.Rotate(0f, rotateSpeed, rotateSpeed);              //Rotate YZ
                break;

            case axisOption.XYZ:
                gameObject.transform.Rotate(rotateSpeed, rotateSpeed, rotateSpeed);     //Rotate XYZ
                break;
        }

        SpriteRenderer myRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (backIsShown)
            myRenderer.sprite = cardBack;
        else
            myRenderer.sprite = cardFront;
    }
}
