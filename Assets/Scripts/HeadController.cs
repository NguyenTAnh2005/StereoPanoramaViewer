using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Mô phỏng xoay đầu bằng chuột (không cần kính VR): giữ chuột phải và rê để xoay.
/// Chỉ có yaw (trái-phải) và pitch (lên-xuống); KHÔNG có roll vì chuột chỉ có 2 chiều.
/// Vị trí đầu do HeadMover quản lý, script này chỉ lo góc xoay.
/// </summary>
public class HeadController : MonoBehaviour
{
    // Độ nhạy chuột (độ xoay trên mỗi đơn vị chuột di chuyển)
    [SerializeField] private float sensitivity = 0.1f;

    private float yaw;    // Góc xoay trái-phải (độ)
    private float pitch;  // Góc xoay lên-xuống (độ)

    void Update()
    {
        // Mỗi frame: đọc lượng chuột di chuyển, cộng vào yaw và pitch,
        // đổi hai góc thành một phép xoay (Quaternion) rồi gán cho đối tượng.
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.rightButton.isPressed)
        {
            Vector2 delta = mouse.delta.ReadValue();
            yaw += delta.x * sensitivity;
            pitch -= delta.y * sensitivity;
            // Giới hạn pitch trong [-89, 89]: tại ±90° góc Euler suy biến (gimbal lock)
            pitch = Mathf.Clamp(pitch, -89f, 89f);

            transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }
}