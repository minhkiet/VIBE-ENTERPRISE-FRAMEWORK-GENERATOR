"""Tests for image-related MCP configuration generation."""

import json
import unittest
import tempfile
from pathlib import Path

from tools.mcp_config_generator import McpConfigGenerator


class TestImageMcpConfig(unittest.TestCase):
    def test_generator_includes_file_ops_by_default(self):
        repo_root = Path(__file__).resolve().parent.parent
        generator = McpConfigGenerator(str(repo_root), target="cursor")

        self.assertIn("file-ops", generator.config.to_dict()["mcpServers"])

    def test_generator_includes_pinterest_by_default(self):
        repo_root = Path(__file__).resolve().parent.parent
        generator = McpConfigGenerator(str(repo_root), target="cursor")

        self.assertIn("pinterest", generator.config.to_dict()["mcpServers"])

    def test_generator_includes_codegraph_by_default(self):
        repo_root = Path(__file__).resolve().parent.parent
        generator = McpConfigGenerator(str(repo_root), target="cursor")

        self.assertIn("codegraph", generator.config.to_dict()["mcpServers"])

    def test_generator_adds_codegraph_server(self):
        repo_root = Path(__file__).resolve().parent.parent
        generator = McpConfigGenerator(str(repo_root), target="cursor")

        generator.add_codegraph()

        server = generator.config.to_dict()["mcpServers"]["codegraph"]
        self.assertEqual(server["command"], generator.python_cmd)
        self.assertTrue(any("framework_server.py" in arg for arg in server["args"]))
        self.assertIn("PYTHONPATH", server["env"])

    def test_generator_adds_file_ops_server(self):
        repo_root = Path(__file__).resolve().parent.parent
        generator = McpConfigGenerator(str(repo_root), target="cursor")

        generator.add_file_ops()

        server = generator.config.to_dict()["mcpServers"]["file-ops"]
        self.assertEqual(server["command"], generator.python_cmd)
        self.assertTrue(any("file_server.py" in arg for arg in server["args"]))
        self.assertIn("PYTHONPATH", server["env"])

    def test_generator_browser_server_uses_supported_pinned_package(self):
        repo_root = Path(__file__).resolve().parent.parent
        generator = McpConfigGenerator(str(repo_root), target="cursor")

        generator.add_browser()

        server = generator.config.to_dict()["mcpServers"]["browser"]
        self.assertEqual(server["command"], "npx")
        self.assertIn("@playwright/mcp@0.0.86", server["args"])
        self.assertNotIn("@modelcontextprotocol/server-browser", server["args"])

    def test_generator_adds_pinterest_server(self):
        repo_root = Path(__file__).resolve().parent.parent
        generator = McpConfigGenerator(str(repo_root), target="cursor")

        generator.add_pinterest()

        server = generator.config.to_dict()["mcpServers"]["pinterest"]
        self.assertEqual(server["command"], generator.python_cmd)
        self.assertTrue(any("pinterest_server.py" in arg for arg in server["args"]))
        self.assertIn("PYTHONPATH", server["env"])

    def test_generator_adds_gflow_cli_server(self):
        repo_root = Path(__file__).resolve().parent.parent
        generator = McpConfigGenerator(str(repo_root), target="cursor")

        generator.add_gflow()

        server = generator.config.to_dict()["mcpServers"]["gflow-cli"]
        self.assertEqual(server["command"], "gflow")
        self.assertEqual(server["args"], ["mcp", "run"])
        self.assertNotIn("--no-spend", server["args"])
        self.assertIn("Nano Banana", server["description"])

    def test_generator_adds_gflow_cli_readonly_server(self):
        repo_root = Path(__file__).resolve().parent.parent
        generator = McpConfigGenerator(str(repo_root), target="cursor")

        generator.add_gflow(read_only=True)

        server = generator.config.to_dict()["mcpServers"]["gflow-cli-readonly"]
        self.assertEqual(server["command"], "gflow")
        self.assertEqual(server["args"], ["mcp", "run", "--no-spend"])
        self.assertIn("read-only", server["description"].lower())

    def test_codex_target_saves_gflow_to_config_toml(self):
        repo_root = Path(__file__).resolve().parent.parent
        generator = McpConfigGenerator(str(repo_root), target="codex")
        generator.add_gflow()

        with tempfile.TemporaryDirectory() as tmp:
            path = Path(tmp) / "config.toml"
            path.write_text(
                '[mcp_servers.existing]\ncommand = "existing"\n\n[features]\nfast_mode = true\n',
                encoding="utf-8",
            )

            generator.save(str(path))
            text = path.read_text(encoding="utf-8")

            self.assertIn("[mcp_servers.gflow-cli]", text)
            self.assertIn('command = "gflow"', text)
            self.assertIn('args = ["mcp", "run"]', text)
            self.assertIn("[mcp_servers.existing]", text)
            self.assertIn("[features]", text)

    def test_packager_stages_all_local_mcp_scripts(self):
        repo_root = Path(__file__).resolve().parent.parent
        script = (repo_root / "vibe-setup-gui-wpf" / "build-zip.ps1").read_text(encoding="utf-8")

        self.assertIn("pinterest_server.py", script)
        self.assertIn("file_server.py", script)
        self.assertIn("framework_server.py", script)
        self.assertIn("mcp_runtime.py", script)
        self.assertIn("tools\\rea", script)
        self.assertIn("$archiveTotal = @(Get-ChildItem -Path $verifyDir -Recurse -File -Force", script)
        self.assertIn("total_files = $archiveTotal", script)

    def test_shared_template_registers_codegraph(self):
        repo_root = Path(__file__).resolve().parent.parent
        template = json.loads(
            (repo_root / "tools" / "mcp-config-template.json").read_text(encoding="utf-8")
        )

        self.assertIn("codegraph", template["mcpServers"])
        self.assertEqual(template["_meta"]["framework_version"], "4.10.0")

    def test_installer_uses_supported_browser_package_and_file_ops(self):
        repo_root = Path(__file__).resolve().parent.parent
        installer = (repo_root / "vibe-setup-gui-wpf" / "Services" / "Installer.cs").read_text(
            encoding="utf-8"
        )

        self.assertIn("@playwright/mcp@0.0.86", installer)
        self.assertNotIn("@modelcontextprotocol/server-browser", installer)
        self.assertIn('entry.ServerKey == "file-ops"', installer)
        self.assertIn('entry.ServerKey == "codegraph"', installer)


if __name__ == "__main__":
    unittest.main()
