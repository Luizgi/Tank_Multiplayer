using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour

{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject serverProjectile;
    [SerializeField] private GameObject clientProjectile;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Collider2D playerCollider;

    [Header("Settings")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float fireRate;
    [SerializeField] private float muzzleFlashDuration;


    private bool shouldFire;
    private float previousFireTimer;
    private float muzzleFlashTimer;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        inputReader.PrimaryFireEvent += HamdlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;

        inputReader.PrimaryFireEvent -= HamdlePrimaryFire;  
    }

    private void Update()
    {
        if (muzzleFlashTimer > 0f)
        {
            muzzleFlashTimer -= Time.deltaTime; 
            if(muzzleFlashTimer <= 0f)
            {
                muzzleFlash.SetActive(false);
            }
        }


        if (!IsOwner)
            return;

        if (!shouldFire)
            return;

        if(Time.time < (1/fireRate) + previousFireTimer)
        {
            return;
        }

        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

        previousFireTimer = Time.time;

    }


    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(serverProjectile, spawnPos, Quaternion.identity);
        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());


        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }

        SpawnDummyProjectileClientRpc(spawnPos, direction);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 direction)
    {
        if(IsOwner)
            return;


        SpawnDummyProjectile(spawnPos, direction);
    }
    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;

        GameObject projectileInstance = Instantiate(clientProjectile, spawnPos, Quaternion.identity);   
        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());


        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }

    }

    private void HamdlePrimaryFire(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }
}
