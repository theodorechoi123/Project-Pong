using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bullet;
    public bool isRocketLauncher;
    public MuzzleFlash mf;
    public Transform firePosition;
    public Transform mainCam;
    public GameObject muzzleFlash;
    public GameObject bulletHole;
    public bool canAutoFire;
    public float timeBetweenShots;
    public string gunName;
    public bool readyToShoot = true;
    private PlayerMovement playerMovement;
    private bool shooting;
    

    [Header("Reloading")]
    public int bulletsAvailable;
    public int totalBullets;
    public int magazineSize;
    public float reloadTime;
    public bool reloading;
    public int pickupBulletAmount;

    [Header("UI")]
    private UICanvasController uiCanvas;

    [Header("Animations / Scoping")]
    public Animator gunAnimator;
    public bool canScope = false;
    public bool isScoped = false;
    public GameObject scopeOverlay;
    public GameObject gunModel;
    public Camera myCamera;
    public float scopedFOV = 15f;
    public float normalFOV;
    public GameObject bloodSplat;
    private string gunAnimationName;
    private int gunSFXNumber;

    [Header("DIFFERENT GUN DMG")]
    public int damageAmount;
    
    

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponentInParent(typeof(PlayerMovement)) as PlayerMovement;
        totalBullets -= magazineSize;
        bulletsAvailable = magazineSize;

        uiCanvas = FindObjectOfType<UICanvasController>();
        normalFOV = myCamera.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        if(PauseMenu.gameIsPaused)
        {
            return;
        }
        if(!playerMovement.isDead)
        {
            Shoot();
            GunManager();
            UpdateAmmoText();
            Scope();
            AnimationManager();
        }
        
    }

    void GunManager()
    {
        if(Input.GetKeyDown(KeyCode.R) && bulletsAvailable < magazineSize && !reloading)
        {
            Reload();
        }
    }

    void AnimationManager()
    {
        switch(gunName)
        {
            case "SniperRifle":
                gunAnimationName = "sniperRifleReload";
                gunSFXNumber = 3;
                break;
            case "DrumGun":
                gunAnimationName = "drumGunReload";
                gunSFXNumber = 4;
                break;
            case "RocketLauncher":
                gunAnimationName = "rocketLauncherReload";
                gunSFXNumber = 5;
                break;
            case "BipodGun":
                gunAnimationName = "bipodGunReload";
                gunSFXNumber = 6;
                break;
            default:
                break;
        }
    }

    void Shoot()
    {
        if(canAutoFire)
        {
            shooting = Input.GetMouseButton(0);
        }
        else
            shooting = Input.GetMouseButtonDown(0);
        if(shooting && readyToShoot && bulletsAvailable > 0 && !reloading)
        {
            readyToShoot = false;
            RaycastHit hit;
            if(Physics.Raycast(mainCam.position, mainCam.forward, out hit, 100f))
            {
                if(Vector3.Distance(mainCam.position, hit.point) > 0f)
                {
                    //firePosition looks at the raycast hit point
                    firePosition.LookAt(hit.point);
                    if(!isRocketLauncher)
                    {
                        if(hit.collider.tag == "Ground")
                        {
                            var bulletHoleInstance = Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal));
                            Destroy(bulletHoleInstance, 5f);
                        }
                    }
                }

                if(hit.collider.tag == "Enemy" && !isRocketLauncher)
                {
                    hit.collider.GetComponent<EnemyHealthSystem>().TakeDamage(damageAmount);
                    AudioManager.instance.PlayerSFX(10);
                    Instantiate(bloodSplat, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }
            else
            {
                //firePosition looks at a position in front of the camera view that looks like the center of the screen
                firePosition.LookAt(mainCam.position + (mainCam.forward * 50f));
            }

            bulletsAvailable--;
            
            if(!isRocketLauncher)
            {
                //muzzle flashing by calling the MuzzleFlash script
                mf.MuzzleFlashing();
            }
            else
            {
                Instantiate(bullet, firePosition.position, firePosition.rotation);
            }
            

            //takes time to resetshot
            StartCoroutine(ResetShot());
            
            
        }
        
    }

    public void AddAmmo()
    {
        totalBullets += pickupBulletAmount;
    }

    void Reload()
    {
        gunAnimator.SetTrigger(gunAnimationName);
        AudioManager.instance.PlayerSFX(gunSFXNumber);
        
        reloading = true;
        
        StartCoroutine(ReloadCoroutine());
    }

    IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadTime);
        int bulletsToAdd = magazineSize - bulletsAvailable;

        if(totalBullets > bulletsToAdd)
        {
            totalBullets -= bulletsToAdd;
            bulletsAvailable = magazineSize;
        }
        else
        {
            bulletsAvailable += totalBullets;
            totalBullets = 0;
        }
        reloading = false;
    }

    IEnumerator ResetShot()
    {
        yield return new WaitForSeconds(timeBetweenShots);
        gunAnimator.SetTrigger("isShooting");
        readyToShoot = true;
    }

    void UpdateAmmoText()
    {
        uiCanvas.ammoText.SetText(bulletsAvailable + "/" + magazineSize);
        uiCanvas.totalAmmoText.SetText(totalBullets.ToString());
    }

    void Scope()
    {
        if(Input.GetMouseButtonDown(1) && canScope)
        {
            isScoped = !isScoped;
            gunAnimator.SetBool("isScoping", isScoped);
        }
        if(isScoped)
        {
            StartCoroutine(OnScoped());
        }
        else
            OnUnscoped();
    }

    void OnUnscoped()
    {
        scopeOverlay.SetActive(false);
        gunModel.SetActive(true);

        myCamera.fieldOfView = normalFOV;
    }

    IEnumerator OnScoped()
    {
        yield return new WaitForSeconds(.15f);
        scopeOverlay.SetActive(true);
        gunModel.SetActive(false);

        myCamera.fieldOfView = scopedFOV;
    }
}
