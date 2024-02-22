using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyShotgun : enemyParent
{
    [SerializeField] float spreadFactor;
    [SerializeField] int bulletsPerShot;

    new void Start()
    {
        base.Start();
        
    }

    new void Update()
    {
        base.Update();
    }

    protected override IEnumerator shoot()
    {
        // Begin shooting
        isShooting = true;

        // Instantiate number of pellets
        for (int i =0;  i < bulletsPerShot; i++)
        {
            // Create a rotation for spread
            Quaternion bulletRot = transform.rotation;
            // Randomize spread
            bulletRot.x += Random.Range(-spreadFactor, spreadFactor);
            bulletRot.y += Random.Range(-spreadFactor, spreadFactor);
            Instantiate(bulletPrefab, shootPosPrefab.position, bulletRot);
        }
        
        yield return new WaitForSeconds(shootRateTest);

        // End shooting
        isShooting = false;
    }

}


