using System;
using System.Collections;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public Camera playerCamera;

    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    public float spreadIntensity = 1f; // Typical spread intensity value
    public float minRecoilDistance = -0.1f; // Minimum recoil distance (can be negative for forward movement)
    public float maxRecoilDistance = 0.1f;  // Maximum recoil distance
    public float returnTime = 0.2f; // Time it takes for the gun to return to its initial position

    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f;
    private AudioSource audioSource;
    public AudioClip fireSound;

    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }
    public ShootingMode currentShootingMode;

    private Vector3 initialPosition;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        audioSource = GetComponent<AudioSource>();
        initialPosition = transform.localPosition; // Save the initial local position of the gun
    }

    void Update()
    {
        if (currentShootingMode == ShootingMode.Auto)
        {
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
        {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (readyToShoot && isShooting)
        {
            burstBulletsLeft = bulletsPerBurst;
            Fire();
        }
    }

    public void Fire()
    {
        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirectionAndASpread().normalized;
        Debug.Log($"Shooting Direction: {shootingDirection}"); // Debug log for shooting direction

        GameObject newBullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        newBullet.transform.forward = shootingDirection;
        newBullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        StartCoroutine(DestroyBulletAfterTime(newBullet, bulletPrefabLifeTime));

        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }
        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("Fire", shootingDelay);
        }

        PlayFireSound();
        ApplyRecoil();
    }

    void PlayFireSound()
    {
        if (audioSource != null && fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    private Vector3 CalculateDirectionAndASpread()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        // Add spread to the direction
        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        Debug.Log($"Spread X: {x}, Spread Y: {y}"); // Debug log for spread values

        return direction + new Vector3(x, y, 0);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject newBullet, float bulletPrefabLifeTime)
    {
        yield return new WaitForSeconds(bulletPrefabLifeTime);
        Destroy(newBullet);
    }

    private void ApplyRecoil()
    {
        float recoilDistance = UnityEngine.Random.Range(minRecoilDistance, maxRecoilDistance);
        Vector3 recoilPosition = initialPosition + transform.forward * recoilDistance;
        StartCoroutine(RecoilRoutine(recoilPosition));
    }

    private IEnumerator RecoilRoutine(Vector3 recoilPosition)
    {
        float elapsedTime = 0f;
        Vector3 startingPosition = transform.localPosition;

        while (elapsedTime < returnTime)
        {
            transform.localPosition = Vector3.Lerp(startingPosition, recoilPosition, elapsedTime / returnTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        startingPosition = transform.localPosition;

        while (elapsedTime < returnTime)
        {
            transform.localPosition = Vector3.Lerp(startingPosition, initialPosition, elapsedTime / returnTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = initialPosition;
    }
}
