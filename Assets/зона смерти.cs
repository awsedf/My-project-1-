using UnityEngine;

public class FallRespawn : MonoBehaviour
{
    public float minY = -10f; // Минимальная высота, после которой игрок возвращается
    public Transform respawnPoint; // Точка возрождения (перетяни в инспекторе)

    void Update()
    {
        // Если игрок упал ниже minY
        if (transform.position.y < minY)
        {
            Debug.Log("Игрок упал в пропасть! Возвращаем...");
            Respawn();
        }
    }

    public void Respawn()
    {
        // Телепортация на точку возрождения
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            
            // Если есть Rigidbody2D - обнуляем скорость
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            Debug.LogError("RespawnPoint не назначен!");
        }
    }
}
