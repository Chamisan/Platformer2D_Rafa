﻿using System.Collections;
using UnityEngine;
public class Player : MonoBehaviour
{
    #region(Variables)
    [Header("Movement")]
    [SerializeField] float movementSpeed;
    [SerializeField] float dashForce;
    [SerializeField] Transform dashPoint;
    [Range(0, 0.3f)] [SerializeField] float movementSmoothness; //Range es la parte que se puede elegir en el inpector con el puntito y esto es suavizado de movimiento
    private Vector3 velocity = Vector3.zero;
    private bool lookRight = true;
    private Rigidbody2D rbPlayer;
    private float xMovement = 0f;

    [Header("Jump")]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpAtkForce;
    [SerializeField] LayerMask groundLayer, groundLayer2; //Layers con que se hará contacto para saber si puede saltar
    [SerializeField] Transform groundCheck; 
    [SerializeField] Vector3 gizmosBoxDimensions; //Un vector que indica las dimensiones del gizmo que detectará el suelo
    [SerializeField] bool isGrounded;

    [Header("Health")]
    [SerializeField] GameObject[] lifes;
    private int numLifes;
    private int maxLifes = 6;

    [Header("Crouch")]
    [SerializeField] BoxCollider2D boxCollStand;
    [SerializeField] BoxCollider2D boxCollCrouch;
    [SerializeField] BoxCollider2D boxCollCrouchFlag;
    [SerializeField] BoxCollider2D boxCollAttack;
    [SerializeField] GameObject windDash;
    [SerializeField] float maxDashVelocity;
    [SerializeField] bool crouchFlag;

    [Header("Canvas")]
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject nextLvlPanel;
    [SerializeField] GameObject gameoverPanel;

    [Header("Attack")] 
    [SerializeField] GameObject fireball;
    [SerializeField] Transform firePoint;
    public bool onAttack;

