using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    [SerializeField] ParticleSystem psBoom;

    public void PlayExplosion() {
        psBoom.Play();
        //TODO(Rok Kos): Use polling
        Destroy(this.gameObject, psBoom.main.duration);
        Debug.LogFormat("<color=green>ExplosionController::</color> <color=red>Playing explosion, will delete in {0} s</color>", psBoom.main.duration);
    }


}
