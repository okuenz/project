using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer sr;
    public Vector2 moveVector;
    public float speed = 10f;

    public Joystick joystick;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        TopCheckRadius = TopCheck.GetComponent<BoxCollider2D>().edgeRadius;
    }

    // Update is called once per frame
    void Update()
    {
        walk();  
        Flip();
        Jump();
        CheckingGround();
        SquatCheck();

        TeleportPlayerToCoordinates();
    }

    public void TeleportPlayerToCoordinates()
    {
        if (transform.position.y < -14f)
        {
            // Перезагрузка сцены
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }


    void walk()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (joystick != null)
        {
            horizontalInput += joystick.Horizontal;
        }

        moveVector.x = horizontalInput;
        anim.SetFloat("moveX", Mathf.Abs(moveVector.x));
        rb.velocity = new Vector2(moveVector.x * speed, rb.velocity.y);
    }

    void Flip()
    {
        if (moveVector.x > 0)
        {
            sr.flipX = false;
        }
        else if (moveVector.x < 0)
        {
            sr.flipX = true;
        }
    }

    public float jumpForce = 3f;
    private int jumpCount = 0;
    public int maxJumpValue = 2;

void Jump()
{
    if (Input.GetKeyDown(KeyCode.Space) && (onGround || (++jumpCount < maxJumpValue))) 
    { 
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    if (onGround)
    {
        jumpCount = 0;
    }
}

void OnCollisionEnter2D(Collision2D col)
{
    if (col.gameObject.CompareTag("Enemy"))  // Проверка, что столкнулись с врагом
    {
        Enemy enemy = col.gameObject.GetComponent<Enemy>();  // Получаем скрипт врага
        if (enemy != null)
        {
            enemy.TakeDamage(100);  // Наносим урон врагу при столкновении с ним
        }
    }
}

    public void JumpButton()
    {
        if (onGround || (++jumpCount < maxJumpValue))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (onGround)
        {
            jumpCount = 0;
        }
    }

    public bool onGround;
    public Transform GroundCheck;
    public float checkRadius = 0.5f;
    public LayerMask Ground;

    void CheckingGround()
    {
        onGround = Physics2D.OverlapCircle(GroundCheck.position, checkRadius, Ground);

        anim.SetBool("onGround", onGround);
    }

    public Transform TopCheck;
    private float TopCheckRadius;
    public LayerMask Roof;
    public Collider2D poseStand;
    public Collider2D poseSquat;

    void SquatCheck()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); // Получаем входное значение по горизонтали с клавиатуры
        float verticalInput = 0f;  // Начальное вертикальное значение

        if (joystick != null)
        {
            horizontalInput += joystick.Horizontal;  // Добавляем входное значение от джойстика
            verticalInput = joystick.Vertical;  // Получаем вертикальное значение от джойстика
        }

        if (Input.GetKey(KeyCode.S) || verticalInput < -0.5)  // Если клавиша "S" нажата или вертикальное значение ниже -0.5, представляющее движение "вниз"
        {
            anim.SetBool("squat", true);
            poseStand.enabled = false;
            poseSquat.enabled = true;
        }
        else
        {
            anim.SetBool("squat", false);
            poseStand.enabled = true;
            poseSquat.enabled = false;
        }
    }
}