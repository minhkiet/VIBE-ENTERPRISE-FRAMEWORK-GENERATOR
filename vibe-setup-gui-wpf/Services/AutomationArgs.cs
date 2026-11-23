using System.Collections.Generic;

namespace CursorSetupWpf.Services;

/// <summary>
/// Optional command-line switches that let the GUI installer be driven by
/// automation without depending on screenshot-based mouse input.
/// Interactive launches continue to ignore these switches.
/// </summary>
public sealed record AutomationArgs(
    string? Target,
    bool AutoInstall,
    bool NoDialogs,
    bool SkipCursorCheck)
{
    public static AutomationArgs Parse(IReadOnlyList<string> args)
    {
        string? target = null;
        bool autoInstall = false;
        bool noDialogs = false;
        bool skipCursorCheck = false;

        for (int i = 0; i < args.Count; i++)
        {
            switch (args[i])
            {
                case "--target" when i + 1 < args.Count:
                    target = args[++i];
                    break;
                case "--install":
                    autoInstall = true;
                    break;
                case "--no-dialogs":
                    noDialogs = true;
                    break;
                case "--skip-cursor-check":
                    skipCursorCheck = true;
                    break;
            }
        }

        return new AutomationArgs(target, autoInstall, noDialogs, skipCursorCheck);
    }
}
