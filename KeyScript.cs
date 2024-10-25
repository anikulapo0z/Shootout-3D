using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
    public GameObject doorCollider;
    public bool keyCollected = false;


    // Start is called before the first frame update
    void Start()
    {
        doorCollider.SetActive(true);
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, 45, 0) * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, Mathf.PingPong(Time.time, 0.5f) + 1.5f, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            keyCollected = true;
            other.gameObject.GetComponent<RaycastPlayer>().IncrementKeyCount();
            Destroy(gameObject);
        }
    }
    
}
