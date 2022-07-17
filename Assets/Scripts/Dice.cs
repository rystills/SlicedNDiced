using EzySlice;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField] Material fruitMat;
    [SerializeField] Material diceMat;
    [SerializeField] GameObject lrPrefab;
    public Knife knife;
    [SerializeField] Rigidbody rb;
    public GameManager gm;
    LineRenderer lr;
    int numIdleFrames = 10;
    public bool readyToCut = false;
    bool midCut = false;
    List<int> sideVals = new List<int>
    {
        3,4,1,6,5,2
    };
    public int targetCutCount;
    public List<LineRenderer> cutRenderers = new List<LineRenderer>();

    void Start()
    {
        // randomize initial rotation and set semi-random "throw" force
        transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        rb.AddForce(new Vector3(Random.Range(1.7f, 2.8f), Random.Range(1.4f, 2.1f), Random.Range(-.3f, .3f))*5,ForceMode.Impulse);
        rb.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))*10, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        // check idle time
        if (rb.velocity.magnitude < .1f)
        {
            if (--numIdleFrames == 0)
            {
                if (transform.position.y > -2)
                {
                    numIdleFrames = 5;
                    transform.position = new Vector3(transform.position.x + 1f, -2, transform.position.z);
                }
                else
                {
                    readyToCut = true;
                    List<Vector3> sideAngles = new List<Vector3>
                    {
                        transform.up,
                        -transform.up,
                        transform.right,
                        -transform.right,
                        transform.forward,
                        -transform.forward
                    };
                    targetCutCount = sideVals[sideAngles.IndexOf(sideAngles.Aggregate((a, b) => Vector3.Angle(a, Vector3.up) < Vector3.Angle(b, Vector3.up) ? a : b))];
                }
            }
        }
    }

    private bool ValidateCut(Vector3 s, Vector3 e)
    {
        s.y -= 2.5f;
        e.y -= 2.5f;
        //Debug.DrawRay(s,(e-s),Color.blue,100);
        return Physics.Raycast(new Ray(s, (e-s).normalized), (e-s).magnitude, LayerMask.GetMask("dice")) &&
               Physics.Raycast(new Ray(e, (s-e).normalized), (s-e).magnitude, LayerMask.GetMask("dice"));
    }

    private void Update()
    {
        if (readyToCut)
        {
            if (Input.GetMouseButtonDown(0))
            {
                midCut = true;
                lr = Instantiate(lrPrefab).GetComponent<LineRenderer>();
                lr.positionCount += 2;
                lr.SetPosition(0, Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 19));
                lr.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 19));
            }
            else if (midCut)
            {
                lr.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 19));
                if (Input.GetMouseButtonUp(0)) {
                    midCut = false;
                    gameObject.layer = LayerMask.NameToLayer("dice");
                    if (!ValidateCut(lr.transform.TransformPoint(lr.GetPosition(0)), lr.transform.TransformPoint(lr.GetPosition(1))))
                    {
                        Destroy(lr.gameObject);
                    }
                    else
                    {
                        cutRenderers.Add(lr);
                    }
                    gameObject.layer = LayerMask.NameToLayer("Default");
                }
            }
        }
    }

    public List<GameObject> Slice()
    {
        List<GameObject> slices = new List<GameObject>() { gameObject };
        foreach (LineRenderer lr in cutRenderers)
        {
            List<GameObject> newSlices = new List<GameObject>();
            for (int i = 0; i < slices.Count; ++i)
            {
                try
                {
                    newSlices.AddRange(slices[i].SliceInstantiate(lr.GetPosition(0) - Vector3.up * 2.5f, Vector3.Cross(lr.GetPosition(1) - lr.GetPosition(0), Vector3.up)));
                    Destroy(slices[i]);
                    slices.RemoveAt(i--);
                    continue;
                }
                catch (System.Exception e)
                {
                    // attempted to slice a non-intersecting poly (or unhandled error)
                }
            }
            slices.AddRange(newSlices);
        }
        MeshRenderer mr;
        foreach (GameObject go in slices)
        {
            go.AddComponent<Rigidbody>().AddExplosionForce(2500, go.transform.position, 100, -20);
            go.AddComponent<MeshCollider>().convex = true;
            List<Material> mats = new List<Material>();
            mr = go.GetComponent<MeshRenderer>();
            mr.GetMaterials(mats);
            for (int i = 0; i < mats.Count; ++i)
            {
                if (mats[i].mainTexture != diceMat.mainTexture)
                {
                    mats[i] = fruitMat;
                }
            }
            mr.materials = mats.ToArray();
        }
        cutRenderers = new List<LineRenderer>();
        return slices;
    }
}
