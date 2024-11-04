using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float speed = 5f;           // Скорость снаряда
    public float lifetime = 3f;        // Время, через которое снаряд уничтожится, если не попадет в цель
    private Transform player;          // Ссылка на игрока
    private Vector2 targetDirection;   // Направление к игроку

    void Start()
    {
        // Найти игрока по тегу (например, "Player") - убедитесь, что у объекта игрока установлен этот тег
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            // Рассчитываем начальное направление к игроку
            targetDirection = (player.position - transform.position).normalized;
        }

        // Уничтожить снаряд через заданное время, если он не попадет в цель
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Если игрок существует, обновляем направление снаряда к текущему положению игрока
        if (player != null)
        {
            targetDirection = (player.position - transform.position).normalized;
        }

        // Двигаем снаряд в направлении игрока
        transform.position += (Vector3)targetDirection * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, если снаряд столкнулся с объектом, уничтожаем его
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}