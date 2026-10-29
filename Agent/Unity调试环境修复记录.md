# Unity 断点调试环境修复记录

> 环境：Windows 11 + VS Code（vstuc 1.3.1）+ Unity 2022.3.62f2c1（china_unity 分支）+ C# Dev Kit 3.40.204
> 结论：能断点调试了。根因是**四层叠加**，每层都会单独导致"断点未验证"，之前只修一层所以一直看起来没进展。
> 说明：按用户要求，本文只作为 md 记录，不写入 jsonl 知识库（内容跨 IDE / 运行时 / Unity 工程生成 / 调试协议，粒度太杂）。

---

## 一、问题现象（开始）

1. 打好断点、开启调试后，**Unity 编辑器卡在 tick 不动**（无响应）。
2. 随后所有断点在 VS Code 里显示为**"未验证断点"**（灰色空心），而 **Unity Console 里没有任何报错**。
3. 目标：恢复"能命中"的调试能力。

---

## 二、排查经过（每一层假设都验证过，含被证伪的弯路）

### 弯路 1：怀疑游戏代码里有阻塞 / 死循环（证伪）

- 全工程搜 `Thread.Sleep` / `.Wait()` / `.Result` / `lock(` / `Monitor.` / `SpinWait` / `Semaphore` → **0 处**（命中的都是 `string.Join`、`Sequence.Join` 同名干扰）。
- 全工程只有 1 个 `Update`：`c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameCamera/CameraObj.cs` 第 9 行。
- 结论：不是主线程被代码阻塞。**"卡在 tick" 是调试器挂起主线程的正常表现**。

顺带记录的"流程会卡住"风险点（不是本次根因，留作以后排查流程不推进用）：

- `c:/Work/SandBox_Card/Assets/GameCode/UnitIntention/Controller/IntentionModule_Ob.cs` 第 26-32 行 `while (!node.IsEnd)`：CFG 配置成环会真死循环（全项目唯一真正的自旋点）。
- `c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameSeqFlow/GameAnimFlow.cs` 第 25-36 / 38-44 行：`current` 只在 `onComplete` 里清空，任何对它的 `Kill()` 都会让全局动画队列永久阻塞。
- `c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameBattle/DeckBuildBattle/Prefab/BattleCard/interface/BattleEndCheckPhase.cs` 第 31 行自递归，收敛依赖 `UnitDeathCheck` 置 `isDead`。
- `c:/Work/SandBox_Card/Assets/GameCode/GameManager/GameSeqFlow/Object/GameAnim.cs` 第 19/25 行：`GameAnim.Flow()` 造出来的是 **Pause 状态**序列，非 Root 流程只 Join 不 Play 就永远不推进。

### 弯路 2：怀疑编译错误导致程序集陈旧（证伪）

- Unity 日志里**确实有过**：`Assets\GameCode\...\BattleCard\interface\Skill.cs(19,59): error CS0246: 类型或命名空间名 'CountMode_Disable' 无法找到`（当时 4 个文件被清空成 0 字节：`CountMode_Able.cs` / `CountMode_Disable.cs` / `DamageSkill.cs` / `ModifierCreateSkill.cs`），但那是历史状态。
- 时间戳证据：`c:/Work/SandBox_Card/Library/ScriptAssemblies/Assembly-CSharp.dll` 与 `.pdb` 同为 19:37:57，晚于最后改动的源码（`GameModify/DamageFlow.cs` 19:36:56）→ 最后一次编译是**成功**的。
- 结论：不是编译错误。

### 弯路 3：怀疑 pdb 缺失 / IDE 工程文件过期（证伪）

- pdb 与 dll 时间戳一致；`Assembly-CSharp.csproj` 已包含新增的 `Skill.cs`、`Object/GameFlow.cs`、`Object/RootGameFlow.cs`，且不含已删除的 `SkillAtom.cs`。
- 结论：不是符号或工程文件列表问题。

### 根因 1：`dotnet.preferCSharpExtension: true` 把 C# Dev Kit 禁用了

- 硬证据：`C:\Users\Fanphic\.vscode\extensions\visualstudiotoolsforunity.vstuc-1.3.1\package.json` 里写着
  `extensionDependencies: ms-dotnettools.vscode-dotnet-runtime, ms-dotnettools.csdevkit`
  → **vstuc 的调试器硬依赖 C# Dev Kit**；而 Dev Kit 从 3.x 起**严格执行** `dotnet.preferCSharpExtension`（旧版本忽略），一旦为 `true` 就整个 Dev Kit 被禁用 → 调试器不存在 → 断点全部"未验证"。
