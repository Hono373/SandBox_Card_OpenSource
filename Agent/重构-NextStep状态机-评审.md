# 下一版重构评审：BattlePhase 链 → NextStep 状态机

> **状态：已封存（2026-10-27）**
>
> 封存原因：优先级转向。架构优化不产生玩家可感知的进度，项目连续 18 个月在重构与内容之间反复，
> 真正的瓶颈是「没有做成一个完整可玩的独立作品」，不是「阶段推进不够优雅」。
>
> **解封条件（满足任一再拿出来）**
> - 要做玩家存档 / 断点续玩 —— 存档是产品需求，不是架构洁癖，本方案是目前唯一干净的实现路径
> - 出现第 3 处绕过链表硬改 `phase.current` 的地方（已有 2 处：`WinnerCheckPhase.cs` 第 40 行、`PressPlayerRoundEnd.cs` 第 7 行）
> - 内容量封顶、进入打磨期
>
> **封存期间明确不做**：Pause/Append/Join 收口、四钩子收敛、终盘判定 dirty 标记。
> 这三项都只影响代码形态，不影响玩家能不能玩到结局。

> 对象：`下个版本` 流程图（NextStep / StepCenter / Pause-Append-Join / 四钩子）
> 定位：**这是重构，不是新写。** 现有代码里已经存在 90% 的零件，缺的是统一的调度语义。
> 核对日期：2026-10-27。当前 `Assets/GameCode` 中 `NextStep`、`StepCenter` 均不存在。

---

## 一、结论

图上那套设计要替换的，是现有三处**已经能跑但不可存档、不可穷尽检查**的机制：

| 现状机制 | 现在怎么做 | 为什么必须换 |
|---|---|---|
| 阶段推进 | `flow.onComplete += () => phaseResult.Start()` 回调递归 | 推进位置活在**调用栈和闭包**里，栈不可序列化、不累积计数就查不出「我在第几层」 |
| 下一个是谁 | `NextPhase` 抽象属性，`=> new XxxPhase()` 每次新建实例 | 是**工厂方法**不是状态值，没有「当前是哪一步」的可读表示 |
| 当前状态判断 | `phase.current is not PlayerPhase` 类型判断散落各处 | 加一个阶段就要全局搜 `is Xxx`，编译器不帮你查漏 |

换成 `NextStep` 之后：推进位置变成**一个可序列化的值**，状态判断变成 **switch 可穷尽**，递归变循环。这是**降风险**，不是加工风险。

---

## 二、现状（代码实测，不是推测）

### 2.1 已经有「当前阶段」容器，且已经进了存档

`C:/Work/SandBox_Card/Assets/GameCode/GameManager/GameSerialize/BattleData/DeckBuildBattlePhase.cs` 第 10 行

```csharp
[Serializable]
public class DeckBuildBattlePhase
{
    [SerializeReference] public BattlePhase current;
```

注意它放在 `GameSerialize/BattleData/` 目录下——**阶段容器本来就是存档结构的一部分**。存档里存的是 `BattlePhase` 的**多态对象实例**，不是一个阶段标识。

### 2.2 阶段链表是硬编码 + 每次 new

`C:/Work/SandBox_Card/Assets/GameCode/GameManager/GameBattle/DeckBuildBattle/BattlePhase/BattlePhase.cs` 第 8 行

```csharp
public abstract class BattlePhase : RootBattleFlow
{
  public abstract BattlePhase NextPhase { get; }
  public abstract BattlePhase Start();
}
```

`C:/Work/SandBox_Card/Assets/GameCode/GameManager/GameBattle/DeckBuildBattle/BattlePhase/PlayerPhase.cs` 第 7 行

```csharp
public override BattlePhase NextPhase => new PlayerPhaseEnd();
```

`current` 里已经有一个 `PlayerPhase` 实例，`NextPhase` 又 `new` 一个 —— 两个对象只用来传一个类型信息。

### 2.3 推进靠 onComplete 回调递归

`C:/Work/SandBox_Card/Assets/GameCode/GameManager/GameBattle/DeckBuildBattle/BattlePhase/NextMoveDecideFlow.cs` 第 20 行

