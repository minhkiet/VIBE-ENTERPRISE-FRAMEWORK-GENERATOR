#nullable enable
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CursorSetupWpf.Helpers;
using CursorSetupWpf.Services;

namespace CursorSetupWpf.ViewModels
{
    public class VibeToolItem : ViewModelBase
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string GatePrefix { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsInstalled { get; set; }
        
        // Localized name that updates with culture changes
        public string LocalizedName => LocalizationService.T($"vibe_tool.{Id}");
        public string LocalizedDescription => LocalizationService.T($"vibe_tool.{Id}_desc");
        
        public void RefreshLocalizedProperties()
        {
            OnPropertyChanged(nameof(LocalizedName));
            OnPropertyChanged(nameof(LocalizedDescription));
        }
    }

    public class HarnessItem : ViewModelBase
    {
        string _name = "";
        string _type = "";
        string _description = "";
        string _installPath = "";
        bool _isInstalled;
        bool _supportsHooks;

        public string Name { get => _name; set => Set(ref _name, value); }
        public string Type { get => _type; set => Set(ref _type, value); }
        public string Description { get => _description; set => Set(ref _description, value); }
        public string InstallPath { get => _installPath; set => Set(ref _installPath, value); }
        public bool IsInstalled 
        { 
            get => _isInstalled; 
            set 
            { 
                if (Set(ref _isInstalled, value))
                {
                    OnPropertyChanged(nameof(StatusText));
                }
            } 
        }
        public bool SupportsHooks { get => _supportsHooks; set => Set(ref _supportsHooks, value); }
        
        // Localized status text
        public string InstalledText => LocalizationService.T("ecc.installed");
        public string NotInstalledText => LocalizationService.T("ecc.not_installed");
        public string StatusText => IsInstalled ? InstalledText : NotInstalledText;
        
        // Public method to refresh localized properties when culture changes
        public void RefreshLocalizedProperties()
        {
            OnPropertyChanged(nameof(InstalledText));
            OnPropertyChanged(nameof(NotInstalledText));
            OnPropertyChanged(nameof(StatusText));
        }
    }

    public class ECCViewModel : ViewModelBase
    {
        readonly Func<string> _workspaceRoot;
        readonly FrameworkRunner _vibeRunner = new();

        // Labels (using methods that call LocalizationService.T)
        public string ECCTitle => LocalizationService.T("ecc.title");
        public string ECCSubtitle => LocalizationService.T("ecc.subtitle");
        public string StatusLabel => LocalizationService.T("ecc.status");
        public string CloneRepoLabel => LocalizationService.T("ecc.clone");
        public string RefreshStatusLabel => LocalizationService.T("ecc.refresh");
        public string EnableHooksLabel => LocalizationService.T("ecc.hooks");
        public string HarnessesTitle => LocalizationService.T("ecc.harnesses");
        public string FeaturesTitle => LocalizationService.T("ecc.features");
        
        // Static localized labels
        public string AgentsLabel => LocalizationService.T("ecc.agents");
        public string SkillsLabel => LocalizationService.T("ecc.skills");
        public string CommandsLabel => LocalizationService.T("ecc.commands");
        public string HooksMemoryLabel => LocalizationService.T("ecc.hooks_memory");
        public string HooksMemoryDesc => LocalizationService.T("ecc.hooks_memory_desc");
        public string AgentShieldLabel => LocalizationService.T("ecc.agent_shield");
        public string AgentShieldDesc => LocalizationService.T("ecc.agent_shield_desc");
        public string WorkflowLabel => LocalizationService.T("ecc.workflow");
        public string WorkflowDesc => LocalizationService.T("ecc.workflow_desc");
        public string InstallCommandsTitle => LocalizationService.T("ecc.install_commands");
        public string ClaudeCodeLabel => LocalizationService.T("ecc.claude_code");
        public string CodexLabel => LocalizationService.T("ecc.codex");
        public string CursorLabel => LocalizationService.T("ecc.cursor");
        public string GuidedSetupLabel => LocalizationService.T("ecc.guided_setup");
        public string ActiveVibeCoderLabel => LocalizationService.T("ecc.active_vibe_coder");
        public string GateLabel => LocalizationService.T("ecc.gate");
        public string NotInstalledLabel => LocalizationService.T("ecc.not_installed");
        public string InstalledLabel => LocalizationService.T("ecc.installed");
        public string InstallButtonLabel => LocalizationService.T("btn.install");
        
