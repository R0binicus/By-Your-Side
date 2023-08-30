using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class SceneManager : MonoBehaviour
{
    public NavMeshSurface surface;

    public Transform objectToRotate;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        objectToRotate.localRotation = Quaternion.Euler(new Vector3(0, 15 * Time.deltaTime, 0) + objectToRotate.localRotation.eulerAngles);
        //surface.BuildNavMesh();
    }

    public void UpdatePath()
    {
        surface.BuildNavMesh();
    }
}