    private Vector3 spawnPosition, checkpointPosition; //Para las posiciones de spawn del personaje
    private Animator animatorPlayer;
    #endregion
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        Time.timeScale = 1;
        gameoverPanel.SetActive(false);
        winPanel.SetActive(false);
        onAttack = false;
        rbPlayer = gameObject.GetComponent<Rigidbody2D>();
        spawnPosition = transform.position;
        animatorPlayer = GetComponent<Animator>();
        boxCollAttack.enabled = false;
        boxCollCrouch.enabled = false;
        boxCollCrouchFlag.enabled = false;
        numLifes = 3;
        if(GameObject.FindGameObjectWithTag("Checkpoint")!=null)
        checkpointPosition = GameObject.FindGameObjectWithTag("Checkpoint").GetComponent<Transform>().position; 
    }
    private void Update()
    {
        xMovement = Input.GetAxisRaw("Horizontal") * movementSpeed * 10;  //Da valor Horizontal 
        Attack();
        Skill();
        Jump();
        Crouch();
        CrouchDash();
    }
    private void FixedUpdate()
    {
        Run();
        Move(xMovement * Time.fixedDeltaTime);  //Función de movimiento a la que se le asigna el valor horizontal 
    }

    #region(Movements)
    private void Crouch()
    {
        if (isGrounded && Input.GetAxisRaw("Vertical") < 0 && Input.GetAxisRaw("Horizontal") == 0)
        {
            boxCollStand.enabled = false;
            boxCollCrouchFlag.enabled = true;
            boxCollCrouch.enabled = true;
            animatorPlayer.SetBool("isCrouching", true);
        }
        else
        {
            if(!crouchFlag)  //Sólo si no está tocando una plataforma con la cabeza puede pararse, sino se la pela
            {
                boxCollStand.enabled = true;
                boxCollCrouch.enabled = false;
                boxCollCrouchFlag.enabled = false;
                animatorPlayer.SetBool("isCrouching", false);
            }
        }
    }
    private void CrouchDash()
    {
        if (isGrounded && Mathf.Abs(rbPlayer.velocity.x) < maxDashVelocity && Input.GetAxisRaw("Vertical") < 0 && Input.GetButton("Fire1") && !onAttack)
        {
            animatorPlayer.SetTrigger("crouchDash");
            // Añade fuerza de impulso que permita avanzar rápido hacia el lado donde está mirando            
            rbPlayer.AddForce(new Vector2(transform.localScale.x * dashForce, 0.5f), ForceMode2D.Impulse);
        }
    }
    private void EndCrouchDash() => GetComponent<Rigidbody2D>().gravityScale = 3.6f;
    private void WindCrouchDash() // Crea la animación del windDash:
    {
        GameObject windDashtemp = Instantiate(windDash, dashPoint.position, dashPoint.rotation);
            windDashtemp.transform.localScale = new Vector3(transform.localScale.x, windDashtemp.transform.localScale.y, windDashtemp.transform.localScale.z);
        Destroy(windDashtemp, 1f);
    }
    private void Jump()
    {
        isGrounded = Physics2D.OverlapBox(groundCheck.position, gizmosBoxDimensions, 0.01f, groundLayer) //Checa si está en el suelo con el contacto que hace el gizmos con el layer que le asignemos para tierra en este caso Platform
        || Physics2D.OverlapBox(groundCheck.position, gizmosBoxDimensions, 0.01f, groundLayer2); //Checa en el Layer 2 que sería el enemigo porque quiero que salte sobre ellos
        // Si no está en el suelo, entonces se establece el valor de "jumping" en verdadero, si sí está en el suelo la animación se desactiva
        animatorPlayer.SetBool("isJumping", !isGrounded);
        if (isGrounded && Mathf.Abs(rbPlayer.velocity.y) < 0.22f && Input.GetButtonDown("Jump")) //Si está en el suelo sin velocidad de caida o subida y presiona salto:
        {
            isGrounded = false;   //No está en el suelo
            rbPlayer.AddForce(new Vector2(0f, jumpForce * 10)); //Se le da impulso de salto, le multipliqué por 10 porque necesita mucha fuerza
        }
        
    }
    private void Run()
    {
        if (Input.GetButton("Run"))
            xMovement *= 1.5f;
    }
    private void Move(float move) 
    {
        Vector3 targetVelocity = new Vector2(move, rbPlayer.velocity.y);
        rbPlayer.velocity = Vector3.SmoothDamp(rbPlayer.velocity, targetVelocity, ref velocity, movementSmoothness); //Esto controla el suavizado del movimiento pero no sé como... :s
            if (move > 0 && !lookRight) //Si su movimiento es positivo y no ve a la derecha, entonces que de vuelta.
                Turn();
            if (move < 0 && lookRight) //Si su movimiento es negativo ve a la derecha, entonces que de vuelta.     
                Turn();      
        animatorPlayer.SetFloat("xMove", Mathf.Abs(xMovement));
    }
    private void Turn() 
    {
        lookRight = !lookRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    #endregion

    #region(Attacks)
    private void Attack()
    {
        if (!animatorPlayer.GetBool("isCrouching") && Input.GetAxisRaw("Vertical") == 0 && Input.GetButtonDown("Fire1")) 
        {
            onAttack = true;
            boxCollAttack.enabled = true;
            animatorPlayer.SetTrigger("attack");
            if (!isGrounded)  //Si está en el aire da un ligero impulso hacia arriba
            rbPlayer.AddForce(new Vector2(0, jumpAtkForce * 10));                
        }
    }
    private void Skill()
    {
        if (!onAttack && Input.GetAxisRaw("Vertical") > 0 && Input.GetButton("Fire1"))
            animatorPlayer.SetTrigger("fireball");         
    }
    private void Fireball()
    {
        GameObject fireballtemp = Instantiate(fireball, firePoint.position, firePoint.rotation);
        Destroy(fireballtemp, 3f);
    }
    private void AttackOff() //Desactiva el ataque y también el colider de ataque
    {
        onAttack = false; 
        boxCollAttack.enabled = false;
    }
#endregion
    private void OnDrawGizmos() //Dibuja el gizmos que hace contacto en el suelo con amarillo para que se pueda ver y manipular
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, gizmosBoxDimensions);
    }
    private IEnumerator Resurection() //Esta es la corrutina que hace que el personaje reinicie posición
    {
        Time.timeScale = 0.04f; //Primero se alenta el juego
        yield return new WaitForSeconds(0.05f); //Tantito tiempo pasa la parte que se realenta
        transform.position = spawnPosition; 
        Time.timeScale = 1; //Y el tiempo de juego vuelve a la normalidad
    }
    private void TakeDamage()
    {
        if (numLifes > 0)
        {
            lifes[numLifes - 1].SetActive(false);
            numLifes--;
        }
        else
            gameObject.SetActive(false);
    }
    private void Win() //Gana, activa el panel del ganador: Win y se pausa el juego
    {
        //Si la escena dos está activada que active el winPanel, sino
        winPanel.SetActive(true);
        Time.timeScale = 0; 
        //Sino que active el panel nextLvl
        //nextLvlPanel.SetActive(true);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Platform"))
            crouchFlag = true;
        if (other.gameObject.CompareTag("Life"))
            if (numLifes < maxLifes)
            {
                numLifes++;
                lifes[numLifes - 1].SetActive(true);
            }
        if (other.gameObject.CompareTag("DeathZone"))
        {
            StartCoroutine(Resurection()); //En esta corrutina de "Resurrección" se reinicia
            TakeDamage();
        }
        if (other.gameObject.CompareTag("Lava"))
            TakeDamage(); 
        if (other.gameObject.CompareTag("Checkpoint"))
            spawnPosition = checkpointPosition + new Vector3(1,2,0); //El vector 3 extra es porque sino queda atorado en el suelo así que lo muevo tantito
        if (other.gameObject.CompareTag("Finish")) //Si el personaje obtiene el objeto meta que tiene el tag Finish, gana :)
            Win();
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Platform"))
            crouchFlag = false;
        if (other.gameObject.CompareTag("Life"))
            Destroy(other.gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Foe") && !onAttack)
            TakeDamage();
        if (collision.gameObject.CompareTag("TotalDeath"))
            gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        gameoverPanel.SetActive(true);
        GameManager.PauseGame();
        Destroy(gameObject, 3);
    }
}