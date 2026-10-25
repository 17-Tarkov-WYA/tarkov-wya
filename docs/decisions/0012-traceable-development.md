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

### commit 的拆分：每個 commit 都要有針對性

這是判斷的框架，不是固定的分類：

- 每個 commit 都應有一個明確的目的：只看該 commit 的變更與訊息，就能說明它做了什麼、為什麼做
- 不要將所有修改一股腦塞進同一個大雜燴 commit，也不要為了拆分而拆分，導致相關變更失去整體意義
- 類別相似、性質相同的變更可以聚合在同一個 commit；多條同類別的意見可以合併處理，並在「為什麼」中完整列出每一條對應的來源
- 一條意見若涉及不同目的或性質的變更（例如同時涉及 UI 與 CI），可以依實際情況拆分為多個 commit
- 拆分與聚合依當次變更的實際內容判斷，不要求一條意見對應一個 commit，也不要求每個 commit 僅對應一條意見
- 下表的性質保留為常見分組方式的參考框架，而非必須遵守的一對一分類：

| 性質 | 內容 | 類型 |
|---|---|---|
| 設定與建置 | 專案檔、相依套件、`.editorconfig`、CI、`.gitignore` 等 | `chore` |
| 程式邏輯 | 程式碼 | `feat`／`fix`／`refactor` |
| UI | XAML、版面、UI 文字 | 依情況用 `feat`／`fix`，摘要中寫出是 UI |
| 驗證實驗 | 驗證實驗的程式與紀錄 | `exp` |
| 測試 | 測試程式與範例資料 | `test` |
| 文件 | README、`docs/`、`CLAUDE.md`、`.claude/` 中的說明 | `docs` |

### commit 規劃

- 主要的 Claude 依當次變更內容決定分組，並在 reviewer 審查前提出完整的 commit 規劃。每個預計建立的 commit 都列出：
    1. **目的／標題**：說明該 commit 要完成什麼
    2. **包含的檔案**：列出預計納入的檔案
    3. **分組理由**：說明為什麼這些變更適合放在同一個 commit，以及必要時為什麼需要與其他變更拆開
    4. **「為什麼」的來源**：列出預計引用的來源，並確保最終 commit 內文具備正確、完整且可查證的依據
    5. **擬好的 commit 訊息**：包含「改了什麼／為什麼」及「順序」行
- 執行環境不支援 `git add -p`：同一次從工作目錄拆分建立 commit 時，以檔案為最小分組單位，同一個檔案不得分配給不同 commit（因逐一驗證失敗而新增修正 commit 時，不在此限）。規劃時考慮檔案間的依賴關係，避免產生無法獨立理解或驗證的分組
- commit 規劃是依實際變更提出的方案，不是固定藍圖；reviewer 可以要求調整分組、合併或拆分
- implementer 在回報中提供變更內容與涉及的檔案、檔案間的依賴關係，以及 commit 分組的建議與理由；implementer 只提供建議，不決定最終分組

### 順序與頭尾

- **不規定先後順序**：順序由主要的 Claude 依當下最適合的方式決定，但必須做到：
    - **每個 commit 單獨都能通過驗證**：建立 commit 之後，tester 逐一切到每個 commit（`git switch --detach <hash>`），執行格式檢查與受影響範圍的建置、測試，記錄每個 commit 的結果，完成後切回子 issue 分支。失敗時依「逐一驗證失敗」處理
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

- 已建立的 commit 不改寫（不 `git commit --amend`、不 `git rebase`）

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
- reviewer 要確認：每一處變更都能對應到 commit 規劃中該 commit「為什麼」引用的來源
    - 找不到依據的變更，一律列為「必須修正」：移除，或列入「需要使用者決定的事」
    - 不可以寫「應該是為了……」「推測是……」這類判斷
- reviewer 回報中的每一個判斷，都要附上依據（檔案:行號、來源原文）；無法從上面 5 類來源確認的事，寫「無依據」並列入「需要使用者決定的事」
- implementer 與主要的 Claude 也一樣：超出意見字面的修改，「為什麼」要寫出依據；沒有依據就不改，先詢問使用者

### 先審查、再 commit

第一次開發、退回修改、CI 失敗後的修正，都是同一個流程：

實作 → tester 驗證工作目錄整體變更 → reviewer 審查工作目錄變更與 commit 規劃 → 依審查意見修正 → 審查通過後依規劃建立 commit → tester 逐一驗證每個 commit → push

