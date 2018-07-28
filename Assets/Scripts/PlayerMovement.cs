using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class PlayerMovement : MonoBehaviour
{
    public Transform shootPoint;
    public Transform trapPoint;
    public float MaxUpdownAngle = 30f;

    public float MoveSpeed = 3f;
    public float RotateSpeed = 3f;
    public Transform m_Cam;                  // A reference to the main camera in the scenes transform

    private PlayerInfo playerInfo;
        private Vector3 m_Move;
    private float updownAngle=0f;
        private Animator anim;                      // Reference to the animator component.
        private Rigidbody playerRigidbody;          // Reference to the player's rigidbody.
#if !MOBILE_INPUT
        private int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
        private float camRayLength = 100f;          // The length of the ray from the camera into the scene.
#endif

    void Awake()
    {
            // Set up references.
        anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        m_Cam = GetComponentInChildren<Camera>().transform;
        playerInfo = PlayerInfo.getinstance();
     }


    void FixedUpdate()
    {
        if (playerInfo.playerstate!=Config.PLAYER_STATE_DEAD && playerInfo.gameState!= Config.GAME_STATE_LOSE)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            m_Move = v * transform.forward + h * transform.right;
            m_Move *= MoveSpeed * Time.deltaTime;
            transform.position += m_Move;
            Rotate();
            // Animate the player.
            Animating(h, v);
        }
    }


    void Rotate()
    {
        //相机旋转
        // x方向任意旋转
        float x = RotateSpeed * Input.GetAxis("Mouse X");
        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles +
            Quaternion.AngleAxis(x, Vector3.up).eulerAngles
        );
        //锁定上下视角，并且改变武器射线发射角度
        updownAngle += RotateSpeed* 0.2f * Input.GetAxis("Mouse Y");
        float currentAngle = -Clamp(updownAngle, MaxUpdownAngle);
        //Debug.Log(-Clamp(updownAngle, MaxUpdownAngle));
        shootPoint.localEulerAngles = new Vector3(currentAngle, 0,0);
        trapPoint.localEulerAngles = new Vector3(trapPoint.localEulerAngles.x + currentAngle, 0, 0);
        m_Cam.localEulerAngles = new Vector3(currentAngle, m_Cam.localEulerAngles.y, m_Cam.localEulerAngles.z);

    }

    public float Clamp(float value, float max)
    {
        if (value > max) return max;
        if (value < -max) return -max;
        return value;
    }

        void Animating(float h, float v)
        {
            // Create a boolean that is true if either of the input axes is non-zero.
            bool walking = h != 0f || v != 0f;

            // Tell the animator whether or not the player is walking.
            anim.SetBool("IsWalking", walking);
        }
}
