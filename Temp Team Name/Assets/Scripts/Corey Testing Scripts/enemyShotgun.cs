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

}


