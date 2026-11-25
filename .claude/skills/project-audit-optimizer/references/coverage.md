# Coverage ledger

Mục tiêu: biết chính xác mình đã đọc gì, chưa đọc gì, và vì sao. "Rà soát toàn bộ" là tuyên bố có thể kiểm chứng được, không phải cảm giác đã lướt qua vài file.

## 1. Đóng băng hiện trạng

Trước khi đọc sâu:

```bash
git status --short
git branch --show-current
rg --files -g '!.git' -g '!node_modules' -g '!dist' -g '!build' -g '!coverage' -g '!out' .
```

Điều chỉnh danh sách exclude theo stack. Đánh dấu mọi file `M`, `A`, `D` hoặc untracked liên quan đến phạm vi: đọc để hiểu ý người dùng, nhưng không ghi đè khi chưa có yêu cầu sửa.

## 2. Phân loại

| Nhóm | Định nghĩa | Cách xử lý |
|---|---|---|
| `critical` | entrypoint, route/screen, component/state, style/token, API/data, auth, config build | Đọc toàn bộ, lần theo luồng dữ liệu. |
| `support` | test, i18n, docs, script, fixture, asset nguồn | Đọc theo rủi ro và theo finding phát hiện được. |
| `generated` | vendor, lockfile, dist, snapshot tự sinh, binary, minified output | Chỉ inventory; mở khi có bằng chứng lỗi nằm ở đó. |

Không tự động coi lockfile hay snapshot là "không quan trọng" nếu đề liên quan dependency, supply chain hoặc regression output.

## 3. Ledger tối thiểu

Ghi bảng này trong báo cáo, không nhất thiết in toàn bộ trong response:

| Path | Nhóm | Trạng thái | Evidence/đã xem | Risk | Ghi chú |
|---|---|---|---|---|---|
| `src/app/` | critical | read | entrypoint, route... | high | ... |
| `src/legacy/` | support | not-read | ngoài phạm vi user | low | ... |

Quy ước trạng thái: `read`, `sampled`, `not-read`, `excluded`, `unverified`. Với thư mục lớn, ghi `sampled` kèm số file/tổng số và pattern đã chọn; không đổi thành `read`.

## 4. Thứ tự đọc ưu tiên rủi ro

1. `AGENTS.md`, README, package manifest, TS/alias config: hiểu stack và lệnh chạy.
2. Entrypoint → router → layout → screen chính → component con.
3. Style/token/theme → responsive rules → icon/image import.
4. State store, API client, query/mutation, error handling.
5. Test theo luồng chính, i18n, config env, CI.
6. Asset và generated chỉ khi Finding dẫn tới đó.

Khi đọc component, luôn hỏi: props/type từ đâu, state thay đổi ở đâu, render nhánh nào khi loading/empty/error, và component này có bản trùng ở chỗ khác không.

## 5. Cross-file checks đáng giá

- Import/export và alias có resolve không; component trùng chức năng ở hai thư mục.
- Token màu/spacing/font được định nghĩa ở một chỗ hay hard-code rải rác.
- Khoá i18n giữa code và file ngôn ngữ có khớp; ngôn ngữ copy có thống nhất không.
- Route param/query/state có đồng bộ với API contract và deep link không.
- Env var có fallback/validation; secret không nằm trong source.
- Error boundary, timeout, cancellation và retry có phủ các async path chính không.

## 6. Dừng hợp lệ

Chỉ xác nhận "đã rà toàn bộ dự án" khi mọi thư mục trong phạm vi có mặt trong ledger, mọi file `critical` ở trạng thái `read`, và phần `not-read`/`excluded` được nêu rõ trong báo cáo. Nếu không đạt, dùng cụm "đã rà phạm vi X, coverage Y/Z file" và liệt kê phần còn lại.
