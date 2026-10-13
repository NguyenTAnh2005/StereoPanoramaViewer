using UnityEngine;

public class DisplacementControl : MonoBehaviour
{
    // Bán kính khi tắt displacement = bán kính cầu ( radius trong SphereMeshGenerator)
    [SerializeField] private float flatRadius = 10f;

    private Material mat;
    // Các giá trị gốc để active lại
    private float nearOrigin, farOrigin;

    void Awake()
    {
        mat = GetComponent<MeshRenderer>().material;
        nearOrigin = mat.GetFloat("_NearRadius");
        farOrigin = mat.GetFloat("_FarRadius");
    }

    public void SetDisplacement(bool on)
    {
        // Nếu tắt thì cho bằng 1 biến flat --> tắt displacement
        mat.SetFloat("_NearRadius", on ? nearOrigin : flatRadius);
        mat.SetFloat("_FarRadius", on ? farOrigin : flatRadius);
    }

}
