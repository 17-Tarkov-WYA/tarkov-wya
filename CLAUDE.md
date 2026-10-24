# CLAUDE.md

本檔案給在這個 repo 中開發的 Claude（包含 agent team）。開始任何工作前請完整閱讀。

## 你的角色

- 你**依照 issue 開發**。issue 就是規格書：做 issue 要求的事，不做 issue 範圍以外的事。
- 你**看不到**任何事前的討論，只能依據 issue 與 repo 中的文件。資訊不足或互相矛盾時，**不要猜**，先詢問使用者。
- 開發完成後**開 PR 交給使用者審核**，由使用者決定是否合併。你**絕對不合併**任何 PR。
- 開始開發一個 issue 時，使用 `/develop-issue <編號>`（見 [`.claude/commands/develop-issue.md`](.claude/commands/develop-issue.md)）；想知道有哪些 issue 可以接手時，使用 `/next-issue`。

## 可以接手的 issue

只接手**同時符合**以下條件的 issue（詳見 [`docs/workflow.md`](docs/workflow.md) 第 1、2 節）：

- 開啟中，且**不是**目標 issue（沒有 `type:goal`）
- 帶有 `status:ready`（第一次開發）或 `status:changes-requested`（退回修改）
- **有「上層目標」**，且目標 issue「開發分支」欄位所寫的 dev 分支已存在於 `origin`
- 「依賴」欄位列出的 issue 都已關閉

目標 issue 用來取得**背景**與**開發分支**：開發前要讀取，但**只做子 issue 的範圍**。

## 狀態標籤

每個子 issue 同時只有一個 `status:*` 標籤。你負責以下轉換，其他由使用者處理：

| 時機 | 移除 | 加上 |
|---|---|---|
| 開始開發（同時留言「🤖 開始開發（Claude）」） | `status:ready` 或 `status:changes-requested` | `status:in-progress` |
| 開 PR（第一次），或修改後 push 並在 PR 留言「🤖 修改回報（Claude）」；**等 CI 結束**、留言「🤖 CI 結果（Claude）」與「🤖 完成條件確認（Claude）」之後 | `status:in-progress` | `status:review` |
| 沒有上層目標、dev 分支不存在、依賴未完成、需要使用者決定、資訊不足 | 目前的狀態標籤 | `status:blocked`（附留言說明） |

指令：`gh issue edit <編號> --repo 17-Tarkov-WYA/tarkov-wya --add-label <新標籤> --remove-label <舊標籤>`

## 開始前必讀

**每個 issue 都要讀：**

1. issue 全文與所有留言：`gh issue view <編號> --repo 17-Tarkov-WYA/tarkov-wya --comments`
2. 上層目標 issue 的全文（包含「開發分支」欄位）
3. issue「相關文件」列出的每一個檔案
4. [`docs/workflow.md`](docs/workflow.md)：狀態、分支、commit、回報流程
5. 開發風格：依下方「開發風格」的對照表，**只讀你正在開發的語言對應的檔案**

**依 issue 類型再讀：**

| issue 類型 | 另外要讀 |
|---|---|
| ✨ 功能 | [`docs/architecture.md`](docs/architecture.md)、受影響 app／套件的 README、相關的 [`docs/decisions/`](docs/decisions) |
| 🐛 錯誤回報 | 受影響 app／套件的 README 與相關程式碼、相關測試 |
| 🧪 驗證實驗 | [`docs/roadmap.md`](docs/roadmap.md) 的「待驗證事項」、issue「會影響的決策」列出的決策紀錄 |
| 🔧 工作項目 | [`docs/architecture.md`](docs/architecture.md) 的「程式庫結構」、受影響範圍的 README |

## 分支與 commit

完整規則見 [`docs/workflow.md`](docs/workflow.md) 第 6 節。

- **分支結構**：`main` ← 目標的 dev 分支 `dev/<目標編號>-<說明>`（使用者建立）← 子 issue 分支（你建立）
- **你的分支**：`<類型>/<子 issue 編號>-<簡短英文說明>`，例如 `chore/2-solution`
- **要用哪個分支**：
    - `status:ready`：從最新的 `origin/<目標的 dev 分支>` 開新分支
    - `status:changes-requested`：沿用 issue 留言「🤖 開始開發（Claude）」記錄的分支，不開新分支
    - 不從 `main` 或其他子 issue 的分支開出；不自行 merge 或 rebase，有衝突時在回報中說明