- 審查前不得先為預計提交的變更建立 commit；審查通過後，才執行 `git commit`
- 審查後的修正直接併入工作目錄中相關的變更，並同步更新 commit 規劃，不另外為審查意見建立修正 commit
- 若有 M，修正後須再次交由 reviewer 審查；確認通過後才能依最終規劃建立 commit
- reviewer 要看：
    - 工作目錄的變更：`git diff <基準>`（基準見「順序與頭尾」），加上 `git status` 列出的未追蹤新檔案（未追蹤檔案不會出現在 `git diff` 中，要直接讀取）
    - 整體差異：`git diff origin/<dev 分支>`
    - commit 規劃
- reviewer 評估 commit 規劃：
    1. **針對性**：每個 commit 是否有明確目的，能否說清楚做了什麼、為什麼做
    2. **分組合理性**：分組理由是否符合實際變更；是否混入不相關的內容，或將應一起理解的變更拆得過細
    3. **聚合與拆分**：同類別的變更是否被不必要地拆散；跨越不同目的或性質的變更是否有合理的拆分理由
    4. **來源完整性**：「為什麼」是否涵蓋每一條相關意見，並附上正確、可查證的來源與必要的行號
    5. **可執行性**：同一次工作目錄拆分的檔案分配是否符合以檔案為最小單位的限制，並考量檔案間依賴關係
    6. **訊息完整性**：擬好的 commit 訊息是否符合既有格式與追蹤性規範
- 分組、理由或來源不合理時，reviewer 提出 M（必須修正）或 S（建議修正），具體說明問題、判斷依據與建議的調整方式
- reviewer 不得要求每條意見各自建立 commit，也不得僅以 commit 的性質分類不同或目的不完全相同為由，直接要求拆分；應依實際變更的關聯性、目的及可理解性判斷

### 逐一驗證失敗

依規劃建立 commit 後，不改寫既有歷史（不 `git commit --amend`、不 `git rebase`）。tester 逐一驗證某個 commit 失敗時：

1. 主要的 Claude 留言記錄 tester 的驗證結果及失敗依據（已有 PR 留言到 PR，尚無 PR 留言到子 issue）
2. 在工作目錄進行修正，並擬定修正 commit 規劃（修正 commit 的「為什麼」引用 tester 驗證結果的留言連結與編號，並保留其他必要的可查證來源）
3. reviewer 只針對該修正內容與修正 commit 規劃進行增量審查（看 `git diff HEAD`，加上 `git status` 列出的未追蹤新檔案；若有 M，依「審查與驗證結果的紀錄」留言並修正）
4. 審查通過後，新增修正 commit，不修改原本已建立的 commit
5. tester 驗證新增的修正 commit，並確認最後一個 commit 通過；已標示失敗並由後續 commit 修正的歷史 commit，不阻擋通過
6. 完成驗證後才 push

- 「## Commits」中，原本驗證失敗的 commit（以及它之後、修正之前因同一個原因失敗的 commit）標示「❌（由 <修正 commit> 修正）」，保留修正 commit 與驗證結果的對應關係
- 修正 commit 也遵守「每個 commit 都要有針對性」：依實際變更的目的、關聯性及可理解性分組，不恢復成「一條意見一個 commit」

### 審查與驗證結果的紀錄

- reviewer、tester 維持唯讀，不執行任何寫入 GitHub 的指令；留言一律由主要的 Claude 發布
- reviewer 的每條意見加上編號：必須修正 M1、M2…，建議 S1、S2…；編號只在同一則審查留言內有效，引用時寫「留言連結＋編號」
- 只要 reviewer 提出 M，主要的 Claude 就留下審查結果紀錄（已有 PR 留言到 PR，尚無 PR 留言到子 issue），保留 reviewer 發現的問題、依據與 M／S 編號
- 修正併入原規劃的 commit 時，該 commit 的「為什麼」引用相關審查留言的連結與編號，並保留原本必要的來源依據
- 沒有 M 的審查結果，摘要寫進開發回報或修改回報
- 留言標題及格式由主要的 Claude 依當次情境決定；子 issue 上的格式與 PR 上保持一致即可，不另訂固定標題
- 嚴禁使用 `gh api`；查閱 PR 與留言統一使用 `gh pr view`（其餘依情境使用 `gh issue view`、`gh pr comment`、`gh issue comment` 等標準 `gh` 子指令）

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