- 这正是"以前能用、突然失效"的原因（扩展自动升级到 3.x 后才开始严格生效）。

### 根因 2：.NET 10 运行时缺失（两处都要）

- Dev Kit 3.40.204 后端组件要求 net10：
  `C:\Users\Fanphic\.vscode\extensions\ms-dotnettools.csdevkit-3.40.204-win32-x64\components\vs-green-server\platforms\win32-x64\node_modules\@microsoft\visualstudio-code-servicehost.win32-x64\Microsoft.VisualStudio.Code.ServiceHost.AspNet.runtimeconfig.json`
  里写的是 `Microsoft.NETCore.App 10.0.0` + **`Microsoft.AspNetCore.App 10.0.0`**；`Microsoft.VisualStudio.Code.Server` / `ServiceController` 也都是 `"tfm": "net10.0"`。
- vstuc 自己也要求 net10：`...visualstudiotoolsforunity.vstuc-1.3.1\bin\UnityDebugAdapter.runtimeconfig.json` 为 `net10.0`（其 changelog v1.2.2 "Move to .NET 10"）。
- 本机当时最高只有 `Microsoft.NETCore.App 9.0.3` / `Microsoft.AspNetCore.App 9.0.3`。

安装过程中的坑：

- `winget install --id Microsoft.DotNet.AspNetCore.10` 报 **"找到已安装的现有包…找不到可用的升级"**（以为已装）；随后再次强制安装报 **退出代码 1618：另一个安装已在进行中**。
- 实际状态：注册表里有 `Microsoft ASP.NET Core Runtime - 10.0.12 (x64)`，但 `C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App` 下只有 6.0.25 / 7.0.14 / 8.0.14 / 9.0.3 —— **注册表有记录、共享框架文件缺失**（安装被中断）。
- 修法：下载官方 `aspnetcore-runtime-10.0.12-win-x64.exe` 后执行 `/repair /quiet /norestart`，文件补回，`dotnet --list-runtimes` 出现 `Microsoft.AspNetCore.App 10.0.12`。

### 根因 3：C# Dev Kit 3.x 只吃 SDK 风格工程，而工程被强制成 Legacy

- Dev Kit 日志（`%APPDATA%\Code\logs\<会话>\window1\exthost\ms-dotnettools.csdevkit\C# 开发工具包.log`）刷出 18 条：
  `项目文件“C:\Work\SandBox_Card\Assembly-CSharp.csproj”的格式不受支持(例如，传统 .NET Framework 项目)…`
  `未能加载项目 '...'。（C# 开发工具包中不支持此项目。）`
  `项目系统初始化已完成。0 项目已加载，18 加载失败。`
- 来源是项目里自带的 hack：
  - `c:/Work/SandBox_Card/Assets/Editor/ForceLegacyProjectGeneration.cs`（`AssetPostprocessor.OnSelectingCSProjectStyle` 返回 `"Legacy"`，注释里写"为绕 .slnx 引发的调试卡死"）
  - `c:/Work/SandBox_Card/.vscode/.vstupatchdisable`（阻止 Unity 把 `dotnet.defaultSolution` 写成 `.slnx`）
- 而 `com.unity.ide.visualstudio@2.0.27` 的 `VisualStudioCodeInstallation.cs` **默认就是 `GeneratorStyle.SDK`**（只有 Visual Studio for Windows 在旧版本下才走 Legacy）→ 也就是说这套 hack 本身就是误判的产物，实际病因是根因 1 和 2。
- **最阴的一个坑**：把 `ForceLegacyProjectGeneration.cs` 改名成 `.cs.disabled` 后，Unity **没有重新编译**（`Library\ScriptAssemblies\Assembly-CSharp-Editor.dll` 时间戳仍是 19:37:58）→ 旧的 AssetPostprocessor 仍在已加载程序集里生效 → 点 `Regenerate project files` 依然生成 Legacy。
  解决：**退出 Play 模式 → `Ctrl+R`（Assets > Refresh）**，或直接**重启 Unity**；确认 `Assembly-CSharp-Editor.dll` 时间戳变化后，再执行 `Edit > Preferences > External Tools > Regenerate project files`。
  生效后根目录才出现 `SandBox_Card.slnx`，csproj 标记变成 `<UnityProjectGeneratorStyle>SDK</UnityProjectGeneratorStyle>`。

