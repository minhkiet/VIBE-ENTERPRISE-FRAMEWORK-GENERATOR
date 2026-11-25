---
name: enterprise-ai-chatbot-integration
description: Tích hợp chatbot và dịch vụ AI xử lý thông tin cho các phân hệ doanh nghiệp gồm CRM, HRM, Sales (Saler) và Hỗ trợ giải quyết sự cố hệ thống qua GPT-AI API (POST /chat/v14 và /chat/v1-v15). Sử dụng khi người dùng muốn xây dựng, kết nối, hoặc mở rộng AI chatbot, trợ lý nội bộ, phân loại ticket, xử lý dữ liệu CRM/HRM/Sales hoặc hỗ trợ kỹ thuật hệ thống.
---

# Enterprise AI Chatbot Integration

## Summary

Hướng dẫn chuẩn để tích hợp chatbot AI và tầng xử lý thông tin thông minh vào 4 phân hệ nghiệp vụ cốt lõi: CRM, HRM, Sales (Saler) và Hỗ trợ giải quyết sự cố hệ thống, sử dụng API gateway `https://gpt-ai-olive.vercel.app/chat/v14` (và các endpoint mở rộng `/chat/v1` đến `/chat/v15`).

## When to Use

- Khi cần tích hợp widget chat AI (HTML/CSS/JS, React, Vue) hoặc service backend vào hệ thống quản trị doanh nghiệp.
- Khi cần xây dựng luồng AI xử lý thông tin cho CRM (khách hàng, ticket, lịch sử chăm sóc), HRM (tuyển dụng, chính sách nhân sự, onboarding), Sales/Saler (tư vấn bán hàng, chấm điểm lead, báo giá), hoặc Hỗ trợ hệ thống (phân tích log lỗi, phân loại sự cố, hướng dẫn khắc phục).
- Khi cần chuẩn hóa cấu trúc gọi API, quản lý context, kiểm soát bảo mật dữ liệu và xử lý lỗi cho chatbot.

## API Contract

- Base URL mặc định: `https://gpt-ai-olive.vercel.app/chat/v14`
- Method: `POST`
- Header bắt buộc: `Content-Type: application/json`
- Single-turn payload (mặc định cho `/chat/v14`):

```json
{"userMessage": "Nội dung câu hỏi hoặc prompt đã ghép ngữ cảnh"}
```

- Multi-turn payload (dùng cho các endpoint hỗ trợ memory và system prompt như `/chat/v1`, `/chat/v6`, `/chat/v12`):

```json
{
  "messages": [
    {"role": "system", "content": "Vai trò và quy tắc nghiệp vụ"},
    {"role": "user", "content": "Yêu cầu người dùng"}
  ]
}
```

- Response chuẩn:

```json
{"reply": "Nội dung phản hồi từ AI"}
```

## Steps

### 1. Xác định phân hệ và chọn chế độ ngữ cảnh

Xác định rõ yêu cầu thuộc nhóm nào trong 4 module để gắn đúng system prompt và dữ liệu đầu vào:

- **CRM**: Tóm tắt lịch sử tương tác khách hàng, phân loại mức độ ưu tiên ticket, trích xuất thông tin liên hệ/nhu cầu, soạn phản hồi CSKH.
- **HRM**: Hỏi đáp chính sách nghỉ phép/lương thưởng/onboarding, sàng lọc mức độ khớp giữa CV và JD, tóm tắt ghi chú phỏng vấn.
- **Sales (Saler)**: Đánh giá lead theo khung BANT (Budget, Authority, Need, Timeline), gợi ý sản phẩm/gói dịch vụ phù hợp, soạn email chào hàng và kịch bản xử lý từ chối.
- **Hỗ trợ hệ thống (System Support)**: Phân loại mức độ sự cố (P1-P4), đọc và chẩn đoán log/mã lỗi, đề xuất checklist khắc phục từng bước, xác định điều kiện chuyển tuyến (escalation) sang kỹ thuật cấp cao.

### 2. Tạo lớp AI Client dùng chung

Viết một module client tập trung (ví dụ `aiService.js`) thay vì gọi `fetch` rải rác trong UI để dễ bảo trì và đổi endpoint:

