using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelUtilities : MonoBehaviour
{
    public void loadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void moveCameraToCanvas(Canvas newCanvasToView)
    {
        Camera.main.transform.position = new Vector3(newCanvasToView.transform.position.x, 
                                                     newCanvasToView.transform.position.y,
                                                     Camera.main.transform.position.z        //Do not move the Camera's Z position
                                                     );
    }

    // Use this for initialization
    //void Start()
    //{

    //}

    // Update is called once per frame
    //void Update()
    //{

    //}
}
