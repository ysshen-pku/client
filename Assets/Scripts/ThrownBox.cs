using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownBox : MonoBehaviour
{
        public const float g = 9.8f;

        public GameObject target;
        public float speed = 5;
        private float verticalSpeed;
        private Vector3 moveDirection;

        private float angleSpeed;
        private float angle;
    private float time;
    private bool isfree;

    void Start()
        {
            float tmepDistance = Vector3.Distance(transform.position, target.transform.position);
            float tempTime = tmepDistance / speed;
            float riseTime, downTime;
            riseTime = downTime = tempTime / 2;
            verticalSpeed = g * riseTime;
            transform.LookAt(target.transform.position);

            float tempTan = verticalSpeed / speed;
            double hu = Mathf.Atan(tempTan);
            angle = (float)(180 / Mathf.PI * hu);
            transform.eulerAngles = new Vector3(-angle, transform.eulerAngles.y, transform.eulerAngles.z);
            angleSpeed = angle / riseTime;

            moveDirection = target.transform.position - transform.position;
        }
        

    void Update()
    {
        if (transform.position.y < target.transform.position.y && !isfree)
        {
            isfree = true;
            StartCoroutine("Free");
            return;
        }
        time += Time.deltaTime;
        float test = verticalSpeed - g * time;
        transform.Translate(moveDirection.normalized * speed * Time.deltaTime, Space.World);
        transform.Translate(Vector3.up * test * Time.deltaTime, Space.World);
        float testAngle = -angle + angleSpeed * time;
        transform.eulerAngles = new Vector3(testAngle, transform.eulerAngles.y, transform.eulerAngles.z);
    }


    IEnumerator Free()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}