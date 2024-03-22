using System.Collections;
using UnityEngine;

public class RotateonButton : MonoBehaviour
{
    public float duration = 1.0f; // D�finir la dur�e de la rotation en secondes
    private bool isRotating = false; // �viter que la rotation se d�clenche plusieurs fois simultan�ment
    public float descentAmount = 1.0f; // Quantit� de descente, ajuster selon le besoin

    // D�marrer la rotation et la descente
    public void StartRotationAndDescent()
    {
        if (!isRotating) // V�rifier si la cam�ra n'est pas d�j� en train de tourner et descendre
        {
            StartCoroutine(RotateAndDescendCamera(100.0f, descentAmount, duration));
        }
    }

    // Ex�cuter la coroutine pour tourner et descendre la cam�ra
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
            // Calculer le ratio du temps �coul� par rapport � la dur�e totale
            float ratio = elapsedTime / duration;
            // Interpoler la rotation actuelle vers la rotation finale en utilisant la rotation globale
            transform.rotation = Quaternion.Lerp(initialRotation, finalRotation, ratio);
            // Interpoler la position actuelle vers la position finale pour r�aliser la descente
            transform.position = Vector3.Lerp(initialPosition, finalPosition, ratio);
            // Incr�menter le temps �coul�
            elapsedTime += Time.deltaTime;
            // Attendre jusqu'au prochain frame
            yield return null;
        }

        // Assurer que la rotation et la position finales sont exactement celles souhait�es
        transform.rotation = finalRotation;
        transform.position = finalPosition;
        isRotating = false;
    }
}
