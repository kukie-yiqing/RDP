# Remote Desk RDP Launcher | 远程桌面控制中心 🚀

[English](#english-version) | [中文说明](#中文版)

---

<a name="english-version"></a>
## English Version

Remote Desk is a modern, secure, and high-performance one-click RDP (Remote Desktop Protocol) launcher built with C# and WPF. It offers a seamless, theme-aware user experience while ensuring enterprise-grade security for your credentials.

### ✨ Key Features
- **Modern UI/UX**: Full synchronization with Windows 10/11 system themes, including dark/light modes and accent colors.
- **Security First**: Credentials are encrypted using the Windows **Data Protection API (DPAPI)** and stored locally in the system Credential Manager. No plaintext passwords or external data uploads.
- **Verified History**: Automatically validates connectivity (TCP Handshake) before persisting history, keeping your list clean from invalid entries.
- **Efficiency Boosters**: Quick integration for Network Share (`\\ip`), dedicated Port input, and multi-monitor array scheduling.
- **Portable Architecture**: Single e-file (.exe) with no external dependencies. Just click and go.

### 🛡️ Security & Privacy
- **Local Encryption**: All data is bound to the current user's DPAPI scope.
- **Self-Contained**: No telemetry, no background services, and no unauthorized network calls.

### 📜 LICENSE & DISCLAIMER (IMPORTANT)
**Version: 1.2.5.0**
**Copyright © 2026 Kukie Zhang. All Rights Reserved.**

1. **Non-Commercial Use Only**: This software is provided for personal, educational, and research purposes. **Commercial redistribution, resale, or integration into commercial products is strictly prohibited.**
2. **"AS-IS" Warranty**: This software is provided "as-is" without warranty of any kind, express or implied.
3. **Limitation of Liability**: In no event shall the author be liable for any damages (including but not limited to loss of data, system failure, or hardware damage) arising out of the use or inability to use this software.
4. **Attribution**: Any modifications or private redistribution must retain original copyright notices.

---

<a name="中文版"></a>
## 中文版本

`Remote Desk` 是一款小巧、精致且功能强大的 Windows 远程桌面一键启动器。它通过美学设计与极客安全逻辑的融合，为高频远程办公用户提供了极致的连接体验。

### ✨ 核心特性
- **现代化美学 UI**：全面支持 Windows 10/11 动态主题同步（深色/浅色模式及系统强调色）。
- **极客安全机制**：基于 Windows **DPAPI** 机制对凭据进行本地强加密，并同步至系统凭据管理器（Credential Manager），开发者承诺**绝不收集或上传**任何用户隐私数据。
- **智能历史验证**：内置后台 TCP 端口握手侦测，只有验证可连通的账号才会存入历史记录，拒绝脏数据堆叠。
- **效率倍增器**：集成一键直达共享文件夹 (`\\ip`)、独立端口管理、以及多显示器调度预设。
- **纯绿色架构**：单文件独立运行，无任何外部库依赖。

### 🛡️ 安全与隐私
- **本地化存储**：所有配置和加密凭据均存储在用户本机的加密域内。
- **无后门/无外联**：程序不含任何遥测插件，不进行未经许可的非必要网络通信。

### 📜 法律声明与许可协议 (重要)
**版本号: 1.2.5.0**
**Copyright © 2026 Kukie Zhang. 保留所有权利。**

1. **仅限非商业用途**：本软件仅供个人学习、研究及日常办公私人使用。**严禁任何形式的商业转售、盈利性打包分发或将其集成至商业化产品中。**
2. **无担保声明**：本软件按“现状”提供，不提供任何明示或暗示的保证（包括但不限于对适销性或特定用途适用性的保证）。
3. **责任限制**：开发者（Kukie Zhang）不对因使用本软件或无法使用本软件而导致的任何后果（包括但不限于数据丢失、系统瘫痪或硬件损耗）承担赔偿责任。
4. **署名要求**：任何对源码的私人修改或衍生版本，必须在显著位置保留原作者的版权署名。

---

### 🚀 Contact / 联系方式
- **Developer**: Kukie Zhang
- **Support Email**: [Kukie.yiqing@outlook.com](mailto:Kukie.yiqing@outlook.com)

---
*"用最优雅的技术，缩短每一次穿越山海的桌面距离。"*
