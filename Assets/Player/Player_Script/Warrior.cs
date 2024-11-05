using Unity.Netcode;
using UnityEngine;

public class Warrior : NetworkBehaviour
{
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] int m_maxHealth = 100;
    [SerializeField] float m_attackRange = 1.0f;
    [SerializeField] int m_attackDamage = 40;
    [SerializeField] float m_attackCooldown = 1.0f;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_Bandit m_groundSensor;
    public Transform attackPoint;
    public LayerMask enemyLayerMask;
    private bool m_grounded = false;
    private bool m_combatIdle = false;
    private bool m_isDead = false;
    private int m_currentHealth;
    private float m_attackTimer;

    // Use this for initialization
    void Start()
    {
        if (!IsOwner)
        {
            enabled = false;
        }

        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();
        m_currentHealth = m_maxHealth;
        m_attackTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        m_attackTimer -= Time.deltaTime;
        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (inputX < 0)
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        // Move
        m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);

        // -- Handle Animations --
        //Death
        if (Input.GetKeyDown("e"))
        {
            m_isDead = !m_isDead;
            m_animator.SetTrigger(m_isDead ? "Death" : "Recover");
        }

        //Attack
        else if (Input.GetMouseButtonDown(0))
        {
            Attack();
            m_attackTimer = m_attackCooldown;
        }

        //Change between idle and combat idle
        else if (Input.GetKeyDown("f"))
            m_combatIdle = !m_combatIdle;

        //Jump
        else if (Input.GetKeyDown("space") && m_grounded)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
            m_animator.SetInteger("AnimState", 2);

        //Combat Idle
        else if (m_combatIdle)
            m_animator.SetInteger("AnimState", 1);

        //Idle
        else
            m_animator.SetInteger("AnimState", 0);

    }

    void Attack()
    {
        m_animator.SetTrigger("Attack");

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, m_attackRange, enemyLayerMask);
        foreach (Collider2D player in hitPlayers)
        {
            if (player.CompareTag("Player") && player.GetComponent<Bandit>())
            {
                player.GetComponent<Bandit>().TakeDamage(m_attackDamage);
            } else if (player.CompareTag("Player") && player.GetComponent<HeroKnight>())
            {
                player.GetComponent<HeroKnight>().TakeDamage(m_attackDamage);
            }
        }
    }

    public void TakeDamage(int m_attackDamage)
    {
        m_currentHealth -= m_attackDamage;
        m_animator.SetTrigger("Hurt");

        if (m_currentHealth < 0)
        {
            Die();
        }
    }

    void Die()
    {
        m_animator.SetTrigger("Death");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        ServerManager.Instance.PlayerDied(OwnerClientId);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, m_attackRange);
    }
}
