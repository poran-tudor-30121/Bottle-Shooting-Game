using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    [SerializeField] GameObject brokenBottlePrefab;
    private bool firstBottleHit = false;
    private BottleSpawner bottleSpawner;
    private ScoreManager scoreManager;

    void Start()
    {
       bottleSpawner = FindObjectOfType<BottleSpawner>();
       scoreManager = FindObjectOfType<ScoreManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (!firstBottleHit)
            {
                bottleSpawner.OnFirstBottleHit(gameObject);
                firstBottleHit = true;
            }    
            Explode();
        }
    }

    void Explode()
    {
        if (scoreManager != null)
        {
            scoreManager.AddScore(10);
        }
        bottleSpawner.OnBottleDestroyed(gameObject);

        GameObject brokenBottle = Instantiate(brokenBottlePrefab, this.transform.position, Quaternion.identity);
        brokenBottle.GetComponent<BrokenBottle>().RandomVelocities();
        StartCoroutine(DestroyBrokenBottle(brokenBottle));
    }

    IEnumerator DestroyBrokenBottle(GameObject brokenBottle)
    {
        yield return new WaitForSeconds(2f);
        Destroy(brokenBottle);
    }
}