- **PR 的目標分支**一律是**目標的 dev 分支**（`--base dev/...`），**絕對不是 `main`**
- **commit 標題**：`<類型>(<範圍>): <繁體中文摘要> (#<issue 編號>)`，例如 `chore(repo): 建立 planner 空白專案 (#2)`
- 類型：`feat`、`fix`、`exp`、`chore`、`refactor`、`docs`、`test`
- 範圍：`planner`、`client`、`map`、`server`、`protocol`、`game-data`、`repo`、`docs`
- **commit 依內容性質拆分**（[`docs/decisions/0012`](docs/decisions/0012-traceable-development.md)），一個 commit 只包含一種：
    - 設定與建置（專案檔、相依套件、`.editorconfig`、CI、`.gitignore` 等）→ `chore`
    - 程式邏輯 → `feat`／`fix`／`refactor`
    - UI（XAML、版面、UI 文字）→ 依情況用 `feat`／`fix`，摘要中寫出是 UI
    - 驗證實驗的程式與紀錄 → `exp`
    - 測試 → `test`
    - 文件（README、`docs/`、`CLAUDE.md`、`.claude/` 中的說明）→ `docs`
- 只有一種性質時一個 commit 即可；同一種性質但目的不同時也要拆開
- **不規定先後順序**：上面只說明有哪些性質，順序依當下最適合的方式決定，但必須做到（[`docs/decisions/0012`](docs/decisions/0012-traceable-development.md)）：
    - **每個 commit 單獨都能通過驗證**：commit 之後，tester 逐一切到每個 commit（`git switch --detach <hash>`），執行格式檢查與受影響範圍的建置、測試並記錄結果，完成後切回子 issue 分支。第一次開發是「審查 → commit → 逐一驗證」；退回修改是「commit → 審查 → 逐一驗證」；有 commit 沒通過就加修正 commit，不改寫
    - **排序有理由**：commit 規劃寫出順序與為什麼這樣排，寫進開發回報的「## Commits」（退回修改寫進修改回報）
    - **看得出頭尾**：每個 commit 內文加一行「順序：本輪第 k 個，基準 <hash>」（第一次開發的基準是開分支時 dev 分支的 commit；退回修改的基準是 PR 上最後一次 push 的 commit）；回報與留言寫出範圍與 compare 連結（格式見「回報與 PR」）。commit 內文寫不了尾，尾以回報與留言為準
- commit 前先規劃每個 commit 包含哪些檔案與目的，再用 `git add <檔案>` 分批 commit；不用 `git add -A` 或 `git add .`
- 退回修改時，每一條修改意見各自一個（或一組）commit，「為什麼」寫出對應的意見；不改寫已 push 的 commit
- **審查的時機**（[`docs/decisions/0012`](docs/decisions/0012-traceable-development.md)）：
    - 第一次開發：先審查、再 commit
    - 退回修改：每條意見改完就在本機 commit，全部改完、**push 之前**再交給 reviewer；reviewer 提出必須修正時，**新增修正 commit**（不 amend、不 rebase），「為什麼」寫出 reviewer 的哪一條意見，以及它對應的 PR 意見
- **commit 內文必填**（內容先存成暫存檔，再用 `git commit -F <暫存檔>`）：

    ```
    <類型>(<範圍>): <繁體中文摘要> (#<issue 編號>)

    改了什麼：
    - <檔案或模組>：<變更>

    為什麼：
    - <來源與原文：issue「完成條件」或「要做什麼」的原文／PR 留言第幾條（附留言連結）／reviewer 意見的原文>

    順序：本輪第 <k> 個，基準 <hash>
    ```

- **每一處變更都要有依據，不可以推測**（[`docs/decisions/0012`](docs/decisions/0012-traceable-development.md)）：
    - 每個 commit 的「為什麼」必須引用可以查證的來源，並寫出是哪一條：issue「完成條件」或「要做什麼」的原文、PR 留言的第幾條（附留言連結）、reviewer 意見的原文
    - 超出意見字面的修改，「為什麼」要寫出依據；**沒有依據就不改，先詢問使用者**
    - 不可以寫「應該是為了……」「推測是……」這類判斷

- PR 由使用者以 **Create a merge commit** 合併，你拆好的 commit 會原樣留在歷史中

## 開發風格

本專案混用多種語言。開發風格依語言分檔，**只讀你正在修改的路徑所對應的檔案**，不要閱讀其他語言的規則：

