using UnityEngine;
using UnityEngine.InputSystem;

// Script này lo về vị trí đầu, góc xoay đầu đã có file khác quản lý
// Đọc WASD ghép thành hướng di chuyển, cộng vào transform position. Bỏ qua thành phần Y.
// Lưu vị trí gốc ở Start, giới hạn khoảng cách so với gốc bằng ClampMagnitude.
public class HeadMover : MonoBehaviour
{
    // Tốc độ dịch đầu (m/s)
    [SerializeField] private float moveSpeed = 0.3f;
    // Khoảng cách tối đa so với tâm sphere (m)
    [SerializeField] private float maxDistance = 0.3f;

    // Vị trí gốc (tâm hình cầu)
    private Vector3 originPosition;

    // Độ lệch hiện tại so với vị trí gốc, để SettingsPanel đọc và hiển thị
    public Vector3 OffsetFromOrigin => transform.position - originPosition;

    void Start()
    {
        // Chạy một lần khi game bắt đầu
        // -> Lưu lại vị trí hiện tại của đầu làm tâm
        originPosition = transform.position;
    }

    void Update()
    {
        // 1. ĐỌC PHÍM: CẬP NHẬT THAM SỐ x (TRÁI PHẢI), z (TIẾN LÙI) TƯƠNG ỨNG
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return; // Không có phím thì bỏ qua

        float x = 0f;
        float z = 0f;
        // Nhấn D --> Sang phải
        if (keyboard.dKey.isPressed) { x += 1f; }
        // Nhấn A --> Sang trái
        if (keyboard.aKey.isPressed) { x -= 1f; }

        // Nhấn W --> Tiến
        if (keyboard.wKey.isPressed) { z += 1f; }
        // Nhấn S --> Lùi
        if (keyboard.sKey.isPressed) { z -= 1f; }

        // Không nhấn gì --> thoát, đỡ tính toán
        if (x == 0f && z == 0f) return;

        // 2. LẤY HƯỚNG PHẢI / TIẾN CỦA ĐẦU, BỎ Y ĐỂ CHỈ DI CHUYỂN NGANG MẶT PHẲNG
        Vector3 right = transform.right;
        Vector3 forward = transform.forward;
        right.y = 0;
        forward.y = 0;

        right.Normalize();
        forward.Normalize();

        // 3. GHÉP THÀNH VECTOR DI CHUYỂN
        Vector3 direction = (right * x + forward * z).normalized;

        // 4. Dịch vị trí theo tốc độ và thời gian của frame
        Vector3 newPosition = transform.position + direction * moveSpeed * Time.deltaTime;

        // 5. GIỚI HẠN DI CHUYỂN
        Vector3 offset = Vector3.ClampMagnitude(newPosition - originPosition, maxDistance);
        transform.position = originPosition + offset;
    }

    // Đưa đầu về vị trí gốc (gọi từ nút "Reset vị trí")
    public void ResetPosition()
    {
        transform.position = originPosition;
    }
}