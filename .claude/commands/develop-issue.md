---
description: 依照 GitHub 子 issue 開發，由 implementer、tester、reviewer 三個角色完成，開 PR 到目標的 dev 分支並更新狀態標籤
argument-hint: <子 issue 編號>
---

依照 issue #$ARGUMENTS 開發。全程遵守 `CLAUDE.md`，狀態與分支規則見 `docs/workflow.md` 第 2、6 節。

以下指令中的 `<repo>` 一律是 `17-Tarkov-WYA/tarkov-wya`。

## 1. 確認可以接手

1. 讀取子 issue：`gh issue view $ARGUMENTS --repo <repo> --comments`
2. 讀取「上層目標」的目標 issue：`gh issue view <目標編號> --repo <repo>`，取得「**開發分支**」欄位的值（以下稱 `<dev 分支>`，例如 `dev/1-skeleton`）
3. 確認 dev 分支存在：`git fetch origin`，再用 `git branch -r --list origin/<dev 分支>` 確認有結果
4. 逐項確認，**任何一項不符合就停下來**：
    - 子 issue 是開啟中的，且不是目標 issue（沒有 `type:goal`）
    - 帶有 `status:ready` 或 `status:changes-requested`
    - 有「上層目標」，目標 issue 有「開發分支」，且該分支存在於 `origin`
    - 「依賴」欄位列出的每個 issue 都已關閉：`gh issue view <依賴編號> --repo <repo> --json state`
5. 不符合時：
    - 狀態標籤不對、是目標 issue：只告訴使用者原因，不修改 issue
    - 沒有上層目標、dev 分支不存在、依賴未關閉：加上 `status:blocked`、移除原本的狀態標籤，並在子 issue 留言說明原因，然後停止

## 2. 了解任務

1. 從目標 issue 了解這個子 issue 在整體中的角色（**只做子 issue 的範圍**）
2. 讀取 `CLAUDE.md`「開始前必讀」中，對應這個 issue 類型的所有檔案
3. 讀取 issue「相關文件」列出的每一個檔案
4. 完成條件、不做的範圍或驗證方式不清楚或互相矛盾時，**先停下來詢問使用者**

## 3. 決定分支

- **`status:ready`（第一次開發）**：依命名規則從最新的 `origin/<dev 分支>` 開新分支

    ```
    git switch -c <類型>/$ARGUMENTS-<簡短英文說明> origin/<dev 分支>
    ```

- **`status:changes-requested`（退回修改）**：找出子 issue 留言中最新一則「🤖 開始開發（Claude）」記錄的分支，沿用它，**不開新分支**

    ```
    git switch <子 issue 分支>
    git pull
    ```

    本機沒有這個分支時：`git switch -c <子 issue 分支> origin/<子 issue 分支>`

    讀取這個分支的 PR 與使用者的修改意見：`gh pr view <子 issue 分支> --repo <repo> --comments`。逐條列出要修改的項目；意見不清楚時先詢問使用者

- 不從 `main` 或其他子 issue 的分支開出；不自行 merge 或 rebase

## 4. 標示開始

1. 留言到子 issue（內容存成暫存檔，不要放在 repo 中）：

    ```markdown
    🤖 開始開發（Claude）

    - 分支：`<子 issue 分支>`
    - 基準：`origin/<dev 分支>` @ <commit hash>（沿用分支時寫「沿用既有分支」）
    - PR 目標：`<dev 分支>`
    - 上層目標：#<目標編號>
    ```

    commit hash 與 issue 編號不加反引號，GitHub 才會自動連結（見 `CLAUDE.md`「回報與 PR」）

    `gh issue comment $ARGUMENTS --repo <repo> --body-file <暫存檔>`

2. 更新狀態：`gh issue edit $ARGUMENTS --repo <repo> --add-label status:in-progress --remove-label <原本的 status 標籤>`

## 5. 開發與審查（agent team）

依序交給三個角色，每一步都把 issue 編號、上層目標、dev 分支、完成條件與相關文件交代清楚。第一次開發、退回修改、CI 失敗後的修正，都是同一個流程（`docs/decisions/0012-traceable-development.md`「先審查、再 commit」）：

implementer → tester（驗證工作目錄整體變更）→ 提出 commit 規劃 → reviewer（審查工作目錄變更與 commit 規劃）→ 依審查意見修正 → 第 6 步依規劃建立 commit → 第 6 步逐一驗證每個 commit → 第 6 步 push

