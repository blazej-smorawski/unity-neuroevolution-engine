using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> spawnedObjects;
    public GameObject spawnedObject;
    public int count = 0;
    public float radius = 100f;
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

            spawnedObjects.Add(Instantiate(spawnedObject, randomPosition,Quaternion.identity));
        }
    }

    IEnumerator SpawnCoroutine()
    {
        while(true)
        {
            foreach(GameObject spawnedObject in spawnedObjects)
            {
                Destroy(spawnedObject);
            }
            Spawn();
            yield return new WaitForSeconds(timeInterval);
        }
    }

    public void Start()
    {
        StartCoroutine(SpawnCoroutine());
    }
}
