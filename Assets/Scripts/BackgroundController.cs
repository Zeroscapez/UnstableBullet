using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float startPos;
    public float length;
    public Camera cam;
    public float scrollSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    // Update is called once per frame
    void Update()
    {
       //Move the background down
       float newPosition = Mathf.Repeat(Time.time * scrollSpeed, length);
        transform.position = new Vector3(transform.position.x, startPos - newPosition, transform.position.z);

     

       float camBottomEdge = cam.transform.position.y - cam.orthographicSize;

        if (transform.position.y + length < camBottomEdge)
        {
            startPos += length; // Reset position to create a looping effect
        }
    }
}
