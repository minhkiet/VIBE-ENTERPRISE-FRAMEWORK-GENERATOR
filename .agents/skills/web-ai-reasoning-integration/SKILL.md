---
name: web-ai-reasoning-integration
description: Tích hợp trợ lý AI suy luận bám mục tiêu có tra cứu Internet trực tiếp (Wikipedia VI/EN, Hacker News) và đọc tệp đính kèm (text, code, JSON, CSV, log, ảnh) vào các hệ thống web/doanh nghiệp (React TSX, HTML/JS, CRM, HRM, Sales, ERP, Support) qua GPT-AI API (POST /chat/v14 và /chat/v1-v15). Sử dụng khi người dùng muốn nhúng AI assistant có web grounding, phân tích file đính kèm, hoặc pipeline suy luận đa bước vào ứng dụng hay hệ thống có sẵn.
---

# Web AI Reasoning Integration

## Summary

Quy trình chuẩn để tích hợp trợ lý AI suy luận bám mục tiêu, kết hợp tra cứu dữ liệu Internet trực tiếp và phân tích tệp đính kèm, vào các hệ thống web hoặc phần mềm doanh nghiệp thông qua API `POST https://gpt-ai-olive.vercel.app/chat/v14` (và các endpoint `/chat/v1` đến `/chat/v15`).

## When to Use

- Khi cần nhúng trợ lý AI dạng React (`.tsx`) hoặc widget web vào hệ thống CRM, HRM, Sales, ERP, cổng nội bộ hoặc dashboard quản trị.
- Khi hệ thống cần AI kết hợp tra cứu Internet (web grounding) để giảm bịa đặt thông tin và dẫn nguồn rõ ràng.
- Khi cần người dùng đính kèm tệp (`.log`, `.csv`, `.json`, `.md`, mã nguồn, ảnh) để AI phân tích cùng ngữ cảnh nghiệp vụ.
- Khi cần kiểm soát đầu ra AI theo mục tiêu cụ thể và chế độ suy luận (`fast`, `deep`, `decision`).

## Architecture Pipeline

Mọi yêu cầu đi qua 3 bước tuần tự có thể hủy ngang bằng `AbortController`:

1. **Goal & Domain Framing**: Khóa mục tiêu (`goal`), phân hệ (`general`, `crm`, `hrm`, `sales`, `support`) và chế độ suy luận (`fast`, `deep`, `decision`).
2. **Context Gathering (Web + Files)**:
   - Gọi song song nguồn web công khai hỗ trợ CORS (Wikipedia VI/EN qua `origin=*`, Hacker News Algolia `hn.algolia.com/api/v1/search`) với timeout 6.5 giây để lấy tối đa 6 đoạn trích kèm URL.
   - Đọc tệp đính kèm trên trình duyệt bằng `FileReader`: tệp văn bản/code/JSON/CSV/log đọc text và cắt gọn tối đa `12000` ký tự/tệp; tệp ảnh tạo thumbnail + metadata; tệp nhị phân khác chỉ gửi metadata.
3. **Grounded AI Synthesis**: Ghép ngày hiện tại, vai trò phân hệ, mục tiêu, ngữ cảnh web và nội dung tệp vào payload gửi tới `POST https://gpt-ai-olive.vercel.app/chat/v14` (`{"userMessage": "..."}` cho `/chat/v14` hoặc `{"messages": [...]}` cho endpoint có memory), sau đó chuẩn hóa phản hồi bằng `extractReply()` ưu tiên khóa `reply`.

## Integration Steps

### 1. Nhúng tầng Service độc lập với UI

Tách logic thành 3 hàm thuần dễ tái sử dụng trong mọi framework (React, Vue, Angular, Vanilla JS):

- `searchInternet(query, config, signal)`: Gọi song song nguồn web đã bật, bắt lỗi từng nguồn để 1 nguồn chậm/lỗi không làm hỏng luồng chính.
- `readAttachedFile(file)`: Phân loại `text`, `image`, `binary`, áp giới hạn độ dài văn bản (`MAX_TEXT_CHARS = 12000`) để tránh tràn payload.
- `runGroundedAI({ prompt, goal, domain, mode, sources, files, endpoint, payloadMode, signal })`: Dựng prompt có cấu trúc, gọi `fetch` với timeout 30 giây và parse `data.reply`.

