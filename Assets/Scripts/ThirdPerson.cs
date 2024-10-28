using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ThirdPerson : MonoBehaviour
{
    [SerializeField] private float velocidadMovimiento, smoothing;
    private float velocidadRotacion;
    private CharacterController cc;
    private Camera cam;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>();
        cam = Camera.main;
        anim=GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        MoverYRotar();
    }

    private void MoverYRotar()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(h, v).normalized;

        if (input.sqrMagnitude > 0)
        {
            anim.SetBool("isWalking", true);
            float anguloRotacion = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            float anguloSuave = Mathf.SmoothDampAngle(transform.eulerAngles.y, anguloRotacion, ref velocidadRotacion, smoothing);
            transform.eulerAngles = new Vector3(0, anguloSuave, 0);
            Vector3 movimiento = Quaternion.Euler(0, anguloRotacion, 0) * Vector3.forward;

            cc.Move(movimiento * velocidadMovimiento * Time.deltaTime);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }
}
