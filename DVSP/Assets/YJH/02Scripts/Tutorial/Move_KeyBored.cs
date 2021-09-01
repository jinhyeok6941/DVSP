using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_KeyBored : MonoBehaviour
{
    int speed = 10;
    void Update()
    {
        MoveOjb();
    }
    private void MoveOjb()
    {
        if (Input.GetKey(KeyCode.I)) transform.position += Vector3.forward * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.K)) transform.position -= Vector3.forward * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.J)) transform.position -= Vector3.right * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.L)) transform.position += Vector3.right * speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.W)) transform.position += Vector3.up * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S)) transform.position -= Vector3.up * speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.A)) transform.Rotate(Vector3.up, -20);
        if (Input.GetKey(KeyCode.D)) transform.Rotate(Vector3.up, 20);
    }
}