        // ECC Stats
        public string ECCVersion => "2.2.2";
        public int AgentCount => 68;
        public int SkillCount => 292;
        public int CommandCount => 94;

        // Vibe Tool Selector
        public ObservableCollection<VibeToolItem> AvailableVibeTools { get; } = new();
        
        VibeToolItem? _selectedVibeTool;
        public VibeToolItem? SelectedVibeTool
        {
            get => _selectedVibeTool;
            set
            {
                if (Set(ref _selectedVibeTool, value) && value != null)
                {
                    SelectedVibeCoder = value.LocalizedName;
                    GatePrefix = value.GatePrefix;
                }
            }
        }
        
        // Vibe Tool Selector labels
        public string VibeCoderLabel => LocalizationService.T("vibe_coder.label");
        public string SelectVibeCoderLabel => LocalizationService.T("vibe_coder.select");

        // Vibe Coder info (mirrors MainViewModel.SelectedAgent)
        string _selectedVibeCoder = "Cursor IDE";
        public string SelectedVibeCoder
        {
            get => _selectedVibeCoder;
            set => Set(ref _selectedVibeCoder, value);
        }

        string _gatePrefix = "K";
        public string GatePrefix
        {
            get => _gatePrefix;
            set => Set(ref _gatePrefix, value);
        }

        bool _isECCInstalled;
        public bool IsECCInstalled { get => _isECCInstalled; set => Set(ref _isECCInstalled, value); }

        public string InstallStatusText => IsECCInstalled
            ? LocalizationService.T("ecc.installed")
            : LocalizationService.T("ecc.not_installed");

        // Harness items
        public ObservableCollection<HarnessItem> HarnessItems { get; } = new();

        // Commands
        public ICommand CloneRepositoryCommand { get; }
        public ICommand RefreshStatusCommand { get; }
        public ICommand InstallHarnessCommand { get; }

        // Logging
        public ObservableCollection<string> LogLines { get; } = new();

        public ECCViewModel(Func<string>? workspaceRoot = null)
        {
            _workspaceRoot = workspaceRoot ?? (() => Environment.CurrentDirectory);
            _vibeRunner.OutputReceived += line => AppendLog($"[VIBE] {line}");

            // Subscribe to culture changes for language refresh
            LocalizationService.CultureChanged += OnCultureChanged;
            
            // Initialize vibe coding tools
            InitializeVibeTools();
            
            // Initialize harnesses
            InitializeHarnesses();

            // Initialize commands
            CloneRepositoryCommand = new AsyncRelayCommand(CloneRepositoryAsync);
            RefreshStatusCommand = new RelayCommand(_ => RefreshStatus());
            InstallHarnessCommand = new AsyncRelayCommand(InstallHarnessAsync);

            // Initial status check
            RefreshStatus();
        }
        
        void OnCultureChanged()
        {
            // Refresh all localized properties when culture changes
            RefreshStrings();
        }
        
