using TMPro;
using UnityEngine;

namespace Karting.Scenes.SelectorSceneAssets
{
    public class TrackDisplayController : MonoBehaviour
    {
        public GameObject trackModels; // Assignez ici le GameObject parent contenant tous les modèles de piste
        private int currentIndex = 0; // Index de la piste actuellement affichée

        public Vector3 rotationSpeed;

        public TMP_Text trackNameText;
        public TMP_Text trackBestPosText;
        public TMP_Text trackBestTimeText;

        private void Start()
        {
            // Assurez-vous qu'au début, seule la première piste est visible
            for (int i = 0; i < trackModels.transform.childCount; i++)
            {
                trackModels.transform.GetChild(i).gameObject.SetActive(i == 0);
            }
            DisplayTrackStatistics();
        }

        public void ChangeTrack(bool previous = false )
        {
            // Désactive la piste actuellement affichée
            trackModels.transform.GetChild(currentIndex).gameObject.SetActive(false);
            var childCount = trackModels.transform.childCount;

            // Active la nouvelle piste
            if(previous)
            {
                
                currentIndex = (currentIndex - 1 + childCount) % childCount;
            }
            else
            {
                currentIndex = (currentIndex + 1) %childCount;
            }
        
            GameObject newTrack = trackModels.transform.GetChild(currentIndex).gameObject;
            newTrack.SetActive(true);
            DisplayTrackStatistics();


        }
    
        public string GetTrackName()
        {
            return trackModels.transform.GetChild(currentIndex).name;
        }
        private void Update()
        {
            // Fait tourner le parent autour de l'axe Y
            transform.Rotate(rotationSpeed);
        }


        private void DisplayTrackStatistics()
        {
            int playerBestPos = PlayerPrefs.GetInt("BestPosition_" + GetTrackName(), 0);

            if (playerBestPos == 0)
            {
                trackBestPosText.text = "N/A";
                trackBestPosText.color = Color.white;
            }
            else
            {
                switch (playerBestPos)
                {
                    case 1:
                        trackBestPosText.text = "1st";
                        trackBestPosText.color = Color.yellow;
                        break;
                    case 2:
                        trackBestPosText.text = "2nd";
                        trackBestPosText.color = Color.grey;
                        break;
                    case 3:
                        trackBestPosText.text = "3rd";
                        trackBestPosText.color = new Color(0.803f, 0.496f, 0.195f, 1);
                        break;
                    default:
                        trackBestPosText.text = playerBestPos + "th";
                        trackBestPosText.color = Color.white;
                        break;
                }
            }

            string playerBestTime = PlayerPrefs.GetString("BestTimeReadable_" + GetTrackName(), "N/A");
            if (playerBestTime == "N/A")
            {
                trackBestTimeText.text = "N/A";
            }
            else
            {
                trackBestTimeText.text = playerBestTime;
            }

            trackNameText.text = GetTrackName();
        }
    }
}