### 2. Kết nối dữ liệu nội bộ của hệ thống đích

Khi nhúng vào ứng dụng có sẵn (CRM/HRM/Sales/ERP/Helpdesk):

- Truyền ngữ cảnh màn hình hiện tại (ví dụ: bản ghi khách hàng, mã ticket, đơn hàng, log hệ thống) vào đầu prompt như một block `[Dữ liệu hệ thống]`.
- Gắn nút kích hoạt nhanh theo ngữ cảnh trang (ví dụ: "Phân tích ticket này", "Đọc log lỗi đính kèm", "Tra cứu đối thủ và gợi ý báo giá").
- Cho phép tắt/bật tra cứu Internet (`webSearchEnabled`) khi người dùng chỉ muốn xử lý dữ liệu nội bộ hoặc tệp đính kèm.

### 3. Xây dựng giao diện minh bạch và kiểm soát lỗi

- Hiển thị thanh tiến trình 3 bước (`1/3 Mục tiêu`, `2/3 Tra cứu Web`, `3/3 Suy luận AI`) kèm nút Hủy (`AbortController`).
- Hiển thị khối nguồn Internet có thể mở/đóng (tên nguồn, tiêu đề, đoạn trích, link mở tab mới) ngay trên câu trả lời của AI.
- Hiển thị chip tệp đính kèm (tên, dung lượng, trạng thái đọc text/metadata, nút gỡ) và hỗ trợ kéo thả (drag-and-drop).
- Bọc `localStorage` trong `try/catch`, không dùng `alert()`/`confirm()`, có nút Thử lại giữ nguyên cả prompt và danh sách tệp khi API lỗi.

### 4. Kiểm thử tích hợp trước khi triển khai

Xác nhận đủ 4 kịch bản sau hoạt động ổn định:

- Gửi câu hỏi bật Internet: kiểm tra nhận được nguồn web, thời gian tra cứu (ms) và phản hồi HTTP `200 OK` chứa `reply`.
- Đính kèm 1 tệp `.json` hoặc `.log` (có thể không nhập chữ): kiểm tra nội dung tệp được đưa vào phân tích và trả về kết luận đúng mục tiêu.
- Bấm Hủy giữa chừng khi đang tra cứu hoặc gọi AI: kiểm tra trạng thái quay về `idle` sạch sẽ, không treo nút gửi.
- Thử nhập sai endpoint hoặc ngắt mạng: kiểm tra hiện thông báo lỗi rõ ràng và nút Thử lại hoạt động.

## Gotchas

- **Bảo mật dữ liệu (Bắt buộc)**: Không gửi mật khẩu, token, API key, dữ liệu khách hàng/nhân sự nhạy cảm (PII) nguyên gốc lên endpoint công cộng `gpt-ai-olive.vercel.app` hoặc vào từ khóa tìm kiếm Internet. Luôn ẩn danh dữ liệu hoặc thay `endpoint` bằng backend LLM nội bộ cho môi trường production.
- **Tệp lớn và tệp nhị phân**: Không đọc toàn bộ tệp hàng chục MB vào chuỗi JSON gửi API. Luôn cắt gọn tệp text và dùng backend parser riêng nếu cần trích xuất nội dung từ PDF, DOCX hoặc XLSX.
- **CORS của nguồn tìm kiếm**: Chỉ gọi trực tiếp từ trình duyệt các API công khai có header CORS hợp lệ (như Wikipedia `origin=*`, HN Algolia). Nếu cần Google/Bing Search hoặc cào trang web bất kỳ, phải đi qua backend proxy của hệ thống.
- **Giới hạn single-turn của `/chat/v14`**: Endpoint `/chat/v14` ưu tiên `{"userMessage": "..."}`. Nếu hệ thống cần hội thoại đa lượt dài, đổi sang chế độ `messages` với endpoint hỗ trợ memory (`/chat/v1`, `/chat/v6`, `/chat/v12`).
