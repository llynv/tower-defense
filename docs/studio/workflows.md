# Studio Workflows

This document defines the step-by-step pipelines that the Tower Defense virtual studio runs for each type of task. Each step specifies the role, the skill to invoke, the expected output, and the handoff to the next step.

---

## How to Read These Pipelines

Each pipeline step follows this format:

```
Step N: [ROLE] — [Action]
  Skill:    [ccgs-skill-name or workflow skill]
  Input:    [What this step receives from the previous step]
  Process:  [What the role does]
  Output:   [What this step produces]
  Handoff:  [Who receives the output next]
```

**Error handling:** If any step fails or raises concerns, the pipeline pauses and the concern is flagged in the gate presentation. The user decides whether to proceed, revise, or abort.

---

## FEATURE Pipeline

**Trigger:** User requests new gameplay, content, system, or feature.
**Example:** "Add a new tower type", "Implement an upgrade system", "Add a second enemy"

### Step 1: Game Designer — Design Spec

```
Skill:    ccgs-quick-design (small change within existing system)
          ccgs-design-system (new system requiring full GDD)
          brainstorming (if request is open-ended or ambiguous)
Input:    User's feature request
Process:
  1. If ambiguous, use brainstorming skill to explore intent
  2. Check existing GDDs for related systems
  3. Write design spec with:
     - What the feature does (player-facing behavior)
     - Data definitions (ScriptableObject fields, values)
     - Interaction with existing systems (dependencies)
     - Acceptance criteria (testable conditions)
Output:   Design spec document (in docs/plans/ or embedded in story)
Handoff:  Lead Programmer
```

### Step 2: Lead Programmer — Architecture Review

```
Skill:    ccgs-lead-programmer
          ccgs-architecture-decision (if new architectural choice needed)
Input:    Design spec from Game Designer
Process:
  1. Review spec for implementability
  2. Identify which existing code is affected (use gitnexus_query)
  3. Run gitnexus_impact on affected symbols
  4. If HIGH/CRITICAL risk: flag and GATE immediately
  5. If new architectural pattern needed: create ADR
  6. Define implementation approach:
     - Which files to create/modify
     - Which patterns to follow
     - Which tests to write
Output:   Approved spec with architecture notes
          ADR document (if applicable)
Handoff:  Gameplay Programmer
```

### Step 3: Gameplay Programmer — Implementation

```
Skill:    ccgs-dev-story + test-driven-development
Input:    Approved spec + architecture notes from Lead Programmer
Process:
  1. Load test-driven-development skill
  2. Write failing tests based on acceptance criteria
  3. Run tests to confirm they fail
  4. Write minimal implementation to pass tests
  5. Run tests to confirm they pass
  6. Refactor if needed (tests must still pass)
  7. Run gitnexus_impact on all modified symbols
  8. Commit with descriptive message
Output:   Working code + passing tests
Handoff:  QA Tester
```

### Step 4: QA Tester — Verification

```
Skill:    ccgs-qa-tester + ccgs-test-evidence-review
Input:    Working code from Gameplay Programmer
Process:
  1. Review acceptance criteria from design spec
  2. Verify each criterion is met
  3. Write additional edge-case tests if gaps found
  4. Check for regressions in related systems
  5. Run full test suite
Output:   Verification report (pass/fail per criterion, any concerns)
Handoff:  GATE
```

### Step 5: GATE — Present to User

```
Skill:    verification-before-completion
Process:
  1. Load verification-before-completion skill
  2. Run final checks (tests pass, no uncommitted changes)
  3. Compile results from all steps
  4. Present gate summary to user
Output:   Gate presentation (see AGENTS.md gate format)
```

---

## BUGFIX Pipeline

**Trigger:** User reports something broken, wrong behavior, or unexpected results.
**Example:** "Towers aren't shooting", "Health bar shows wrong value", "Game crashes on wave 3"

### Step 1: QA Tester — Bug Report

