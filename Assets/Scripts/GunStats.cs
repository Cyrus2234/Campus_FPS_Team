using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class GunStats : ScriptableObject
{
    public GameObject bullet;
    public int maxAmmo;
    public float shootRate, reloadTime;
}