```csharp
flow.onComplete += () => phaseResult.Start();
```

`Start()` → `Init()` → `FlowBuild()` 建序列 → 播完 `onComplete` → 又 `Start()`。**回合数越多，这条链在逻辑上就一直往下挂**，虽然 DOTween 回调是扁平的，但「谁接着谁」这个信息完全在闭包里，序列化时抓不到。

### 2.4 已经有人在手动改 current，且是绕过链表的

`C:/Work/SandBox_Card/Assets/GameCode/GameManager/GameBattle/DeckBuildBattle/Prefab/BattleCard/interface/WinnerCheckPhase.cs` 第 40 行

```csharp
DeckBuildBattle.Active.phase.current = new BattlePhaseEnd().Start();
```

`C:/Work/SandBox_Card/Assets/GameCode/GameManager/GameBattle/DeckBuildBattle/BattlePhase/PressPlayerRoundEnd.cs` 第 7 行

```csharp
DeckBuildBattle.Active.phase.current = nextPhase;
```

**有两条并行的推进路径**：一条走 `NextMoveDecideFlow` 的回调链，一条直接改 `current`。这正是图上 `#修正器` 要接管「先手修正 / 下回合修正」的原因——这两处硬改就是在补链表不够用的洞。

### 2.5 状态判断靠类型

`C:/Work/SandBox_Card/Assets/GameCode/GameManager/GameBattle/DeckBuildBattle/Prefab/BattleCard/interface/BattleCardInteractive.cs` 第 55 行、第 93 行

```csharp
if (DeckBuildBattle.Active.phase.current is not PlayerPhase) return false;
```

---

## 三、图上的设计对应到现有代码：三处替换

| 图上 | 替换掉什么 | 迁移动作 |
|---|---|---|
| `NextStep` 状态位 | `[SerializeReference] BattlePhase current` + `NextPhase` 抽象属性 | 阶段类保留（它们承载 FlowBuild 逻辑），但**不再互相引用**；新增 `NextStep` 枚举，`DeckBuildBattlePhase` 只存 `public NextStep step;` |
| `StepCenter 循环执行NextStep` | `onComplete += () => phaseResult.Start()` | 改成 `while(!isGameOver) Dispatch(step)`，`Dispatch` 内 switch 派发到阶段的 `Start()`，阶段不再自己决定下一步 |
| `写入NextStep` | `phase.current = new XxxPhase()` 硬改 | 统一成唯一出口，禁止外部直接赋值（把 `current` 改成 private set） |

**要注意**：阶段类（`PlayerPhase` 等）不要删。它们现在做的事（`FlowBuild` 建序列、`DeckBuildBattle.Active.phase.current = this`）正好对应图上的「阶段处理函数」。删了要重写 40 个 Flow 的挂载方式，白干。

---

## 四、迁移路径（四步，每步可独立验收）

### 第一步：加 NextStep 枚举，不改行为（纯增量，随时可回滚）

1. 新增 `NextStep` 枚举，成员照图：`BattleStart, PlayerPhaseStart, PlayerPhase, PlayerPhaseEnd, EnemyPhaseStart, EnemyPhase, EnemyPhaseEnd, NextPhaseConfirm, GameOver`
2. `DeckBuildBattlePhase` 增加 `public NextStep step;`，**保留 `current` 不动**
3. 各阶段的 `Start()` 里，第一行 `DeckBuildBattle.Active.phase.step = NextStep.Xxx`
4. 在 `NextMoveDecideFlow` 回调和两处硬改那里，**同时**写 `step`

验收：跑一局，在 `写入NextStep` 处打日志，确认每个阶段的 `step` 值和 `current` 的类型一一对应、次数一致。**这一步不改任何逻辑，跑不通就是埋点位置找错了。**

### 第二步：StepCenter 接管派发

1. 新增 `StepCenter`，`while(!isGameOver)` 读 `step`，switch 派发
2. 各阶段的 `NextPhase` 抽象属性删掉，`NextMoveDecideFlow` 的回调递归改成「算完写 step 就返回」
3. `#修正器` 接管 `PressPlayerRoundEnd` / `WinnerCheckPhase` 的硬改

