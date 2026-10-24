# 與 Claude Code 整合開發 (Claude Code Integration)

本文件給**使用者**：說明如何設定環境、啟動負責開發的 Claude，以及如何審核成果。開發的 Claude 要遵守的規則寫在 [`CLAUDE.md`](../CLAUDE.md)。決策依據見 [`decisions/0010`](decisions/0010-claude-code-integration.md)、[`decisions/0012`](decisions/0012-traceable-development.md)。

## 1. repo 中的 Claude Code 設定

| 檔案 | 用途 |
|---|---|
| [`CLAUDE.md`](../CLAUDE.md) | 開發規則；Claude Code 在此 repo 啟動時自動讀取 |
| [`.claude/commands/next-issue.md`](../.claude/commands/next-issue.md) | `/next-issue [目標編號]`：列出目前可以接手的子 issue |
| [`.claude/commands/develop-issue.md`](../.claude/commands/develop-issue.md) | `/develop-issue <編號>`：依子 issue 開發的完整流程（含狀態標籤與回報） |
| [`.claude/agents/implementer.md`](../.claude/agents/implementer.md) | agent team：實作 |
| [`.claude/agents/tester.md`](../.claude/agents/tester.md) | agent team：測試（不修改程式碼） |
| [`.claude/agents/reviewer.md`](../.claude/agents/reviewer.md) | agent team：審查（不修改程式碼） |
| [`.claude/settings.json`](../.claude/settings.json) | 允許與禁止的指令（允許開 PR；禁止合併／關閉／核准 PR、merge、push 到 main） |

個人設定請放在 `.claude/settings.local.json` 或 `CLAUDE.local.md`，這兩個檔案不會上傳。

## 2. 第一次設定

### 2.1 安裝工具

