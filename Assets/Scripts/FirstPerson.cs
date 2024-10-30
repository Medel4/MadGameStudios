using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FirstPerson: MonoBehaviour
{
    [SerializeField] private float velocidadMovimiento;
    private CharacterController cc;
    private Camera cam;

    public float range = 5f; // Rango de detección
    

    // Start is called before the first frame update
    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>();
        cam = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {

        CogerObjetos();

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(h, v).normalized;
        transform.rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);

        if (input.sqrMagnitude > 0)
        {
            float anguloRotacion = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            Vector3 movimiento = Quaternion.Euler(0, anguloRotacion, 0) * Vector3.forward;

            cc.Move(movimiento * velocidadMovimiento * Time.deltaTime);
        }


    }


    void CogerObjetos()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, range))
            {
                // Verificamos si el objeto que colisiona tiene el tag "Destructible"
                if (hit.collider.CompareTag("Destructible"))
                {
                    Destroy(hit.collider.gameObject); // Destruye el objeto
                }
            }
        }

    }
}
