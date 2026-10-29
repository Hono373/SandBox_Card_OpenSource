# CardSandBox

Unity 2022.3 回合制 DBG（Deck-Building Game）卡牌游戏的**代码快照**。

单人开发约 18 个月，500+ 个自有类。这个仓库只保留代码与文档，用来说明战斗系统、修正器引擎与动画编排的架构设计。

---

## 这个仓库是什么

- 完整的游戏逻辑与编辑器工具链源码（`Assets/GameCode`，530 个 C# 文件）
- 战斗、修正器、意图、剧情、存档、资源配置等子系统的实现
- 架构文档：`Doc/`（数据链路、技术说明）与 `Agent/`（架构总结、代码规范、问题清单）

## 这个仓库不是什么

**不是开箱即玩的完整游戏。** 为了干净地开源，以下内容被刻意移除：

| 移除项 | 原因 | 影响 |
|---|---|---|
| 美术资源（贴图 / Spine 骨骼 / 预制体 / 场景） | 素材版权 | 无法在编辑器里直接运行 |
| Odin Inspector | 商业插件，不可分发 | 由 `Assets/Plugins/OdinShim` 提供同名空特性，编译正常，Inspector 失去分组与按钮美化 |
| DOTween Pro | 商业插件，不可分发 | 项目实际只用了 DOTween 免费版 API，移除后无功能缺失 |

代码本身**可以完整编译**（四个程序集均为 0 错误），这是本快照的验收标准。

---

## 环境

| 项 | 值 |
|---|---|
| Unity | 2022.3.62f2c1 |
| 渲染 | URP 14 |
| 序列化 | Newtonsoft.Json（`TypeNameHandling.Auto` 多态还原） |
| 资源管理 | YooAsset 2.1.2 |
| 补间 | DOTween（免费版，随仓库分发） |
| 骨骼动画 | Spine Runtimes（随仓库分发，编辑 `.skel` 需自备 Spine 授权） |

打开方式：用上面的 Unity 版本打开本目录即可。`Packages/manifest.json` 会自动拉取依赖。

---

## 架构总览

整个战斗系统被抽象成一条数据流加一条表现流，两流分离：

```
获取数据  →  修正数据  →  消费数据  →  动画编排
(Create)     (Modify)     (Apply)      (Sequence)
```

- **获取**：读配置创建运行时数据载体（`BattleCardCurrent` 等），静态工厂 `Create()`
- **修正**：Modifier 系统对同一份数据反复修正——一份数据、多次修正、一次消费
- **消费**：结算伤害 / 治疗 / 状态，产出"这一下发生了什么"的结果
- **编排**：结算结果转 DOTween Sequence 演出，先算清楚再演出来

数据只描述"是什么"，逻辑负责"怎么做"。多态不是数据驱动的对立面，而是数据驱动内部丰富数据形式的手段（`[SerializeReference]` 多态数据 = 异构序列 / 树）。

### 核心子系统

**修正器引擎（GameModify）** — 全局修正管道，`Modifier` 实例（数据 + 效果列表 + 计数）注册到 `GameModifyCenter`，任意数据节点需要修正时遍历全局 modifier × effect，用 `is` 路由命中即改 context 数据。

**战斗流程（BattlePhase）** — `PlayerRound → EnemyRound → RoundEnd` 三阶段循环，阶段抽象为 Start / End / Modify 三类节点。

**意图系统（UnitIntention）** — 树形意图结构 + 条件筛选 + 加权随机选择，意图数据与 UI 展示分离。

**动画编排（GameSeqFlow）** — 全局 DOTween Sequence 串行播放队列，入队 / 播放 / 完成自动释放。动画原子（`GameAnim`）先构建再统一播放，避免队列构建时刻与播放时刻的状态错位。

**配置表（GameDB）** — 自研 JSONL 配置库，一行一条记录，同 key 后写覆盖，附 `.old` 备份瘦身。编辑器侧 `InfoCreator<T>` 提供刷新 / 写入 / 瘦身工作台。

**剧情编辑器（StoryLinker）** — 基于 Unity GraphView 的节点编辑器，输出 `StoryInfo.asset`（Start / Dialogue / Option / End / Note 节点）。字段编辑依赖 Odin 的 `PropertyTree`，未安装 Odin 时节点仍可显示与连线，展开编辑会提示需要安装。

---

## 目录结构

```
Assets/
├── GameCode/                # 核心代码
│   ├── GameManager/
│   │   ├── GameAsset/       # YooAsset 资源管理封装
│   │   ├── GameExcel/       # JSONL 配置库（GameDB / InfoCreator）
│   │   ├── GameSerialize/   # 存档体系
│   │   ├── GameModify/      # 修正器引擎 + Object/（动画流程、Modifier）
│   │   ├── GameBattle/      # 战斗系统（阶段 / 卡牌 / 单位）
│   │   ├── GameEvent/       # 修正上下文（Phase / Bonus / Snapshot）
│   │   ├── GameSeqFlow/     # 动画原子与全局队列
│   │   ├── GameQuery/       # 战斗数据查询
│   │   └── GameStory/ GameScene/ GameLanguage/ GameCamera/ GameLoading/ GameMenu/
│   ├── UnitIntention/       # 意图系统
│   ├── StoryLinker/         # 剧情节点编辑器（GraphView）
│   ├── UIAnim/ UITool/      # UI 动画与工具
│   ├── Interface/           # 接口层（IManager / IAssetLoad / IBattleUnit）
│   └── Editor/              # 编辑器校验（FlowTweenCreateValidator 等）
├── DataBase/                # 早期 Luban 生成的配置代码（遗留，独立 asmdef）
├── Editor/                  # 编辑器扩展脚本
├── Plugins/
│   ├── OdinShim/            # 未安装 Odin 时的空特性兼容层
│   ├── DOTweenPlus/         # 自研 DOTween 扩展（Graph 位移等）
│   ├── Like.DrawAnim/       # 自研绘制动画
│   ├── Demigiant/           # DOTween 免费版 + DemiLib
│   ├── Spine/               # Spine Runtimes
│   └── luban_unity-main/    # Luban 的 Unity 支持代码（MIT）
Doc/                         # 技术说明、卡表数据链路、无限列表
Agent/                       # 架构总结、代码规范、问题清单、重构评审
```

---

## 第三方组件

| 组件 | 许可 | 说明 |
|---|---|---|
| DOTween + DemiLib | Demigiant 免费版授权 | 见 `THIRD-PARTY-NOTICES.md` |
| Spine Runtimes | Spine Runtimes License | 使用或修改 Spine 资源需自备 Spine 编辑器授权 |
| Luban (Unity 支持部分) | MIT | 仅作历史遗留保留，当前配置流程已改自研 JSONL |
| YooAsset | MIT | 通过 UPM 拉取，不在仓库内 |
| Newtonsoft.Json | MIT | Unity 官方包 `com.unity.nuget.newtonsoft-json` |

Odin Inspector 不在本条列表内——**它没有被分发**。`Assets/Plugins/OdinShim` 是独立编写的空特性兼容层，不包含 Odin 的任何代码。

---

## 关于素材

本仓库不包含任何第三方游戏的素材、代码或数据。玩法机制本身不受版权保护，但卡牌数值与文本均为本项目自行编写。

---

## License

MIT
