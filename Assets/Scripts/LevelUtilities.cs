using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelUtilities : MonoBehaviour
{
    public LevelUtilities()
    {
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void MoveCameraToCanvas(Canvas newCanvasToView)
    {
        Camera.main.transform.position = new Vector3(newCanvasToView.transform.position.x, 
                                                     newCanvasToView.transform.position.y,
                                                     Camera.main.transform.position.z        //Do not move the Camera's Z position
                                                     );
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
