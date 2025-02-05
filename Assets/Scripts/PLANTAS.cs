using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLANTAS : MonoBehaviour
{

    [SerializeField]private float tamanomaximo=1f;
    private float tamanoactual;
    [SerializeField]private float tamanoinicial= 0.00001f;
    private float velocidadCrecimiento;
    // Start is called before the first frame update
    private void Awake()
    {
        tamanomaximo = 1f;
        velocidadCrecimiento = 1.4F * Time.deltaTime;
        tamanoinicial = 0.0001f;
        transform.localScale = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        tamanoactual = transform.localScale.magnitude;
        if (tamanoactual < tamanomaximo)
        {
            transform.localScale += new Vector3(tamanoinicial , tamanoinicial , tamanoinicial) *velocidadCrecimiento;

        }
    }
}
