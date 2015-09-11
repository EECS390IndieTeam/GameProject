using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gun : MonoBehaviour {

    public GameObject bulletPrefab;
    public int maxBulletsBeforeRecycle = 5;

	//Variables for Grenade
	public GameObject grenadePrefab;
	public int grenadeAmmo;

    private GameObject bullet;
    private Queue<GameObject> activeBullets;
    private Rigidbody bulletRigidBody;

    private Vector3 offset = new Vector3(0.0f, 0.0f, 1.0f);
    private Vector3 firingForce = new Vector3(0.0f, 0.0f, 1000.0f);

    private WaitForSeconds bulletTimeout = new WaitForSeconds(5.0f);
    private bool firing = false;
        
    void Start()
    {
        activeBullets = new Queue<GameObject>();
    }
	void Update () {
        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.C))
        {
            Fire();
            StartCoroutine(TimeOut());
        }

		/*
			 * --------------------------CREATE PROJECTILE---------------------------------------------------- 
			 */
		if (Input.GetKeyDown(KeyCode.G) && grenadeAmmo > 0) {
			Vector3 grenadePosition = GameObject.FindGameObjectWithTag("Player").transform.position;
			Quaternion grenadeRotation = GameObject.FindGameObjectWithTag("Player").transform.rotation;
			Instantiate(grenadePrefab, grenadePosition, grenadeRotation);
			grenadeAmmo--;
		}
	}
	
	private void Fire () {
        if(activeBullets.Count <= maxBulletsBeforeRecycle - 1)
        {
            bullet = Instantiate(bulletPrefab) as GameObject;
            activeBullets.Enqueue(bullet);
        }
        else
        {
            activeBullets.Enqueue(activeBullets.Dequeue());
            bullet = activeBullets.Peek();
            activeBullets.Enqueue(activeBullets.Dequeue());
        }
        bulletRigidBody = bullet.GetComponent<Rigidbody>();

        bullet.transform.position = transform.position + offset;
        bulletRigidBody.AddForce(firingForce);
    }

    private IEnumerator TimeOut()
    {
        yield return bulletTimeout;
        if (bullet != null)
        {
            Destroy(activeBullets.Dequeue());
        }

    }
    
}
