using UnityEngine;

/// <summary>
/// Sinh lưới mặt cầu (mesh) bằng code, dùng làm "màn hình" cho ảnh panorama equirectangular.
/// Mặt tam giác được lật vào trong để camera đặt ở tâm cầu nhìn thấy ảnh.
/// Có thể dựng lại lưới lúc chạy qua Rebuild() (được SettingsPanel gọi khi đổi mức chia lưới).
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SphereMeshGenerator : MonoBehaviour
{
    // Số cột: chia quanh đường xích đạo (kinh độ, longitude)
    public int loSegments = 1024;
    // Số hàng: chia từ cực Bắc xuống cực Nam (vĩ độ, latitude)
    public int laSegments = 512;
    // Bán kính quả cầu (m)
    public float radius = 10f;

    Mesh Build()
    {
        // =============== 1. CẤP PHÁT MẢNG DỮ LIỆU ===============
        // vertices: danh sách các điểm 3D
        // uvs: mỗi điểm tương ứng với vị trí nào trên ảnh 2D (0 -> 1)
        // triangles: danh sách nối điểm nào với điểm nào để thành tam giác

        int cols = loSegments + 1;  // Số đỉnh mỗi hàng (nhiều hơn số ô 1 đỉnh)
        int rows = laSegments + 1;  // Số đỉnh mỗi cột
        Vector3[] vertices = new Vector3[cols * rows];
        Vector2[] uvs = new Vector2[cols * rows];

        // Mỗi 3 phần tử liên tiếp của mảng này là chỉ số 3 đỉnh của một tam giác.
        // Mỗi ô lưới gồm 2 tam giác, mỗi tam giác 3 đỉnh nên nhân 6.
        int[] triangles = new int[loSegments * laSegments * 6];

        // =============== 2. TẠO CÁC ĐỈNH + UV ===============
        // u, v tính theo vị trí tương đối trong lưới (đã đi được bao nhiêu phần quãng đường).
        // Ví dụ: tổng 8 ô, đang ở ô thứ 6 thì 6/8 = 0.75.
        //
        // Kinh độ (phi - φ): góc đi vòng quanh quả cầu, một vòng đầy đủ là 2π.
        // Vĩ độ (theta - θ): góc đi từ cực Bắc xuống cực Nam, chỉ cần nửa vòng là π.
        // Quét phi từ 0 -> 2π và theta từ 0 -> π là đủ phủ kín mặt cầu,
        // quét nhiều hơn sẽ vẽ trùng điểm, tốn công vô ích.
        //
        // Công thức toạ độ (hệ toạ độ cầu -> Descartes):
        //   x = r * sin(θ) * cos(φ)
        //   y = r * cos(θ)
        //   z = r * sin(θ) * sin(φ)

        for (int i = 0; i < rows; i++)
        // i: hàng, đi từ cực Bắc xuống cực Nam
        {
            float v = (float)i / laSegments;  // toạ độ v: 0 -> 1
            float theta = v * Mathf.PI;       // vĩ độ: 0 -> π

            for (int j = 0; j < cols; j++)
            // j: cột, đi vòng quanh theo chiều ngang
            {
                float u = (float)j / loSegments;   // toạ độ u: 0 -> 1
                float phi = u * 2f * Mathf.PI;     // kinh độ: 0 -> 2π

                int index = i * cols + j;  // lưới 2D -> mảng 1D
                vertices[index] = new Vector3(
                    radius * Mathf.Sin(theta) * Mathf.Cos(phi),  // x
                    radius * Mathf.Cos(theta),                   // y
                    radius * Mathf.Sin(theta) * Mathf.Sin(phi)   // z
                );

                // Cần lật cả hai trục:
                //  - 1 - v: v chạy từ cực Bắc xuống, còn v = 0 của texture Unity nằm ở đáy ảnh.
                //  - 1 - u: camera nhìn từ BÊN TRONG cầu nên ảnh bị đảo trái-phải, lật u để bù lại.
                uvs[index] = new Vector2(1f - u, 1f - v);
            }
        }

        // =============== 3. NỐI CÁC ĐIỂM THÀNH TAM GIÁC ===============
        // Mỗi ô lưới (i, j) có 4 góc:
        //   a ---- b      a: trên-trái    b: trên-phải
        //   |      |      c: dưới-trái    d: dưới-phải
        //   c ---- d

        int t = 0;  // Con trỏ vị trí trong mảng triangles
        for (int i = 0; i < laSegments; i++)
        {
            for (int j = 0; j < loSegments; j++)
            {
                int a = i * cols + j;
                int b = a + 1;      // Cùng hàng, cột kế bên
                int c = a + cols;   // Cùng cột, hàng kế dưới (nhảy một hàng = + cols)
                int d = c + 1;

                // Thứ tự đỉnh quyết định mặt nào được vẽ: Unity chỉ vẽ mặt có
                // các đỉnh xếp THEO CHIỀU KIM ĐỒNG HỒ khi nhìn từ camera.
                // Thứ tự gốc (a, b, c) quay ra mặt ngoài cầu; đổi thành (a, c, b)
                // vì để mặt trong hướng về camera đặt ở tâm.

                // Tam giác 1: a, c, b (gốc là a, b, c)
                triangles[t++] = a;
                triangles[t++] = c;
                triangles[t++] = b;
                // Tam giác 2: b, c, d (gốc là b, d, c)
                triangles[t++] = b;
                triangles[t++] = c;
                triangles[t++] = d;
            }
        }

        // =============== 4. ĐÓNG GÓI THÀNH MESH ===============
        Mesh mesh = new Mesh();
        // UInt32 để mesh có thể vượt 65.535 đỉnh (mức lưới lớn như 4096x2048 có ~8,4 triệu đỉnh)
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    void Start()
    {
        GetComponent<MeshFilter>().sharedMesh = Build();
    }

    // Dựng lại lưới với số cột/hàng mới (gọi từ SettingsPanel khi đổi mức chia lưới).
    public void Rebuild(int lo, int la)
    {
        loSegments = lo;
        laSegments = la;

        MeshFilter filter = GetComponent<MeshFilter>();
        // Huỷ mesh cũ để giải phóng bộ nhớ, tránh rò rỉ khi đổi mức liên tục
        if (filter.sharedMesh != null)
        {
            Destroy(filter.sharedMesh);
        }
        filter.sharedMesh = Build();
    }
}