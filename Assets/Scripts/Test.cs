using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    [SerializeField]
    private GameObject cubePrefab;
    [SerializeField]
    private GameObject spherePrefab;

    // Update is called once per frame
    void Update()
    {
       //si la touche 1 est pressée
       if (Input.GetKeyDown("[1]"))
       {
           // créer game object cube prefb
              Instantiate(cubePrefab, new Vector3(0, 0, 0), Quaternion.identity);
              
       }
       //si la touche 2 est pressée
       if (Input.GetKeyDown("[2]"))
       {
           // créer game object cube prefab
           
           Instantiate(spherePrefab, new Vector3(0, 0, 0), Quaternion.identity);
              
       }
           
    }
}