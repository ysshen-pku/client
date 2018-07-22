using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[Serializable]
public class MouseLook
{
    public float XSensitivity = 6f;//X灵敏度
    public float YSensitivity = 6f;//Y灵敏度
    public bool clampVerticalRotation = true;//是否夹紧视角
    public float MinimumX = -90F;//绕x轴旋转角度，低头极限
    public float MaximumX = 90F;//绕x轴旋转角度，抬头极限
    public bool smooth;//是否开启平滑视角过度
    public float smoothTime = 2f;//平滑时间


    private Quaternion m_CharacterTargetRot;//角色旋转的目标角度（角色旋转会带动相机旋转）
    private Quaternion m_CameraTargetRot;//相机旋转的目标角度（相机旋转不会带动角色旋转）


    public void Init(Transform character, Transform camera)
    {
        m_CharacterTargetRot = character.localRotation;
        m_CameraTargetRot = camera.localRotation;
    }


    public void LookRotation(Transform character, Transform camera)
    {
        float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
        float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;
        //请注意，mouseX转换为y轴旋转，mouseY转换为x轴旋转
        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);//左右视角
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);//上下视角

        if (clampVerticalRotation)//夹紧x轴旋转角度[-90,90]，亦即上下视角,默认开启
            m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);


        if (smooth)
        {   //划动视角时，是否开启平滑模式，slerp进行插值，默认关闭
            character.localRotation = Quaternion.Slerp(character.localRotation,

            m_CharacterTargetRot,
            smoothTime * Time.deltaTime);
            camera.localRotation = Quaternion.Slerp(camera.localRotation,

            m_CameraTargetRot,
            smoothTime * Time.deltaTime);
        }
        else
        {
            character.localRotation = m_CharacterTargetRot;
            camera.localRotation = m_CameraTargetRot;
        }
    }


    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;
        //见下文对四元数的解释

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        //Mathf.Clamp将数值归入到指定区间，如数值已在区间内无变化，若数值超出区间则就近归入到区间端点。
        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);
        //Tan为三角函数
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}

