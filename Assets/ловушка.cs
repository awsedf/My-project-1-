using UnityEngine;

public class Spikes : MonoBehaviour
{
    [Tooltip("Точка возрождения игрока (если не задана, ищет объект с тегом 'Respawn')")]
    public Transform respawnPoint;

    private void Start()
    {
        // Автоматически ищем точку спавна, если не задана вручную
        if (respawnPoint == null)
        {
            GameObject respawnObj = GameObject.FindGameObjectWithTag("Respawn");
            if (respawnObj != null)
            {
                respawnPoint = respawnObj.transform;
            }
            else
            {
                Debug.LogError("Не найдена точка спавна! Создайте объект с тегом 'Respawn'");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && respawnPoint != null)
        {
            other.transform.position = respawnPoint.position;
            Debug.Log("Игрок возвращен на точку спавна");
        }
    }
}
