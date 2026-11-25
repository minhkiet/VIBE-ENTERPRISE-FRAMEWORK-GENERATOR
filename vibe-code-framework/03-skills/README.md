# Skills Index

> **Version:** 1.1.0 | **Updated:** 2026-11-25

---

## Tổng quan

`03-skills/` chứa các skills dùng chung cho mọi Vibe Coding tool. Mỗi skill được đóng gói trong thư mục riêng với file `SKILL.md` theo chuẩn.

## Cấu trúc Skill

```
skill-name/
├── SKILL.md              # Bắt buộc - Mô tả skill
├── README.md             # Tùy chọn - Hướng dẫn chi tiết
├── references/           # Tài liệu tham khảo
│   ├── architecture.md
│   ├── examples.md
│   └── checklist.md
└── templates/           # Mẫu code
    ├── template-1.ts
    └── template-2.ts
```

## Các skills trong bộ này

| Skill | Mô tả |
|-------|--------|
| [spec-driven-development](./spec-driven-development/) | Master SDD workflow orchestrator (GitHub Spec Kit compatible) |
| [speckit-specify](./speckit-specify/) | Spec Kit - User & functional specification authoring (spec.md) |
| [speckit-plan](./speckit-plan/) | Spec Kit - Technical architecture blueprinting (plan.md) |
| [speckit-tasks](./speckit-tasks/) | Spec Kit - Atomic task breakdown & verification (tasks.md) |
| [speckit-implement](./speckit-implement/) | Spec Kit - TDD Red-Green-Refactor execution & tracking |
| [reverse-engineer-anything](./reverse-engineer-anything/) | REA universal reverse engineering for native binaries, .NET, JS, APK, WASM |
| [claude-mem](./claude-mem/) | Persistent memory management with SQLite + FTS5 decisions recall |
| [create-feature](./create-feature/) | Tạo feature mới từ spec đến implementation |
| [create-api](./create-api/) | Thiết kế và implement REST/GraphQL API |
| [create-ui](./create-ui/) | Thiết kế và implement UI components |
| [database-migration](./database-migration/) | Tạo và quản lý database migrations |
| [code-review](./code-review/) | Review code theo quality gates |
| [debug-error](./debug-error/) | Debug và fix lỗi systematic |
| [fullstack-web](./fullstack-web/) | Fullstack web app (ASP.NET + React) |
| [security-scan](./security-scan/) | Security vulnerability scan |
| [frontend-design](./frontend-design/) | External frontend design skill; pending owner review |
| [project-audit-optimizer](./project-audit-optimizer/) | Rà soát codebase theo coverage ledger; tối ưu UI/UX, responsive/chống che khuất, hiệu năng, ổn định và thị giác có bằng chứng |
| [enterprise-ai-chatbot-integration](./enterprise-ai-chatbot-integration/) | Tích hợp AI chatbot GPT-AI cho CRM, HRM, Sales và hỗ trợ hệ thống |
| [web-ai-reasoning-integration](./web-ai-reasoning-integration/) | AI reasoning có web grounding và phân tích tệp đính kèm cho ứng dụng web/doanh nghiệp |
| [browser-testing-with-devtools](./browser-testing-with-devtools/) | E2E browser testing & automated UI verification |
| [code-simplification](./code-simplification/) | Code refactoring & YAGNI complexity reduction |
| [context-engineering](./context-engineering/) | Context window optimization & relevance filtering |
| [doubt-driven-development](./doubt-driven-development/) | Critical validation & edge-case stress testing |
| [incremental-implementation](./incremental-implementation/) | Atomic, surgical code step implementation |
| [interview-me](./interview-me/) | Interactive requirements elicitation & alignment |
| [planning-and-task-breakdown](./planning-and-task-breakdown/) | Task planning & dependency ordering |
| [source-driven-development](./source-driven-development/) | Codebase-grounded architecture & synthesis |
| [testing](./testing/) | Test suite design, execution & coverage analysis |
| [sepay](./sepay/) | SePay Vietnam Payments (Bank Transfer, Webhook, VietQR, VA, e-Invoice, Gateway, CLI/MCP) |
| [payos](./payos/) | PayOS Open Banking Gateway API v2 (VietQR, Payment Links, Webhook Checksum HMAC-SHA256) |
| [pay2s](./pay2s/) | Pay2S Vietnam Bank Webhook & Recipient Name Lookup (Bearer Token Auth, Tra cứu chủ tài khoản) |

### External bundle: Claude-BugHunter

Nguồn: [elementalsouls/Claude-BugHunter](https://github.com/elementalsouls/Claude-BugHunter), pinned tại commit `6a2153d9932adf41fa4d7f5dee626047c5005a83`.
84 skill đã được import append-only vào Resource Hub và nhân bản sang `.cursor/skills`, `.agents/skills`, `.claude/skills`; không skill cũ nào bị ghi đè.
Chỉ dùng cho bug bounty / external red-team trong phạm vi được cấp quyền.
Provenance: `vibe-code-framework/00-governance/records/external-skills.json`.
License và attribution: `vibe-code-framework/00-governance/records/licenses/claude-bughunter/`.
Trạng thái governance: `SOT-EXTERNAL`, `NOT_GRANTED`, `PENDING_REVIEW`.

## Ánh xạ sang Cursor

```
03-skills/create-api/SKILL.md → .cursor/skills/create-api/SKILL.md
03-skills/create-ui/SKILL.md  → .cursor/skills/create-ui/SKILL.md
...
```

## Ánh xạ sang Codex

```
03-skills/create-api/SKILL.md → .agents/skills/create-api/SKILL.md
03-skills/create-ui/SKILL.md  → .agents/skills/create-ui/SKILL.md
...
```

## Bổ sung skill từ GitHub

Dùng importer append-only tại `tools/skill_installer.py` để lấy một thư mục
skill thật có `SKILL.md` từ GitHub vào Resource Hub:

```powershell
python tools/skill_installer.py add-github `
  --repo owner/repository `
  --skill-path skills/example `
  --ref main
```

Thêm `--dry-run` để tải và kiểm tra trước mà không ghi file.

Importer không ghi đè thư mục đã tồn tại, không xóa hoặc migration `.cursor`,
và ghi provenance external vào
`00-governance/records/external-skills.json`. Skill mới ở trạng thái
`SOT-EXTERNAL`/`PENDING_REVIEW`; không có approval nào được suy diễn.
