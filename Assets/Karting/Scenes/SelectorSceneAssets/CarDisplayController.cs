using UnityEngine;
using KartGame.KartSystems;
using UnityEngine.UI;

namespace Karting.Scenes.SelectorSceneAssets
{
    public class CarDisplayController : MonoBehaviour
    {
        public GameObject rallyCarModels; // Assignez ici le GameObject parent contenant tous les modèles de voiture

        // ReSharper disable once CommentTypo
        private int currentIndex = 0; // Index de la voiture actuellement affichée

        private int nbUnlockedCars; // Nombre de voitures débloquées

        public Vector3 rotationSpeed;

        public GameObject StatsPanel;

        public Material holographicMaterial;

        public Button okButton;

        private void Start()
        {
            // Assurez-vous qu'au début, seule la première voiture est visible
            for (int i = 0; i < rallyCarModels.transform.childCount; i++)
            {
                rallyCarModels.transform.GetChild(i).gameObject.SetActive(i == 0);
            }

            nbUnlockedCars = PlayerPrefs.GetInt("UnlockedCars", 5);

            ActivateHolographs();
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
            // On ne peut pas selectionner une voiture non debloquee
            if (currentIndex >= nbUnlockedCars)
            {
                okButton.interactable = false;
            }
            else
            {
                okButton.interactable = true;
            }
            
            GameObject newCar = rallyCarModels.transform.GetChild(currentIndex).gameObject;
            newCar.SetActive(true);

            StatsPanel.GetComponent<StatsPanel>().UpdateStats(newCar);
        
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

        // affiche en hologramme toutes les voitures non debloquees
        private void ActivateHolographs()
        {
            for (int i = nbUnlockedCars; i < rallyCarModels.transform.childCount; i++)
            {

                // Apply the holographic material to the car body
                Renderer[] renderers = rallyCarModels.transform.GetChild(i).GetComponentsInChildren<Renderer>();
                if (renderers != null)
                {
                    foreach (Renderer renderer in renderers)
                    {
                        renderer.material = holographicMaterial;
                    }
                }
                else
                {
                    Debug.LogWarning("Renderers component not found on car.");
                }
            }
        }
    }
}

