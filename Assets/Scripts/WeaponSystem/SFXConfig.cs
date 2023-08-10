using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapons/SFXConfig")]
public class SFXConfig : ScriptableObject
{
    //Could be a list for variance
    [SerializeField] private AudioClip _shootClip;
    [SerializeField] private AudioClip _ejectClip;
    [SerializeField] private AudioClip _loadClip;
    
    public AudioClip GetShootClip()
    {
        return _shootClip;
    }

    public AudioClip GetEjectClip()
    {
        return _ejectClip;
    }

    public AudioClip GetLoadClip()
    {
        return _loadClip;
    }
}
