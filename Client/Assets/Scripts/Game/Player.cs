using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;

    [SerializeField]
    private float health;
    [SerializeField]
    private float maxHealth;

    public MeshRenderer meshRenderer;

    public void Initialize(int id, string username)
    {
        this.id = id;
        this.username = username;

        health = maxHealth;
    }

    public void SetHealth(float health)
    {
        this.health = health;

        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        meshRenderer.enabled = false;
    }

    public void Respawn()
    {
        meshRenderer.enabled = true;

        SetHealth(maxHealth);
    }
}
