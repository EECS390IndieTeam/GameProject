using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

    public GameObject bulletPrefab;

    private GameObject bullet;
    private Rigidbody bulletRigidBody;

    private Vector3 offset = new Vector3(0.0f, 0.0f, 1.0f);
    private Vector3 firingForce = new Vector3(0.0f, 0.0f, 1000.0f);

    private WaitForSeconds bulletTimeout = new WaitForSeconds(5.0f);
    private bool firing = false;
        
    void Start()
    {
    }
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
            StartCoroutine(TimeOut());
        }
	}
	
	private void Fire () {
        if(bullet == null)
        {
            bullet = Instantiate(bulletPrefab) as GameObject;
            bulletRigidBody = bullet.GetComponent<Rigidbody>();
        }
        else
        {
            bulletRigidBody.isKinematic = true;
            bulletRigidBody.isKinematic = false;
        }
        bullet.transform.position = transform.position + offset;
        bulletRigidBody.AddForce(firingForce);
    }

    private IEnumerator TimeOut()
    {
        yield return bulletTimeout;
        if (bullet != null)
        {
            Destroy(bullet);
        }

    }
    
}
