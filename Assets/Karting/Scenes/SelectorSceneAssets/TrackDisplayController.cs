using UnityEngine;

namespace Karting.Scenes.SelectorSceneAssets
{
    public class TrackDisplayController : MonoBehaviour
    {
        public GameObject trackModels; // Assignez ici le GameObject parent contenant tous les modèles de piste
        private int currentIndex = 0; // Index de la piste actuellement affichée

        public Vector3 rotationSpeed;

        private void Start()
        {
            // Assurez-vous qu'au début, seule la première piste est visible
            for (int i = 0; i < trackModels.transform.childCount; i++)
            {
                trackModels.transform.GetChild(i).gameObject.SetActive(i == 0);
            }
        }

        public void ChangeCar(bool previous = false )
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
    }
}


