# 0008. 優先開發 planner 與 client，map 與 server 暫停

- 狀態：已採用
- 日期：2026-10-24

## 背景

四個 app 無法同時開發，需要決定優先順序。

## 決策

- 優先開發 **planner** 與 **client**
- **map** 與 **server** 暫停，只保留 README 與預定技術；待單人的進度規劃可以使用後再恢復

## 理由

- planner 的進度追蹤只需要 client，不依賴 map 與 server，可以最早交付單人可用的功能
- 進度追蹤依賴 log 的驗證結果，越早驗證風險越低

## 影響

- 除非 issue 明確要求，不修改 `apps/map`、`apps/server`
- 截圖定位功能（需要 map）一併延後
