<div align="center">

# 🗺️ Kế hoạch chi tiết

#### Design by Claude

### 🥽 Stereo Panorama Viewer

![Chặng](https://img.shields.io/badge/S%E1%BB%91%20ch%E1%BA%B7ng-7-1E88E5?style=for-the-badge)
![Unity](https://img.shields.io/badge/Unity-URP-000000?style=for-the-badge&logo=unity&logoColor=white)
![Blender](https://img.shields.io/badge/Blender-F5792A?style=for-the-badge&logo=blender&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![Git](https://img.shields.io/badge/Git-F05032?style=for-the-badge&logo=git&logoColor=white)

</div>

---

## 📑 Mục lục

- [🧰 Mỗi công cụ làm gì, code nhiều không?](#-mỗi-công-cụ-làm-gì-code-nhiều-không)
- [📋 Kế hoạch chi tiết](#-kế-hoạch-chi-tiết)
  - [🧱 Chặng 0: Chuẩn bị nền](#-chặng-0-chuẩn-bị-nền)
  - [🎨 Chặng 1: Dữ liệu đầu vào](#-chặng-1-dữ-liệu-đầu-vào)
  - [🌐 Chặng 2: Xem được panorama 360° (sản phẩm tối thiểu)](#-chặng-2-xem-được-panorama-360-sản-phẩm-tối-thiểu)
  - [👀 Chặng 3: Stereo hai mắt](#-chặng-3-stereo-hai-mắt)
  - [✨ Chặng 4: Displacement shader (lõi của đề tài)](#-chặng-4-displacement-shader-lõi-của-đề-tài)
  - [🚀 Chặng 5: Thị sai khi đầu dịch chuyển + hiệu năng](#-chặng-5-thị-sai-khi-đầu-dịch-chuyển--hiệu-năng)
  - [📝 Chặng 6: Báo cáo (làm song song, chốt cuối)](#-chặng-6-báo-cáo-làm-song-song-chốt-cuối)

---

## 🧰 Mỗi công cụ làm gì, code nhiều không?

| Công cụ              | Vai trò                                                                           | Mức độ code                                        |
| :------------------- | :-------------------------------------------------------------------------------- | :------------------------------------------------- |
| 🎨 **Blender**       | Tạo dữ liệu đầu vào: ảnh panorama màu (trái/phải) và ảnh depth                    | Không code                                         |
| 🎮 **Unity**         | Dựng scene, tạo material, làm shader bằng Shader Graph, gán script, build, đo FPS | Không code, đây là phần chiếm nhiều thời gian nhất |
| 💻 **Visual Studio** | Chỉ để viết các script C#                                                         | Nhẹ                                                |
| 🔀 **Git**           | Quản lý phiên bản                                                                 | Chỉ gõ lệnh                                        |

> [!NOTE]
> Code C# ước chừng 4 đến 5 script nhỏ, tổng khoảng 200 đến 300 dòng: sinh lưới cầu, điều khiển đầu, quản lý stereo, UI chỉnh tham số, đo FPS. Phần khó nhất (shader) nằm trong Shader Graph nên không phải viết code.

---

## 📋 Kế hoạch chi tiết

### 🧱 Chặng 0: Chuẩn bị nền

| Công cụ                                 | Việc                                                                                                                                                               | Tác dụng / ghi báo cáo                                                                                                              |
| :-------------------------------------- | :----------------------------------------------------------------------------------------------------------------------------------------------------------------- | :---------------------------------------------------------------------------------------------------------------------------------- |
| 🎮 Unity Hub                            | Tạo project bằng template **3D (URP)**, đặt tên rõ ràng                                                                                                            | URP là pipeline có sẵn Shader Graph và nhẹ hơn, giúp đạt 90+ FPS. Ghi rõ phiên bản Unity trong báo cáo.                             |
| 🎮 Unity > Preferences > External Tools | Chọn **Visual Studio** làm trình soạn script                                                                                                                       | Để mở script từ Unity có gợi ý code. Cần cài workload "Game development with Unity" trong Visual Studio.                            |
| 🔀 Git                                  | `git init` ngay trong thư mục project, thêm `.gitignore` chuẩn cho Unity (bỏ `Library/`, `Temp/`, `Obj/`, `Logs/`, `UserSettings/`, `Builds/`) rồi commit đầu tiên | Thư mục `Library/` rất nặng và tự sinh lại được, không nên đưa vào repo. Chỉ cần commit `Assets/`, `Packages/`, `ProjectSettings/`. |
| 🔀 Git                                  | Đẩy lên GitHub (repo private)                                                                                                                                      | Có bản sao lưu, và là bằng chứng quá trình làm cho báo cáo.                                                                         |
| 📁 Cấu trúc thư mục                     | Trong `Assets/` tạo: `Scripts/`, `Shaders/`, `Materials/`, `Textures/`, `Scenes/`                                                                                  | Tách rõ thành phần, phục vụ tiêu chí kiến trúc phần mềm (40% điểm).                                                                 |

> 📌 **Mốc commit:** `chore: init Unity URP project`

---

### 🎨 Chặng 1: Dữ liệu đầu vào

| Công cụ    | Việc                                                                                                                     | Tác dụng / ghi báo cáo                                                                                                 |
| :--------- | :----------------------------------------------------------------------------------------------------------------------- | :--------------------------------------------------------------------------------------------------------------------- |
| 🎨 Blender | Dựng cảnh đơn giản có vật gần, vật xa rõ rệt (hành lang, phòng có vài đồ vật)                                            | Cảnh có độ sâu khác nhau thì hiệu ứng thị sai mới nhìn rõ khi demo.                                                    |
| 🎨 Blender | Render bằng **Cycles**, camera loại **Panoramic > Equirectangular**, tỉ lệ ảnh 2:1 (ví dụ 4096×2048)                     | Equirectangular là cách trải mặt cầu thành ảnh phẳng, khớp với UV của quả cầu bên Unity.                               |
| 🎨 Blender | Render **cặp trái/phải**: hai camera cách nhau khoảng 6.4 cm (khoảng cách hai mắt, IPD), hoặc dùng chức năng Stereoscopy | Mỗi mắt thấy một góc nhìn hơi khác, đó là nguồn tạo cảm giác 3D (chênh lệch hai mắt).                                  |
| 🎨 Blender | Xuất **depth map** từ Z pass (Compositor), chuẩn hóa về 0–1, lưu ảnh xám                                                 | Đây là dữ liệu độ sâu mà shader sẽ đọc. **Ghi lại quy ước**: sáng là gần hay xa, vì shader phải dùng đúng quy ước này. |
| 📁 Thư mục | Lưu ảnh vào `Assets/Textures/`                                                                                           | Dữ liệu có thể tái lập.                                                                                                |

> 📌 **Mốc commit:** `feat: add panorama and depth inputs` (nếu ảnh quá nặng thì cân nhắc Git LFS hoặc giảm độ phân giải)

---

### 🌐 Chặng 2: Xem được panorama 360° (sản phẩm tối thiểu)

| Công cụ          | Việc                                                                                                                                 | Tác dụng / ghi báo cáo                                                                                   |
| :--------------- | :----------------------------------------------------------------------------------------------------------------------------------- | :------------------------------------------------------------------------------------------------------- |
| 🎮 Unity         | Import ảnh. Với **depth map: bỏ tick sRGB, tắt mipmap**                                                                              | Depth là dữ liệu số, không phải màu. Để sRGB thì giá trị bị bẻ cong và độ sâu lệch. Lỗi này rất hay gặp. |
| 💻 Visual Studio | Viết `SphereMeshGenerator.cs`: sinh lưới cầu với nhiều đỉnh (ví dụ 256×128), UV kiểu equirectangular, **lật mặt tam giác vào trong** | Sphere mặc định của Unity quá ít đỉnh để dịch chuyển mượt, và phải nhìn từ bên trong quả cầu.            |
| 🎮 Unity         | Tạo material unlit dán ảnh panorama lên quả cầu, đặt camera ở tâm                                                                    | Kiểm tra pipeline cơ bản trước khi thêm độ phức tạp.                                                     |
| 💻 Visual Studio | Viết `HeadController.cs`: xoay camera bằng chuột (yaw/pitch)                                                                         | Mô phỏng xoay đầu (3 bậc tự do quay) mà không cần kính.                                                  |

> 📌 **Mốc commit:** `feat: basic equirectangular viewer`. Đến đây đã có bản nộp tối thiểu.

---

### 👀 Chặng 3: Stereo hai mắt

| Công cụ          | Việc                                                                                               | Tác dụng / ghi báo cáo                                               |
| :--------------- | :------------------------------------------------------------------------------------------------- | :------------------------------------------------------------------- |
| 🎮 Unity         | Tạo 2 Layer: `LeftEye`, `RightEye`. Tạo 2 quả cầu, mỗi quả gán một ảnh (trái/phải) và một layer    | Mỗi mắt chỉ được thấy ảnh của mắt đó.                                |
| 🎮 Unity         | Tạo 2 camera, đặt **Culling Mask** tương ứng, **Viewport Rect** chia đôi màn hình (0–0.5 và 0.5–1) | Mô phỏng màn hình kính VR chia hai bên mà không cần phần cứng.       |
| 💻 Visual Studio | Viết `StereoRig.cs` gom hai camera thành một "đầu", có tham số IPD                                 | Một đối tượng điều khiển cả hai mắt, gọn và dễ giải thích kiến trúc. |

> 📌 **Mốc commit:** `feat: side-by-side stereo rendering`

---

### ✨ Chặng 4: Displacement shader (lõi của đề tài)

| Công cụ               | Việc                                                                                                                                                                                                                                          | Tác dụng / ghi báo cáo                                                                                                              |
| :-------------------- | :-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | :---------------------------------------------------------------------------------------------------------------------------------- |
| 🧩 Unity Shader Graph | Tạo Lit/Unlit Shader Graph. Phần **vertex**: đọc depth bằng node **Sample Texture 2D LOD**, nhân với `DepthScale`, đẩy vị trí đỉnh **dọc hướng từ tâm ra ngoài** (dùng chính vị trí đỉnh đã chuẩn hóa, an toàn hơn pháp tuyến khi mặt bị lật) | Biến quả cầu phẳng thành bề mặt có hình khối của cảnh. Đây là cách tạo thị sai chuyển động. **Chụp ảnh toàn bộ graph cho báo cáo.** |
| 🧩 Shader Graph       | Phần **fragment**: đọc ảnh màu nối vào màu hiển thị                                                                                                                                                                                           | Giữ nguyên hình ảnh gốc, chỉ thay đổi hình học.                                                                                     |
| 🧩 Shader Graph       | Khai báo biến `DepthScale` (và thử thêm `MinRadius`/`MaxRadius`)                                                                                                                                                                              | Cho chỉnh được ở Inspector và lúc chạy, phục vụ so sánh bật/tắt.                                                                    |
| 🎮 Unity              | Áp shader cho cả hai quả cầu                                                                                                                                                                                                                  | Cả hai mắt đều có độ sâu nhất quán.                                                                                                 |

> 📌 **Mốc commit:** `feat: depth displacement shader`

---

### 🚀 Chặng 5: Thị sai khi đầu dịch chuyển + hiệu năng

| Công cụ          | Việc                                                                                                      | Tác dụng / ghi báo cáo                                                                                                                                 |
| :--------------- | :-------------------------------------------------------------------------------------------------------- | :----------------------------------------------------------------------------------------------------------------------------------------------------- |
| 💻 Visual Studio | Mở rộng `HeadController`: **WASD để tịnh tiến camera**                                                    | Mô phỏng đầu dịch chuyển (thêm 3 bậc tự do tịnh tiến). Vật gần dịch nhiều, vật xa dịch ít: đây là thứ cần chứng minh trong báo cáo.                    |
| 💻 Visual Studio | Viết `SettingsUI.cs`: thanh trượt `DepthScale`, nút bật/tắt displacement                                  | Demo trực tiếp so sánh trước/sau.                                                                                                                      |
| 💻 Visual Studio | Viết `FpsCounter.cs` hiển thị FPS                                                                         | Đo tiêu chí 90+ FPS (20% của 40% điểm hiệu năng).                                                                                                      |
| 🎮 Unity         | Thử vài mức lưới (ví dụ 64×32, 128×64, 256×128, 512×256), ghi FPS từng mức bằng **Profiler** và **Stats** | Có số liệu chứng minh đánh đổi giữa độ mượt hình khối và hiệu năng. **Đo trên bản Build**, không đo trong Editor vì Editor chậm hơn. Tắt VSync khi đo. |
| 🎮 Unity         | Build ra file chạy thử                                                                                    | Chạy ổn khi nộp, và số FPS thật hơn.                                                                                                                   |

> 📌 **Mốc commit:** `feat: head translation, UI and perf measurements`

---

### 📝 Chặng 6: Báo cáo (làm song song, chốt cuối)

| Việc                                                                                                                                           | Tác dụng / ghi báo cáo                                                                                    |
| :--------------------------------------------------------------------------------------------------------------------------------------------- | :-------------------------------------------------------------------------------------------------------- |
| 📸 **Chụp ảnh liên tục khi làm**: scene Blender, ảnh depth, graph shader, trước/sau khi bật displacement                                       | Không phải quay lại chụp bù. Phần lớn nội dung báo cáo là ảnh của các chặng trên.                         |
| 🧭 Vẽ **UML**: sơ đồ lớp/thành phần gồm `SphereMeshGenerator`, `StereoRig`, `HeadController`, `SettingsUI`, `FpsCounter` và quan hệ giữa chúng | Chứng minh kiến trúc phần mềm rõ ràng (40% điểm).                                                         |
| 📊 Bảng **FPS theo độ chia lưới** và nhận xét                                                                                                  | Số liệu cho tiêu chí hiệu năng.                                                                           |
| 📘 Gắn từng quyết định thiết kế với **phần lý thuyết LaValle** bạn đã đọc (độ sâu, hai mắt, thị sai)                                           | Chứng minh bạn hiểu lý thuyết cảm nhận VR, đây là mục tiêu chính của đề tài (40% điểm cảm nhận/kỹ thuật). |
| ⚠️ Viết mục **Hạn chế**: ví dụ vùng bị hở/kéo giãn khi dịch đầu xa, depth map chỉ có một lớp nên không che khuất đúng                          | Cho thấy bạn hiểu giới hạn của phương pháp, thường được điểm cao.                                         |

---

# Kết thúc file kế hoạch
