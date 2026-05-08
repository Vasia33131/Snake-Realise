using UnityEngine;

public class TaleCollisionDestroyer : MonoBehaviour
{
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem deathEffect;
    [SerializeField] private float destroyDelay = 0.5f;

    private bool isDestroyed = false;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (isDestroyed) return;

        RecursivePositionRepeater repeater = col.GetComponent<RecursivePositionRepeater>();

        if (repeater != null)
        {
            isDestroyed = true;
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        // Проигрываем эффект смерти
        if (deathEffect != null)
        {
            ParticleSystem effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration);
        }

        // Вызываем Game Over
        GameManager.Instance.GameOver();

        // Отключаем все коллайдеры у родительского объекта
        Collider2D[] parentColliders = transform.parent.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in parentColliders)
        {
            collider.enabled = false;
        }

        // Уничтожаем родительский объект с задержкой
        Destroy(transform.parent.gameObject, destroyDelay);
    }
}