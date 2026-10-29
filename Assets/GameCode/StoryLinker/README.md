# StoryLinker — 剧情节点编辑器

基于 Unity GraphView + Odin Inspector 的对话树编辑器，输出为 `StoryInfo.asset`。

## 入口

菜单 → 我的工具 → StoryLinker.Test

## 节点类型

| 节点 | 功能 |
|------|------|
| Start | 剧情入口，配置标题/背景/BGM |
| Dialogue | 对白内容，配置说话人和文本 |
| Option | 玩家选项分支 |
| End | 剧情终点 |
| Note | 策划备注，不参与运行时逻辑（不导出） |

## 工具栏

新建、保存、加载、框选、复位视图、打开资源列表。

## 输出

保存为 `StoryInfo.asset`（ScriptableObject），运行时直接读取。

## 程序集

| 程序集 | 平台 | 说明 |
|--------|------|------|
| GameModule.StoryLinker.Runtime | Runtime | 运行时数据结构 |
| GameModule.StoryLinker.Editor | Editor | GraphView 编辑器窗口和节点 |

## 依赖

- Odin Inspector
- GameBase.CardSandBox
