using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Dịch chuyển vị trí đầu bằng phím WASD để quan sát thị sai (motion parallax).
/// Chỉ di chuyển trên mặt phẳng ngang (bỏ thành phần Y) và bị giới hạn quanh vị trí gốc
/// bằng ClampMagnitude. Góc xoay đầu do HeadController quản lý.
/// </summary>
public class HeadMover : MonoBehaviour
{
    // Tốc độ dịch đầu (m/s)
    [SerializeField] private float moveSpeed = 0.3f;
    // Khoảng cách tối đa so với tâm cầu (m)
    [SerializeField] private float maxDistance = 0.3f;

    // Vị trí gốc (tâm hình cầu), lưu lại ở Start
    private Vector3 originPosition;

    // Độ lệch hiện tại so với vị trí gốc, để SettingsPanel đọc và hiển thị
    public Vector3 OffsetFromOrigin => transform.position - originPosition;

    void Start()
    {
        // Lưu vị trí hiện tại của đầu làm tâm
        originPosition = transform.position;
    }

    void Update()
    {
        // 1. ĐỌC PHÍM: x (trái-phải), z (tiến-lùi)
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;  // Không có bàn phím thì bỏ qua

        float x = 0f;
        float z = 0f;
        if (keyboard.dKey.isPressed) { x += 1f; }  // D: sang phải
        if (keyboard.aKey.isPressed) { x -= 1f; }  // A: sang trái
        if (keyboard.wKey.isPressed) { z += 1f; }  // W: tiến
        if (keyboard.sKey.isPressed) { z -= 1f; }  // S: lùi

        // Không nhấn phím nào thì thoát sớm, đỡ tính toán
        if (x == 0f && z == 0f) return;

        // 2. LẤY HƯỚNG PHẢI / TIẾN CỦA ĐẦU, BỎ Y để chỉ di chuyển trên mặt phẳng ngang
        Vector3 right = transform.right;
        Vector3 forward = transform.forward;
        right.y = 0;
        forward.y = 0;

        right.Normalize();
        forward.Normalize();

        // 3. GHÉP THÀNH VECTOR DI CHUYỂN (normalized để đi chéo không nhanh hơn)
        Vector3 direction = (right * x + forward * z).normalized;

        // 4. Dịch vị trí theo tốc độ và thời gian của frame
        Vector3 newPosition = transform.position + direction * moveSpeed * Time.deltaTime;

        // 5. GIỚI HẠN: không cho đi xa quá maxDistance so với vị trí gốc
        Vector3 offset = Vector3.ClampMagnitude(newPosition - originPosition, maxDistance);
        transform.position = originPosition + offset;
    }

    // Đưa đầu về vị trí gốc (gọi từ nút "Reset vị trí" trong SettingsPanel)
    public void ResetPosition()
    {
        transform.position = originPosition;
    }
}