using Microsoft.VisualStudio.TestTools.UnitTesting;
using CursorSetupWpf.Services;

namespace CursorSetupWpf.Tests;

[TestClass]
public class AutomationArgsTests
{
    [TestMethod]
    public void Parse_ReadsTargetAndAllAutomationFlags()
    {
        var parsed = AutomationArgs.Parse(new[]
        {
            "--target", @"C:\Temp\vibe smoke",
            "--install",
            "--no-dialogs",
            "--skip-cursor-check",
        });

        Assert.AreEqual(@"C:\Temp\vibe smoke", parsed.Target);
        Assert.IsTrue(parsed.AutoInstall);
        Assert.IsTrue(parsed.NoDialogs);
        Assert.IsTrue(parsed.SkipCursorCheck);
    }

    [TestMethod]
    public void Parse_DefaultsToInteractiveMode()
    {
        var parsed = AutomationArgs.Parse(new[] { "--unknown", "value" });

        Assert.IsNull(parsed.Target);
        Assert.IsFalse(parsed.AutoInstall);
        Assert.IsFalse(parsed.NoDialogs);
        Assert.IsFalse(parsed.SkipCursorCheck);
    }
}