- [Claude Code](https://claude.com/claude-code)
- .NET SDK 10
- GitHub CLI：

  ```powershell
  winget install --id GitHub.cli
  ```

- push 分支使用你電腦上既有的 SSH 金鑰

### 2.2 建立 GitHub token

GitHub → 右上角頭像 → **Settings → Developer settings → Personal access tokens → Fine-grained tokens → Generate new token**：

| 設定 | 值 |
|---|---|
| Resource owner | `17-Tarkov-WYA` |
| Repository access | Only select repositories → `tarkov-wya` |
| Expiration | 90 天以內 |
| Issues | **Read and write** |
| Metadata | Read-only（必選） |
| Pull requests | **Read and write**（開 PR、讀取審核意見） |
| Contents 與其他 | **No access**（合併 PR 需要 Contents 的寫入權限，不給就無法合併） |

> 選不到 `17-Tarkov-WYA` 時，到 organization 的 **Settings → Personal access tokens** 允許使用 fine-grained token。

### 2.3 把 token 交給 GitHub CLI

在**你自己的** PowerShell 視窗執行（不要透過 Claude，避免 token 出現在對話中）：

```powershell
[Environment]::SetEnvironmentVariable("GH_TOKEN", "<你的 token>", "User")
```

重新開啟終端機後確認：

```powershell
gh issue list --repo 17-Tarkov-WYA/tarkov-wya
```

> **絕對不要**把 token 貼到對話、issue、commit 或任何檔案中。外流時到 token 頁面按 **Revoke** 立即撤銷。

### 2.4 保護 main 與 dev 分支（建議）

repo 的 **Settings → Rules → Rulesets → New branch ruleset**：

- Target：`main`，以及 **Include by pattern** `dev/**`（同一個 ruleset 可以設定多個目標）
- 勾選 **Restrict deletions**（目標完成後保留 dev 分支，見 [`decisions/0012`](decisions/0012-traceable-development.md)）
- 勾選 **Require a pull request before merging**
- 勾選 **Require status checks to pass** → 選「格式檢查」（之後加上建置）
- 勾選 **Block force pushes**
- **不要**勾選「Require approvals」：Claude 開的 PR 作者會顯示為你的帳號，GitHub 不允許作者核准自己的 PR，勾了你也無法合併

設定後，任何人（包含你與 Claude）都不能直接 push 到 `main` 或 dev 分支，只能透過 PR 合併；由你按下合併就是審核。

> 「建立」dev 分支不受影響：從 issue 的 **Create a branch** 建立新分支不需要 PR。

## 3. 建立目標、dev 分支與子 issue

1. 用「🎯 目標」範本建立目標 issue，「開發分支」填 `dev/<這個 issue 的編號>-<簡短英文說明>`，例如 `dev/1-skeleton`
2. **建立 dev 分支**：目標 issue 右側 **Development → Create a branch**，名稱填上一步的分支名稱，來源選 `main`
3. 在目標 issue 右側按 **Create sub-issue**，用 ✨／🐛／🧪／🔧 範本建立子 issue，「上層目標」**必填**目標的編號，有先後順序時填「依賴」
4. 子 issue 的完成條件、不做的範圍、相關文件、驗證方式都寫好後，加上 **`status:ready`**

> 沒有 `status:ready`、沒有上層目標、或目標的 dev 分支還沒建立的 issue，Claude 都不會接手。

## 4. 交給 Claude 開發

在 `tarkov-wya` 資料夾啟動 Claude Code：

- 想知道有哪些可以做：

    ```
    /next-issue
    /next-issue <目標編號>
    ```

- 開始開發一個子 issue：

    ```
    /develop-issue <子 issue 編號>
    ```

Claude 會依序：確認狀態、上層目標、dev 分支與依賴 → 讀取子 issue、目標與相關文件 → 從 dev 分支開出子分支 → 留言「🤖 開始開發（Claude）」並改為 `status:in-progress` → implementer 實作 → tester 驗證 → reviewer 審查 → 依內容性質分批 commit（每個 commit 內文寫出改了什麼、為什麼與順序）→ tester 逐一驗證每個 commit → push 分支 → **開 PR 到 dev 分支**（內容為「🤖 開發回報（Claude）」，第一行 `Closes #<編號>`）→ 等 CI 結束，在 PR 留言「🤖 CI 結果（Claude）」→ 在子 issue 留言「🤖 完成條件確認（Claude）」→ 改為 `status:review`。

所以看到 `status:review` 時，CI 已經結束，可以直接審核。

同時開發多個子 issue 時，每個 issue 使用一個獨立的 Claude Code 工作階段（或 worktree），避免互相干擾。

## 5. 掌握開發狀態

| 想知道 | 看哪裡 |
|---|---|
| 目標完成多少 | 目標 issue 上的子 issue 完成比例 |
| 等我審核的 | **Pull requests** 頁面，或 issue 列表篩選 `is:open label:status:review` |
| 開發中的 | `is:open label:status:in-progress` |
| 卡住的 | `is:open label:status:blocked`（原因在 issue 留言） |
| 用哪個分支 | issue 留言「🤖 開始開發（Claude）」 |
| 完成條件驗證到哪裡 | 子 issue 留言「🤖 完成條件確認（Claude）」（以最新的一則為準） |
| CI 結果 | PR 留言「🤖 CI 結果（Claude）」 |
| reviewer 的審查結果、tester 的逐一驗證結果 | PR 留言「🤖 審查結果（Claude reviewer）」「🤖 逐一驗證結果（Claude tester）」（只在產生修正 commit 時留言；還沒有 PR 時在子 issue） |

各狀態標籤的意思與轉換見 [`workflow.md`](workflow.md) 第 2 節。

## 6. 審核成果

打開 Claude 開的 PR，依序檢查：

- [ ] PR 上方顯示的**目標分支是目標的 dev 分支**（例如 `dev/1-skeleton`），不是 `main`
- [ ] PR 內容（開發回報）：每一條完成條件都是 ✅，且附有**實際執行的指令與結果**
- [ ] 子 issue 的「🤖 完成條件確認（Claude）」留言：逐條原文引用完成條件，沒有 ❌；👤 的項目自己確認過。issue 內容沒有被修改（標題旁沒有「edited」）
- [ ] 「偏離或未完成」「需要使用者決定的事」都已處理
- [ ] **Commits**：依內容性質拆分（設定、程式、UI、測試、文件），每個 commit 內文都有「改了什麼」「為什麼」；回報「## Commits」的「單獨驗證」欄若有 ❌，都寫出由哪個 commit 修正
- [ ] **Files changed**：變更內容沒有超出「不做的範圍」
- [ ] **Checks** 與「🤖 CI 結果（Claude）」留言：CI 全部通過
- [ ] 自己執行一次驗證方式（例如 `dotnet build`）

之後：

- **需要修改**：在 PR 的 **Conversation** 留言，**逐條寫出要改什麼**（行內留言只當補充，Claude 不一定讀得到）；子 issue 的標籤改成 `status:changes-requested`，再執行 `/develop-issue <編號>`。Claude 會在同一個分支依每條意見分別在本機 commit，push 前由 reviewer 審查、tester 逐一驗證每個 commit，再 push（PR 自動更新），在 PR 留言「🤖 修改回報（Claude）」與新的「🤖 CI 結果（Claude）」，並在子 issue 另留一則新的「🤖 完成條件確認（Claude）」
- **通過**：按 **Create a merge commit** 合併到 dev 分支（保留每個 commit）；工作流程「關閉已合併的子 issue」會自動關閉子 issue；合併後刪除子分支
- 「🤖 完成條件確認」只代表 Claude 已自行驗證；**你按下合併才是審核結果**
- **不要了**：自己關閉 PR，並視情況調整子 issue
- **卡住**（`status:blocked`）：看 issue 留言中的原因，處理後把標籤改回 `status:ready`；**已經有 PR 時**（例如 CI 失敗無法修正）改為 `status:changes-requested`，並在 PR 留言說明處理方式或接下來要改什麼，Claude 才會沿用原本的分支與 PR

## 7. 完成目標

1. 子 issue 全部關閉後，在 dev 分支上對照目標 issue 的「整體完成條件」與「驗收方式」做最後審核；有缺漏就新增子 issue
2. 通過後開 PR：**base 選 `main`、compare 選 dev 分支**，內容寫 `Closes #<目標編號>`
3. CI 通過後按 **Create a merge commit**（保留每個子 issue 的合併點與 commit），目標 issue 自動關閉
4. **保留** dev 分支，不要刪除：ruleset 禁止刪除 `dev/**`，用來防止誤刪；保留的 dev 分支也是這個目標的紀錄

同時進行多個目標、`main` 已經前進時，先把 `main` 合併進 dev 分支（在 dev 分支的頁面開 PR：base 選 dev 分支、compare 選 `main`），處理完衝突再繼續。

## 8. 防護層

| 層 | 防止的事 | 失效時 |
|---|---|---|
| `CLAUDE.md` 規則 | 超出範圍、違反禁止事項 | 由下列各層補上 |
| `.claude/settings.json` 禁止清單 | 合併／關閉／核准 PR、merge、push 到 main 等指令 | 規則以指令開頭比對，無法涵蓋所有寫法 |
| token 權限 | **合併 PR**、透過 API 修改程式碼內容 | 只能讀寫 issue 與 PR |
| `main` 的 ruleset | 直接 push 到 `main`、CI 未通過就合併 | — |
| 使用者審核 | 一切；**只有你能按下合併** | — |
