using KartGame.KartSystems;
using TMPro;
using UnityEngine;

// source : https://github.com/TheDeveloper10/Unity-Speedometer/blob/master/Speedometer.cs
namespace KartGame.UI
{
    public class Speedometer : MonoBehaviour
    {
        public TextMeshProUGUI Speed;
        public bool AutoFindKart = true;
        public ArcadeKart KartController;

        public Rigidbody target;

        public float maxSpeed = 0.0f; // The maximum speed of the target ** IN KM/H **

        public float minSpeedArrowAngle;
        public float maxSpeedArrowAngle;

        [Header("UI")]
        public RectTransform arrow; // The arrow in the speedometer

        void Start()
        {
            if (AutoFindKart)
            {
                ArcadeKart kart = FindObjectOfType<ArcadeKart>();
                KartController = kart;
            }

            if (!KartController)
            {
                gameObject.SetActive(false);
            }
        }

        // Update is called once per frame
        void Update()
        {
            float speed = KartController.Rigidbody.velocity.magnitude;
            float realSpeed = Mathf.FloorToInt(speed * 3.6f);
            if(Speed != null)
                Speed.text = string.Format($"{realSpeed} km/h");
            if (arrow != null)
                arrow.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(minSpeedArrowAngle, maxSpeedArrowAngle, realSpeed / maxSpeed));
        }
    }
}