验收：同样的对局，日志里阶段序列和第一步完全一致。**序列不一致就是语义变了，不要往下走。**

### 第三步：存档只存 step

1. `DeckBuildBattlePhase.current` 加 `[JsonIgnore]`
2. 读档改为「读 step → 由 StepCenter 重建对应阶段实例」
3. 老存档兼容：`step` 为默认值时按 `#玩家阶段` 处理（对应图上 Tips「老存档只有 1 种情况」）

验收：进阶段自动存档（`PlayerPhase.Start()` 里已有 `GameSerialize.Save()`）→ 退出 → 读档 → 能接着打。**这一步跑通，重构的核心价值就拿到了，后面全是优化。**

### 第四步：Pause/Append/Join 收口（可以最后做，也可以不做）

现有 `GameAnim.Flow()` + `GameAnimFlow.Enqueue()` 已经能做串行队列，缺的是「建槽后由别人填充」。这块如果只为存档服务，**优先级最低**——它不影响正确性，只影响表现层的编排手感。

---

## 五、动手前必须定死的六件事

| # | 待定项 | 不定会怎样 |
|---|---|---|
| 1 | `NextStep` 的语义：**当前所处的步**，不是下一步。`StepCenter` 读它、执行它、处理函数末尾改写它 | 两种语义混用 → 读档后重复执行一步或跳一步 |
| 2 | `#玩家阶段` 的循环回边：出牌/用道具/结束回合之后，step 写回 `PlayerPhase` 还是别的 | 图上是缺口。不写死，玩家出牌中途存档恢复不回去 |
| 3 | 存档时机：现有 `PlayerPhase.Start()` 进阶段就 `GameSerialize.Save()`，而 Sequence 还要播 2 秒 | 这 2 秒内存档 = 逻辑已到下阶段、画面还停在上一阶段，读档动画跳变。要么禁止非 idle 存档，要么接受跳变 |
| 4 | `#玩家阶段开始` 没有「推送Sequence到QueuePlayer」，`#敌人阶段开始` 有 | 疑似漏了。核对是不是被 Join 到 `#Battle启动` 的序列里 |
| 5 | `#终盘判定事件` 出现 8+ 次（阶段开始/结束、出牌前后、每个单位行动前后） | 每次全量遍历单位 + 建 Pause，大部分时候没人死，纯空转。加 dirty 标记 |
| 6 | 修正器与事件的分工：`#阶段开始事件` 第一步就是 `#修正器 XX阶段开始时` | 两层职责重叠。定死：**修正器只改数值和拦截，事件只收集和触发** |

---

## 六、这套重构值多少

**工程上**：把「我在哪一步」从调用栈搬进字段，代价是一个枚举，换来的是可存档、可断点、可穷尽检查。这是回合制游戏该有的形态。

**面试上**：比 500 个类有用。因为它是**被痛点逼出来的重构**，而不是学来的架构：

- 上一版的问题：`AnimProcessManager` 队列构建时捕获的状态 ≠ 播放时的状态
- 这一版的手段：状态由 `NextStep` 显式推进，队列只负责「这一步的演出」
- 可验证的证据：`NextMoveDecideFlow.cs` 第 20 行那行闭包递归 → 迁移后消失

讲的时候不要讲「我设计了状态机」，讲「我发现推进位置存在闭包里，所以存档做不了，然后把它挪进字段」。

---

## 附：图上 `#终盘判定事件` 里值得单独讲的细节

```
循环 退场单位清单.Count>0
  遍历 退场单位清单 → 设置退场 / 归档信息 / #阶段开始事件 退场时 / Append 退场动画
  清空退场单位清单
  遍历 所有单位 → #修正器 死亡修正（例如 抵抗死亡）→ hp0 且未退场 加入清单
```

先清空再重扫，处理「一个单位死亡触发另一个单位死亡」的链式反应。这个细节是设计深度的证据，比任何架构名词都有说服力。
