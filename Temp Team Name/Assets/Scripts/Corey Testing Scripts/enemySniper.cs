using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics.Internal;
using UnityEngine;

public class enemySniper : enemyParent, IDamage
{
    LineRenderer sniperLine;
    [SerializeField] int sniperDamage;
    [SerializeField] int sniperShootDistance;
    [SerializeField] int sniperWeaponCone;
    [SerializeField] int timeBeforeShot;
    new void Start()
    {
        base.Start();
        sniperLine = GetComponent<LineRenderer>();
    }

    new void Update()
    {
        base.Update();
        
    }

    protected override IEnumerator shoot()
    {
        // Begin shooting
        isShooting = true;

        // Sets sniper line at player position & turn on
        sniperLine.SetPosition(0, headPrefab.position);
        sniperLine.SetPosition(1, hit.point);
        sniperLine.enabled = true;

        // Finds where player is before coroutine
        playerDir = gameManager.instance.player.transform.position - headPrefab.position;

        // Debug
        Debug.DrawRay(headPrefab.position, playerDir);

        yield return new WaitForSeconds(shootRateTest);

        Vector3 playerDirNew = gameManager.instance.player.transform.position - headPrefab.position;
        angleToPlayer = Vector3.Angle(playerDirNew, transform.forward);


        // Shoot Raycast
        if (Physics.Raycast(headPrefab.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= sniperWeaponCone)
            {
                IDamage dmg = hit.collider.GetComponent<IDamage>();

                if (hit.transform != transform && dmg != null)
                {
                    dmg.TakeDamage(sniperDamage);
                }
            }
        }

        // Disable line after shot
        sniperLine.enabled = false;

        // End shooting
        isShooting = false;
        
    }
}


