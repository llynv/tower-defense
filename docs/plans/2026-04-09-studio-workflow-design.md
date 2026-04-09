# Studio Workflow Design

**Goal:** Transform the AI assistant into a virtual game studio with 8 core roles that automatically route tasks through the correct pipeline -- from design through implementation to QA -- simulating how a real indie studio operates.

**Approach:** AGENTS.md acts as the concise command center (router + rules). Detailed role definitions, workflow pipelines, and phase guides live in `docs/studio/` and are loaded on-demand.

## Project Context

- **Engine:** Unity 6000.3.6f1, URP 2D, C#
- **Game:** 2.5D lane-based tower defense with Tiny Swords assets
- **Current state:** Vertical slice complete (combat, waves, economy, placement, UI)
- **Current phase:** Production

## File Structure

```
AGENTS.md                      -- Command center: project identity, team roster,
                                  task router, pipeline summaries, gate rules,
                                  phase detection, GitNexus section (kept as-is)

docs/studio/roles.md           -- 8 role definitions with responsibilities,
                                  ownership boundaries, skill mappings

docs/studio/workflows.md       -- Pipeline definitions with step-by-step
                                  skill invocations, artifact handoffs, error handling

docs/studio/phase-guide.md     -- Lifecycle phases, entry/exit criteria,
                                  active pipelines per phase, transition rules
```

## Studio Team (8 Roles)

### 1. Studio Producer
- **Owns:** Sprint planning, milestone tracking, scope control, cross-role coordination
- **Skills:** ccgs-producer, ccgs-sprint-plan, ccgs-sprint-status, ccgs-scope-check, ccgs-estimate, ccgs-milestone-review, ccgs-gate-check
- **Key rule:** Default entry point for all tasks. Classifies requests and routes to correct pipeline.

### 2. Creative Director
- **Owns:** Game vision, tone, aesthetic direction, resolves cross-department conflicts
- **Skills:** ccgs-creative-director, ccgs-brainstorm
- **Key rule:** Only invoked for vision-level decisions or when departments conflict.

### 3. Game Designer
- **Owns:** Mechanics, systems, balance, GDDs, progression curves, economy
- **Skills:** ccgs-game-designer, ccgs-design-system, ccgs-quick-design, ccgs-design-review, ccgs-balance-check, ccgs-economy-designer, ccgs-systems-designer, ccgs-level-designer
- **Key rule:** Every feature gets a design spec before code is written.

### 4. Lead Programmer
- **Owns:** Code architecture, coding standards, API design, code review, tech debt
- **Skills:** ccgs-lead-programmer, ccgs-architecture-decision, ccgs-create-architecture, ccgs-code-review, ccgs-tech-debt, ccgs-create-control-manifest
- **Key rule:** Reviews all code for architecture compliance. Creates ADRs for significant decisions.

### 5. Gameplay Programmer
- **Owns:** Implements mechanics, tower logic, enemy behavior, combat systems, UI code
- **Skills:** ccgs-gameplay-programmer, ccgs-dev-story, ccgs-unity-specialist, ccgs-ui-programmer
- **Key rule:** Follows TDD. Uses systematic-debugging for bugs. Implements what the designer specifies.

### 6. Technical Artist
- **Owns:** Shaders, VFX, sprite sorting, art pipeline, visual performance
- **Skills:** ccgs-technical-artist, ccgs-unity-shader-specialist, ccgs-perf-profile, ccgs-asset-audit
- **Key rule:** Consulted for any visual/rendering changes. Owns the art-to-engine pipeline.

### 7. QA Lead
- **Owns:** Test strategy, bug triage, quality gates, release readiness
- **Skills:** ccgs-qa-lead, ccgs-qa-plan, ccgs-smoke-check, ccgs-regression-suite, ccgs-bug-triage, ccgs-release-checklist
- **Key rule:** Defines what "done" means. No feature ships without QA sign-off.

### 8. QA Tester
- **Owns:** Test case writing, bug reports, test execution, manual verification
- **Skills:** ccgs-qa-tester, ccgs-bug-report, ccgs-test-evidence-review, ccgs-test-helpers
- **Key rule:** Writes test cases before verification. Every bug gets a structured report.

## Task Classification

The Producer classifies every user request into one of these pipeline types:

| Classification | Trigger Examples | Pipeline |
|---|---|---|
| FEATURE | "add a new tower type", "implement upgrade system" | Design -> Architecture -> Implement -> QA |
| BUGFIX | "towers aren't shooting", "fix the health bar" | Report -> Debug -> Fix -> Verify |
| BALANCE | "enemies are too fast", "tune the economy" | Analyze -> Adjust -> Verify |
| POLISH | "improve the VFX", "optimize performance" | Assess -> Implement -> Verify |
| PLANNING | "plan next sprint", "what should we work on" | Analyze -> Plan -> Approve |
| DESIGN-ONLY | "design an upgrade system", "brainstorm new enemy types" | Vision -> Design -> Approve |

## Pipeline Definitions

