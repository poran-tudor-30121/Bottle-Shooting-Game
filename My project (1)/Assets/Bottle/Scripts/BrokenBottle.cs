using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenBottle : MonoBehaviour
{
    [SerializeField] GameObject[] pieces;
    [SerializeField] float velMultiplier = 2f;
    [SerializeField] float timeBeforeDestroying = 1f;
    float timeSpan = 1f;
    AudioSource audioSource;
    [SerializeField] AudioClip breakSound;

    void Start()
    {
       
        Destroy(this.gameObject, timeBeforeDestroying);
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.PlayOneShot(breakSound);
        
    }
    
    public void RandomVelocities()
    {
        for(int i = 0; i <= pieces.Length - 1; i++)
        {
            float xVel = UnityEngine.Random.Range(0f, 1f);
            float yVel = UnityEngine.Random.Range(0f, 1f);
            float zVel = UnityEngine.Random.Range(0f, 1f);
            Vector3 vel = new Vector3(velMultiplier * xVel, velMultiplier * yVel, velMultiplier * zVel);
            pieces[i].GetComponent<Rigidbody>().velocity = vel;
        }
        StartCoroutine(DestroyPiece(pieces));
    }
    IEnumerator DestroyPiece(GameObject[] pieces)
    {
        yield return new WaitForSeconds(timeSpan); // Wait for 2 seconds
        for(int i=0;i<=pieces.Length-1;i++)
        {
            Destroy(pieces[i]);
        }
    }
}