        void RefreshStrings()
        {
            // Notify all localized properties have changed
            OnPropertyChanged(nameof(ECCTitle));
            OnPropertyChanged(nameof(ECCSubtitle));
            OnPropertyChanged(nameof(StatusLabel));
            OnPropertyChanged(nameof(CloneRepoLabel));
            OnPropertyChanged(nameof(RefreshStatusLabel));
            OnPropertyChanged(nameof(HarnessesTitle));
            OnPropertyChanged(nameof(FeaturesTitle));
            OnPropertyChanged(nameof(AgentsLabel));
            OnPropertyChanged(nameof(SkillsLabel));
            OnPropertyChanged(nameof(CommandsLabel));
            OnPropertyChanged(nameof(HooksMemoryLabel));
            OnPropertyChanged(nameof(HooksMemoryDesc));
            OnPropertyChanged(nameof(AgentShieldLabel));
            OnPropertyChanged(nameof(AgentShieldDesc));
            OnPropertyChanged(nameof(WorkflowLabel));
            OnPropertyChanged(nameof(WorkflowDesc));
            OnPropertyChanged(nameof(InstallCommandsTitle));
            OnPropertyChanged(nameof(ClaudeCodeLabel));
            OnPropertyChanged(nameof(CodexLabel));
            OnPropertyChanged(nameof(CursorLabel));
            OnPropertyChanged(nameof(GuidedSetupLabel));
            OnPropertyChanged(nameof(ActiveVibeCoderLabel));
            OnPropertyChanged(nameof(GateLabel));
            OnPropertyChanged(nameof(NotInstalledLabel));
            OnPropertyChanged(nameof(InstalledLabel));
            OnPropertyChanged(nameof(InstallButtonLabel));
            OnPropertyChanged(nameof(InstallStatusText));
            OnPropertyChanged(nameof(ECCVersion));
            OnPropertyChanged(nameof(AgentCount));
            OnPropertyChanged(nameof(SkillCount));
            OnPropertyChanged(nameof(CommandCount));
            OnPropertyChanged(nameof(VibeCoderLabel));
            OnPropertyChanged(nameof(SelectVibeCoderLabel));
            OnPropertyChanged(nameof(SelectedVibeCoder));
            
            // Refresh vibe tool localized properties
            foreach (var tool in AvailableVibeTools)
            {
                tool.RefreshLocalizedProperties();
            }
            
            // Refresh harness items status text
            foreach (var item in HarnessItems)
            {
                item.RefreshLocalizedProperties();
            }
        }

        void InitializeVibeTools()
        {
            AvailableVibeTools.Clear();
            foreach (var tool in VibeCli.SupportedTools)
            {
                AvailableVibeTools.Add(new VibeToolItem
                {
                    Id = tool.Replace('-', '_'),
                    GatePrefix = tool switch
                    {
                        "cursor" => "K",
                        "windsurf" => "W",
                        "claude-code" => "C",
                        "antigravity" => "AG",
                        "cline" => "CL",
                        "roo" => "R",
                        "copilot" => "CP",
                        "aider" => "AI",
                        _ => "CX",
                    },
                });
            }
            
            // Set default selection
            _selectedVibeTool = AvailableVibeTools.FirstOrDefault(t => t.Id == "cursor") 
                ?? AvailableVibeTools.FirstOrDefault();
            if (_selectedVibeTool != null)
            {
                _selectedVibeCoder = _selectedVibeTool.LocalizedName;
                _gatePrefix = _selectedVibeTool.GatePrefix;
            }
        }

        void InitializeHarnesses()
        {
            HarnessItems.Clear();
            AddHarness("cursor", ".cursor", "ecc.harness_cursor_desc");
            AddHarness("windsurf", ".windsurf", "vibe_tool.windsurf_desc");
            AddHarness("claude-code", ".claude", "ecc.harness_claude_desc");
            AddHarness("antigravity", ".gemini", "ecc.harness_antigravity_desc");
            AddHarness("cline", ".cline", "vibe_tool.cline_desc");
            AddHarness("roo", ".roo", "vibe_tool.roo_desc");
            AddHarness("copilot", ".github", "ecc.harness_copilot_desc");
            AddHarness("aider", ".aider.conf.yml", "vibe_tool.aider_desc");
            AddHarness("codex", ".codex", "ecc.harness_codex_desc");
        }

        void AddHarness(string toolId, string installPath, string descriptionKey)
        {
            HarnessItems.Add(new HarnessItem
            {
                Name = LocalizationService.T($"vibe_tool.{toolId.Replace('-', '_')}"),
                Type = toolId,
                Description = LocalizationService.T(descriptionKey),
                InstallPath = installPath,
                SupportsHooks = false,
            });
        }

