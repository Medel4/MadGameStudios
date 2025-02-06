using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLANTAS : MonoBehaviour
{

    [SerializeField]private float tamanomaximo=1.6f;
    private float tamanoactual;
    [SerializeField]private float tamanoinicial= 0.00001f;
    private float velocidadCrecimiento;
    [SerializeField] private bool funcionar=false;
    // Start is called before the first frame update
    private void Awake()
    {
        tamanomaximo = 1.6f;
        velocidadCrecimiento = 1.4F * Time.deltaTime;
        tamanoinicial = 0.001f;
        transform.localScale = new Vector3(0.000001f, 0.000001f, 0.000001f);
    }

    // Update is called once per frame
    void Update()
    {
        

         if (funcionar==true)
        {
            tamanoactual = transform.localScale.magnitude;
            if (tamanoactual < tamanomaximo)
            {
                transform.localScale += new Vector3(tamanoinicial, tamanoinicial, tamanoinicial) * velocidadCrecimiento;

            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("aire"))
        {
            funcionar = true;
           
        }
        else if (other.CompareTag("volagua"))
        {
         
            funcionar = false;
        }
         
    }

}
