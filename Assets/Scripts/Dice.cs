using EzySlice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField] Material fruitMat;
    [SerializeField] Rigidbody rb;
    int numIdleFrames = 5;

    void Start()
    {
        // randomize initial rotation and set semi-random "throw" force
        transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        rb.AddForce(new Vector3(Random.Range(1.5f, 2.7f), Random.Range(.8f, 1.4f), Random.Range(-.2f, .2f))*5,ForceMode.Impulse);
        rb.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))*10, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            GameObject[] halves = gameObject.SliceInstantiate(transform.position, Vector3.up);
            Destroy(gameObject);
            MeshRenderer mr;
            foreach (GameObject go in halves)
            {
                List<Material> mats = new List<Material>();
                mr = go.GetComponent<MeshRenderer>();
                mr.GetMaterials(mats);
                mats[1] = fruitMat;
                mr.materials = mats.ToArray();
            }
        }
        if (rb.velocity.magnitude < .1f)
        {
            if (--numIdleFrames == 0)
            {
                Debug.Log("idle");
            }
        }
    }
}