### 根因 4：`launch.json` 用了 vstuc 不支持的属性

- vstuc 的 attach 配置只声明三个属性：`endPoint`、`logFile`、`projectPath`。
- 原 `launch.json` 写的是旧版残留的 `"projectName": "SandBox_Card"`（不合法），已改为 `"projectPath"`，并加上 `"logFile"` 以便拿到适配器全量协议日志。

### 根因 5（表象放大器）：5 个永远无法验证的"坏断点"

断点本身也有问题，它们是"怎么看都是灰色"的直接来源，详见第五节。

---

## 三、最终修复清单

| # | 位置 | 改动 |
|---|---|---|
| 1 | `c:/Work/SandBox_Card/.vscode/settings.json` 第 61 行 | `"dotnet.preferCSharpExtension"` 由 `true` 改为 **`false`**（恢复 Dev Kit，才能有 vstuc 调试器） |
| 2 | 同上 第 66 行 | `"dotnet.defaultSolution"` 交由 Unity 自动写成 **`SandBox_Card.slnx`**（原为 `SandBox_Card.sln`） |
| 3 | `c:/Work/SandBox_Card/Assets/Editor/ForceLegacyProjectGeneration.cs` | 改名为 **`.cs.disabled`**（`.meta` 同步改名），停用强制 Legacy |
| 4 | `c:/Work/SandBox_Card/.vscode/.vstupatchdisable` | **删除**（不再阻止 Unity 写 `.slnx`） |
| 5 | 项目根目录 | 删除 19 个 Legacy 工程文件（`Assembly-CSharp*.csproj` 等 + `SandBox_Card.sln`） |
| 6 | `c:/Work/SandBox_Card/.vscode/launch.json` | `projectName` → **`projectPath: "c:/Work/SandBox_Card"`**，并加 **`logFile: "c:/Work/SandBox_Card/Library/vstuc-debug.log"`** |
| 7 | 系统运行时 | `Microsoft.NETCore.App 10.0.12`（用户已装）+ `Microsoft.AspNetCore.App 10.0.12`（用官方 exe `/repair` 补回） |
| 8 | Unity | 退出 Play → `Ctrl+R` → `Edit > Preferences > External Tools > Regenerate project files`（生成 `SandBox_Card.slnx` + SDK 风格 csproj） |
| 9 | VS Code 断点面板 | 手动清掉 5 个坏断点（见第五节） |

---

## 四、结果与验证证据

- 工程风格：`Assembly-CSharp.csproj` 内容为 `<UnityProjectGeneratorStyle>SDK</UnityProjectGeneratorStyle>`，根目录存在 `SandBox_Card.slnx`，`.vscode/settings.json` 里 `dotnet.defaultSolution = SandBox_Card.slnx`。
- 重编生效：`Library\ScriptAssemblies\Assembly-CSharp-Editor.dll` 时间戳由 19:37:58 → **20:13:45**。
- Dev Kit 侧（`...\exthost\ms-dotnettools.csdevkit\C# 开发工具包.log`）：
  `Default solution: SandBox_Card.slnx` → `Completed: Opening a solution (3873ms)`，**不再有"格式不受支持 / 未能加载项目"**。
- vstuc 协议侧（`c:/Work/SandBox_Card/Library/vstuc-debug.log`）：
  - `R (attach-2): "success":true` + `initialized` 事件 → **附加成功**
  - `E (module): ...Library\ScriptAssemblies\Assembly-CSharp.dll (id 104)` → **脚本程序集已加载**
  - `E (breakpoint): {"reason":"changed","verified":true,"source":{...DamageFlow.cs},"line":61}` → **断点验证成功**
  - `ProjectFileMapper: unable to map ...` 共 32 条，**全部是 DOTween 自带源码路径（`D:\DG\_Develop\...`），本项目路径 0 条** → 映射管线正常
- 清理后 `state.vscdb` 里只剩 1 个断点 `DamageFlow.cs:61`，实测**断点命中、调试成功**。

---

## 五、坏断点清单（"未验证"永远消不掉的原因）

调试协议日志里记录了你当时设置的 6 个断点，其中 5 个是永久灰色的：

