# 游戏逻辑 Bug 扫描清单（2026-10-08）

> 扫描范围：`c:/Work/SandBox_Card/Assets/GameCode`（出牌结算、修正器、回合阶段、意图 AI、牌堆）
> 说明：扫描期间 `SkillApplyPhase.cs` 被并发编辑（第 32 行补上了 `this.costUsed = costUsed;`，此前缺失会导致 X 费卡段数算成 0），下方结论以最终状态为准。
> 位置书写约定：完整绝对路径用反引号包裹，行号写在反引号外。

---

## 一、明确功能性错误

### 1. 阶段开始的死亡检查流程被丢弃，动画/死亡技能全不播放
- 位置：`c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameBattle/DeckBuildBattle/BattlePhase/OnPhaseStart.cs` 第 19 行
- 现象：只调用了 `battleEndCheck.Start()`，却没把 `battleEndCheck.flow` 接入父 `flow`（既无 `Join` 也无 `Append`）。`BattleEndCheckFlow.FlowBuild` 里组装的死亡动画、`DeathSkillPhase`、递归的下一轮检查序列都不会播放；只有死亡判定逻辑因同步执行而生效，单位会「无动画地死掉」。
- 建议：改为 `flow.Join(battleEndCheck.flow);`。

### 2. 阶段开始的技能收集（PhaseSkillCollect）从未触发
- 位置：`c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameBattle/DeckBuildBattle/BattlePhase/OnPhaseStart.cs` 第 5 行、第 15 行
- 现象：`skillPhase` 只是 `new()`，从未 `.Start(...)`，因此 `PhaseSkillCollect.ModifyAndApply` 从不执行，`phaseSkills` 恒为空，「阶段开始/结束结算技能」功能整体失效。
- 对照：`c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameBattle/DeckBuildBattle/Prefab/BattleCard/interface/DeathSkillPhase.cs` 第 15 行是有 `skillPhase.Start(this, true)` 的。

### 3. 「禁用卡牌」标志永远不会为 true
- 位置：`c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameBattle/DeckBuildBattle/Prefab/BattleCard/BattleCardModify_Useable.cs` 第 19 行、第 24 行
- 现象：两个分支都把 `current.useBlock` 置为 `false`。而 `useBlock == true` 才是「被禁用」（见 `c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameBattle/DeckBuildBattle/Prefab/BattleCard/interface/BattleCardInteractive.cs` 第 42 行：弹 `useBlockDesc` 并拒绝出牌）。因此靠修饰器/状态禁卡的机制完全不可达。
- 建议：按语义这两处疑似应为 `= true`（并补 `useBlockDesc`），但需求意图不完全明确，建议确认后再改。

### 4. GameTag 体系断链，LifeSteal 永不生效
- 位置：`c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameSerialize/CardInfo/GameTag.cs` 第 7 行；`c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameModify/LifeSteal.cs`
- 现象：`GameTag.Settle(GameModify)` 在整个 `Assets/GameCode` 内没有任何调用点（`Modifier.Settle` 调的是 `ModifierEffect.Settle`，不是 `GameTag`）。因此 `LifeSteal.Apply` 永远不会被调用，吸血等 tag 效果是死代码。

### 5. 意图条件系统是空实现
- 位置：`c:/Work/SandBox_Card/Assets/GameCode/UnitIntention/Info/Intention_ConditionGroup.cs` 第 12 行
- 现象：`Result()` 直接 `return true;`，完全不读 `rules`。于是 `c:/Work/SandBox_Card/Assets/GameCode/UnitIntention/Base/UnitIntention_SkillGroup.cs` 第 44 行的条件筛选恒通过，`Intention_Condition.Check()` 也没有任何调用点——意图的「筛选」那一半是无效的。

### 6. 修正器效果体系几乎为空（只有 1 个实现）
- 位置：`c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameModify/Object/DamageModifyEffect.cs`
- 现象：全项目 `ModifierEffect` 的具体实现只有 `DamageModifyEffect`（`ModifierEffect<ActiveModify_Damage>`）一个。`PassiveModify_Damage` / `ActiveModify_Heal` / `PassiveModify_Heal` 以及费用/频率/范围/意图相关修正上下文，都没有任何 effect 去写 `valueModifi`。结果是除「主动伤害 ±layer」外的 buff/debuff 行为全部无实现。需确认是否为待做项。

---

## 二、实现缺口 / 潜在错误

### 7. 阵亡单位 UI 从不回收（注释与实现不符）
- 位置：`c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameBattle/DeckBuildBattle/BattleUnitContainer/DefeatFlow.cs` 第 17-24 行
- 现象：注释写「结束后销毁 UI 并从容器回收」，但 `FlowBuild` 只做抖动/淡出/泛红，没有销毁或从 `unitObjDict` 移除；`c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameBattle/DeckBuildBattle/BattleUnitContainer/BattleUnitContainer.cs` 也没有移除单位 UI 的接口。阵亡单位会以透明泛红状态常驻容器与字典。

### 8. 多段结算的「逻辑次数」与「动画次数/目标」不一致
- 位置：`c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameModify/SkillApplyPhase.cs` 第 44-55 行（逻辑）与第 71-76 行（动画）
- 现象：`Start()` 内按 `countResult × 每段重算的 targets` 同步结算，但 `SkillApplyPhaseFlow` 只拿 `skillApplyPhase.targets`（最后一段的结果）重复播 `countResult` 次。当 `range.active == true` 使每段目标集合变化时，动画会打错目标/错次数（当前样例卡 `active == false`，属潜在问题）。

