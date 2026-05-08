using UnityEngine;

public class TaleCollisionDestroyer : MonoBehaviour
{
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem deathEffect;
    [SerializeField] private float destroyDelay = 0.5f;

    private bool _destroyed;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (_destroyed) return;

        if (col.GetComponent<RecursivePositionRepeater>() == null)
            return;

        _destroyed = true;
        HandleDeath();
    }

    private void HandleDeath()
    {
        if (deathEffect != null)
        {
            ParticleSystem effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration);
        }

        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();

        Transform parent = transform.parent;
        if (parent != null)
        {
            Collider2D[] parentColliders = parent.GetComponentsInChildren<Collider2D>();
            for (int i = 0; i < parentColliders.Length; i++)
                parentColliders[i].enabled = false;

            Destroy(parent.gameObject, destroyDelay);
        }
    }
}
