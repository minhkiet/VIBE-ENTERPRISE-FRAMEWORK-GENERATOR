import importlib.util
import os
import subprocess
import sys
import unittest
from pathlib import Path
from unittest.mock import patch


REPO_ROOT = Path(__file__).resolve().parents[1]
ZALO_MCP_PATH = REPO_ROOT / "tools" / "api_integrations" / "zalo_mcp_tool.py"
VIBE_CLI_PATH = REPO_ROOT / "tools" / "vibe_framework_cli.py"
IMPORT_SNIPPET = (
    "import sys; sys.modules['requests'] = None; "
    "import tools.api_integrations; print('import-ok')"
)


class TestApiIntegrationsImport(unittest.TestCase):
    def test_package_import_survives_missing_requests(self):
        result = subprocess.run(
            [sys.executable, "-c", IMPORT_SNIPPET],
            cwd=REPO_ROOT,
            capture_output=True,
            text=True,
        )
        self.assertEqual(result.returncode, 0, result.stdout + result.stderr)
        self.assertIn("import-ok", result.stdout)

    def test_zalo_mcp_import_does_not_autoinstall_requests(self):
        spec = importlib.util.spec_from_file_location("_zalo_mcp_probe", ZALO_MCP_PATH)
        module = importlib.util.module_from_spec(spec)

        with patch.dict(sys.modules, {"requests": None}), patch("os.system") as system:
            spec.loader.exec_module(module)
            with self.assertRaises(RuntimeError):
                module.ZaloMCPClient("token")

        system.assert_not_called()

    def test_vibe_cli_import_does_not_autoinstall_dependencies(self):
        spec = importlib.util.spec_from_file_location("_vibe_cli_probe", VIBE_CLI_PATH)
        module = importlib.util.module_from_spec(spec)

        with patch.dict(sys.modules, {"click": None, "rich": None}), patch("subprocess.run") as run:
            with self.assertRaises(ImportError) as ctx:
                spec.loader.exec_module(module)

        run.assert_not_called()
        self.assertIn("pip install click rich", str(ctx.exception))


if __name__ == "__main__":
    unittest.main()