- 審查前不得先為預計提交的變更建立 commit；審查通過後，才執行 `git commit`

1. **implementer**：實作（退回修改時，只依 PR 上的修改意見修改）；完成時回報變更內容與涉及的檔案、檔案間的依賴關係，以及 commit 分組的建議與理由
2. **tester**：依完成條件與驗證方式實際執行並記錄結果
3. **提出 commit 規劃**（你）：參考 implementer 的建議，依實際變更決定分組（規則見 `CLAUDE.md`「分支與 commit」）。每個預計建立的 commit 列出：目的／標題、包含的檔案、分組理由、「為什麼」的來源、擬好的 commit 訊息
    - 決定本輪的**基準**（每一次 push 就是一輪）：分支還沒 push 過時，是開分支時 dev 分支的 commit（「🤖 開始開發」留言中的基準）；之後的每一次 push（包含退回修改、第 7 步 2 CI 失敗後的修正），一律是 push 之前的 `origin/<子 issue 分支>`（`git log -1 --format=%h origin/<子 issue 分支>`）
    - 以檔案為最小分組單位，同一個檔案不分配給不同 commit；考慮檔案間的依賴關係
4. **reviewer**：告訴它 dev 分支、子 issue 分支與本輪基準，並交給它 commit 規劃
    - 工作目錄的變更：`git diff <基準>`，加上 `git status` 列出的未追蹤新檔案
    - 整體差異：`git diff origin/<dev 分支>`
    - commit 規劃：評估針對性、分組合理性、聚合與拆分、來源完整性、可執行性、訊息完整性（見 `.claude/agents/reviewer.md`）
    - 不使用 `git diff origin/<dev 分支>...HEAD`（審查時還沒有 commit，結果會是空的）

reviewer 提出意見時，交回 implementer 依審查意見修正：

- 審查後的修正直接併入工作目錄中相關的變更，並同步更新 commit 規劃；不另外為審查意見建立修正 commit
- 有 M 時：先留下審查結果紀錄（已有 PR 留言到 PR，尚無 PR 留言到子 issue），保留 reviewer 發現的問題、依據與 M／S 編號；修正後再次經過 tester 與 reviewer，直到沒有 M。修正併入的 commit，「為什麼」引用審查留言的連結與編號
- 沒有 M 的審查結果，摘要寫進開發回報或修改回報
- reviewer、tester 是唯讀的，留言一律由你發布；留言標題及格式依當次情境決定，子 issue 上的格式與 PR 上保持一致（`CLAUDE.md`「Agent team」的「審查與驗證結果的紀錄」）

## 6. commit、逐一驗證與 push

規則見 `CLAUDE.md`「分支與 commit」與 `docs/decisions/0012-traceable-development.md`。

1. **依規劃分批 commit**（審查通過後）：每個 commit 的訊息先存成暫存檔，標題結尾引用 `(#$ARGUMENTS)`，**內文必填**「改了什麼」「為什麼」，並加一行「順序：本輪第 k 個，基準 <hash>」

    ```
    git add <這個 commit 的檔案>
    git commit -F <暫存檔>
    ```

    不用 `git add -A` 或 `git add .`
2. 確認：`git status` 沒有遺漏的檔案；`git log --format="%h %s" origin/<dev 分支>..HEAD` 的順序與規劃相同
3. **逐一驗證每個 commit**（tester）：tester 用 `git switch --detach <hash>` 逐一切到本輪的每個 commit，執行格式檢查與受影響範圍的建置、測試，記錄每個 commit 的結果，完成後 `git switch <子 issue 分支>` 切回。有 commit 沒通過時：
    1. 留言記錄 tester 的驗證結果及失敗依據（已有 PR 留言到 PR，尚無 PR 留言到子 issue）
    2. 交回 implementer 在工作目錄進行修正，並擬定修正 commit 規劃（修正 commit 的「為什麼」引用 tester 驗證結果的留言連結與編號，並保留其他必要的可查證來源）
    3. reviewer 只針對該修正內容與修正 commit 規劃進行增量審查（看 `git diff HEAD`，加上 `git status` 列出的未追蹤新檔案；若有 M，依第 5 步留言並修正）
    4. 審查通過後，新增修正 commit，不修改原本已建立的 commit
    5. tester 驗證新增的修正 commit，並確認最後一個 commit 通過；已標示失敗並由後續 commit 修正的歷史 commit，不阻擋通過
    - 「## Commits」中，原本驗證失敗的 commit（以及它之後、修正之前因同一個原因失敗的 commit）標示「❌（由 <修正 commit> 修正）」
    - 修正 commit 也遵守「每個 commit 都要有針對性」，不恢復成「一條意見一個 commit」
