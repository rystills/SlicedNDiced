using EzySlice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField] Material fruitMat;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
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
    }
}
