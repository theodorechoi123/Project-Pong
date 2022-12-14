using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [Header("Muzzle Flash")]
    public GameObject muzzleFlash;
    public Animator animator;
    public float mzMaxTimer = 1f;
    public float timer = 0f;
    public bool isFlashing;
    [Header("SFX")]
    public AudioManager audioManager;
    public WeaponsSwitchSystem switchSystem;
    private int sfxNumber;
    private string currentGunName;

    [Header("Recoil")]
    public Recoil recoil;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        currentGunName = switchSystem.activeGun.gunName;
        AudioManager();
        timer += Time.deltaTime;
        if(timer > mzMaxTimer)
        {
            muzzleFlash.SetActive(false);
            isFlashing = false;
        }
    }

    public void MuzzleFlashing()
    {
        isFlashing = true;
        muzzleFlash.SetActive(true);
        audioManager.PlayerSFX(sfxNumber);
        animator.SetTrigger("isShooting");
        recoil.RecoilFire();
        timer = 0f;
    }

    void AudioManager()
    {
        switch(currentGunName)
        {
            case "SniperRifle":
                sfxNumber = 0;
                break;
            case "DrumGun":
                sfxNumber = 1;
                break;
            case "BipodGun":
                sfxNumber = 2;
                break;
            default:
                break;
        }
    }

}
