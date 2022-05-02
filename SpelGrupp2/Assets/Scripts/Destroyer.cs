using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public float lifeTime = 10f;
    public float waitToDestroy = 0.5f;

    private void Update()
    {
        if (lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
            {
                Detstruction();
            }
        }
    }
    private void Detstruction()
    {
        Destroy(gameObject);
    }
}
