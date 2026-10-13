## Bức tranh chung của head mover

`Update()` chạy **mỗi frame** (khoảng 60 lần/giây). Mỗi lần chạy, script trả lời 3 câu hỏi theo thứ tự:

1. Người dùng đang bấm phím nào? (bước 1)
2. Phím đó nghĩa là đi về hướng nào trong thế giới 3D? (bước 2 và 3)
3. Đi được bao xa trong 1 frame, và có vượt giới hạn không? (bước 4 và 5)

## Phần khai báo và `Start`

```csharp
[SerializeField] private float moveSpeed = 0.2f;
[SerializeField] private float maxDistance = 0.3f;
private Vector3 originPosition;
```

- `private` giữ biến kín trong class. `[SerializeField]` bảo Unity vẫn hiện biến đó trong Inspector để bạn chỉnh số khi đang Play mà không phải sửa code.
- `originPosition` không có `[SerializeField]` vì nó tự tính ở `Start`.

```csharp
void Start() { originPosition = transform.position; }
```

`transform.position` đổi liên tục khi bạn đi. Nếu không lưu lại điểm xuất phát, bước 5 sẽ không biết "xa tâm" là xa so với đâu. `Vector3` là kiểu **giá trị** nên dòng này **sao chép** vị trí tại thời điểm đó. Về sau `transform.position` đổi thì `originPosition` vẫn đứng yên.

## Bước 1: đọc phím thành hai số

```csharp
float x = 0f; float z = 0f;
if (D) x += 1;  if (A) x -= 1;
if (W) z += 1;  if (S) z -= 1;
```

Kết quả: `x` và `z` chỉ có thể là -1, 0 hoặc 1.

| Bấm            | x   | z                 |
| -------------- | --- | ----------------- |
| W              | 0   | 1                 |
| W + D          | 1   | 1                 |
| A + D cùng lúc | 0   | (triệt tiêu nhau) |

Dùng `+=` và `-=` thay vì gán `=` để hai phím ngược nhau triệt tiêu thay vì phím sau ghi đè phím trước. Dòng `if (x == 0f && z == 0f) return;` thoát sớm cho đỡ tính toán khi không bấm gì.

## Bước 2: lấy hướng phải/tiến rồi "ép phẳng"

`transform.right` và `transform.forward` là hai **mũi tên độ dài 1 trong thế giới**, chỉ về phía "phải" và "trước mặt" của `Head`. Chúng đổi theo góc xoay mà `HeadController` đang đặt.

Vấn đề: giả sử bạn ngước lên 45°. Khi đó `forward ≈ (0, 0.7, 0.7)`, có thành phần y. Nếu cứ dùng nguyên, bấm W sẽ làm đầu **bay lên**. Mình muốn đi ngang, nên:

```csharp
forward.y = 0;   // → (0, 0, 0.7)
forward.Normalize();   // → (0, 0, 1)
```

- `y = 0` bỏ thành phần lên/xuống.
- `Normalize()` kéo mũi tên về lại độ dài 1. Sau khi bỏ y, vector bị ngắn đi (0.7 thay vì 1), không chuẩn hoá thì ngước cao sẽ đi chậm hơn.

`right` hầu như không có y vì không xoay roll, nhưng làm cho cùng kiểu cho chắc.

## Bước 3: ghép thành hướng

```csharp
Vector3 direction = (right * x + forward * z).normalized;
```

Ví dụ nhìn về hướng +Z, bấm W + D: `right = (1,0,0)`, `forward = (0,0,1)`, `x = 1`, `z = 1`.

- `right*x + forward*z = (1, 0, 1)`, có **độ dài ≈ 1.41**.
- `.normalized` đưa về `(0.707, 0, 0.707)`, độ dài 1.

Không chuẩn hoá thì đi chéo nhanh hơn đi thẳng khoảng 41%. Đây là lỗi game phổ biến.

## Bước 4: từ hướng thành quãng đường

```csharp
direction * moveSpeed * Time.deltaTime
```

Về đơn vị: `moveSpeed` là m/s, `deltaTime` là số giây của frame vừa rồi, nhân lại ra **mét đi được trong frame đó**.

Ví dụ với 60 FPS: `deltaTime ≈ 0.0167 s`, mỗi frame đi `0.2 × 0.0167 ≈ 0.0033 m`. Sau 60 frame (1 giây) vừa đủ 0.2 m. Với 120 FPS, mỗi frame đi nửa quãng nhưng có gấp đôi số frame, nên 1 giây vẫn là 0.2 m. Đó là lý do cần `deltaTime`: tốc độ không phụ thuộc máy mạnh hay yếu. Điều này cũng liên quan trực tiếp đến phép đo FPS ở phần sau của chặng 5.

Lưu ý `newPosition` mới chỉ là vị trí **dự định**, chưa gán cho `transform`.

## Bước 5: giới hạn khoảng cách

```csharp
Vector3 offset = Vector3.ClampMagnitude(newPosition - originPosition, maxDistance);
transform.position = originPosition + offset;
```

- `newPosition - originPosition` là vector **độ lệch** từ tâm đến vị trí dự định.
- `ClampMagnitude(v, max)` nghĩa là: nếu `v` dài ≤ max thì giữ nguyên, nếu dài hơn thì **giữ nguyên hướng nhưng cắt độ dài về max**.
- Cộng lại với tâm để ra vị trí thật.

Ví dụ: `offset = (0.25, 0, 0.2)` có độ dài ≈ 0.32 > 0.3. Nó bị co về cùng hướng, độ dài 0.3, tức khoảng `(0.234, 0, 0.187)`. Đầu bị "chặn" ở biên nhưng vẫn **trượt dọc biên** được thay vì đứng khựng.