4. 把 commit 規劃寫進開發回報的「## Commits」（退回修改時寫進修改回報的「## Commits」）：範圍（基準 → 第一個 → 最後一個，附 compare 連結）、每個 commit 的順序、hash、標題、為什麼、單獨驗證的結果（第 6 步 3），以及排序理由（格式見 `CLAUDE.md`「回報與 PR」）
5. 完成驗證後，push 子 issue 分支：`git push -u origin <子 issue 分支>`
- 已建立的 commit **不**改寫（不 `git commit --amend`、不 `git rebase`、不 force push）
- **不** push 到 `main` 或任何 `dev/*` 分支、**不** merge、**不**關閉 issue

## 7. 開 PR 與回報

依 `CLAUDE.md` 的「回報與 PR」撰寫，內容都存成暫存檔。**依序**進行，前一步沒完成不進行下一步：

1. **開 PR 或回報修改**

    - **第一次完成**：開 PR **到 dev 分支**，內容為「開發回報」（第一行 `Closes #$ARGUMENTS`，含「## Commits」；「## 驗證」的 CI 一列寫「見 PR 留言『🤖 CI 結果』」，不寫「之後另外留言」）

        ```
        gh pr create --repo <repo> --base <dev 分支> --head <子 issue 分支> --title "<commit 格式的標題>" --body-file <暫存檔>
        ```

        開完後用 `gh pr view <PR 編號> --repo <repo> --json baseRefName` 確認目標分支是 `<dev 分支>`，**不是 `main`**

    - **退回修改後完成**：push 後 PR 會自動更新；在 PR 留言「修改回報」

        ```
        gh pr comment <PR 編號> --repo <repo> --body-file <暫存檔>
        ```

2. **等 CI 結束**

    ```
    gh pr checks <PR 編號> --repo <repo> --watch
    ```

    - 剛開 PR 或剛 push 時，檢查可能還沒出現（顯示沒有檢查）：稍等後再執行一次
    - **失敗**：在 PR 留言「🤖 CI 結果（Claude）」，寫出失敗原因與修正方式；修正依第 5、6 步進行（先審查、再 commit；這是新的一輪）並 push，再回到這一步重新等待
    - **無法修正**（需要使用者決定、資訊不足）：在 PR 留言「🤖 CI 結果（Claude）」說明原因，在子 issue 留言並改為 `status:blocked`，然後停止。子 issue 的留言要註明「已有 PR，處理後請改為 `status:changes-requested`」（沿用既有分支與 PR，不要改回 `status:ready`）
    - **全部通過**才進行下一步

3. **留言 CI 結果**：在 PR 留言「🤖 CI 結果（Claude）」，列出每項檢查與結果（格式見 `CLAUDE.md`）

    ```
    gh pr comment <PR 編號> --repo <repo> --body-file <暫存檔>
    ```

4. **留言完成條件確認**：在子 issue 留言「🤖 完成條件確認（Claude）」（格式見 `CLAUDE.md`）

    ```
    gh issue comment $ARGUMENTS --repo <repo> --body-file <暫存檔>
    ```

    - 逐條**原文引用**子 issue「完成條件」的每一項，順序與原文相同，不可省略、改寫或合併；標示 ✅ 已完成／❌ 未完成／👤 需要使用者確認，並寫出如何確認
    - 退回修改後完成時，**另外再留一則新的**，不編輯舊留言
    - **不修改 issue 內容**，不修改目標 issue，也不在目標 issue 留言

5. **更新 issue 狀態**（CI 結果與完成條件確認都留言之後）：`gh issue edit $ARGUMENTS --repo <repo> --add-label status:review --remove-label status:in-progress`

其他：

- **無法完成**（需要使用者決定、資訊不足）：不開 PR；在子 issue 留言說明原因，改為 `status:blocked`
- **絕對不合併、關閉或核准 PR**
- 在對話中顯示同樣的回報，並告訴使用者 PR 連結、目標分支、CI 結果與目前的狀態標籤
