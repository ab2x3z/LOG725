using System.Collections;
using UnityEngine;

public class RotateonButton : MonoBehaviour
{
    public float duration = 1.0f; // Définir la durée de la rotation en secondes
    private bool isRotating = false; // Éviter que la rotation se déclenche plusieurs fois simultanément
    public float descentAmount = 1.0f; // Quantité de descente, ajuster selon le besoin

    // Démarrer la rotation et la descente
    public void StartRotationAndDescent()
    {
        if (!isRotating) // Vérifier si la caméra n'est pas déjà en train de tourner et descendre
        {
            StartCoroutine(RotateAndDescendCamera(100.0f, descentAmount, duration));
        }
    }

    // Exécuter la coroutine pour tourner et descendre la caméra
    IEnumerator RotateAndDescendCamera(float angle, float descent, float duration)
    {
        isRotating = true;
        Quaternion initialRotation = transform.rotation;
        // Calcul de la rotation finale en utilisant la rotation globale
        Quaternion finalRotation = Quaternion.Euler(transform.eulerAngles + Vector3.up * angle);
        Vector3 initialPosition = transform.position;
        Vector3 finalPosition = initialPosition - Vector3.up * descent; // Calculer la position finale pour la descente
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            // Calculer le ratio du temps écoulé par rapport à la durée totale
            float ratio = elapsedTime / duration;
            // Interpoler la rotation actuelle vers la rotation finale en utilisant la rotation globale
            transform.rotation = Quaternion.Lerp(initialRotation, finalRotation, ratio);
            // Interpoler la position actuelle vers la position finale pour réaliser la descente
            transform.position = Vector3.Lerp(initialPosition, finalPosition, ratio);
            // Incrémenter le temps écoulé
            elapsedTime += Time.deltaTime;
            // Attendre jusqu'au prochain frame
            yield return null;
        }

        // Assurer que la rotation et la position finales sont exactement celles souhaitées
        transform.rotation = finalRotation;
        transform.position = finalPosition;
        isRotating = false;
    }
}
