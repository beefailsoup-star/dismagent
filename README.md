# Dism Agent

**PowerShell AI Assistant** — 在終端機裡用自然語言描述你想要的指令，讓 AI 解釋並選擇是否執行。

```
dismagent> 列出所有正在執行的 Windows 服務

◆ AI Analysis ─────
這條命令使用 Get-Service 搭配 Where-Object 來篩選狀態為 Running 的服務：
  Get-Service | Where-Object { $_.Status -eq 'Running' }

Execute? (y/n):
```

## 安裝

### 快速安裝

下載 [`dism-agent.zip`](dism-agent.zip)，解壓縮後執行：

```powershell
setup.exe
```

或手動執行：

```powershell
dismagent --install
```

安裝程式會：
- 複製執行檔到 `%LOCALAPPDATA%\DismAgent\`
- 加入使用者 PATH
- 自動建立設定檔

安裝後開啟**新的**終端機，輸入 `dismagent` 即可啟動。

### 設定 AI 提供者

```powershell
dismagent setup
```

或直接在 REPL 裡輸入：

```
dismagent> dismagent setup
```

## 使用方式

### 一般模式

直接輸入 PowerShell 指令或自然語言描述：

```
dismagent> 顯示磁碟 c 的使用情況
dismagent> Get-Process | Sort-Object CPU -Descending | Select -First 5
```

AI 會用繁體中文解釋該指令，詢問是否執行，輸入 `y` 即執行。

### 指令

| 指令 | 說明 |
|---|---|
| `dismagent` | 開啟新視窗進入 REPL 互動模式 |
| `dismagent --install` | 安裝到系統 |
| `dismagent setup` | 設定 AI / 搜尋提供者 |
| `dismagent config` | 顯示目前設定 |
| `dismagent --repl` | 直接進入 REPL（不開新視窗） |
| `help` | 顯示說明（REPL 內） |
| `exit` / `quit` | 離開 REPL |

## 支援的 AI 提供者

| 提供者 | 預設模型 | 設定方式 |
|---|---|---|
| **Ollama** (本地) | `llama3` | 下載後執行 `ollama serve`，安裝時可自動 pull 模型 |
| **Kimi** (Moonshot) | `moonshot-v1-8k` | platform.moonshot.cn 取得 API Key |
| **GLM** (智譜) | `glm-4` | open.bigmodel.cn 取得 API Key |
| **DeepSeek** | `deepseek-chat` | platform.deepseek.com 取得 API Key |
| **Doubao** (字節跳動) | `ep-xxxx` | console.volcengine.com 開啟 ARK 取得 API Key |
| **Mimo** | `mimo-chat` | mimo.ai 取得 API Key |

## 支援的搜尋提供者

| 提供者 | 需要 API Key |
|---|---|
| Google Custom Search | 是 |
| DuckDuckGo | 否 |
| 無 | — |

## 設定檔位置

```
%APPDATA%\DismAgent\config.json
```

## 專案結構

```
src/
├── DismAgent/          # 主程式（REPL、AI 解釋、指令執行）
│   ├── Program.cs
│   └── Services/
│       ├── AiService.cs        # 呼叫 AI API（Ollama / OpenAI 相容）
│       ├── CommandService.cs   # 執行 PowerShell 命令
│       └── ConfigService.cs    # 讀寫設定檔（純手刻 JSON，無反射）
└── DismSetup/          # 安裝引導程式（動畫式終端機介面）
    └── Program.cs
```

## 技術特點

- **PublishTrimmed = true** — 單檔、自包含、無執行時期依賴
- **零反射 JSON** — 所有 JSON 操作皆使用 `JsonDocument.Parse` 與手動字串建構，迴避 AOT 修剪問題
- **無第三方套件** — 僅使用 .NET BCL 內建類別庫
- **即時下載進度** — Ollama pull 顯示進度條、速度、剩餘時間
- **繁體中文** — AI 解釋與介面皆為繁體中文

## 從原始碼建置

```powershell
dotnet publish src\DismAgent\DismAgent.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true
dotnet publish src\DismSetup\DismSetup.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true
```
