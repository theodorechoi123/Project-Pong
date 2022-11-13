using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bullet;
    public Transform firePosition;
    public Transform mainCam;
    public GameObject muzzleFlash;
    public GameObject bulletHole;
    
    public MuzzleFlash mf;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Shoot();
    }

    void Shoot()
    {
        if(Input.GetMouseButtonDown(0))
        {
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
            mf.MuzzleFlashing();
            Instantiate(bullet, firePosition.position, firePosition.rotation);
        }
        
    }
}
