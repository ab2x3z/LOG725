using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowDropdownScript : MonoBehaviour
{
    public Dropdown resolution;
    public Toggle fullScreen;
    // Start is called before the first frame update
    void Start()
    {
      resolution = GameObject.Find("ResDropd").GetComponent<Dropdown>();
      fullScreen = GameObject.Find("isFullscreen").GetComponent<Toggle>();
      Screen.SetResolution(640, 480, FullScreenMode.Windowed);

    }

    // Update is called once per frame
    void Update()
    {
        bool isFS = fullScreen.isOn;
        FullScreenMode mode = (fullScreen.isOn) ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
        switch (resolution.value)
        {
            case 1:
                Screen.SetResolution(640, 480, mode);
            break;
            case 2:
                Screen.SetResolution(1920, 1080, mode);
                break;


        }
        
    }
}
