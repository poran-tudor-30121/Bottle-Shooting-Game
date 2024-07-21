using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    // Start is called before the first frame update

    private void OnCollisionEnter(Collision collision)
    {
       if(collision.gameObject.CompareTag("Wall"))
        {
       
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Target"))
        {
            print("hit Target");
            
        }
    }
 
}
