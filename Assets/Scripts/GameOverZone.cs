
using UnityEngine;

public class GameOverZone : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Game Over");

            Destroy(other.gameObject);
        }
    }
}
