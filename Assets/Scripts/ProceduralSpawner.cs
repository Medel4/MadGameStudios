using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ProceduralSpawner : MonoBehaviour
{
    [SerializeField] private Terrain terrain; // El terreno donde se generarán los objetos.
    [SerializeField] private GameObject[] prefabs; // Array de prefabs que se generarán.
    [SerializeField] private float spawnInterval = 1f; // Intervalo de tiempo entre spawns (en segundos).
    [SerializeField] private int initialSpawnCount = 10; // Cantidad inicial de objetos a spawnnear.
    [SerializeField] private int spawnBatchCount = 10; // Número de objetos a spawnear por intervalo.
    [SerializeField] private float minDistanceBetweenSpawns = 5f; // Distancia mínima entre objetos spawnneados.

    [SerializeField] public float currentSpawnTimer = 0f; // Contador público para depuración.

    private List<Vector3> usedPositions = new List<Vector3>(); // Para evitar posiciones repetidas.

    private void Start()
    {
        // Spawn inicial.
        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnObject();
        }

        // Inicia el spawn periódico.
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

            // Instancia el prefab en la posición generada, emparentado al objeto que lleva el script.
            GameObject instance = Instantiate(prefab, position, Quaternion.identity);
            instance.transform.parent = this.transform;

            // Si está en contacto con "volagua", destrúyelo inmediatamente.
            if (IsInContactWithVolagua(instance.transform.position))
            {
                Debug.LogWarning("Objeto destruido por contacto con volagua.");
                Destroy(instance);
            }
            else
            {
                Debug.Log($"Objeto {prefab.name} creado en la posición {position}.");
            }
        }
        else
        {
            Debug.LogWarning("No se pudo spawnear debido a una posición inválida.");
        }
    }

    private Vector3 GetRandomPositionOnTerrain()
    {
        for (int attempt = 0; attempt < 10; attempt++) // Intenta 10 veces encontrar una posición válida.
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

        Debug.LogWarning("No se encontró una posición válida.");
        return Vector3.zero; // Si no encuentra una posición válida, regresa Vector3.zero.
    }

    private bool IsPositionValid(Vector3 position)
    {
        foreach (var usedPosition in usedPositions)
        {
            if (Vector3.Distance(position, usedPosition) < minDistanceBetweenSpawns)
            {
                return false; // Muy cerca de otra posición.
            }
        }
        return true;
    }

    private bool IsInContactWithVolagua(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 1f); // Radio de 1 unidad para comprobar colisión.
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("volagua"))
            {
                Debug.Log("La posición está en contacto con volagua.");
                return true; // La posición está en contacto con "volagua".
            }
        }
        return false;
    }
}
