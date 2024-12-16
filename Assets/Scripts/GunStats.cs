using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class GunStats : ScriptableObject
{
    public GameObject Model;
    public int GunDamage;
    public float FireRate;
    public int GunDist;
    public int ammoCur, ammoMax;

    public ParticleSystem hitEffect;
    public AudioClip[] GunSound;
    public float GunSoundVol;
}