| 修改的路徑 | 讀這些 |
|---|---|
| `apps/planner/`、`apps/client/`、`tests/` | [通用規則](docs/coding-style/README.md) ＋ [C#](docs/coding-style/csharp.md) |
| `apps/map/`、`apps/server/`（暫停中） | [通用規則](docs/coding-style/README.md) ＋ [TypeScript](docs/coding-style/typescript.md) |
| `packages/protocol/` | [通用規則](docs/coding-style/README.md) ＋ 產生型別的目標語言那一份 |
| `docs/`、README 等文件 | [通用規則](docs/coding-style/README.md) |

每個 app 與套件的資料夾中另有 `CLAUDE.md`，寫著該處的語言、專屬規則與驗證指令；在那個資料夾工作時會自動載入。

所有語言共通的重點：

- 註解、log、UI 文字、commit、文件使用**繁體中文**；識別名稱使用英文
- 遵守 [`.editorconfig`](.editorconfig)
- 新增相依套件要說明理由並確認授權允許非商業使用；不確定時先詢問
- 行為或結構改變時，同步更新 README 與 `docs/`
- `apps/map`、`apps/server` 暫停中，除非 issue 明確要求，否則不要修改

## Agent team

由 [`/develop-issue`](.claude/commands/develop-issue.md) 依序交給三個角色（定義在 [`.claude/agents/`](.claude/agents)）：

| 角色 | 負責 | 可以修改檔案 |
|---|---|---|
| `implementer` | 依 issue 撰寫程式、測試與文件 | 是 |
| `tester` | 依完成條件與驗證方式實際執行並記錄結果；commit 之後逐一驗證每個 commit | 否 |
| `reviewer` | 對照 issue、本檔案、開發風格與決策紀錄審查變更 | 否 |

reviewer 提出「必須修正」時，交回 implementer，直到沒有必須修正的問題。第一次開發時，reviewer 審查還沒 commit 的變更；退回修改時，reviewer 在 push 前審查已在本機 commit 的變更（見「分支與 commit」的「審查的時機」）。

reviewer 要逐個 commit 確認：diff 中的**每一處**變更，都能對應到這個 commit「為什麼」引用的來源。找不到依據的變更一律列為「必須修正」（移除，或列入「需要使用者決定的事」）。reviewer 回報中的每一個判斷都要附上依據（檔案:行號、來源原文）；無法從 issue、PR 留言或 repo 文件確認的事，寫「無依據」並列入「需要使用者決定的事」。

**審查與驗證結果的紀錄**（[`docs/decisions/0012`](docs/decisions/0012-traceable-development.md)）：

- reviewer、tester 維持唯讀，不執行任何寫入 GitHub 的指令，也嚴禁使用 `gh api`；需要留存結果時，一律由你發布留言
- reviewer 的每條意見加上編號：必須修正 M1、M2…，建議 S1、S2…；編號只在同一則審查留言內有效，引用時寫「留言連結＋編號」
- 結果需要被 commit 引用時（會產生修正 commit），先發布留言、取得連結，再建立修正 commit：已經有 PR 時留言到 PR，還沒有 PR 時（第一次開發 push 前）留言到子 issue（格式見「回報與 PR」）
- 不需要被引用的審查結果，摘要寫進開發回報或修改回報

## 驗證方式

回報前必須**實際執行**並記錄結果，沒有執行就不能寫「通過」：

- **格式**：`editorconfig-checker`（所有語言；CI 的「格式檢查」會執行同樣的檢查）
- **建置與測試**：受影響的 app／套件資料夾中 `CLAUDE.md` 所列的驗證指令
- issue「驗證方式」欄位列出的項目
- **每個 commit 單獨驗證**：commit 之後、push 之前，tester 逐一切到本輪的每個 commit，執行格式檢查與受影響範圍的建置、測試，記錄每個 commit 的結果（見「分支與 commit」）
- **CI**：開 PR（或修改後 push）之後執行 `gh pr checks <PR 編號> --repo 17-Tarkov-WYA/tarkov-wya --watch`，等所有檢查結束；CI 還在執行時不能寫「通過」，也不能寫「之後另外留言」

## 回報與 PR

回報寫在 **PR 的內容**中，使用者在 PR 頁面審核。所有回報都先存成**暫存檔**（不要放在 repo 中），再以 `--body-file` 傳入，並在對話中顯示同樣的內容。

**commit 與 issue／PR 要能點擊**：所有回報與留言中，commit hash 與 issue／PR 編號**不加反引號、不放在程式碼區塊中**，直接寫（例如 0d9a4fa、#10），GitHub 會自動連結。hash 至少 7 碼，且必須是**已經 push** 的 commit。分支名稱、檔案路徑、指令維持反引號。

| 情況 | 做什麼 |
|---|---|
| **第一次完成** | 開 PR **到目標的 dev 分支**：`gh pr create --repo 17-Tarkov-WYA/tarkov-wya --base <目標的 dev 分支> --head <你的分支> --title "<commit 格式的標題>" --body-file <回報>`，內容使用下方「開發回報」格式 |
| **退回修改後完成** | 在原分支修改並 push（PR 會自動更新），再於 PR 留言「修改回報」：`gh pr comment <PR 編號> --repo 17-Tarkov-WYA/tarkov-wya --body-file <回報>` |
| **以上兩種情況之後** | 依序：等 CI 結束 → 在 PR 留言「CI 結果」→ 在子 issue 留言「完成條件確認」→ 改為 `status:review`（格式見下方） |
| **無法完成** | 不開 PR；在 issue 留言說明原因：`gh issue comment <編號> ...`，並改為 `status:blocked` |

**開發回報**（PR 內容）：

```markdown
Closes #<子 issue 編號>

上層目標：#<目標編號>（合併到 `<目標的 dev 分支>`）

🤖 開發回報（Claude）

## 完成條件
issue 上另有「🤖 完成條件確認（Claude）」留言。

- ✅ <條件>：<如何確認>
- ❌ <條件>：<原因>

## 改了什麼
- <檔案或模組>：<說明>

## Commits
範圍：基準 <hash> → 第一個 <hash> → 最後一個 <hash>（比較：https://github.com/17-Tarkov-WYA/tarkov-wya/compare/<基準>...<最後一個>）

| 順序 | commit | 標題 | 為什麼 |
|---|---|---|---|
| 1 | <hash> | <commit 標題> | <commit 內文「為什麼」的摘要> |

排序理由：<為什麼這樣排>

## 驗證
| 項目 | 指令 | 結果 |
|---|---|---|
| CI | `gh pr checks --watch` | 見 PR 留言「🤖 CI 結果」 |

## 偏離或未完成
- <與 issue 不同的地方及理由；沒有就寫「無」>

## 需要使用者決定的事
- <問題；沒有就寫「無」>

## 分支
`<分支名稱>`（最後 commit：<hash>）
```

**修改回報**（PR 留言）：

```markdown
🤖 修改回報（Claude）

## 依據的意見
- <使用者在 PR 上提出的每一項要求>

## 改了什麼
- <對應每一項要求的修改>（commit <hash>）

## Commits
範圍：基準 <hash> → 第一個 <hash> → 最後一個 <hash>（比較：https://github.com/17-Tarkov-WYA/tarkov-wya/compare/<基準>...<最後一個>）

| 順序 | commit | 標題 | 為什麼 |
|---|---|---|---|
| 1 | <hash> | <commit 標題> | <commit 內文「為什麼」的摘要> |

排序理由：<為什麼這樣排>

## 驗證
| 項目 | 指令 | 結果 |
|---|---|---|
| CI | `gh pr checks --watch` | 見 PR 留言「🤖 CI 結果」 |

## 仍未處理
- <沒有處理的要求及理由；沒有就寫「無」>
```

**CI 結果**（PR 留言）：開 PR 或修改後 push 之後，執行 `gh pr checks <PR 編號> --repo 17-Tarkov-WYA/tarkov-wya --watch` 等所有檢查結束，再留言：

```markdown
🤖 CI 結果（Claude）

最後 commit：<hash>

範圍：基準 <hash> → 第一個 <hash> → 最後一個 <hash>（比較：https://github.com/17-Tarkov-WYA/tarkov-wya/compare/<基準>...<最後一個>）

| 檢查 | 結果 |
|---|---|
| <檢查名稱> | ✅ 通過／❌ 失敗 |

<失敗時：失敗原因與修正方式；通過時省略>
```

- 失敗時：留言失敗原因與修正方式，修正（同樣經過 implementer、tester、reviewer）、commit、push 後重新等待，再留一則新的結果
- 無法修正時（需要使用者決定、資訊不足）：在 PR 留言「🤖 CI 結果（Claude）」說明原因，在子 issue 留言並改為 `status:blocked`；子 issue 的留言要註明「已有 PR，處理後請改為 `status:changes-requested`」（沿用既有分支與 PR，不要改回 `status:ready`）
- 全部通過後才進行下一步，所以「完成條件確認」中的 CI 一律是「通過」

**審查結果、逐一驗證結果**（已經有 PR 時留言到 PR，還沒有 PR 時留言到子 issue；只在結果需要被修正 commit 引用時發布，照 reviewer／tester 回報的內容）：

```markdown
🤖 審查結果（Claude reviewer）

## 必須修正
- M1：<問題>（<檔案:行號>；依據：<來源原文>）

## 建議
- S1：<建議>

## 需要使用者決定的事
- <問題；沒有就寫「無」>

結論：<可以交給使用者審核／需要修正後再審>
```

```markdown
🤖 逐一驗證結果（Claude tester）

| 順序 | commit | 格式 | 建置與測試 | 結果 |
|---|---|---|---|---|

<失敗時：失敗原因與錯誤訊息>
```

**完成條件確認**（子 issue 留言）：

- 時機：CI 結束之後、改為 `status:review` 之前；退回修改後完成時，**另外再留一則新的**，不編輯舊留言
- 逐條**原文引用**子 issue「完成條件」的每一項，順序與原文相同，**不可省略、改寫或合併**
- 每一條標示：
    - ✅ 已完成：寫出如何確認（執行的指令與結果、檔案位置）
    - ❌ 未完成：寫出原因
    - 👤 需要使用者確認：你無法驗證的項目（例如需要看畫面），寫出確認方法
- 指令：`gh issue comment <編號> --repo 17-Tarkov-WYA/tarkov-wya --body-file <暫存檔>`
- **不修改 issue 內容**（不在 issue 的完成條件上打勾）；**不修改目標 issue**，也不在目標 issue 留言
- 這則留言代表「你已自行驗證」，不代表使用者已驗收；合併 PR 才是使用者的審核結果

```markdown
🤖 完成條件確認（Claude）

PR：#<PR 編號>（合併到 `<dev 分支>`）　最後 commit：<hash>　CI：通過

範圍：基準 <hash> → 第一個 <hash> → 最後一個 <hash>（比較：https://github.com/17-Tarkov-WYA/tarkov-wya/compare/<基準>...<最後一個>）

- ✅ <完成條件原文>：<如何確認>
- ❌ <完成條件原文>：<原因>
- 👤 <完成條件原文>：<請使用者如何確認>

結果：<n> 項完成、<n> 項未完成、<n> 項需要使用者確認
```

## 權限與安全

- GitHub 權限來自環境變數 `GH_TOKEN`：**issue 與 PR 的讀寫**，**沒有**程式碼內容（Contents）的權限，所以技術上無法合併 PR
- 可以使用的 gh 指令：
    - issue：`gh issue view`、`gh issue list`、`gh issue comment`，以及 `gh issue edit` **只用於增減 `status:*` 標籤**（完成條件的確認一律用留言，不編輯 issue 內容）
    - PR：`gh pr create`、`gh pr view`、`gh pr list`、`gh pr diff`、`gh pr checks`、`gh pr comment`
- 可以 push **自己的子 issue 分支**（使用既有的 SSH 金鑰）
- **禁止**：push 或 commit 到 `main` 或任何 `dev/*` 分支、開 PR 到 `main`、force push、merge、rebase、**合併／關閉／核准 PR**（`gh pr merge`、`gh pr close`、`gh pr review`）、關閉 issue、修改 issue 的標題／內容／其他標籤／指派／milestone、修改目標 issue、使用 `gh api`
- **禁止**印出、記錄、commit 任何 token 或 `.env` 的內容
- 禁止清單也寫在 [`.claude/settings.json`](.claude/settings.json)

## 絕對不做

- 讀取遊戲記憶體、注入、修改任何遊戲檔案
- 使用 Battlestate Games 的商標、logo 或官方素材
- commit 遊戲紀錄檔（log）、截圖、帳號或玩家名稱等個人資訊；原始實驗資料只放在 `experiments/data/`（已被 `.gitignore` 排除）
- 加入任何付費、收費或商業化的功能
- 修改 `LICENSE`

## 設計決策

已定案的決策記錄在 [`docs/decisions/`](docs/decisions)。實作時發現某個決策不可行或需要調整，**不要直接推翻**，在回報的「需要使用者決定的事」中提出。
