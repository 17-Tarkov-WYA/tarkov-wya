# 0012. 可追蹤的開發紀錄：commit 拆分、合併方式、CI 回報與完成條件確認

- 狀態：已採用
- 日期：2026-10-24

## 背景

第一次完整跑完流程（目標 #1 → 子 issue #2 → PR #3）後，發現：

- PR #3 的變更集中在少數 commit，git 歷史看不出每一部分改了什麼、為什麼改
- 決策 0011 規定子 issue 合併時把所有 commit 壓成一個，合併後一個子 issue 只剩一個 commit，拆得再細也會被合成一個
- `/develop-issue` 開 PR 後馬上檢查 CI，CI 還在執行，回報寫「結果會另外留言」，之後卻沒有留言
- 子 issue 完成後，issue 上看不出哪些完成條件已經驗證

## 決策

### commit 依內容性質拆分

一個 commit 只包含一種性質：

| 性質 | 內容 | 類型 |
|---|---|---|
| 設定與建置 | 專案檔、相依套件、`.editorconfig`、CI、`.gitignore` 等 | `chore` |
| 程式邏輯 | 程式碼 | `feat`／`fix`／`refactor` |
| UI | XAML、版面、UI 文字 | 依情況用 `feat`／`fix`，摘要中寫出是 UI |
| 驗證實驗 | 驗證實驗的程式與紀錄 | `exp` |
| 測試 | 測試程式與範例資料 | `test` |
| 文件 | README、`docs/`、`CLAUDE.md`、`.claude/` 中的說明 | `docs` |

- 只有一種性質時，一個 commit 即可；同一種性質但目的不同時，也要拆開
- **不規定先後順序**：上表只說明有哪些性質。順序由 Claude 依當下最適合的方式決定，但必須做到：
    - **每個 commit 單獨都能通過驗證**：commit 之後，tester 逐一切到每個 commit（`git switch --detach <hash>`），執行格式檢查與受影響範圍的建置、測試，記錄每個 commit 的結果，完成後切回子 issue 分支。第一次開發是「審查 → commit → 逐一驗證」；退回修改是「commit → 審查 → 逐一驗證」；有 commit 沒通過就加修正 commit，不改寫
        - 目標仍是每個 commit 單獨通過驗證；失敗時不改寫，新增修正 commit
        - 失敗的 commit，以及它之後、修正之前因同一個原因失敗的 commit，在「## Commits」的「單獨驗證」欄標示「❌（由 <修正 commit> 修正）」
        - tester 重新驗證時，只驗證上次驗證之後新增的 commit，並確認最後一個 commit 通過；已標示 ❌ 且有修正 commit 的，不再阻擋整輪
    - **排序有理由**：commit 規劃寫出順序與為什麼這樣排，寫進開發回報的「## Commits」（退回修改寫進修改回報）
    - **看得出頭尾**：
        - 每個 commit 內文加一行「順序：本輪第 k 個，基準 <hash>」
        - **每一次 push 就是一輪**。分支還沒 push 過時，基準是開分支時 dev 分支的 commit；之後的每一次 push（包含退回修改、CI 失敗後的修正），基準一律是 push 之前的 `origin/<子 issue 分支>`
        - 開發回報、修改回報、CI 結果、完成條件確認都寫出範圍「基準 <hash> → 第一個 <hash> → 最後一個 <hash>」，並附 compare 連結 https://github.com/17-Tarkov-WYA/tarkov-wya/compare/<基準>...<最後一個>
        - commit 內文寫不了尾（寫的時候後面的 commit 還不存在，也不能改寫），尾以回報與留言為準

### commit 訊息內文必填

標題格式不變（決策 [0009](0009-development-conventions.md)），但**內文為必填**：

```
<類型>(<範圍>): <繁體中文摘要> (#<issue 編號>)

改了什麼：
- <檔案或模組>：<變更>

為什麼：
- <來源（5 類之一，見「每一處變更都要有依據」）與原文>

順序：本輪第 <k> 個，基準 <hash>
```

- 退回修改時，每一條修改意見各自一個（或一組）commit，「為什麼」寫出對應的意見
- 不改寫已 push 的 commit

### 每一處變更都要有依據，不可以推測

