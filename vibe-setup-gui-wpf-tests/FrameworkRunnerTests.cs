using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using CursorSetupWpf.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CursorSetupWpf.Tests;

[TestClass]
public sealed class FrameworkRunnerTests
{
    [TestMethod]
    public void FindPython_ReturnsRunnableExecutable()
    {
        var method = typeof(FrameworkRunner).GetMethod("FindPython", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);

        var python = (string)method.Invoke(null, null)!;
        var psi = new ProcessStartInfo
        {
            FileName = python,
            Arguments = "--version",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        using var process = Process.Start(psi);
        Assert.IsNotNull(process);
        Assert.IsTrue(process.WaitForExit(3000), "Python probe did not exit in time.");
        Assert.AreEqual(0, process.ExitCode, $"Python probe failed for: {python}");
    }

    [TestMethod]
    public async Task RunCommandAsync_UsesWorkspaceWithSpaces_ForVibeStatus()
    {
        var tempRoot = Path.GetFullPath(Path.GetTempPath());
        var workspace = Path.GetFullPath(Path.Combine(tempRoot, "vibe runner it " + Guid.NewGuid().ToString("N").Substring(0, 8)));
        Assert.IsTrue(workspace.StartsWith(tempRoot, StringComparison.OrdinalIgnoreCase));
        Directory.CreateDirectory(workspace);

        try
        {
            var lines = new List<string>();
            var logs = new List<string>();
            var runner = new FrameworkRunner();
            runner.OutputReceived += line => lines.Add(line);
            runner.LogAppended += line => logs.Add(line);

            await runner.RunCommandAsync("vibe status --json", workspace, 60);

            Assert.IsTrue(lines.Count > 0, "CLI produced no output.");
            var json = string.Join(Environment.NewLine, lines);
            Assert.IsTrue(json.TrimStart().StartsWith("{"), $"Unexpected CLI output: {json}\nRUNNER LOG: {string.Join(Environment.NewLine, logs)}");
            using var doc = JsonDocument.Parse(json);
            var actual = doc.RootElement.GetProperty("workspace_root").GetString();
            Assert.AreEqual(workspace, actual, StringComparer.OrdinalIgnoreCase);
        }
        finally
        {
            if (Directory.Exists(workspace))
            {
                Directory.Delete(workspace, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task RunCommandAsync_InstallsCursorIntoWorkspaceWithSpaces()
    {
        var tempRoot = Path.GetFullPath(Path.GetTempPath());
        var workspace = Path.GetFullPath(Path.Combine(tempRoot, "vibe install it " + Guid.NewGuid().ToString("N").Substring(0, 8)));
        Assert.IsTrue(workspace.StartsWith(tempRoot, StringComparison.OrdinalIgnoreCase));
        Directory.CreateDirectory(workspace);

        try
        {
            var logs = new List<string>();
            var runner = new FrameworkRunner();
            runner.LogAppended += line => logs.Add(line);

            await runner.RunCommandAsync("vibe sync --tool cursor", workspace, 60);
            await runner.RunCommandAsync("vibe install-plugins --tool cursor", workspace, 60);

            var rules = Path.Combine(workspace, ".cursorrules");
            Assert.IsTrue(File.Exists(rules), $"Expected {rules} to be created. Log:\n{string.Join(Environment.NewLine, logs)}");
        }
        finally
        {
            if (Directory.Exists(workspace))
            {
                Directory.Delete(workspace, recursive: true);
            }
        }
    }

    [TestMethod]
    public void PublishConfiguration_BundlesVibeFrameworkModule()
    {
        var projectPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..",
            "vibe-setup-gui-wpf", "CursorSetupWpf.csproj"));
        Assert.IsTrue(File.Exists(projectPath), $"Missing project file: {projectPath}");

        var project = File.ReadAllText(projectPath);
        StringAssert.Contains(project, @"Include=""..\vibe_framework\**\*""");
        StringAssert.Contains(project, "<CopyToPublishDirectory>Always</CopyToPublishDirectory>");
    }

    [TestMethod]
    public void PublishConfiguration_GeneratesPayloadZip()
    {
        var projectPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..",
            "vibe-setup-gui-wpf", "CursorSetupWpf.csproj"));
        Assert.IsTrue(File.Exists(projectPath), $"Missing project file: {projectPath}");

        var project = File.ReadAllText(projectPath);
        StringAssert.Contains(project, "BuildPayloadZipAfterPublish");
        StringAssert.Contains(project, "build-zip.ps1");
    }

    [TestMethod]
    public async Task ProbePythonAsync_FindsUsableInterpreter()
    {
        var tempRoot = Path.GetFullPath(Path.GetTempPath());
        var workspace = Path.Combine(tempRoot, "vibe python probe " + Guid.NewGuid().ToString("N").Substring(0, 8));
        Directory.CreateDirectory(workspace);

        try
        {
            var runtime = await new GitHubSkillService().ProbePythonAsync(workspace);

            Assert.IsTrue(runtime.IsAvailable, runtime.Detail);
            Assert.IsFalse(string.IsNullOrWhiteSpace(runtime.Command), "Probe did not report a command.");
            StringAssert.Contains(runtime.Version, "Python");
        }
        finally
        {
            if (Directory.Exists(workspace)) Directory.Delete(workspace, recursive: true);
        }
    }
}
