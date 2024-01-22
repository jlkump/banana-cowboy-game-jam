using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractor : MonoBehaviour
{

    public float gravity = -10.0f;

    public void Attract(Transform body)
    {
        Vector3 targetDir = (body.position - transform.position).normalized;
        //Vector3 bodyUp = body.up;

        //body.rotation = Quaternion.FromToRotation(bodyUp, targetDir) * body.rotation;
        body.GetComponent<Rigidbody>().AddForce(targetDir * gravity);
    }

    public void Reorient(Transform body)
    {
        Vector3 targetDir = (body.position - transform.position).normalized;
        Vector3 bodyUp = body.up;

        body.rotation = Quaternion.FromToRotation(bodyUp, targetDir) * body.rotation;
    }
}
