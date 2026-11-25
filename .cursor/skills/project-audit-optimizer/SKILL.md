---
name: project-audit-optimizer
description: "Rà soát codebase theo phạm vi người dùng yêu cầu rồi tối ưu UI/UX, responsive/chống che khuất, hiệu năng, ổn định và thị giác (màu, icon, ảnh, hiệu ứng) bằng bằng chứng đo được. Dùng khi đề yêu cầu đọc/soi toàn bộ dự án, kiểm tra mọi file/thư mục, audit giao diện, sửa giao diện bị che, tối ưu tốc độ hoặc làm giao diện đẹp hơn đúng ngữ cảnh. Không dùng cho sửa một lỗi đơn lẻ, viết feature mới hoặc review bảo mật chuyên sâu khi chưa có yêu cầu audit tổng hợp."
---

# Project Audit Optimizer

Skill này trả lời ba câu: dự án đang ra sao, chỗ nào cần sửa, và bằng chứng ở đâu. Nó là lớp điều phối audit; không thay thế skill chuyên sâu và không tự biến một đánh giá thành quyền sửa code.

## Chế độ

| Chế độ | Khi nào | Hành vi |
|---|---|---|
| `review` | Mặc định. Đề nói "rà soát", "kiểm tra", "xem giúp", "đọc dự án" | Chỉ đọc, đo, chụp và báo cáo. Không sửa file. |
| `fix` | Đề nói rõ "sửa", "tối ưu", "làm cho ổn định", hoặc duyệt findings từ `review` | Chỉ sửa finding đã chấp nhận hoặc low-risk nằm đúng phạm vi. Mỗi diff phải truy về một finding ID. |
| `verify` | Sau khi sửa, hoặc đề yêu cầu "kiểm tra lại" | Chạy lại đúng baseline đã dùng, so sánh trước/sau, cập nhật trạng thái finding. |

Nếu đề vừa audit vừa sửa, chạy `review` trước, trình bày findings, rồi mới vào `fix`. Không hỏi lại khi phạm vi đã rõ; khi thiếu thông tin thì ghi rõ giả định trong báo cáo.

## Bất biến

1. **Coverage trước kết luận.** Không nói "đã rà toàn bộ dự án" khi chưa có ledger liệt kê mọi thư mục và phân loại mọi file. File không đọc phải được ghi là `not-read` kèm lý do.
2. **Bằng chứng trước khuyến nghị.** Mỗi finding có `file:line` hoặc lệnh/chỉ số/ảnh chụp tương ứng. Không viết "có thể bị che" khi chưa mở trang.
3. **Đo trước khi tối ưu.** Mọi nhận định về tốc độ, ổn định hay bộ nhớ cần baseline và measurement sau thay đổi. Nếu không đo được, ghi `unverified`, không đoán số.
4. **Ngữ cảnh áp đảo template.** Dùng token, component, icon set, ngôn ngữ copy và phong cách đang có của dự án. Không áp palette/layout mẫu chỉ vì skill gợi ý.
5. **Scope và an toàn.** `git status` trước khi đọc. Không ghi đè file đang có thay đổi của người dùng khi chưa được yêu cầu; không xoá, cài dependency, deploy hay đụng dịch vụ ngoài phạm vi.
6. **Trung thực về giới hạn.** Thiếu browser/device, thiếu quyền chạy build, hoặc dữ liệu thật không sẵn thì ghi rõ phần nào chưa kiểm chứng được.

## Luồng

1. **Chốt phạm vi + baseline.** Xác định root, mục tiêu, chế độ và các lệnh build/test/lint hiện có. Ghi `git status`, commit/branch, lệnh chạy app, entrypoint. Đọc `AGENTS.md`/hướng dẫn dự án trước khi đọc code.
2. **Lập ledger phủ.** Đọc [references/coverage.md](references/coverage.md) khi phạm vi là toàn dự án hoặc nhiều thư mục. Phân loại file critical/support/generated, đánh dấu đã đọc hay không, rồi mới chọn file để đọc sâu theo risk.
3. **Đọc theo lớp.** entrypoint → routing/screen → component/state → style/token → asset → API/data → test → config/CI. Với mỗi luồng người dùng, lần theo dữ liệu từ input đến render và ngược lại; ghi chỗ thiếu loading/empty/error, state không đồng bộ, import chết, contract không khớp.
4. **Kiểm chứng runtime.** Đọc [references/runtime-checks.md](references/runtime-checks.md) khi đề liên quan UI, màn hình thiết bị, thành phần bị che, tốc độ hoặc ổn định. Chụp các viewport đại diện, đi luồng chính, xem console/network. Nếu không có công cụ browser/device, ghi `unverified` thay vì kết luận bằng tưởng tượng.
5. **Soi thị giác theo ngữ cảnh.** Đọc [references/visual-context.md](references/visual-context.md) khi đề nhắc màu sắc, icon, hình ảnh, hiệu ứng, "đẹp", "đúng ngữ cảnh". Đối chiếu với sản phẩm, đối tượng dùng và token hiện có trước khi đề xuất thay đổi.
6. **Xếp hạng và báo cáo.** Dùng [assets/report-template.md](assets/report-template.md). Mỗi finding có ID, mức P0–P3, confidence, evidence, tác động và cách verify. Tách rời "quan sát", "suy luận" và "đề xuất".
7. **Sửa + verify, nếu được phép.** Sửa nhỏ nhất có thể, giữ formatting hiện có. Chạy lại đúng lệnh baseline, so sánh metric/ảnh, cập nhật findings thành `fixed` hoặc `not-fixed`. Dừng trước thay đổi kiến trúc lớn, cài dependency hoặc tác động ngoài dự án khi chưa có duyệt.

## Điều kiện thoát

Trước khi kết luận phải có đủ:

- [ ] Ledger ghi rõ phạm vi, số file đã đọc, số file excluded và số file `not-read`.
- [ ] Mọi finding có evidence cụ thể; không có khuyến nghị đứng riêng không có finding.
- [ ] Ít nhất một viewport mobile và một desktop đã kiểm chứng, hoặc ghi rõ lý do không kiểm được.
- [ ] Nhận định hiệu năng/ổn định có baseline + measurement, hoặc ghi `unverified`.
- [ ] Không có file dirty của người dùng bị ghi đè; mọi diff (nếu có) nằm trong phạm vi đã duyệt.
- [ ] Báo cáo nêu rõ giả định, hạng mục chưa verify và cách chạy lại.

## Tích hợp skill có sẵn

Nếu môi trường có các skill chuyên biệt, dùng chúng như bộ kiểm tra con và giữ kết quả trong ledger: `repo-scan` cho inventory, `ui-ux`/`ui_frontend-review` cho review giao diện, `accessibility` cho A11y, `browser-testing-with-devtools` cho kiểm chứng runtime, `perf_perf-optimization` cho measurement, `systematic-debugging` cho lỗi có reproduction, `security-review` khi có dấu hiệu lộ dữ liệu hoặc lỗi auth. Không sao chép luật chi tiết của chúng vào skill này.
