using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;

    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    private Transform shootOrigin;

    [SerializeField]
    private float gravity = -9.81f;
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float jumpSpeed = 5f;
    [SerializeField]
    private float health;

    public float Health => health;

    [SerializeField]
    private float maxHealth = 100f;

    private bool[] inputs;

    private float yVelocity;

    private void Awake()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
    }

    public void Initialize(int id, string username)
    {
        this.id = id;
        this.username = username;

        health = maxHealth;

        inputs = new bool[5];
    }

    public IEnumerator Start()
    {
        while (true)
        {
            if (health <= 0f)
            {
                yield break;
            }

            Vector2 inputDirection = Vector2.zero;

            if (inputs[0])
            {
                inputDirection.y += 1;
            }
            if (inputs[1])
            {
                inputDirection.y -= 1;
            }
            if (inputs[2])
            {
                inputDirection.x -= 1;
            }
            if (inputs[3])
            {
                inputDirection.x += 1;
            }

            Move(inputDirection);

            yield return new WaitForFixedUpdate();
        }
    }

    private void Move(Vector2 direction)
    {
        Vector3 moveDirection = transform.right * direction.x + transform.forward * direction.y;

        moveDirection *= moveSpeed;

        if (characterController.isGrounded)
        {
            yVelocity = 0f;

            if (inputs[4])
            {
                yVelocity = jumpSpeed;
            }
        }

        yVelocity += gravity;

        moveDirection.y = yVelocity;

        characterController.Move(moveDirection);

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    public void SetInput(bool[] inputs, Quaternion rotation)
    {
        this.inputs = inputs;

        transform.rotation = rotation;
    }

    public void Shoot(Vector3 viewDirection)
    {
        if (Physics.Raycast(shootOrigin.position, viewDirection, out RaycastHit hit, 25f))
        {
            // Player Layer
            if (hit.collider.gameObject.layer == 8)
            {
                hit.collider.GetComponent<Player>().TakeDamage(50f);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (health <= 0)
        {
            return;
        }

        health -= damage;

        if (health <= 0)
        {
            health = 0f;

            characterController.enabled = false;

            transform.position = new Vector3(Random.Range(-10f, 10f), 40f, Random.Range(-10f, 10f));

            ServerSend.PlayerPosition(this);

            StartCoroutine(Respawn());

            return;
        }

        ServerSend.PlayerHealth(this);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        health = maxHealth;

        characterController.enabled = true;

        ServerSend.PlayerRespawned(this);
    }
}