### FEATURE Pipeline
```
1. [Game Designer]     Write design spec
                       - Small change: ccgs-quick-design
                       - New system: ccgs-design-system
                       - Output: design spec document

2. [Lead Programmer]   Architecture review
                       - Review spec for implementability
                       - Create ADR if new architectural decision needed
                       - Output: approved spec + ADR (if needed)

3. [Gameplay Programmer] Implementation
                       - Write failing tests first (TDD)
                       - Implement to pass tests
                       - Skill: ccgs-dev-story + test-driven-development
                       - Output: working code + passing tests

4. [QA Tester]         Verification
                       - Write additional test cases
                       - Verify acceptance criteria from design spec
                       - Skill: ccgs-qa-tester + ccgs-test-evidence-review
                       - Output: test report

5. [GATE]              Present to user for approval
                       - Summary of what was designed, built, and tested
                       - Any open concerns from any role
```

### BUGFIX Pipeline
```
1. [QA Tester]         Bug report
                       - Structured report: repro steps, severity, context
                       - Skill: ccgs-bug-report
                       - Output: bug report

2. [Gameplay Programmer] Debug and fix
                       - Use systematic-debugging skill
                       - Write regression test for the bug
                       - Output: fix + regression test

3. [QA Tester]         Verify fix
                       - Confirm bug is fixed
                       - Check for regressions
                       - Skill: ccgs-test-evidence-review
                       - Output: verification report

4. [GATE]              Present to user
```

### BALANCE Pipeline
```
1. [Game Designer]     Balance analysis
                       - Run ccgs-balance-check on relevant data
                       - Propose specific number changes with rationale
                       - Output: balance change proposal

2. [Gameplay Programmer] Apply changes
                       - Modify ScriptableObject data / config values
                       - Output: updated data files

3. [QA Tester]         Verify
                       - Ensure game still functions with new values
                       - Output: verification report

4. [GATE]              Present to user
```

### POLISH Pipeline
```
1. [Technical Artist / Lead Programmer] Assessment
                       - Technical Artist for visual/rendering issues
                       - Lead Programmer for performance/architecture issues
                       - Output: improvement plan

2. [Gameplay Programmer] Implementation
                       - Apply improvements
                       - Output: working changes

3. [QA Tester]         Verify
                       - Ensure no regressions
                       - Output: verification report

4. [GATE]              Present to user
```

### PLANNING Pipeline
```
1. [Producer]          Analysis and planning
                       - Assess current state (ccgs-sprint-status or ccgs-project-stage-detect)
                       - Plan next steps (ccgs-sprint-plan)
                       - Output: sprint plan or milestone plan

2. [GATE]              Present plan to user for approval
```

### DESIGN-ONLY Pipeline
```
1. [Creative Director] Vision check
                       - Validate request aligns with game vision
                       - Only invoked if design touches core identity
                       - Output: vision alignment confirmation

2. [Game Designer]     Design authoring
                       - Full GDD or quick design spec
                       - Skill: ccgs-design-system or ccgs-quick-design
                       - Output: design document

3. [GATE]              Present design to user for approval
```

## Gate Rules

Gates are approval checkpoints where the AI pauses and presents results to the user.

### Mandatory Gates (always stop)
- End of every pipeline (final approval)
- Phase transitions (e.g., moving from Production to Polish)
- Any HIGH/CRITICAL risk from GitNexus impact analysis
- Architecture decisions that constrain future options (ADRs)
- Design specs for new systems (not quick-designs for small changes)

### Optional Gates (AI proceeds unless the change is risky)
- Quick-design specs for small changes within existing systems
- Balance tweaks within existing value ranges
- Bug fixes for clearly-defined issues

### Gate Presentation Format
```
## [ROLE] [PIPELINE] Complete

**What was done:**
- [Bullet summary of each step and its output]

**Artifacts produced:**
- [List of files created/modified]

**Open concerns:**
- [Any issues flagged by any role during the pipeline]

**Decision needed:**
- [Approve / Request changes / Reject]
```

## Phase Detection

The Producer auto-detects the current phase on session start by checking for artifacts:

| Phase | Required Artifacts | Missing Artifacts |
|---|---|---|
| Concept | (none) | docs/game-concept.md |
| Pre-Production (Design) | game-concept.md | docs/design/gdd-*.md |
| Pre-Production (Architecture) | GDDs exist | docs/architecture/architecture.md |
| Production | Architecture + code exists | All core features per GDDs |
| Polish | Core features complete | Performance/balance targets met |
| Release | Polish targets met | Release checklist complete |

Current project state: **Production** (vertical slice complete, design docs exist, code is active).

## Integration with Existing Tools

### GitNexus
The existing GitNexus section in AGENTS.md stays unchanged. The studio workflow inherits its rules:
- Impact analysis before editing symbols
- detect_changes before committing
- HIGH/CRITICAL risk warnings trigger a mandatory gate

### CCGS Skills
Skills are invoked by name via the `skill` tool. The AGENTS.md tells the AI WHICH skill to use for each pipeline step. The skill content provides the HOW.

### Brainstorming Skill
The existing brainstorming skill is used as the entry point for DESIGN-ONLY and FEATURE pipelines when the request is open-ended ("I want something like...").

### Test-Driven Development
The TDD skill is mandatory for the Gameplay Programmer role during implementation steps.

### Verification Before Completion
The verification-before-completion skill is mandatory before any gate presentation.

## What This Does NOT Cover

- Asset creation (sprites, models, audio) -- this is a code-focused studio
- Marketing, community management, monetization
- Multiplayer networking (not relevant to this project)
- Platform-specific builds or store submissions (premature)
