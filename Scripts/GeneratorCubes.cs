
using System.Collections.Generic;
using UnityEngine;


public class GeneratorCubes : MonoBehaviour
{
    // Reference to block prefab
    [SerializeField] public GameObject cube;

    public List<GameObject> cubesGenerated;
    // Total cubes to be generated
    public int totalCubes = 5;

 
    /**
     * Initialize block with the hand grab indicated and the 3D position
     */
    public void Intilialize( List<Vector3> positions)
    {


        GenerateCubes(positions);
    }

    private void GenerateCubes( List<Vector3> positions)
    {
        foreach (Vector3 cubePosition in positions)
        {
            var c = Instantiate(cube);
            c.transform.position = cubePosition;
            cubesGenerated.Add(c);
        }

    }

            

    


}