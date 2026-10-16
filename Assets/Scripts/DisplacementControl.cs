using UnityEngine;

/// <summary>
/// Bật/tắt hiệu ứng displacement (đẩy đỉnh theo depth) của một quả cầu panorama.
/// Cách tắt: đặt NearRadius và FarRadius của shader bằng đúng bán kính cầu,
/// khi đó mọi đỉnh nằm trên mặt cầu phẳng ban đầu. SettingsPanel gọi SetDisplacement().
/// </summary>
public class DisplacementControl : MonoBehaviour
{
    // Bán kính khi tắt displacement = bán kính cầu (radius trong SphereMeshGenerator)
    private float flatRadius;

    private Material mat;
    // Giá trị gốc của shader, lưu lại để bật displacement trở lại
    private float nearOrigin, farOrigin;

    void Awake()
    {
        flatRadius = GetComponent<SphereMeshGenerator>().radius;
        // Dùng .material (không phải .sharedMaterial) để mỗi quả cầu có bản sao riêng,
        // chỉnh lúc chạy không làm thay đổi asset material gốc.
        mat = GetComponent<MeshRenderer>().material;
        nearOrigin = mat.GetFloat("_NearRadius");
        farOrigin = mat.GetFloat("_FarRadius");
    }

    public void SetDisplacement(bool on)
    {
        // Tắt: đặt cả Near và Far bằng flatRadius -> không còn chênh lệch theo depth
        mat.SetFloat("_NearRadius", on ? nearOrigin : flatRadius);
        mat.SetFloat("_FarRadius", on ? farOrigin : flatRadius);
    }
}