```
Skill:    ccgs-bug-report
Input:    User's bug description
Process:
  1. Reproduce the issue (or document reproduction steps)
  2. Classify severity:
     - CRITICAL: game crash, data loss, unplayable
     - HIGH: major feature broken, no workaround
     - MEDIUM: feature partially broken, workaround exists
     - LOW: cosmetic, minor annoyance
  3. Write structured bug report:
     - Summary
     - Steps to reproduce
     - Expected behavior
     - Actual behavior
     - Severity classification
     - Affected systems
Output:   Structured bug report
Handoff:  Gameplay Programmer
```

### Step 2: Gameplay Programmer — Debug and Fix

```
Skill:    systematic-debugging
Input:    Bug report from QA Tester
Process:
  1. Load systematic-debugging skill
  2. Form hypothesis about root cause
  3. Use gitnexus_query to find relevant execution flows
  4. Use gitnexus_context on suspect functions
  5. Write a regression test that reproduces the bug (test should FAIL)
  6. Fix the root cause
  7. Run regression test (should now PASS)
  8. Run full test suite (no new failures)
  9. Run gitnexus_impact on modified symbols
Output:   Fix + regression test
Handoff:  QA Tester
```

### Step 3: QA Tester — Verify Fix

```
Skill:    ccgs-qa-tester
Input:    Fix from Gameplay Programmer
Process:
  1. Confirm original bug is fixed
  2. Run regression test suite
  3. Spot-check related features for regressions
  4. Run full test suite
Output:   Verification report
Handoff:  GATE
```

### Step 4: GATE — Present to User

```
Skill:    verification-before-completion
Process:  Same as FEATURE pipeline gate
```

---

## BALANCE Pipeline

**Trigger:** User requests tuning, difficulty adjustment, or economy changes.
**Example:** "Enemies are too fast", "Towers cost too much", "Wave 3 is too hard"

### Step 1: Game Designer — Balance Analysis

```
Skill:    ccgs-balance-check
Input:    User's balance concern
Process:
  1. Identify the relevant data (ScriptableObjects, config values)
  2. Analyze current values and their relationships
  3. Propose specific number changes with rationale:
     - What value(s) to change
     - Current value → proposed value
     - Why this change addresses the concern
     - Potential side effects on other systems
Output:   Balance change proposal (specific numbers, not vague directions)
Handoff:  Gameplay Programmer
```

### Step 2: Gameplay Programmer — Apply Changes

```
Skill:    (none required — direct data modification)
Input:    Balance change proposal from Game Designer
Process:
  1. Modify the specified ScriptableObject values or config files
  2. If changes require code modifications, follow TDD
  3. Run existing tests to ensure nothing breaks
Output:   Updated data files (and code if needed)
Handoff:  QA Tester
```

### Step 3: QA Tester — Verify

```
Skill:    ccgs-qa-tester
Input:    Updated data from Gameplay Programmer
Process:
  1. Run test suite — no failures
  2. Verify the balance change addresses the original concern
  3. Spot-check for unintended side effects
Output:   Verification report
Handoff:  GATE
```

### Step 4: GATE — Present to User

```
Skill:    verification-before-completion
Process:  Same as FEATURE pipeline gate
```

---

## POLISH Pipeline

**Trigger:** User requests visual improvement, UX refinement, or performance optimization.
**Example:** "Improve the VFX", "Fix sprite sorting", "Optimize frame rate"

### Step 1: Assessment

```
Role:     Technical Artist (visual/rendering issues)
          Lead Programmer (performance/architecture issues)
Skill:    ccgs-technical-artist or ccgs-perf-profile
Input:    User's polish request
Process:
  1. Identify what needs improvement
  2. Assess current state
  3. Propose specific improvements with expected impact
  4. For performance: profile and identify bottlenecks
  5. For visuals: identify the visual gap and solution
Output:   Improvement plan with priorities
Handoff:  Gameplay Programmer
```

### Step 2: Gameplay Programmer — Implementation

```
Skill:    ccgs-gameplay-programmer (or ccgs-unity-shader-specialist for shader work)
Input:    Improvement plan
Process:
  1. Implement the improvements
  2. Follow TDD where applicable
  3. Run gitnexus_impact on modified symbols
Output:   Working improvements
Handoff:  QA Tester
```

