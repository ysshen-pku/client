using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform player;
    public float CameraSmoothTime = 0;
    public float RotateSpeed =6f;
    private Vector3 velocity = Vector3.zero;


    void Update()
    {
        //相机跟随旋转
        float x = RotateSpeed * Input.GetAxis("Mouse X");
        float y = RotateSpeed * Input.GetAxis("Mouse Y");
        //以下为相机与角色同步旋转是
        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles +
            Quaternion.AngleAxis(x, Vector3.up).eulerAngles
        );//原理： 物体当前的欧拉角 + 鼠标x轴上的增量所产生的夹角

        player.rotation = Quaternion.Euler(
            player.transform.rotation.eulerAngles +
            Quaternion.AngleAxis(x, Vector3.up).eulerAngles
        );

    }
    /*
    void LateUpdate()
    {
        Vector3 nextpos = target.forward * -1 * distanceH + target.up * distanceV + target.position;

        this.transform.position = Vector3.Lerp(this.transform.position, nextpos, smoothSpeed * Time.deltaTime); //平滑插值

        this.transform.LookAt(target);
    }
    */
}
