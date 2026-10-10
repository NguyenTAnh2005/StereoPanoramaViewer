using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SphereMeshGenerator:MonoBehaviour
{
    // Số phần chia theo chiều dọc -> vĩ độ [longitude]
    public int loSegments = 512;
    // Số phần chia theo chiều ngang -> kinh độ [latitude]
    public int laSegments = 256;
    // Bán kính
    public float radius = 10f;

    Mesh Build()
    {
        // =============== 1. CẤP PHÁT MẢNG DỮ LIỆU LƯU TRŨ ==========
        // Tạo danh sách lưu trữ dữ liệu 
        // vertices: Danh sách các điểm 3D 
        // uv: mỗi điểm tương ứng với tọa độ vị trí nào trên ảnh 2D (0 -> 1)
        // triangles: danh sách nối điểm nào với điểm nào để thành tam giác

        int cols = loSegments + 1;  // Số đỉnh mỗi hàng ( nhiều hơn số ô 1 đỉnh)
        int rows = laSegments + 1;  // Số đỉnh mỗi cột 
        Vector3[] vertices = new Vector3[cols * rows];
        Vector2[] uvs = new Vector2[cols * rows];

        // Lưu trữ số thứ tự của các đỉnh vertices, cứ 3 phần tử liên tiếp thì tạo 1 tam giác,
        // mỗi ô thì được cấu từ 2 tam giác, mỗi tam giác có 3 điểm nên ta x6
        int[] triangles = new int[loSegments * laSegments * 6];

        // =============== 2. TẠO CÁC ĐỈNH + UV ==========
        // uv tính dựa trên vị trí tương đổi hiện tại trong lưới
        // tức là đỉnh đã đi được bao nhiêu phần trăm quãng đường.  
        // Ví dụ: có tổng 8 ô (ngang hay dọc), đang ở ô thứ 6 -> 6/8 = 0.75

        // Kinh độ (phi) là góc đi vòng quanh quả cầu, như đi một vòng quanh đường xích đạo. Một vòng đầy đủ là 360° = 2π.
        // Vĩ độ (theta) là góc đi từ cực Bắc xuống cực Nam.Đi dọc một đường kinh tuyến từ cực này sang cực kia chỉ là nửa vòng tròn, tức 180° = π.
        // Ở bên dưới, khi quét vẽ điểm thì chỉ cần 0 -> π cho vĩ độ và 
        // 0 -> 2π cho kinh độ là đã đủ để vẽ hẳn full mặt cầu, nếu nhiều hơn
        // sẽ có hiện tượng vẽ trùng điểm, tốn công.
        // Công thức tính tọa độ
        // x = r * sin theta * cos phi
        // y = r * cos theta
        // z = r * sin theta * sin phi

        for (int i = 0; i< rows; i++)
            // i: hàng, đi từ bắc xuống nam
        {
            float v = (float)i / laSegments; // tọa độ v: 0 -> 1
            float theta = v * Mathf.PI; // Vĩ độ : 0 -> π

            for (int j = 0; j < cols; j++)
            // j: cột, đi xuay ngang
            {
                float u = (float)j / loSegments; // tọa độ u: 0 -> 1
                float phi = u * 2f * Mathf.PI; // Kinh độ: 0 -> 2π

                int index = i * cols + j;  // lưới 2D -> mảng 1D
                vertices[index] = new Vector3(
                    radius * Mathf.Sin(theta) * Mathf.Cos(phi), // x
                    radius * Mathf.Cos(theta),                  // y
                    radius * Mathf.Sin(theta) * Mathf.Sin(phi)  // z
                );
                // uv đã tính sẵn theo vị trí cột hay hàng tuy nhiên 
                // Vì dán ảnh vào mặt trong nên u và v  bị đảo ngược nên ta cần 1f - u/v 
                uvs[index] = new Vector2(
                    1f-u, 1f - v
                );

            }
        }

        // =============== 3. NỐI CÁC ĐIỂM THÀNH TAM GIÁC ==========
        // Mỗi ô lưới (i, j) có 4 góc:
        //   a ---- b      a: trên-trái    b: trên-phải
        //   |      |      c: dưới-trái    d: dưới-phải
        //   c ---- d

        int t = 0; // Biến trỏ vị trí bên trong mảng triangles
        for (int i = 0; i < laSegments; i++)
        {
            for (int j = 0; j<loSegments; j++)
            {
                int a = i * cols + j;
                int b = a + 1;     // Cùng hàng, cột kế bên
                int c = a + cols; // Cùng cột, hàng kế dưới ( nhảy một hàng = + 1 cols)
                int d = c + 1;

                // Tam giác 1: a, b, c
                // Tam giác 1 sau: a, c, b => ảnh lật ngược vào trong => cam nhìn thấy 
                triangles[t++] = a;
                triangles[t++] = c;
                triangles[t++] = b;
                // Tam giác 2: b, d, c
                // Tam giác 2 sau: b, c, d => tương tự như trên
                triangles[t++] = b;
                triangles[t++] = c;
                triangles[t++] = d;
            }
        }

        // =============== 4. ĐÓNG GÓI THÀNH MESH ==========
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;

    }

    void Start()
    {
        GetComponent<MeshFilter>().mesh = Build();    
    }
}