### Step 3: QA Tester — Verify

```
Skill:    ccgs-qa-tester
Input:    Improvements from Gameplay Programmer
Process:
  1. Verify improvement meets the plan
  2. Run test suite — no regressions
  3. For performance: confirm measurable improvement
  4. For visuals: confirm visual correctness
Output:   Verification report
Handoff:  GATE
```

### Step 4: GATE — Present to User

```
Skill:    verification-before-completion
Process:  Same as FEATURE pipeline gate
```

---

## PLANNING Pipeline

**Trigger:** User asks about priorities, sprint planning, milestone tracking, or "what should we work on".
**Example:** "Plan next sprint", "What should we work on next", "How is the sprint going"

### Step 1: Producer — Analysis and Planning

```
Skill:    ccgs-sprint-status (for status check)
          ccgs-sprint-plan (for new sprint planning)
          ccgs-project-stage-detect (for phase assessment)
          ccgs-scope-check (for scope review)
Input:    User's planning request
Process:
  1. Assess current state:
     - Check for in-progress work (git status, open branches)
     - Check existing sprint plan (if any)
     - Check design docs for unimplemented features
  2. Based on request, either:
     - Report current status
     - Create new sprint plan
     - Review scope
     - Recommend priorities
Output:   Sprint plan, status report, or priority recommendation
Handoff:  GATE
```

### Step 2: GATE — Present to User

```
Process:  Present plan/status for user approval
```

---

## DESIGN-ONLY Pipeline

**Trigger:** User wants design work without implementation.
**Example:** "Design an upgrade system", "Brainstorm new enemy types", "How should multiplayer work"

### Step 1: Creative Director — Vision Alignment

```
Skill:    ccgs-creative-director
Input:    User's design request
Process:
  1. Check if the request touches core game identity
  2. If yes: validate alignment with vision, provide constraints
  3. If no: pass through to Game Designer
Output:   Vision alignment confirmation (or constraints)
Handoff:  Game Designer

Note: This step is SKIPPED if the request clearly fits within existing game vision
(e.g., "design a new tower type" for a tower defense game doesn't need vision review).
```

### Step 2: Game Designer — Design Authoring

```
Skill:    ccgs-design-system (full system)
          ccgs-quick-design (small addition)
          brainstorming (open-ended exploration)
Input:    User's request + vision constraints (if any)
Process:
  1. If open-ended: use brainstorming skill first
  2. Author the design document:
     - Player-facing behavior
     - Data definitions
     - System interactions
     - Acceptance criteria
  3. Save to docs/plans/
Output:   Design document
Handoff:  GATE
```

### Step 3: GATE — Present to User

```
Process:  Present design for user feedback/approval
```

---

## Cross-Pipeline Rules

### GitNexus Integration
- **Before modifying code:** The Gameplay Programmer MUST run `gitnexus_impact` on all symbols they plan to modify
- **Before committing:** MUST run `gitnexus_detect_changes` to verify scope
- **If HIGH/CRITICAL risk:** Pipeline pauses, concern is escalated to a mandatory gate

### Skill Loading
- Load each skill via the `skill` tool when the step begins
- Follow the skill's process exactly
- Produce the skill's required artifacts

### Handoff Format
Each role hands off to the next with:
1. A summary of what they did
2. The artifacts they produced (files, documents)
3. Any concerns or flags for the next role

### Pipeline Failure
If a step fails:
1. Document what failed and why
2. Attempt to resolve within the role's authority
3. If unresolvable: escalate to the gate as an open concern
4. The user decides whether to proceed, revise, or abort

### Combining Pipelines
Some requests trigger multiple pipelines. For example:
- "Add a new tower and fix the targeting bug" → FEATURE + BUGFIX
- "Redesign the economy and rebalance everything" → DESIGN-ONLY → BALANCE

The Producer identifies these and runs them sequentially, with gates between each.
