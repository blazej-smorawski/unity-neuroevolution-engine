using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool spawnOnInterval = false;
    public List<GameObject> spawnedObjects;
    public GameObject spawnedObject;
    public int count = 0;
    public float radius = 100f;
    public float atPositionRadius = 10f;
    public float atPositionOffsetY = 0.5f;
    public float positionY = 0f;
    public float timeInterval = 10f;
    public virtual void Spawn()
    {
        for(int i = 0; i < count; i++)
        {
            float a = UnityEngine.Random.Range(0f, 1f);
            float b = UnityEngine.Random.Range(0f, 1f);

            if(b<a)
            {
                float temp = b;
                b = a;
                a = temp;
            }

            Vector3 randomPosition = new Vector3(b * radius * Mathf.Cos(2 * Mathf.PI * a / b) + transform.position.x, positionY, b * radius * Mathf.Sin(2 * Mathf.PI * a / b) + transform.position.z);

            SpawnAt(randomPosition);
        }
    }

    public void SpawnAt(Vector3 position)
    {
        Vector3 randomVector = new Vector3(atPositionRadius,0,0);
        randomVector = Quaternion.AngleAxis(UnityEngine.Random.Range(0f,360f), Vector3.up) * randomVector;
        randomVector.x += position.x;
        randomVector.y += position.y;
        randomVector.z += position.z;
        randomVector.y += atPositionOffsetY;
        spawnedObjects.Add(Instantiate(spawnedObject, randomVector, Quaternion.identity));
        //kekw

    }

    public void DestroySpawnedObjects()
    {
        foreach (GameObject spawnedObject in spawnedObjects)
        {
            Destroy(spawnedObject);
        }
    }

    IEnumerator SpawnCoroutine()
    {
        while(true)
        {
            Spawn();
            yield return new WaitForSeconds(timeInterval);
        }
    }

    public void Start()
    {
        if (spawnOnInterval)
        {
            StartCoroutine(SpawnCoroutine());
        }
    }
}
