using UnityEngine;
using UnityEngine.SceneManagement;

namespace Karting.Scenes.SelectorSceneAssets
{
    public class SceneLoader : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }
    
        // fetches the car selected from the script in the GameObject Car_Selector
        public void LoadScene()
        {
            //find the GameObject named Car_Selector
            GameObject carSelector = GameObject.Find("Car_Selector");
            //find the script named Car Display Controller 
            CarDisplayController carDisplayController = carSelector.GetComponent<CarDisplayController>();
            //get the name of the car selected
            string carName = carDisplayController.GetCarName();
            
            //find the GameObject named Track_Selector
            GameObject trackSelector = GameObject.Find("Track_Selector");
            //find the script named Track Display Controller
            TrackDisplayController trackDisplayController = trackSelector.GetComponent<TrackDisplayController>();
            //get the name of the track selected
            string trackName = trackDisplayController.GetTrackName();
            
            Debug.Log("Loading scene with car: " + carName + " and track: " + trackName);
            
            //give the car selected to the next scene
            PlayerPrefs.SetString("car", carName);
            
            
            //load the scene with the car and track selected
            
            SceneManager.LoadSceneAsync(trackName);
            
        }

        public void ReloadScene()
        {
            Time.timeScale = 1; //unfreeze game from pause
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    
        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
