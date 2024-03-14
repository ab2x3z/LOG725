using KartGame.KartSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class WindowDropdownScript : MonoBehaviour
{
    public TMP_Dropdown resolution;
    public Toggle fullScreen;
    Dictionary<int, int[]> disponibleGame = new Dictionary<int, int[]>
    {
        {0, new int[] {720 , 480}},
        {1, new int[] {1280 , 720}},
        {2, new int[] {1920, 1080}},
        {3, new int[] {2560, 1080}},
        {4, new int[] {2560, 1440}},
        {5, new int[] {3840, 2160}}
    };
    // Start is called before the first frame update
    void Start()
    {
        resolution = gameObject.GetComponent<TMP_Dropdown>();
        Resolution r = Screen.currentResolution;
        bool resAlreadyLoaded = false;
        if (resolution.options.Count > 0)
        {
            resAlreadyLoaded = true;
        }
        if (!resAlreadyLoaded) {
            foreach (KeyValuePair<int, int[]> rd in disponibleGame)
            {
                    List<string> t = new List<string> { String.Format("{0}x{1}", rd.Value[0], rd.Value[1]) };
                    resolution.AddOptions(t);
            }
        }
        foreach (KeyValuePair<int, int[]> rd in disponibleGame)
        {
            if (rd.Value[0] == r.width && rd.Value[1] == r.height)
            {
                resolution.SetValueWithoutNotify(rd.Key);
            }
            break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        FullScreenMode mode = (fullScreen.isOn) ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        if (disponibleGame.ContainsKey(resolution.value))
        {
           int w = disponibleGame[resolution.value][0];
           int h = disponibleGame[resolution.value][1];
            Screen.SetResolution(w, h, mode);
        }
    }
}
