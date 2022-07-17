using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    public bool animating = false;
    Vector3 initialPos;
    [SerializeField] BoxCollider col;
    [SerializeField] Rigidbody rb;
    [SerializeField] MeshCollider funnelCol;
    [SerializeField] GameManager gm;
    int delayFrames = 0;
    List<GameObject> slices;

    private void FixedUpdate()
    {
        if (delayFrames > 0 && --delayFrames == 0)
        {
            col.enabled = true;
            funnelCol.enabled = false;
        }
    }

    private void Start()
    {
        initialPos = transform.position;
    }
    public void animate(List<GameObject> slices)
    {
        animating = true;
        transform.position = initialPos;
        delayFrames = (int)(60 * (1 / gm.speed));
        this.slices = slices;
    }

    private void Update()
    {
        if (animating && delayFrames == 0)
        {
            transform.Translate(Vector3.forward * 55 * Time.deltaTime * (gm.speed));
            if (transform.position.x > 18)
            {
                animating = false;
                col.enabled = false;
                funnelCol.enabled = true;
                rb.velocity = Vector3.zero;
                gm.incrementScore();
                slices.ForEach(s => Destroy(s));
            }
            else
            {
                foreach (GameObject slice in slices)
                {
                    if (slice.transform.position.x < transform.position.x + 1.5f)
                    {
                        slice.transform.position = new Vector3(transform.position.x + 1.5f, slice.transform.position.y, slice.transform.position.z);
                    }
                }
            }
        }
    }
}
