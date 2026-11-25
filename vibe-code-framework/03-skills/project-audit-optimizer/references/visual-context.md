# Visual context

Màu, icon, ảnh và hiệu ứng chỉ "đẹp" khi chúng nói đúng việc sản phẩm đang làm. Bắt đầu từ ngữ cảnh, sau đó mới đánh giá thực thi.

## 1. Trích ngữ cảnh trước khi phán xét

Tìm và ghi lại:

- Sản phẩm/service là gì, đối tượng dùng chính, việc người dùng cần hoàn thành ở màn này.
- Brand/token đang có: palette, typography, radius, spacing, elevation, icon library.
- Quy ước nền tảng: web admin, mobile consumer, dashboard dữ liệu, checkout... kỳ vọng khác nhau.
- Ngôn ngữ/giọng copy và trạng thái cảm xúc mong muốn (tin cậy, khẩn cấp, thân thiện...).
- Ảnh/icon/hiệu ứng hiện có đang mang nghĩa gì; đừng đổi nghĩa khi chỉ định sửa hình thức.

Nếu dự án chưa có token, ghi chỗ nên đặt token mới và xác nhận với người dùng trước khi đổi diện rộng. Không hỏi từng mã màu; hỏi một lần khi lựa chọn thay đổi nhận diện.

## 2. Màu sắc

- Ưu tiên token hiện có. Mã hex mới chỉ xuất hiện khi định nghĩa token mới có lý do.
- Vai trò rõ: background, surface, border, text primary/secondary, brand, success, warning, danger, info. Cùng một vai trò thì cùng một giá trị ở mọi màn.
- Contrast tối thiểu 4.5:1 cho body text, 3:1 cho large text và thành phần UI. Kiểm tra cả hover/focus/disabled/dark mode.
- Màu trạng thái phải có tín hiệu phụ (icon/chữ), không chỉ dùng màu.
- Ý nghĩa màu theo ngữ cảnh: đỏ là lỗi/huỷ trong admin, nhưng có thể là "đang live" trong thiết bị; green không luôn có nghĩa thành công ở mọi thị trường.
- Gradient/shadow dùng để phân tầng hoặc dẫn mắt, không phải mặc định trang trí cho mọi card.

## 3. Icon

- Một hệ icon/stroke width/kích thước cho cùng một nhóm hành động; không trộn emoji, icon svg tự vẽ và icon library khi project đã chọn một bộ.
- Icon đi với nhãn khi hành động có rủi ro hoặc không quen; icon-only phải có accessible name và tooltip.
- Metaphor nhất quán: cùng một icon không được mang hai nghĩa ở hai màn. Icon trang trí thì `aria-hidden`, không cạnh tranh với icon chức năng.
- Hit area đủ lớn, trạng thái hover/active/focus rõ; icon không làm chữ bị cắt hoặc dán vào cạnh.

## 4. Hình ảnh

- Ảnh phải đúng chủ đề sản phẩm và đúng ngữ cảnh màn hình; ảnh stock generic, logo sai hoặc ảnh người không khớp dữ liệu là finding.
- Alt text mô tả chức năng/nội dung; ảnh trang trí thì alt rỗng có chủ đích.
- `srcset`/format hiện đại/kích thước thực tế, lazy load dưới viewport, priority cho LCP; crop đúng focal point.
- Có fallback khi ảnh lỗi, khung giữ chỗ đúng kích thước để không CLS.
- Với mockup: dữ liệu giả phải phủ ca biên (tên dài, ảnh thiếu, số lớn), và phải báo rằng ảnh/số là giả.

## 5. Hiệu ứng

- Hiệu ứng trả lời một hành động hoặc giải thích thay đổi trạng thái; không chạy vô hạn ở UI thường trực.
- Ưu tiên transform/opacity, thời gian ngắn (~150–300ms cho vi tương tác), có easing nhất quán.
- Một điểm nhấn chuyển động cho màn là đủ; fade-slide từng section, hover mọi card là dấu hiệu trang trí tự động, không phải thiết kế.
- `prefers-reduced-motion` phải được tôn trọng; hiệu ứng không che chữ, không trì hoãn thao tác, không làm khó đọc nội dung cập nhật.
- Motion của modal/drawer/dropdown phải giữ focus và không làm nội dung phía dưới bị mất khi đóng.

## 6. Tự critique trước khi báo cáo

Hỏi lần lượt: thứ bậc có rõ trong 3 giây không? Khoảng cách có nhịp không? Có đúng phong cách dự án không? Có phần tử nào đang cố gây ấn tượng nhưng không mang thông tin không? Nếu bỏ một phần tử trang trí đi, màn có kém đi không? Nếu không, bỏ nó.

Không đưa finding kiểu "nên dùng màu X cho hiện đại hơn" khi chưa chỉ ra token/đối tượng/ngữ cảnh. Mỗi đề xuất thị giác phải kèm `file:line` của token/component liên quan và ảnh before/after cùng viewport.
