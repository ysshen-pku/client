using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform target;
    public float CameraSmoothTime = 0;
    public float RotateSpeed =6f;
    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        //相机跟随旋转
        float x = RotateSpeed * Input.GetAxis("Mouse X");
        float y = RotateSpeed * Input.GetAxis("Mouse Y");
        //以下为相机与角色同步旋转是
        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles +
            Quaternion.AngleAxis(x, Vector3.up).eulerAngles
        );//原理： 物体当前的欧拉角 + 鼠标x轴上的增量所产生的夹角

        target.rotation = Quaternion.Euler(
            target.transform.rotation.eulerAngles +
            Quaternion.AngleAxis(x, Vector3.up).eulerAngles
        );//同理
          //------------------------------------------------------>>>>>>>>
          //相机跟随移动
        Vector3 TargetCameraPosition = target.transform.TransformPoint(new Vector3(0, 4.5f, -5.5f));//获取相机跟随的相对位置，再转为世界坐标

        transform.position = Vector3.SmoothDamp(
            transform.position,
            TargetCameraPosition,
            ref velocity,
            CameraSmoothTime, //最好为0
            Mathf.Infinity,
            Time.deltaTime
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