- Dùng `AbortController` với timeout (khuyến nghị 20-30 giây) để tránh treo giao diện.
- Chuẩn hóa hàm `extractReply(data)` đọc ưu tiên `data.reply`, fallback sang `data.response`, `data.message`, `data.text` hoặc chuỗi raw.
- Hỗ trợ chuyển đổi linh hoạt giữa payload `{"userMessage": "..."}` (gộp system prompt + dữ liệu vào đầu chuỗi khi dùng `/chat/v14`) và `{"messages": [...]}` (khi dùng endpoint có memory).
- Có cơ chế retry có kiểm soát (tối đa 1-2 lần cho lỗi mạng tạm thời) và hiển thị thông báo lỗi rõ ràng trên UI.

### 3. Thiết kế Prompt Template và ép cấu trúc đầu ra

Với các tác vụ xử lý thông tin nghiệp vụ (không chỉ chat tự do), đóng gói prompt theo 3 phần rõ ràng:

1. **Role & Scope**: Giới hạn AI chỉ trả lời trong phạm vi module (CRM/HRM/Sales/System Support) và không bịa số liệu ngoài dữ liệu cung cấp.
2. **Context Injection**: Truyền dữ liệu thực từ hệ thống (JSON khách hàng, mô tả lỗi, bảng giá, quy định HR) đã lược bỏ trường nhạy cảm.
3. **Output Format**: Yêu cầu trả về Markdown ngắn gọn cho chat UI hoặc JSON cố định (ví dụ `{"intent": "...", "priority": "P2", "summary": "...", "nextAction": "..."}`) khi cần lưu ngược lại vào database.

### 4. Tích hợp UI Chatbot và bộ định tuyến (Domain Router)

- Bổ sung bộ chọn ngữ cảnh/module (`CRM`, `HRM`, `Sales`, `System Support`) ngay trên giao diện chat hoặc tự động nhận diện từ trang hiện tại.
- Cung cấp sẵn các nút thao tác nhanh (quick actions) theo từng module: "Tóm tắt ticket", "Chấm điểm lead", "Kiểm tra chính sách HR", "Phân tích log lỗi".
- Luôn hiển thị trạng thái kết nối, thời gian phản hồi (ms), nút Hủy request và nút Copy/Áp dụng kết quả vào form nghiệp vụ.

### 5. Kiểm thử và xác minh

Trước khi bàn giao hoặc đưa vào sử dụng, kiểm tra đủ 4 tiêu chí sau:

- Gửi thử request mẫu `{"userMessage": "test"}` tới `https://gpt-ai-olive.vercel.app/chat/v14` và xác nhận nhận được HTTP `200 OK` cùng trường `reply`.
- Thử ngắt mạng hoặc gửi payload lỗi để xác nhận UI hiện trạng thái lỗi thân thiện và nút Thử lại hoạt động.
- Kiểm tra đầu ra của ít nhất 1 kịch bản cho mỗi module (CRM, HRM, Sales, System Support) đảm bảo đúng định dạng.
- Rà soát payload gửi đi để đảm bảo không rò rỉ dữ liệu mật.

## Gotchas

- **Bảo mật dữ liệu (Quan trọng)**: `gpt-ai-olive.vercel.app` là API công cộng của bên thứ ba. Không bao giờ gửi mật khẩu, API key, token, số CCCD/tài khoản ngân hàng, bảng lương chi tiết hoặc dữ liệu cá nhân nhạy cảm (PII) nguyên gốc lên endpoint này. Luôn ẩn danh (masking) trước khi gọi API hoặc chuyển sang LLM nội bộ/doanh nghiệp cho dữ liệu mật.
- **Khác biệt giữa các endpoint**: `/chat/v14` tối ưu cho single-turn `{"userMessage": "..."}`. Nếu cần ngữ cảnh đa lượt hoặc `messages` array, cần gộp lịch sử ngắn gọn vào `userMessage` hoặc chuyển sang endpoint hỗ trợ memory (như `/chat/v1`, `/chat/v6`, `/chat/v12`).
- **Hallucination về thời gian và dữ liệu nội bộ**: Các model qua gateway có thể không biết ngày giờ hiện tại hoặc chính sách nội bộ mới nhất. Luôn truyền kèm mốc thời gian hiện tại và đoạn dữ liệu tham chiếu trực tiếp trong prompt.
- **Giới hạn độ dài ngữ cảnh**: Cắt tỉa lịch sử hội thoại theo cửa sổ trượt (sliding window, giữ 4-8 lượt gần nhất) để tránh vượt giới hạn token hoặc tăng độ trễ.
