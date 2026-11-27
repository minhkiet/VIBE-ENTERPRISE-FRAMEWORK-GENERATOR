# Superpowers Plugin Rules (v7.0.2 Methodology)

Whenever building new capabilities, fixing bugs, or refactoring, the agent MUST adhere to the Superpowers methodology:

## 1. Using Superpowers (Meta Bootstrap)
- Invoke relevant skills BEFORE any response or action (including clarifying questions, exploratory reads, or planning).
- Never rationalize skipping a skill: process skills (brainstorming, systematic-debugging) precede domain implementation skills.

## 2. Brainstorming Before Implementation
- Draw out user intent, explore technical tradeoffs, and write a concrete specification/design before writing code.
- Ask questions only when the answer would materially change what gets built.

## 3. Worktree & Environment Isolation
- Use isolated workspaces (`using-git-worktrees`) on separate branches to keep main worktree clean.

## 4. Atomic Planning
- Decompose complex features into bite-sized tasks (2-5 minutes each) with explicit file paths and verification steps.

## 5. Subagent-Driven Development & Executing Plans
- Fresh subagents per task with two-stage reviews (spec compliance, then code quality).
- Maker vs independent Verifier pattern.

## 6. Strict Test-Driven Development (TDD)
- Enforce RED-GREEN-REFACTOR cycle. Test must fail first for the expected reason before minimal implementation code is written.

## 7. Systematic Debugging
- 4-Phase Protocol: Reproduce -> Isolate -> Hypothesize -> Verify. Zero guesswork or trial-and-error fixes.

## 8. Verification Before Completion
- Verify all tests pass, syntax is clean, and real evidence is gathered before claiming a task is done.