- 每個 commit 的「為什麼」必須引用可以查證的來源，並寫出是哪一條。來源以「**事後打得開**」為標準，統一為 5 類：

    | 來源 | 引用方式 |
    |---|---|
    | 子 issue、目標 issue | issue 編號＋欄位＋原文 |
    | PR 留言（使用者意見） | 留言連結＋第幾條＋原文 |
    | reviewer 審查結果 | 留言連結＋意見編號（M1、S1…）＋原文 |
    | tester 驗證結果、CI 結果 | 留言連結（或開發回報的「## 驗證」）、CI run 連結 |
    | repo 中的文件 | `路徑:行號 @ commit` |

- 對話內容、沒有留下紀錄的判斷，不能作為來源
- reviewer 要逐個 commit 確認：diff 中的**每一處**變更，都能對應到這個 commit「為什麼」引用的來源
    - 找不到依據的變更，一律列為「必須修正」：移除，或列入「需要使用者決定的事」
    - 不可以寫「應該是為了……」「推測是……」這類判斷
- reviewer 回報中的每一個判斷，都要附上依據（檔案:行號、來源原文）；無法從上面 5 類來源確認的事，寫「無依據」並列入「需要使用者決定的事」
- implementer 與主要的 Claude 也一樣：超出意見字面的修改，「為什麼」要寫出依據；沒有依據就不改，先詢問使用者

### 審查的時機：第一次開發先審查，退回修改先 commit

- **第一次開發**：先審查、再 commit
- **退回修改**：每條意見改完就在本機 commit（一條意見一個或一組 commit），全部改完、**push 之前**再交給 reviewer。reviewer 要看：
    - 本輪全部的變更：`git diff origin/<自己的分支>`
    - 本機的每一個 commit：逐一 `git show <hash>`，確認每個 commit 只包含它對應的那一條意見
    - 整體的變更：`git diff origin/<dev 分支>`
- reviewer 提出必須修正時，**新增修正 commit**，不改寫既有的 commit（不 amend、不 rebase）；「為什麼」寫出 reviewer 的哪一條意見，以及它對應的 PR 意見
- push 前，最後一次審查之後新增的 commit（包含逐一驗證失敗後的修正 commit），都要再交給 reviewer，只審查新增的 commit
- reviewer 意見的 commit 拆分：
    - 必須修正（M）：一條一個 commit；同一個根本原因造成的多條可以合併，「為什麼」列出所有編號
    - 建議（S）：可以合成一個「處理審查建議」的 commit，「為什麼」逐一列出編號

### 審查與驗證結果要留下可以點開的紀錄

- reviewer、tester 維持唯讀，不執行任何寫入 GitHub 的指令；需要留存結果時，一律由主要的 Claude 發布留言
- reviewer 的每條意見加上編號：必須修正 M1、M2…，建議 S1、S2…；編號只在同一則審查留言內有效，引用時寫「留言連結＋編號」
- 結果需要被 commit 引用時（會產生修正 commit），先發布留言、取得連結，再建立修正 commit：
    - 已經有 PR（退回修改、CI 失敗後）：留言到 PR，標題為「🤖 審查結果（Claude reviewer）」或「🤖 逐一驗證結果（Claude tester）」
    - 還沒有 PR（第一次開發 push 前）：留言到子 issue
- 不需要被引用的審查結果，摘要寫進開發回報或修改回報
- reviewer、tester 嚴禁使用 `gh api`；查看 PR、issue 或留言一律用 `gh pr view`（例如 `gh pr view --comments`）、`gh issue view`

### 所有 PR 一律 Create a merge commit

子 issue 分支 → dev 分支、dev 分支 → `main`，都用 **Create a merge commit** 合併。

### 等 CI 結束再回報

- 開 PR（或退回修改後 push）之後，Claude 執行 `gh pr checks <PR 編號> --watch`，等所有檢查結束
- 通過：在 PR 留言「🤖 CI 結果（Claude）」，列出每項檢查與結果
- 失敗：在 PR 留言失敗原因與修正方式，修正、commit、push 後重新等待，再留言新的結果；無法修正時改為 `status:blocked`
- 開發回報中的 CI 一列寫「見 PR 留言『🤖 CI 結果』」，不寫「之後另外留言」

### 在子 issue 留言確認完成條件