### 9. 意图权重在 Create 时丢失（潜在崩溃）
- 位置：`c:/Work/SandBox_Card/Assets/GameCode/UnitIntention/Base/BattleUnitSkill.cs` 第 13-19 行；`c:/Work/SandBox_Card/Assets/GameCode/UnitIntention/Base/UnitIntention_SkillGroup.cs` 第 11-19 行；`c:/Work/SandBox_Card/Assets/GameCode/UnitIntention/WeightedRandomSelector.cs` 第 27 行
- 现象：两处 `Create()` 都没有复制 `weight`（也没复制 `conditionBox`）。一旦某单位使用 `UnitIntention_SkillGroup`，运行时子意图权重全为 0，`WeightedRandomSelector` 会抛 `ArgumentException("总权重大于0")`。当前 5 个 UnitInfo（`c:/Work/SandBox_Card/Assets/GameRes/Info/UnitInfo/`）直接用 `BattleUnitSkill`，暂未触发。

### 10. 抽牌上限判断用 `==` 而非 `>=`
- 位置：`c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameSerialize/BattleData/DeckBuildBattleDeck.cs` 第 60 行
- 现象：`if (handPile.Count() + i == DeckBuildBattleRule.Get().cardLimit) break;`。若手牌已达/超过上限（如上限被下调），等号永不成立，会继续抽牌超出上限。建议改为 `>=`。

### 11. 平局会同时弹「失败」和「胜利」
- 位置：`c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameBattle/DeckBuildBattle/Prefab/BattleCard/interface/WinnerCheckPhase.cs` 第 37-53 行
- 现象：`isMateDefeat || isEnemyDefeat` 时两个分支各自注册消息，双方同时全灭时会先后弹 Lose、Win。同处第 41 行 `new BattlePhaseEnd().Start()` 与后续 `PhaseAdvanceFlow` 的 `phase.Start()` 可能对同一 `BattlePhaseEnd` 实例重复启动 flow。

### 12. 治疗数字与实际回血不符
- 位置：`c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameModify/HealFlow.cs` 第 20 行、第 25-28 行；`c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameBattle/DeckBuildBattle/Prefab/BattleCard/interface/Heal.cs` 第 28-38 行
- 现象：HP 已在 `Heal.Apply` 按 `maxHP` 截断，但 `TakeHealAnim(heal.value, ...)` 弹的是未截断的原始值（溢疗时数字偏大）。第 25-28 行 `HealCalculate` 是空方法且无调用点，属残留死代码。

### 13. 出牌结算的死字段 / 被丢弃的 flow 实例
- 位置：`c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameModify/BattleCardUsePhase.cs` 第 39 行；`c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameModify/SkillApplyPhase.cs` 第 8 行、第 14 行、第 15 行、第 53 行
- 现象：`BattleCardUsePhase.Start` 第 39 行 `var skillPhaseFlow = new SkillApplyPhaseFlow().Start(skillPhase);` 创建后被丢弃（真正播放的实例在 `BattleCardUsePhaseFlow.FlowBuild` 里另建）；`SkillApplyPhase.gameFlows`（写入不读）、`skillPhases`、`skillApplyPhaseFlow` 均未被使用。

---

## 三、死代码 / 笔误（低危）

- `c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameSerialize/BattleData/BattleUnitDict.cs` 第 63-64 行：`GetHash(CampEnum.All)` 把 `campDict[CampEnum.Mate]` 加了两遍（应为 Mate + Enemy）。HashSet 去重，功能无影响，但显然是复制粘贴笔误。
- 护盾系统未实现：`c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameBattle/DeckBuildBattle/Prefab/BattleUnit/BattleUnit.cs` 第 48-55 行 `TakeShieldDmg` 无调用点且语义可疑（「受伤」却 `+=`），`c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameBattle/DeckBuildBattle/Prefab/BattleUnit/BattleUnitShieldUI.cs` 主体被注释。
- `c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameBattle/DeckBuildBattle/Prefab/BattleUnit/BattleUnitCurrent.cs` 第 8-11 行：`nearDeath` 只读不写（恒 false）、`target` 未被使用 → 濒死机制未接。
- `c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameModify/Object/IModifierSettle.cs`：接口无任何实现者。
- `c:/Work/SandBox_Card/Assets/GameCode/UnitIntention/Base/BattleUnitSkill.cs` 第 27 行：`var rangeResult = range.GetTargetList(caster);` 结果未使用。
- `c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameBattle/DeckBuildBattle/Prefab/BattleUnit/BattleUnit.cs` 第 21 行：`BattleUnit.Create` 直接把 SO 配置里的 `Skill` 实例赋给运行时 `skill`（`BattleUnitSkill.GetResult()` 返回 `this`），存在运行时修正写回污染配置的风险；被 `RefreshUnitIntentions` 的 `Create()` 克隆覆盖后掩盖，属隐患。

---

## 四、已验证「不是 bug」的点（避免误改）

- DOTween 的 `flow.onComplete` 是公有字段，`+=` 可正常累加，多处注册回调没有问题。
- `c:/Work/SandBox_Card/Assets/GameCode/UnitIntention/WeightedRandomSelector.cs` 的 `SelectIndex` 二分边界逻辑正确。
- `c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameBattle/DeckBuildBattle/Prefab/BattleCard/interface/RangeRandom.cs` 的部分洗牌（Fisher-Yates）正确。
