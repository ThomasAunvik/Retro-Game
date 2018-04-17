using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {

    public float shootingRange = 200;
    public float shootingDelay = 2;
    public bool bypassPlayer = false;

    float shootTime = 0;

    public GameObject bullet;
    public Transform bulletSpawn;

    public LayerMask playerMask;

	void Start () {
		
	}
	
	void Update () {
        RaycastHit2D hit = Physics2D.Raycast(bulletSpawn.position, transform.right, shootingRange, playerMask);
        if (hit.transform)
        {
            Player player = hit.transform.GetComponent<Player>();
            if (player)
            {
                if(Time.timeSinceLevelLoad > shootTime + shootingDelay)
                {
                    if (!GameManager.instance.freeze)
                    {
                        ShootBullet();
                    }
                    shootTime = Time.timeSinceLevelLoad;
                }
            }
        }

        if (bypassPlayer && Time.timeSinceLevelLoad > shootTime + shootingDelay)
        {
            if (!GameManager.instance.freeze)
            {
                ShootBullet();
            }
            shootTime = Time.timeSinceLevelLoad;
        }
	}

    public void ShootBullet()
    {
        GameObject bullet = Instantiate(this.bullet, bulletSpawn.position, bulletSpawn.rotation, bulletSpawn);
        bullet.GetComponent<Bullet>().cannon = this;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(bulletSpawn.position, bulletSpawn.position + (transform.right * shootingRange));
    }
}
