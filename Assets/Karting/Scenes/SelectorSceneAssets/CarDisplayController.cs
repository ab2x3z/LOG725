using UnityEngine;

namespace Karting.Scenes.SelectorSceneAssets
{
    public class CarDisplayController : MonoBehaviour
    {
        public GameObject rallyCarModels; // Assignez ici le GameObject parent contenant tous les modèles de voiture
        // ReSharper disable once CommentTypo
        private int currentIndex = 0; // Index de la voiture actuellement affichée

        public Vector3 rotationSpeed;

        private void Start()
        {
            // Assurez-vous qu'au début, seule la première voiture est visible
            for (int i = 0; i < rallyCarModels.transform.childCount; i++)
            {
                rallyCarModels.transform.GetChild(i).gameObject.SetActive(i == 0);
            }
        }

        public void ChangeCar(bool previous = false )
        {
            // Désactive la voiture actuellement affichée
            rallyCarModels.transform.GetChild(currentIndex).gameObject.SetActive(false);
            var childCount = rallyCarModels.transform.childCount;
            
            // Active la nouvelle voiture
            if(previous)
            {
               
                currentIndex = (currentIndex - 1 + childCount) % childCount;
            }
            else
            {
                currentIndex = (currentIndex + 1) % childCount;
            }
        
            GameObject newCar = rallyCarModels.transform.GetChild(currentIndex).gameObject;
            newCar.SetActive(true);
        
            // set la position sur le sol, au centre de l'object parent
            //newCar.transform.position = rallyCarModels.transform.position;
        
        
        }
    
        public string GetCarName()
        {
            return rallyCarModels.transform.GetChild(currentIndex).name;
        }
        private void Update()
        {
            // Fait tourner le parent autour de l'axe Y
            transform.Rotate(rotationSpeed);
        }
    }
}

