using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maceta : MonoBehaviour
{
    [SerializeField] private GameObject[] plantaMaceta;  
    [SerializeField] private float vida; 
    private bool viva; 

   
    private List<GameObject> plantasActivas = new List<GameObject>();

    public bool Viva { get => viva; set => viva = value; }

   


    void Start()
    {
        // Desactiva todas las plantas al principio
        for (int i = 0; i < plantaMaceta.Length; i++)
        {
            plantaMaceta[i].SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("PlantaA"))
        {
            Destroy(other.gameObject); 

            
            int plantaIndex = Random.Range(0, plantaMaceta.Length);
            plantaMaceta[plantaIndex].SetActive(true); 

            
            plantasActivas.Add(plantaMaceta[plantaIndex]);

          
            Viva = true;
        }
    }

    
    public List<GameObject> ObtenerPlantasActivas()
    {
        return plantasActivas;
    }

   
    public void DesactivarPlantas()
    {
        foreach (var planta in plantasActivas)
        {
            planta.SetActive(false);
        }

        plantasActivas.Clear(); 
    }
}