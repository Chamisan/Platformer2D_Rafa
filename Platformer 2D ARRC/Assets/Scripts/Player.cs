using System.Collections;
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

    [Header("SFX")]
    [SerializeField] AudioClip audioFruit;
    [SerializeField] AudioClip audioDash;
    [SerializeField] AudioClip audioJump;
    [SerializeField] AudioClip audioFire;
    [SerializeField] AudioClip [] audioSword;
    private AudioSource audioSrc;

    private Vector3 spawnPosition, checkpointPosition; //Para las posiciones de spawn del personaje
    private Animator animatorPlayer;
    #endregion
    private void Start()
    {
        GameManager.PlayGame();
        gameoverPanel.SetActive(false);
        winPanel.SetActive(false);
        onAttack = false;
        rbPlayer = gameObject.GetComponent<Rigidbody2D>();
        spawnPosition = transform.position;
        animatorPlayer = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();
        boxCollAttack.enabled = false;
        boxCollCrouch.enabled = false;
        boxCollCrouchFlag.enabled = false;
        if (GameObject.FindGameObjectWithTag("Checkpoint") != null)
            checkpointPosition = GameObject.FindGameObjectWithTag("Checkpoint").GetComponent<Transform>().position;
        numLifes = GameManager.lifesPlayer;
        foreach(GameObject hearth in lifes)  
        {
            // Obtener el índice del objeto hearth
            int hearthIndex = System.Array.IndexOf(lifes, hearth);
            // Si el índice es menor que numLifes, activar el objeto, de lo contrario desactivarlo
            if (hearthIndex < numLifes)
                hearth.SetActive(true);
            else
                hearth.SetActive(false);
        } //Esto me ayuda a pasar el número de vidas más fácil al siguiente nivel que emparentando no pude :s
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
            //PlaySound(audioDash);
            // Añade fuerza de impulso que permita avanzar rápido hacia el lado donde está mirando            
            rbPlayer.AddForce(new Vector2(transform.localScale.x * dashForce, 0.3f), ForceMode2D.Impulse);
        }
    }
    private void WindCrouchDash() // Crea la animación del windDash:
    {
        GameObject windDashtemp = Instantiate(windDash, dashPoint.position, dashPoint.rotation);
        AudioManager.instance.PlaySound(audioJump);
        windDashtemp.transform.localScale = new Vector3(transform.localScale.x, windDashtemp.transform.localScale.y, windDashtemp.transform.localScale.z);
        Destroy(windDashtemp, 1f);
    }
    private void Jump()
    {
        isGrounded = Physics2D.OverlapBox(groundCheck.position, gizmosBoxDimensions, 0.01f, groundLayer) //Checa si está en el suelo con el contacto que hace el gizmos con el layer que le asignemos para tierra en este caso Platform
        || Physics2D.OverlapBox(groundCheck.position, gizmosBoxDimensions, 0.01f, groundLayer2); //Checa en el Layer 2 que sería el enemigo porque quiero que salte sobre ellos        
        // Si no está en el suelo, entonces se establece el valor de "jumping" en verdadero, si sí está en el suelo la animación se desactiva        
        animatorPlayer.SetBool("isJumping", !isGrounded);
        if (isGrounded && Mathf.Abs(rbPlayer.velocity.y) < 0.2f && Input.GetButtonDown("Jump")) //Si está en el suelo sin velocidad de caida o subida y presiona salto:
        {
            isGrounded = false;
            animatorPlayer.SetFloat("blendJump", 0);
            //PlaySound(audioJump);
            rbPlayer.AddForce(new Vector2(0f, jumpForce * 10)); //Se le da impulso de salto, le multipliqué por 10 porque necesita mucha fuerza
            WindCrouchDash(); //Le agregué una animación de polvo al salto
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
            //animatorPlayer.SetFloat("blendJump", 1f);
            onAttack = true;
            boxCollAttack.enabled = true;
            animatorPlayer.SetTrigger("attack");
            if (!isGrounded)  //Si está en el aire da un ligero impulso hacia arriba
            {
                animatorPlayer.SetFloat("blendJump", 1f);
                rbPlayer.AddForce(new Vector2(0, jumpAtkForce * 10));
            }
                            
        }
    }
    private void Skill()
    {
        if (!onAttack && Input.GetAxisRaw("Vertical") > 0 && Input.GetButton("Fire1"))
            if(isGrounded)
                animatorPlayer.SetTrigger("fireball");
            else
            animatorPlayer.SetFloat("blendJump", 1f);  //Está muy rota la habilidad aún hay que mejorarla así que le asigné 1 de ataque
    }
    // Las siguientes dos funciones van al final de la animación como eventos:
    private void Fireball()
    {
        AudioManager.instance.PlaySound(audioFire);
        GameObject fireballtemp = Instantiate(fireball, firePoint.position, firePoint.rotation);
        Destroy(fireballtemp, 3f);
    }
    private void AttackOff() //Desactiva el ataque y también el colider de ataque
    {
        onAttack = false; 
        boxCollAttack.enabled = false;
    }
    private void SoundSwords() => AudioManager.instance.PlaySound(audioSword[Random.Range(0, audioSword.Length)]);

    #endregion

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
            ModifyPoints(-25);
            animatorPlayer.SetTrigger("hurt");
        }
        else
        {
            gameoverPanel.SetActive(true);
            GameManager.PauseGame();
            gameObject.SetActive(false);
        }
    }
    private void Win()  
    {
        winPanel.SetActive(true);
        GameManager.lifesPlayer = numLifes;
        GameManager.PauseGame();
        if (GameManager.points > PlayerPrefs.GetInt("MaxPoints"))
            PlayerPrefs.SetInt("MaxPoints", GameManager.points);
    }
    private void ModifyPoints(int points)
    {
        if (GameManager.points + points >= 0) //Si la resta es mayor a cero entonces que le quite sino que lo deje en 0
            GameManager.points += points;
    }
    /*public void PlaySound(AudioClip clip)
    {
        audioSrc.PlayOneShot(clip);
        Debug.Log("Reproduciendo clip de audio: " + clip.name);
    }*/
    private void OnDrawGizmos() //Dibuja el gizmos que hace contacto en el suelo con amarillo para que se pueda ver y manipular
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, gizmosBoxDimensions);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Platform"))
            crouchFlag = true;
        if (other.gameObject.CompareTag("Life"))
            //AudioManager.instance.PlaySound(audioFruit);
        if (numLifes < maxLifes)
            {
                numLifes++;
                ModifyPoints(50);
                lifes[numLifes - 1].SetActive(true);
                Destroy(other.gameObject);
            }
            else
            {
                ModifyPoints(200);
                Destroy(other.gameObject);
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
            if (other.gameObject.name == "dragonEye")
            {
                AudioSource audioSource = other.gameObject.GetComponent<AudioSource>();
                if (audioSource != null && audioSource.clip != null)
                    AudioManager.instance.PlaySound(audioSource.clip);
            }

    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Platform"))
            crouchFlag = false;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Foe") && !onAttack)
            TakeDamage();
        if (collision.gameObject.CompareTag("TotalDeath"))
        {
            gameoverPanel.SetActive(true);
            gameObject.SetActive(false);
            GameManager.PauseGame();
        }
    }
}