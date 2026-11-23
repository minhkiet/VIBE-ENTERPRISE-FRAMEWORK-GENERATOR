#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CursorSetupWpf.Services
{
    /// <summary>
    /// Builds arguments for the real <c>vibe_framework</c> CLI used by the GUI.
    /// Kept free of WPF dependencies so it can be unit-tested directly.
    /// </summary>
    public static class VibeCli
    {
        // The nine tool ids accepted by `vibe_framework vibe --tool`.
        public static readonly string[] SupportedTools =
        {
            "cursor", "windsurf", "claude-code", "antigravity", "cline",
            "roo", "copilot", "aider", "codex",
        };

        /// <summary>
        /// Returns the process arguments after the Python executable.
        /// </summary>
        public static IReadOnlyList<string> BuildArguments(string command, string targetPath)
        {
            var parts = command.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            var args = new List<string> { "-m", "vibe_framework" };
            args.AddRange(parts);
            // `vibe` uses --workspace; all legacy framework commands use --root.
            args.Add(parts.Length > 0 && string.Equals(parts[0], "vibe", System.StringComparison.OrdinalIgnoreCase)
                ? "--workspace"
                : "--root");
            args.Add(targetPath);
            return args;
        }

        /// <summary>
        /// Human-readable command for the GUI log.
        /// </summary>
        public static string FormatCommand(string executable, IEnumerable<string> arguments)
        {
            var sb = new StringBuilder(executable);
            foreach (var arg in arguments)
            {
                sb.Append(' ');
                if (arg.Any(char.IsWhiteSpace))
                {
                    sb.Append('"').Append(arg.Replace("\"", "\\\"")).Append('"');
                }
                else
                {
                    sb.Append(arg);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Maps a GUI harness id to a CLI tool id. Unsupported ids return null.
        /// </summary>
        public static string? MapTool(string? toolId)
        {
            var normalized = toolId?.Trim().ToLowerInvariant().Replace('_', '-');
            return normalized switch
            {
                "cursor" => "cursor",
                "windsurf" => "windsurf",
                "claude" or "claude-code" => "claude-code",
                "antigravity" or "gemini" or "gemini-cli" => "antigravity",
                "cline" => "cline",
                "roo" or "roo-code" => "roo",
                "copilot" => "copilot",
                "aider" => "aider",
                "codex" => "codex",
                _ => null,
            };
        }
    }
}
