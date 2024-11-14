using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float speed = 5f;           // Скорость снаряда
    public float lifetime = 3f;        // Время, через которое снаряд уничтожится, если не попадет в цель
    private Transform player;          // Ссылка на игрока

    void Start()
    {
        // Найти игрока по тегу (например, "Player") - убедитесь, что у объекта игрока установлен этот тег
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // Уничтожить снаряд через заданное время, если он не попадет в цель
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (player != null)
        {
            // Обновляем направление и поворачиваем снаряд к игроку
            Vector2 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Двигаем снаряд в направлении игрока
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Уничтожаем снаряд при столкновении с любым объектом
        Destroy(gameObject);
    }
}