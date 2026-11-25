# Runtime checks

Code đọc được chưa chứng minh giao diện hiển thị đúng. Phần này chuyển mỗi nghi ngờ thành một quan sát có thể lặp lại: lệnh chạy, viewport, thao tác, ảnh chụp, console/network.

## Baseline trước đã

Ghi lại cùng lúc:

- Lệnh build/lint/test và kết quả thô (pass/fail, thời gian nếu có).
- URL/route, trạng thái dữ liệu đang dùng (mock/real), tài khoản hoặc điều kiện tiên quyết.
- Console error/warning, failed request, HTTP status bất thường.
- Metric frontend nếu đo được: LCP, INP, CLS, TTFB, bundle size, long task.
- Ảnh chụp màn hình ở viewport đại diện trước khi sửa.

Không so sánh metric giữa máy khác nhau, network khác nhau hoặc dữ liệu khác nhau mà không nói rõ điều kiện.

## Ma trận viewport tối thiểu

| Nhóm | Kích thước | Mục tiêu |
|---|---|---|
| Mobile nhỏ | 320×568 | Không tràn ngang, nút chính vẫn thấy. |
| Mobile phổ biến | 375×812 | Luồng chính đúng, bottom nav/FAB không che nội dung. |
| Tablet | 768×1024 | Chuyển bố cục đúng, bảng không ép. |
| Desktop | 1440×900 | Information hierarchy rõ, không khoảng trống vô nghĩa. |
| Xoay ngang / zoom | 812×375, zoom 200% | Nội dung không bị cắt khi chiều cao nhỏ. |

Thêm 200% text scale, bàn phím ảo, dark mode và RTL nếu dự án hỗ trợ. Với app điện thoại, thêm safe area và device có notch. Nếu dùng thiết bị thật/simulator, điều khiển bằng device tool của môi trường; nếu dùng browser, ghi URL và viewport đã mở.

## Kiểm tra bị che/clip — phải đi từng mục

- Header/footer `fixed`, bottom nav, tab bar, FAB, snackbar/toast, modal, dropdown, tooltip, popover: có đè lên input đang focus, nút xác nhận hoặc dòng cuối danh sách không?
- `z-index` và stacking context: phần tử nổi có nằm trên đúng lớp, hay bị backdrop/overflow cắt mất?
- `overflow: hidden/auto` trên parent: nội dung dài, menu mở rộng, focus ring có bị cắt không?
- `100vh` so với `100dvh`, `env(safe-area-inset-*)`: nội dung có nằm dưới tai thỏ, thanh địa chỉ trình duyệt hoặc thanh hệ điều hành không?
- Scroll nested: cuộn trong bảng/drawer có giật, chặn cuộn trang hoặc mất vị trí khi mở modal không?
- Khi bàn phím mở: input, label lỗi và nút hành động có còn trong viewport không?
- Văn bản dài/đa ngôn ngữ/số lớn: ellipsis có làm mất nghĩa, badge có tràn, bảng có cần horizontal scroll có chủ đích không?
- Tap target tối thiểu ~44–48 px; khoảng cách giữa hai nút dễ nhầm có đủ không?

Với mỗi lỗi, chụp ảnh trước/sau cùng viewport cùng thao tác. Ghi rõ "bị che bởi X trong trạng thái Y", không chỉ "responsive chưa tốt".

## Luồng tương tác

Chọn 1–3 luồng quan trọng nhất (đăng nhập, tạo/sửa bản ghi, thanh toán, xem danh sách...). Với mỗi luồng kiểm tra:

- Trạng thái ban đầu, loading, empty, error, success và retry.
- Điều hướng bằng bàn phím: focus thấy rõ, thứ tự đúng, modal trap/release đúng.
- Nhãn cho screen reader, alt ảnh, thông báo lỗi có ý nghĩa.
- Hành động lặp: double click, submit hai lần, quay lại trang trước, mạng chậm/mất kết nối.
- Console sạch sau thao tác; failed request có xử lý không.

## Hiệu năng và ổn định

- LCP/INP/CLS so với baseline; image LCP có preload/priority, có `width/height` hoặc aspect-ratio.
- Animation dùng `transform`/`opacity`; hiệu ứng nằm ở lớp nổi, không gây layout thrashing.
- Đ asset nặng: lazy load dưới viewport, code split route/component nặng, font `font-display: swap`.
- Listener, timer, subscription, request cũ có cleanup/cancel; retry có backoff và giới hạn; timeout có mặt.
- Error boundary hoặc fallback ở ranh giới component/route; lỗi không nuốt im lặng, không log dữ liệu nhạy cảm.
- Memory/CPU sau vài lượt thao tác lặp: có tăng không? Nếu không đo được thì ghi `unverified`.

Mọi con số đưa vào báo cáo phải kèm điều kiện đo. Nếu thay đổi làm metric xấu đi, finding chuyển thành `not-fixed` dù code trông "sạch" hơn.