| 断点 | 原因（已核实） |
|---|---|
| `...\Prefab\BattleCard\interface\BattleCardModify_BattleCardTimmingSys.cs:2` | **文件不存在**（该路径下实际是 `BattleCardModify_BattleCardTimmingGroup.cs` / `BattleCardModify_BattleTimming.cs`），改名/删除后留下的孤儿断点 |
| `...\DeckBuildBattle\BattlePhase\BattlePhaseStart.cs:90`、`:93` | 文件**总共只有 28 行**，行号越界 |
| `...\Prefab\BattleUnit\BattleUnit.cs:127` | 第 127 行是类结尾的 **`}`**，没有可执行指令 |
| （保留）`...\GameModify\DamageFlow.cs:20`、`:61` | 均为方法体内可执行语句 |

Mono 软调试器的行为：断点是**方法被 JIT 加载后**才绑定，所以刚 attach 上时灰、触发到那段代码时自动变实心并命中，属正常。**永久灰色的只有**：文件已删除 / 行号越界 / 落在 `}`、空行、`//`、`using`、字段声明 上。

---

## 六、可复用结论与避坑

1. **"卡在 tick" 先判性质**：看 CPU 占用——≈0% 是调试器挂起主线程（预期行为）；跑满才是死循环。另外**先 Play 再 attach**，不要 attach 完再 Play：Play 会触发 domain reload，此时命中断点会让重载永远完不成，编辑器就"卡在 tick"了。（VSTU 官方文档也承认"调试时 Unity 有时会崩溃/冻结"是已知问题。）
2. **"未验证断点"按三层顺序排查**：① 调试器是否存在（vstuc → 依赖 C# Dev Kit，别禁用 Dev Kit）② 运行时是否齐（Dev Kit 和 vstuc 都要 **net10**，且 AspNetCore.App 与 NETCore.App 都要）③ 工程是否 SDK 风格（Dev Kit 3.x 只吃 SDK 风格；Unity 的 VS Code 集成默认就是 SDK，**不要**用 `OnSelectingCSProjectStyle` 改成 Legacy）。
3. **改 / 删 `.cs` 后 Unity 不一定立刻重编**（Play 模式下会挂起等待），必须核对 `Library\ScriptAssemblies\*.dll` 时间戳；必要时退出 Play → `Ctrl+R` 或重启 Unity，否则旧程序集（含旧 AssetPostprocessor）继续生效，会让你误判"改了没用"。
4. **`winget` 说"已安装、无升级"但文件缺失** → 说明安装被中断过，用官方安装包 `/repair`（或 `/uninstall` + `/install`）。
5. **排查用的三类日志**（都很有用）：
   - vstuc 全量 DAP 协议日志：`c:/Work/SandBox_Card/Library/vstuc-debug.log`（靠 `launch.json` 的 `logFile` 打开；Library 已被 gitignore）
   - Dev Kit 日志：`%APPDATA%\Code\logs\<会话>\window1\exthost\ms-dotnettools.csdevkit\C# 开发工具包.log`
   - Unity 日志：`%LOCALAPPDATA%\Unity\Editor\Editor.log`（**只读末尾 300KB**，整读会卡几十秒）
6. **VS Code 断点存储位置**（想批量处理时用）：`%APPDATA%\Code\User\workspaceStorage\<哈希>\state.vscdb`，SQLite 表 `ItemTable`，键 **`debug.breakpoint`**（不是 `debug.breakpoints`），条目结构为 `{"id","enabled","uri":{"fsPath":...},"lineNumber":N}`。**修改前必须完全关闭 VS Code**，否则退出时会被内存状态覆盖。
7. **判断"实验是否成功"要给出可核对的产物**：本次用到的判据是 `Assembly-CSharp-Editor.dll` 时间戳、csproj 里的 `UnityProjectGeneratorStyle=SDK`、Dev Kit 的 `Default solution:` 行、vstuc 的 `verified:true` 事件——比"界面看起来对不对"可靠得多。

---

## 附：本次涉及的外部路径

- VS Code 扩展：`C:\Users\Fanphic\.vscode\extensions\visualstudiotoolsforunity.vstuc-1.3.1`、`ms-dotnettools.csdevkit-3.40.204-win32-x64`、`ms-dotnettools.csharp-2.160.4-win32-x64`
- .NET 运行时根：`C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App`、`...\Microsoft.NETCore.App`
- Unity 安装：`C:\WorkSpace\2022.3.62f2\Editor\Unity.exe`
