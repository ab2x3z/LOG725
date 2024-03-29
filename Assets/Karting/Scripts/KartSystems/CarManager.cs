
using UnityEngine;

namespace KartGame.KartSystems
{
    public class CarManager : MonoBehaviour
    {
        // Start is called before the first frame update

        private GameObject selectedCar;
        private GameObject instantiatedCar;
        private ArcadeKart playerKart;
   
        void Awake()
        {
            //va chercher la voiture sélectionnée dans playerprefs
            string carName = PlayerPrefs.GetString("car");
            Debug.Log("Car selected: "+ carName);
            
            // va chercher le prefab de la voiture
            selectedCar = Resources.Load("Cars/Prefabs/RallyCars/"+ carName) as GameObject;
            Debug.Log("Loading scene with car: "+ selectedCar);
            // instancie la voiture aux coordonnées de l'objet parent
            instantiatedCar = Instantiate(selectedCar, transform.position, transform.rotation);
            playerKart = instantiatedCar.GetComponent<ArcadeKart>();
        }
        

        public GameObject GetCar()
        {
            return instantiatedCar;
        }
        
        public ArcadeKart GetKart()
        {
            return playerKart;
        }
        
        
        // Update is called once per frame
        void Update()
        {
            // on update la position du gameobject pour qu'elle soie celle de l'enfant
            transform.position = instantiatedCar.transform.position;
            transform.rotation = instantiatedCar.transform.rotation;
        }
    }

}
