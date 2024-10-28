using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FirstPerson: MonoBehaviour
{
    [SerializeField] private float velocidadMovimiento;
    private CharacterController cc;
    private Camera cam;
    
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

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(h, v).normalized;

        if (input.sqrMagnitude > 0)
        {
            float anguloRotacion = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            transform.eulerAngles = new Vector3(0, anguloRotacion, 0);
            Vector3 movimiento = Quaternion.Euler(0, anguloRotacion, 0) * Vector3.forward;

            cc.Move(movimiento * velocidadMovimiento * Time.deltaTime);
        }
    }
}
