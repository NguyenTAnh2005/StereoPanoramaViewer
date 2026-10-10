using UnityEngine;
using UnityEngine.InputSystem;

public class HeadController : MonoBehaviour
{
    // Mô phỏng chuyển động đầu: 
    // yaw - trái phải
    // pitch - lên xuống
    // KHÔNG mô phòng roll vì chuột chỉ có 2 chiều yaw và pitch
    [SerializeField] private float sensitivity = 0.1f;
    private float yaw;
    private float pitch;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Ở mỗi frame thì đọc lượng chuột di chuyển
        // Cộng vào 2 biến yaw và pitch 
        // pitch giới hạn trong khoảng -89 --> + 89 vì nếu bằng 90 sẽ gây ra hiện tượng gimblock
        // Biến 2 góc thành một phép xuay rồi gán cho camera
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.rightButton.isPressed)
        {
            Vector2 delta = mouse.delta.ReadValue();
            yaw += delta.x * sensitivity;
            pitch -= delta.y * sensitivity;
            pitch = Mathf.Clamp(pitch, -89f, 89f);

            transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }
}
