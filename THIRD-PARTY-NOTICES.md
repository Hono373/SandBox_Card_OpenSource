# 第三方组件声明

本仓库包含以下第三方组件。各组件的著作权归原作者所有，使用需遵守各自的许可条款。

---

## DOTween / DemiLib

- 位置：`Assets/Plugins/Demigiant/`
- 作者：Daniele Giardini（Demigiant）
- 官网：https://dotween.demigiant.com/
- 版本：免费版（Free 版，不含 Pro 功能）

**DOTween 免费版授权要点**（完整条款以官网 LICENSE 为准）：

- 可在任意数量的商业或非商业项目中使用，无需付费
- 可随项目一同分发（包括公开的源码仓库）
- 不得单独再分发、出售或作为独立产品销售 DOTween 本身
- 保留原始的 `readme.txt` 与版权声明

本仓库**不包含 DOTween Pro**。Pro 是付费版本，其 DLC 与组件（`DOTweenAnimation`、`DOTweenPath`）不可分发。本项目的动画逻辑全部基于免费版 API（`DOTween.To`、`Sequence`、`TweensById`）与自研扩展 `Assets/Plugins/DOTweenPlus/`，因此不受影响。

---

## Spine Runtimes

- 位置：`Assets/Plugins/Spine/`
- 作者：Esoteric Software
- 官网：https://esotericsoftware.com/spine-runtimes
- 许可：Spine Runtimes License Agreement

**要点**：

- Runtime 代码可随项目分发，需保留版权声明
- 使用 Spine 导出的 `.skel` / `.atlas` 资源，以及用 Spine 编辑器修改动画，需要拥有相应的 Spine 编辑器授权
- 本仓库不包含任何 `.skel` / `.atlas` 骨骼资源（`Assets/Spine/` 已移除），仅保留运行时代码

完整许可见 https://esotericsoftware.com/spine-runtimes-license

---

## Luban

- 位置：`Assets/Plugins/luban_unity-main/`
- 作者：focus-creative-games
- 仓库：https://github.com/focus-creative-games/luban
- 许可：MIT

本项目的配置流程已切换为自研 JSONL 配置库，此目录仅作历史遗留保留，当前运行时不参与。

---

## YooAsset

- 通过 UPM 拉取（`Packages/manifest.json` 中 `com.tuyoogame.yooasset: 2.1.2`），不在本仓库内
- 许可：MIT

---

## Newtonsoft.Json for Unity

- 通过 UPM 拉取（`com.unity.nuget.newtonsoft-json: 3.2.1`），不在本仓库内
- 许可：MIT

---

## Unity 内置包

URP、TextMeshPro、Timeline、UGUI、Test Framework 等均为 Unity 官方包，按其随附条款使用。

---

## 未包含的组件

**Odin Inspector**（Sirenix）是商业插件，持有者无权再分发。本仓库不包含其任何代码、DLL 或配置文件。

`Assets/Plugins/OdinShim/OdinShim.cs` 是独立编写的兼容层，提供一组同名空特性使代码在未安装 Odin 时仍可编译运行。它不使用、不复制 Odin 的任何实现，与 Sirenix 无关。
