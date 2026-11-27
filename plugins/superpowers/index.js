/**
 * Vibe Framework Superpowers Plugin (v7.0.2)
 * 
 * Exposes core Superpowers commands in Cursor, Windsurf, Cline, and Roo Code:
 * Bootstrap, Brainstorming, Planning, TDD, Systematic Debugging, and Diagnostics.
 */

const vscode = require('vscode');

function activate(context) {
    const commands = [
        {
            id: 'vibe-framework.superpowers.bootstrap',
            message: 'Superpowers: Session bootstrap active. Always invoke relevant skills before responding.'
        },
        {
            id: 'vibe-framework.superpowers.brainstorm',
            message: 'Superpowers: Brainstorming skill activated. Drawing out intent and requirements before code.'
        },
        {
            id: 'vibe-framework.superpowers.plan',
            message: 'Superpowers: Writing-plans skill activated. Decomposing into 2-5 minute bite-sized tasks.'
        },
        {
            id: 'vibe-framework.superpowers.tdd',
            message: 'Superpowers: TDD skill activated. Strict Red-Green-Refactor cycle enforced.'
        },
        {
            id: 'vibe-framework.superpowers.debug',
            message: 'Superpowers: Systematic debugging activated (Reproduce -> Isolate -> Hypothesize -> Verify).'
        },
        {
            id: 'vibe-framework.superpowers.diagnose',
            message: 'Superpowers: Diagnosing superpowers activated. Analyzing transcript evidence.'
        }
    ];

    for (const cmd of commands) {
        context.subscriptions.push(
            vscode.commands.registerCommand(cmd.id, async () => {
                vscode.window.showInformationMessage(cmd.message);
            })
        );
    }
}

function deactivate() {}

module.exports = {
    activate,
    deactivate
};