        async Task CloneRepositoryAsync(object? _)
        {
            try
            {
                LogLines.Add($"[ECC] {LocalizationService.T("ecc.cloning_repo")}...");

                var cachePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".cache", "cursor-ecc", "ecc");

                // Check if already exists
                if (Directory.Exists(cachePath))
                {
                    LogLines.Add($"[ECC] {LocalizationService.T("ecc.repo_exists")}");
                    IsECCInstalled = true;
                    return;
                }

                // Clone using git
                var gitClone = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "git",
                        Arguments = $"clone --branch main --depth 1 https://github.com/affaan-m/ECC \"{cachePath}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                    }
                };

                gitClone.Start();
                await gitClone.WaitForExitAsync();

                if (gitClone.ExitCode == 0)
                {
                    LogLines.Add($"[ECC] {LocalizationService.T("ecc.repo_cloned")}: {cachePath}");
                    IsECCInstalled = true;
                    RefreshStatus();
                }
                else
                {
                    var error = await gitClone.StandardError.ReadToEndAsync();
                    LogLines.Add($"[ECC] {LocalizationService.T("ecc.clone_failed")}: {error}");
                }
            }
            catch (Exception ex)
            {
                LogLines.Add($"[ECC] {LocalizationService.T("ecc.error")}: {ex.Message}");
            }
        }

        void RefreshStatus()
        {
            var possiblePaths = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cache", "cursor-ecc", "ecc"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".claude"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ecc"),
            };

            IsECCInstalled = possiblePaths.Any(Directory.Exists);

            var workspace = _workspaceRoot();
            foreach (var item in HarnessItems)
            {
                item.IsInstalled = ResolveHarnessPath(workspace, item.InstallPath) is string path &&
                    (Directory.Exists(path) || File.Exists(path));
            }
        }

        static string? ResolveHarnessPath(string workspace, string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return null;
            if (relativePath.StartsWith("~"))
            {
                var suffix = relativePath.Substring(1).TrimStart('/', '\\');
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), suffix);
            }

            return Path.IsPathRooted(relativePath)
                ? relativePath
                : Path.Combine(workspace, relativePath);
        }

        void AppendLog(string line)
        {
            if (Application.Current?.Dispatcher.CheckAccess() == true)
            {
                LogLines.Add(line);
                return;
            }

            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() => LogLines.Add(line));
                return;
            }

            LogLines.Add(line);
        }

        async Task InstallHarnessAsync(object? parameter)
        {
            if (parameter is not HarnessItem harness)
                return;

            try
            {
                var tool = VibeCli.MapTool(harness.Type);
                if (tool == null)
                {
                    AppendLog($"[VIBE] Tool '{harness.Type}' is not supported by vibe sync.");
                    return;
                }

                AppendLog($"[VIBE] {LocalizationService.T("ecc.installing")} {harness.Name}...");
                var workspace = _workspaceRoot();
                var commands = new[]
                {
                    $"vibe sync --tool {tool}",
                    $"vibe install-plugins --tool {tool}",
                };

                foreach (var command in commands)
                {
                    int? exitCode = null;
                    Action<int> onExit = code => exitCode = code;
                    _vibeRunner.ProcessExited += onExit;
                    AppendLog($"[VIBE] {command} --workspace \"{workspace}\"");
                    try
                    {
                        await _vibeRunner.RunCommandAsync(command, workspace, 120);
                    }
                    finally
                    {
                        _vibeRunner.ProcessExited -= onExit;
                    }

                    if (exitCode != 0)
                    {
                        AppendLog($"[VIBE] {harness.Name} {LocalizationService.T("ecc.install_failed")} (exit {exitCode?.ToString() ?? "unknown"})");
                        return;
                    }
                }

                AppendLog($"[VIBE] {harness.Name} {LocalizationService.T("ecc.install_success")}");
                harness.IsInstalled = true;
            }
            catch (Exception ex)
            {
                AppendLog($"[VIBE] {LocalizationService.T("ecc.error")}: {ex.Message}");
            }
        }
    }
}
