using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ProceduralSpawner : MonoBehaviour
{
    [SerializeField] private Terrain terrain; // El terreno donde se generar�n los objetos.
    [SerializeField] private GameObject[] prefabs; // Array de prefabs que se generar�n.
    [SerializeField] private float spawnInterval = 1f; // Intervalo de tiempo entre spawns (en segundos).
    [SerializeField] private int initialSpawnCount = 10; // Cantidad inicial de objetos a spawnnear.
    [SerializeField] private int spawnBatchCount = 10; // N�mero de objetos a spawnear por intervalo.
    [SerializeField] private float minDistanceBetweenSpawns = 5f; // Distancia m�nima entre objetos spawnneados.

    [SerializeField] public float currentSpawnTimer = 0f; // Contador p�blico para depuraci�n.

    private List<Vector3> usedPositions = new List<Vector3>(); // Para evitar posiciones repetidas.

    private void Start()
    {
        // Spawn inicial.
        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnObject();
        }

        // Inicia el spawn peri�dico.
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            currentSpawnTimer = spawnInterval;

            while (currentSpawnTimer > 0)
            {
                currentSpawnTimer -= Time.deltaTime;
                yield return null;
            }

            Debug.Log("Intentando spawnear un nuevo lote de objetos...");
            for (int i = 0; i < spawnBatchCount; i++)
            {
                SpawnObject();
            }
        }
    }

    private void SpawnObject()
    {
        if (prefabs.Length == 0 || terrain == null)
        {
            Debug.LogWarning("No hay prefabs o terreno asignado.");
            return;
        }

        Vector3 position = GetRandomPositionOnTerrain();

        if (position != Vector3.zero)
        {
            // Selecciona un prefab al azar.
            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];

            // Instancia el prefab en la posici�n generada, emparentado al objeto que lleva el script.
            GameObject instance = Instantiate(prefab, position, Quaternion.identity);
            instance.transform.parent = this.transform;

            // Si est� en contacto con "volagua", destr�yelo inmediatamente.
            if (IsInContactWithVolagua(instance.transform.position))
            {
                Debug.LogWarning("Objeto destruido por contacto con volagua.");
                Destroy(instance);
            }
            else
            {
                Debug.Log($"Objeto {prefab.name} creado en la posici�n {position}.");
            }
        }
        else
        {
            Debug.LogWarning("No se pudo spawnear debido a una posici�n inv�lida.");
        }
    }

    private Vector3 GetRandomPositionOnTerrain()
    {
        for (int attempt = 0; attempt < 10; attempt++) // Intenta 10 veces encontrar una posici�n v�lida.
        {
            float terrainWidth = terrain.terrainData.size.x;
            float terrainLength = terrain.terrainData.size.z;

            float randomX = Random.Range(0, terrainWidth);
            float randomZ = Random.Range(0, terrainLength);
            float y = terrain.SampleHeight(new Vector3(randomX, 0, randomZ)) + terrain.GetPosition().y;

            Vector3 randomPosition = new Vector3(randomX + terrain.GetPosition().x, y, randomZ + terrain.GetPosition().z);

            if (IsPositionValid(randomPosition))
            {
                usedPositions.Add(randomPosition);
                return randomPosition;
            }
        }

        Debug.LogWarning("No se encontr� una posici�n v�lida.");
        return Vector3.zero; // Si no encuentra una posici�n v�lida, regresa Vector3.zero.
    }

    private bool IsPositionValid(Vector3 position)
    {
        foreach (var usedPosition in usedPositions)
        {
            if (Vector3.Distance(position, usedPosition) < minDistanceBetweenSpawns)
            {
                return false; // Muy cerca de otra posici�n.
            }
        }
        return true;
    }

    private bool IsInContactWithVolagua(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 1f); // Radio de 1 unidad para comprobar colisi�n.
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("volagua"))
            {
                Debug.Log("La posici�n est� en contacto con volagua.");
                return true; // La posici�n est� en contacto con "volagua".
            }
        }
        return false;
    }
}
