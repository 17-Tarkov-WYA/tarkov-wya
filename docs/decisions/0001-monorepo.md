# 0001. 採用單一 monorepo

- 狀態：已採用
- 日期：2026-10-24

## 背景

舊版把 client、map、server 分成三個 repo。修改事件名稱或資料格式時要三個 repo 一起改，協定容易不同步。新架構的各個 app 之間共用協定與遊戲資料，關係更緊密。

## 決策

所有程式放在單一 repo `17-Tarkov-WYA/tarkov-wya`：

- `apps/`：planner、client、map、server，各自獨立、各有 README
- `packages/`：protocol（事件與資料格式）、game-data（遊戲資料）
- `docs/`：架構、規劃、流程與決策紀錄

## 理由

- **意義分開**：每個 app 有自己的資料夾、README 與用途，可以單獨使用
- **維護方便**：協定與遊戲資料只維護一份；一次變更、一套 CI、一個版本號
- 考慮過維持多個 repo，但需要跨 repo 同步版本與發佈流程，維護成本高

## 影響

- 跨 app 的變更在同一個分支、同一個 PR 中完成
- 舊的 `client-python`、`map`、`server` repo 不再開發
