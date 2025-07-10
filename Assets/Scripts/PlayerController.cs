using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float velocidad = 5f; // Velocidad de movimiento del jugador

    public float fuerzaSalto = 10f; // Fuerza del salto
    public float fuerzarebote= 10f; // Fuerza del rebote al recibir daño
    public float longitudRayo = 0.1f; // Longitud del rayo para detectar el suelo
    public float AnchoRayo = 0.1f; // Ancho del rayo para detectar el suelo
    public float offsetRayoX = 0f; // Desplazamiento del rayo en el eje X
    public float offsetRayoY = 0f; // Desplazamiento del rayo en el eje Y
    public LayerMask capaSuelo; // Capa del suelo para la detección

    private bool enSuelo; // Variable para verificar si el jugador está en el suelo
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

        if(!getdamage)
        transform.position = new Vector3(posicion.x + velocidadX, posicion.y, posicion.z);

        // Usar BoxCast en lugar de Raycast para incluir el ancho con offset
        Vector3 posicionRayo = transform.position + new Vector3(offsetRayoX, offsetRayoY, 0);
        RaycastHit2D hit = Physics2D.BoxCast(posicionRayo, new Vector2(AnchoRayo, 0.05f), 0f, Vector2.down, longitudRayo, capaSuelo);
        enSuelo = hit.collider != null;

        if (enSuelo && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))&& !getdamage)
        {
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
        }
        animator.SetBool("ensuelo", enSuelo);
        animator.SetBool("getdamage", getdamage);

    }

    public void GetDamage(Vector2 direction, int cantdamage)
    {
        if (!getdamage)
        {
            getdamage = true;
            Vector2 rebote = new Vector2(transform.position.x - direction.x, 1).normalized;
            rb.AddForce(rebote*fuerzarebote, ForceMode2D.Impulse);
        }
    }
    public void ResetDamage()
    {
        getdamage = false;
        rb.linearVelocity = Vector2.zero; 
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