- 使用者要能從 git 歷史追蹤每一部分改了什麼、為什麼改；每個 commit 都有針對性，看 commit 就知道每一部分各自的變更
- 壓成一個 commit 的合併方式會把拆好的 commit 合成一個，所以子 issue 也改用 merge commit；`git log --first-parent` 仍能看到一個子 issue 一個合併點，不會失去「一個子 issue 一個單位」的好處
- 不規定固定順序，由當下判斷最適合的排法；改以「每個 commit 單獨都能通過驗證」確認排出來的順序可行，並以 commit 內文的「順序」與回報中的範圍看出每一輪的頭尾（PR #10 第二輪意見第 3 條 https://github.com/17-Tarkov-WYA/tarkov-wya/pull/10#issuecomment-6384650208 ）。每個 commit 都能單獨通過驗證，之後要找出問題出在哪個 commit 也比較容易
- commit 的拆分改為框架、由 reviewer 評估（PR #10 第四輪意見 https://github.com/17-Tarkov-WYA/tarkov-wya/pull/10#issuecomment-6387732906 ）：commit 的目的是「每個 commit 都要有針對性」，「不要將所有修改一股腦塞進同一個大雜燴 commit，也不要為了拆分而拆分」，所以不要求「一條意見對應一個 commit」
- 取代的舊規則（#6 的 issue 內容不修改，見本決策「在子 issue 留言確認完成條件」，所以在這裡寫明取代關係）：
    - #6「要做什麼」一：「一個 commit 只包含一種性質」「同一種性質但目的不同時，也要拆開」「退回修改時，每一條修改意見各自一個（或一組）commit」
    - #6「要做什麼」三：「commit 前先依性質規劃 commit」「implementer.md：完成時的回報要依性質分組」
    - #6「完成條件」：「develop-issue.md 包含：依性質拆分 commit」「implementer.md 的回報要求依性質分組」（改為 commit 規劃與 implementer 的分組建議）
    - PR #10 第二輪意見第 1 條：退回修改先 commit、push 前再審查（https://github.com/17-Tarkov-WYA/tarkov-wya/pull/10#issuecomment-6384650208 ）
    - PR #10 第三輪意見第 1 條：「結果需要被 commit 引用時（會產生修正 commit），先發布留言」（改為有 M 就留言）；第 6 條：必須修正一條一個 commit、建議合成一個（https://github.com/17-Tarkov-WYA/tarkov-wya/pull/10#issuecomment-6386551274 ）
- 當初先 commit 再審查，是因為 commit 以檔案為單位、又不能用 `git add -p`，「一條意見一個 commit」和「先審查、再 commit」無法同時做到（PR #10「🤖 修改回報」 https://github.com/17-Tarkov-WYA/tarkov-wya/pull/10#issuecomment-6384501111 ）；不再要求一條意見一個 commit 之後，流程統一為先審查、再 commit，審查後的修正併入相關的 commit
- CI 通常要幾分鐘，開 PR 後馬上檢查只會看到「執行中」；等結束再回報，使用者看到 `status:review` 時就能直接審核
- 考慮過讓 Claude 在 issue 內容的完成條件上打勾：使用者寫的原始需求會被修改，也分不出是誰確認的，所以改為另外留言；留言保留每一次的確認紀錄，退回修改後也看得出前後差異

## 影響

- 決策 [0011](0011-goals-and-status.md) 的「合併方式」被本決策取代，0011 的狀態改為「部分被 0012 取代（合併方式）」
- `docs/workflow.md`、`docs/claude-code.md`、`CLAUDE.md`、`.claude/commands/develop-issue.md`、`.claude/agents/implementer.md`、`.claude/agents/reviewer.md`、`.claude/agents/tester.md`、`.github/PULL_REQUEST_TEMPLATE.md` 依本決策更新
- 開發回報（PR 內容）與修改回報加上「## Commits」：範圍（附 compare 連結），以及每個 commit 的順序、hash、標題、為什麼、單獨驗證，與排序理由
- implementer 回報變更內容、檔案間的依賴與 commit 分組建議；主要的 Claude 提出 commit 規劃，reviewer 評估
- `status:review` 改為在 CI 結果與完成條件確認都留言之後才加上
- `docs/workflow.md`、`docs/claude-code.md` 的「完成目標」移除「刪除 dev 分支」的步驟
- GitHub 上的合併選項由使用者手動調整
