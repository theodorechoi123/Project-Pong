using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Shooting")]
    public MuzzleFlash mf;
    public GameObject bullet;
    public Transform firePosition;
    public Transform mainCam;
    public GameObject muzzleFlash;
    public GameObject bulletHole;
    public bool canAutoFire;
    public float timeBetweenShots;

    private bool shooting, readyToShoot = true;

    [Header("Reloading")]
    public int bulletsAvailable;
    public int totalBullets;
    public int magazineSize;
    public float reloadTime;
    
    private bool reloading;

    [Header("UI")]
    private UICanvasController uiCanvas;

    
    
    

    // Start is called before the first frame update
    void Start()
    {
        totalBullets -= magazineSize;
        bulletsAvailable = magazineSize;

        uiCanvas = FindObjectOfType<UICanvasController>();
    }

    // Update is called once per frame
    void Update()
    {
        Shoot();
        GunManager();
        UpdateAmmoText();
    }

    void GunManager()
    {
        if(Input.GetKeyDown(KeyCode.R) && bulletsAvailable < magazineSize && !reloading)
        {
            Reload();
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
                if(Vector3.Distance(mainCam.position, hit.point) > 2f)
                {
                    //firePosition looks at the raycast hit point
                    firePosition.LookAt(hit.point);

                    if(hit.collider.tag == "Ground")
                    {
                        var bulletHoleInstance = Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal));
                        Destroy(bulletHoleInstance, 5f);
                    }
                    if(hit.collider.tag == "Enemy")
                    {
                        Destroy(hit.collider.gameObject);
                    }
                }
            }
            else
            {
                //firePosition looks at a position in front of the camera view that looks like the center of the screen
                firePosition.LookAt(mainCam.position + (mainCam.forward * 50f));
            }

            bulletsAvailable--;
            
            //muzzle flashing by calling the MuzzleFlash script
            mf.MuzzleFlashing();
            //creates a bullet
            Instantiate(bullet, firePosition.position, firePosition.rotation, firePosition);

            //takes time to resetshot
            StartCoroutine(ResetShot());

            
        }
        
    }

    void Reload()
    {
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

        reloading = true;
        StartCoroutine(ReloadCoroutine());
    }

    IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadTime);

        reloading = false;
    }

    IEnumerator ResetShot()
    {
        yield return new WaitForSeconds(timeBetweenShots);
        readyToShoot = true;
    }

    void UpdateAmmoText()
    {
        uiCanvas.ammoText.SetText(bulletsAvailable + "/" + magazineSize);
        uiCanvas.totalAmmoText.SetText(totalBullets.ToString());
    }
}
