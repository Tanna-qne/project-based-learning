# Windows 安装与首次验证（DesignBuilder 7.3.1.003）

当前代码是 **0.1 骨架**：使用官方插件结构，提供中文窗口、只读状态摘要和 Dify Chat API 接入。尚未在用户的 DesignBuilder 安装中编译或加载验证。

## 先准备

- 已授权的 DesignBuilder 7.3.1.003，并确认安装目录中存在：
  - `C:\Program Files (x86)\DesignBuilder\Lib\DB.Api.dll`
  - `C:\Program Files (x86)\DesignBuilder\Lib\DB.Extensibility.Contracts.dll`
- Visual Studio 2022 Community，安装“.NET 桌面开发”工作负载和 .NET Framework 4.8 targeting pack。
- Dify 中建立一个 Chatflow/聊天助手应用，并在“访问 API”页面创建 **App API Key**。不要使用 Dify 账户密码。

## 编译

1. 在 GitHub 切换到 `designbuilder-ai-tutor` 分支并下载 ZIP，或用 Git 克隆该分支。
2. 用 Visual Studio 打开 `src\DBAILearningAssistant\DBAILearningAssistant.csproj`。
3. 选择 Release → Build。
4. 若提示找不到两个 DB DLL，检查实际安装路径；只在项目引用中改成你电脑的路径。
5. 输出位于 `src\DBAILearningAssistant\bin\Release\`。

## 安装

1. 新建：
   `%LOCALAPPDATA%\DesignBuilder\User Plugins\DBAILearningAssistant\`
2. 从 Release 输出目录复制：
   - `DBAILearningAssistant.dll`
   - `DB.Api.dll`
   - `DB.Extensibility.Contracts.dll`
3. 完全退出并重启 DesignBuilder。
4. 顶部菜单应出现 **DB AI 学习助手 → 打开助手**。

卸载时关闭 DB 并删除上述 `DBAILearningAssistant` 文件夹。

## 连接 Dify

1. 在助手窗口点击“设置”。
2. API 地址：Dify Cloud 通常为 `https://api.dify.ai/v1`；自托管时填自己的 `/v1` 地址。
3. 填写 Dify **App API Key**。密钥只保存在本机：
   `%LOCALAPPDATA%\DBAILearningAssistant\settings.json`
4. 问：“我现在位于哪个模型层级？下一步适合学习什么？”

## 第一轮验证记录

请依次记录结果，不要把 API Key 截图或提交 GitHub：

- [ ] 插件菜单出现
- [ ] 中文窗口打开
- [ ] 无模型时显示 `项目已打开: False`
- [ ] 打开示例模型后变为 `True`
- [ ] 从 Building 切换到 Zone/Surface 后，对应 index 变化
- [ ] Dify 能回答并保持连续对话
- [ ] 关闭模型后不继续引用旧模型状态

若插件未出现，先打开 DB 的日志（Tools → Show log file），搜索 `DBAILearningAssistant`、`MEF`、`load` 或 `exception`，只复制相关错误段落。

## 已知边界

- 0.1 通过打开窗口、切换界面和“刷新状态”读取上下文；尚未验证 7.3.1.003 的事件订阅。
- 当前状态不等于屏幕视觉内容，无法读取鼠标指向、输入框焦点或尚未提交的文字。
- 首轮只发送模型文件名，不发送完整本机路径。
- 插件只读，不修改模型、不点击按钮、不启动模拟。
- 官方示例仓库当前目标为 .NET Framework 4.8；最终兼容性以本机 7.3 DLL 编译与加载结果为准。

官方上游：
- https://github.com/DesignBuilderSoftware/db-plugins
- https://designbuilder.co.uk/helpv7.3/api_ref/namespace_d_b_1_1_api.html
