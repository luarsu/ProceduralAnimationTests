using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveMind : MonoBehaviour {

    Mesh mesh;
    public MeshFilter mFilter;
    //public SkinnedMeshRenderer mFilter;
    public Vector3[] vertices;
    public List<Bee> hive;
    public GameObject bee;

    public Vector3 lastPosition;


    // Use this for initialization
    
    void Start () {
        mesh = mFilter.sharedMesh;
        vertices = mesh.vertices;
        //.Log(vertices.Length);

        spawnBees();

    }
	
	// Update is called once per frame
	void Update () {

        if(lastPosition != transform.position)
        {
            for (int i = 0; i < hive.Count; i++)
            {
                hive[i].UpdateTarget(vertices[i] + transform.position);

            }
        }
        lastPosition = transform.position;
    }

    void spawnBees()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            GameObject beeObject = Instantiate(bee, new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5)), Quaternion.identity);
            Bee beeInstance = beeObject.GetComponent<Bee>();
            hive.Add(beeInstance);

            beeInstance.UpdateTarget(vertices[i] + transform.position);

        }
    }
}