- issue 內容是使用者寫的原始需求，Claude **不修改**（維持決策 [0011](0011-goals-and-status.md)「不能修改 issue 的其他內容」），改為在子 issue 下方留言「🤖 完成條件確認（Claude）」
- 時機：CI 結束之後、改為 `status:review` 之前；退回修改後完成時，**另外再留一則新的**，不編輯舊留言
- 內容：逐條**原文引用**子 issue「完成條件」的每一項，順序與原文相同，不可省略、改寫或合併；每一條標示：
    - ✅ 已完成：寫出如何確認（執行的指令與結果、檔案位置）
    - ❌ 未完成：寫出原因
    - 👤 需要使用者確認：Claude 無法驗證的項目（例如需要看畫面），寫出確認方法
- 格式見 [`CLAUDE.md`](../../CLAUDE.md)「回報與 PR」
- **不修改目標 issue**，也不在目標 issue 留言；目標的完成條件由使用者最後驗收
- 這則留言代表「Claude 已自行驗證」，不代表使用者已驗收；**合併 PR 才是使用者的審核結果**

### 回報中的 commit 要能點擊

回報與留言中的 commit hash 與 issue／PR 編號不加反引號，讓 GitHub 自動連結，從回報可以直接點到對應的 commit；hash 至少 7 碼，且必須是已經 push 的 commit。

### 目標完成後保留 dev 分支

dev 分支合併到 `main` 後**不刪除**：ruleset 禁止刪除 `dev/**`，用來防止誤刪；保留的 dev 分支也是目標的紀錄。

## 理由

- 使用者要能從 git 歷史追蹤每一部分改了什麼、為什麼改；依性質拆分後，看 commit 就知道設定、程式、文件各自的變更
- 壓成一個 commit 的合併方式會把拆好的 commit 合成一個，所以子 issue 也改用 merge commit；`git log --first-parent` 仍能看到一個子 issue 一個合併點，不會失去「一個子 issue 一個單位」的好處
- 不規定固定順序，由當下判斷最適合的排法；改以「每個 commit 單獨都能通過驗證」確認排出來的順序可行，並以 commit 內文的「順序」與回報中的範圍看出每一輪的頭尾（PR #10 第二輪意見第 3 條 https://github.com/17-Tarkov-WYA/tarkov-wya/pull/10#issuecomment-6384650208 ）。每個 commit 都能單獨通過驗證，之後要找出問題出在哪個 commit 也比較容易
- PR #10 第一輪的修改意見中，第 1、2、4 條都改到 `CLAUDE.md` 或 `.claude/commands/develop-issue.md`；commit 以檔案為單位，又不能用互動式的 `git add -p`，所以「一條意見一個 commit」和「先審查、再 commit」無法同時做到（PR #10 的「🤖 修改回報」 https://github.com/17-Tarkov-WYA/tarkov-wya/pull/10#issuecomment-6384501111 ）。使用者選擇保留一條意見一個 commit，把審查移到 push 之前（PR #10 第二輪意見第 1 條 https://github.com/17-Tarkov-WYA/tarkov-wya/pull/10#issuecomment-6384650208 ）
- CI 通常要幾分鐘，開 PR 後馬上檢查只會看到「執行中」；等結束再回報，使用者看到 `status:review` 時就能直接審核
- 考慮過讓 Claude 在 issue 內容的完成條件上打勾：使用者寫的原始需求會被修改，也分不出是誰確認的，所以改為另外留言；留言保留每一次的確認紀錄，退回修改後也看得出前後差異

## 影響

- 決策 [0011](0011-goals-and-status.md) 的「合併方式」被本決策取代，0011 的狀態改為「部分被 0012 取代（合併方式）」
- `docs/workflow.md`、`docs/claude-code.md`、`CLAUDE.md`、`.claude/commands/develop-issue.md`、`.claude/agents/implementer.md`、`.claude/agents/reviewer.md`、`.claude/agents/tester.md` 依本決策更新
- 開發回報（PR 內容）與修改回報加上「## Commits」：範圍（附 compare 連結），以及每個 commit 的順序、hash、標題、為什麼、單獨驗證，與排序理由
- implementer 完成時依性質分組回報修改的檔案，主要的 Claude 據此拆 commit
- `status:review` 改為在 CI 結果與完成條件確認都留言之後才加上
- `docs/workflow.md`、`docs/claude-code.md` 的「完成目標」移除「刪除 dev 分支」的步驟
- GitHub 上的合併選項由使用者手動調整
