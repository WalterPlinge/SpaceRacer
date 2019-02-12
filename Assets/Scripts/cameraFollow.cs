
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public GameObject ship;
    public float shipX;
    public float shipY;
    public float shipZ;

    //update is called every frame
    void Update()
    {
        shipX = ship.transform.eulerAngles.x;
        shipY = ship.transform.eulerAngles.y;
        shipZ = ship.transform.eulerAngles.z;

        transform.eulerAngles = new Vector3(shipX-shipX,shipY,shipZ-shipZ);

    }
}

