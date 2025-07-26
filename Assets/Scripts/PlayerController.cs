using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float velocidad = 5f; // Velocidad de movimiento del jugador
    public int hp = 3;
    public float fuerzaSalto = 5f; // Fuerza del salto
    public float fuerzarebote= 10f; // Fuerza del rebote al recibir daño
    public float longitudRayo = 0.44f; // Longitud del rayo para detectar el suelo
    public float AnchoRayo = 0.38f; // Ancho del rayo para detectar el suelo
    public float offsetRayoX = -0.01f; // Desplazamiento del rayo en el eje X
    public float offsetRayoY = 0f; // Desplazamiento del rayo en el eje Y
    public LayerMask capaSuelo; // Capa del suelo para la detección

    private bool enSuelo; // Variable para verificar si el jugador está en el suelo
    private bool atack; // Variable para verificar si el jugador está atacando
    private bool death;
    private bool getdamage;
    private Rigidbody2D rb; // Componente Rigidbody2D del jugador

    public Animator animator;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Obtener el componente Rigidbody2D del jugador
    }

    // Update is called once per frame
    void Update()
    {
        if (!(hp<1 || hp == 0))
        {

            float velocidadX = Input.GetAxis("Horizontal") * velocidad * Time.deltaTime;

            animator.SetFloat("movement", velocidadX * velocidad);

            if (velocidadX > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);

            }
            if (velocidadX < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }

            Vector3 posicion = transform.position;

            if (!getdamage)
                transform.position = new Vector3(posicion.x + velocidadX, posicion.y, posicion.z);

            // Usar BoxCast en lugar de Raycast para incluir el ancho con offset
            Vector3 posicionRayo = transform.position + new Vector3(offsetRayoX, offsetRayoY, 0);
            RaycastHit2D hit = Physics2D.BoxCast(posicionRayo, new Vector2(AnchoRayo, 0.05f), 0f, Vector2.down, longitudRayo, capaSuelo);
            enSuelo = hit.collider != null;

            if (enSuelo && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && !getdamage)
            {
                rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
            }

            if ((Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Mouse0)) && !atack && !getdamage)
            {
                atacando();
            }
        }


        animator.SetBool("ensuelo", enSuelo);
        animator.SetBool("getdamage", getdamage);
        animator.SetBool("atack", atack);
        animator.SetInteger("hp", hp);

    }

    public void GetDamage(Vector2 direction, int cantdamage)
    {
        if (!getdamage)
        {
            getdamage = true;
            hp -= cantdamage;

            if (!(hp < 1 || hp == 0))
            {
                
                Vector2 rebote = new Vector2(transform.position.x - direction.x, 1).normalized;
                rb.AddForce(rebote * fuerzarebote, ForceMode2D.Impulse);
            }        
        }
    }
    public void ResetDamage()
    {
        getdamage = false;
        rb.linearVelocity = Vector2.zero; 
    }
    public void atacando()
    {
        atack = true;
    }
    public void desactivarAtaque()
    {
        atack = false;
    }
    void OnDrawGizmos()
    {
        // Dibujar un rectángulo para visualizar la detección del suelo con ancho y offset
        Gizmos.color = Color.red;
        Vector3 centro = transform.position + new Vector3(offsetRayoX, offsetRayoY, 0);
        Vector3 tamaño = new Vector3(AnchoRayo, longitudRayo, 0);
        Gizmos.DrawWireCube(centro + Vector3.down * (longitudRayo / 2), tamaño);
    }
        
}
