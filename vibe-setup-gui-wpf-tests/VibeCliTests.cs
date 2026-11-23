using System.Linq;
using CursorSetupWpf.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CursorSetupWpf.Tests;

[TestClass]
public sealed class VibeCliTests
{
    private const string Target = @"D:\work spaces\vibe target";

    [TestMethod]
    public void BuildArguments_UsesWorkspaceForVibeCommands()
    {
        var args = VibeCli.BuildArguments("vibe sync --tool cursor", Target);

        CollectionAssert.AreEqual(
            new[] { "-m", "vibe_framework", "vibe", "sync", "--tool", "cursor", "--workspace", Target },
            args.ToArray());
    }

    [TestMethod]
    public void BuildArguments_UsesRootForLegacyCommands()
    {
        var args = VibeCli.BuildArguments("scan", Target);

        CollectionAssert.AreEqual(
            new[] { "-m", "vibe_framework", "scan", "--root", Target },
            args.ToArray());
    }

    [TestMethod]
    public void FormatCommand_QuotesTargetWithSpaces()
    {
        var command = VibeCli.FormatCommand("python", VibeCli.BuildArguments("vibe status", Target));

        Assert.AreEqual($"python -m vibe_framework vibe status --workspace \"{Target}\"", command);
    }

    [DataTestMethod]
    [DataRow("claude_code", "claude-code")]
    [DataRow("CLAUDE-CODE", "claude-code")]
    [DataRow("roo", "roo")]
    [DataRow("cursor", "cursor")]
    public void MapTool_MapsGuiIdsToCliIds(string input, string expected)
    {
        Assert.AreEqual(expected, VibeCli.MapTool(input));
    }

    [DataTestMethod]
    [DataRow("grok")]
    [DataRow("zed")]
    [DataRow("kimi")]
    [DataRow("")]
    public void MapTool_RejectsUnsupportedIds(string input)
    {
        Assert.IsNull(VibeCli.MapTool(input));
    }

    [TestMethod]
    public void SupportedTools_ContainAllNineCliTools()
    {
        CollectionAssert.AreEqual(
            new[]
            {
                "cursor", "windsurf", "claude-code", "antigravity", "cline",
                "roo", "copilot", "aider", "codex",
            },
            VibeCli.SupportedTools);
    }
}
