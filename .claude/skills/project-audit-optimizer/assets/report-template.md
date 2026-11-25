# Project Audit Report

> Sao chép khung này vào báo cáo. Xoá mục không dùng; không giữ placeholder trong bản giao.

## 1. Phạm vi và điều kiện

- Chế độ: `review` / `fix` / `verify`
- Root/phạm vi:
- Branch/commit:
- Mục tiêu người dùng:
- Lệnh build/test/lint:
- Lệnh chạy app/URL:
- Giả định đã chọn:
- Hạn chế (không có browser/device, không có data thật, ...):

## 2. Coverage

| Hạng mục | Số lượng | Ghi chú |
|---|---|---|
| Thư mục trong phạm vi | | |
| File `read` | | |
| File `sampled` | | kèm pattern |
| File `excluded` | | generated/vendor |
| File `not-read` / `unverified` | | kèm lý do |

## 3. Baseline

| Tín hiệu | Trước | Sau | Điều kiện đo |
|---|---|---|---|
| Build/test | | | lệnh + kết quả |
| LCP / INP / CLS | | | viewport + network |
| Console error | | | route + thao tác |
| Kích thước ảnh/bundle | | | file/route |

Ghi `unverified` nếu không đo được. Không suy số từ code.

## 4. Findings

| ID | Mức | Trục | Evidence | Quan sát | Tác động | Đề xuất | Trạng thái |
|---|---|---|---|---|---|---|---|
| A-01 | P0/P1/P2/P3 | code/UI/responsive/perf/stability/visual | `file:line`, ảnh, lệnh | | | | open/fixed/not-fixed |

- `P0`: chặn luồng chính, mất dữ liệu, lỗi bảo mật nghiêm trọng.
- `P1`: suy giảm rõ UX/hiệu năng/ổn định ở luồng quan trọng.
- `P2`: lỗi hiển thị hoặc thị giác có ảnh hưởng rõ nhưng có workaround.
- `P3`: tinh chỉnh, rủi ro thấp.

Tách biệt ba loại câu: **quan sát** (có evidence), **suy luận** (vì sao quan trọng), **đề xuất** (nên làm gì). Không gộp suy luận vào evidence.

## 5. UI/UX và responsive proof

| Route/trạng thái | Viewport | Kết quả | Ảnh before/after | Lỗi bị che/clip |
|---|---|---|---|---|
| | | | | |

## 6. Kế hoạch sửa theo ưu tiên

| Thứ tự | Finding | Việc cần làm | File dự kiến | Cách verify | Rủi ro |
|---|---|---|---|---|---|
| | | | | | |

## 7. Sau khi sửa

- Lệnh đã chạy lại:
- Metric khác biệt:
- Finding chuyển trạng thái:
- Hạng mục vẫn `unverified`:
