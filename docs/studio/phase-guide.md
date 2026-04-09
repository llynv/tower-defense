# Development Phase Guide

This document defines the lifecycle phases for the Tower Defense project. Each phase has entry criteria, exit criteria (gates), key deliverables, and rules for which pipelines are active.

---

## Phase Overview

```
Concept → Pre-Production (Design) → Pre-Production (Architecture) → Production → Polish → Release
```

The studio auto-detects the current phase on session start. If uncertain, the Producer asks the user.

---

## Phase 1: Concept

### What Happens Here
The game idea is explored, refined, and documented into a single game concept document that everyone can reference.

### Entry Criteria
- A game idea exists (even if vague)

### Exit Criteria (Gate to Pre-Production)
- `docs/plans/game-concept.md` exists and is approved
- Core pillars defined (what makes this game fun)
- Target audience identified
- Scope boundaries set (what this game is NOT)

### Detection Signal
No `docs/plans/game-concept.md` exists.

### Active Pipelines
- **DESIGN-ONLY** (brainstorming, concept exploration)
- **PLANNING** (initial scope and priority setting)

### Key Deliverables
| Artifact | Location | Owner |
|----------|----------|-------|
| Game concept document | `docs/plans/game-concept.md` | Creative Director + Game Designer |

### Primary Roles
- Creative Director (vision)
- Game Designer (mechanics concept)
- Producer (scope)

### Skills Used
- `ccgs-brainstorm` — guided concept ideation
- `ccgs-creative-director` — vision decisions
- `brainstorming` — general creative exploration

---

## Phase 2: Pre-Production (Design)

### What Happens Here
The game concept is decomposed into individual systems. Each system gets a Game Design Document (GDD) that fully specifies its mechanics, data, and interactions.

### Entry Criteria
- Game concept document exists and is approved

### Exit Criteria (Gate to Architecture)
- All core systems have GDDs (`docs/plans/gdd-*.md`)
- Cross-GDD consistency review passes
- Art bible exists (if visual identity is important)
- Systems dependency map is complete

### Detection Signal
Concept exists but no `docs/plans/gdd-*.md` files.

### Active Pipelines
- **DESIGN-ONLY** (GDD authoring, system design)
- **PLANNING** (design sprint planning)

### Key Deliverables
| Artifact | Location | Owner |
|----------|----------|-------|
| System GDDs | `docs/plans/gdd-*.md` | Game Designer |
| Systems index | `docs/plans/systems-index.md` | Game Designer |
| Art bible | `docs/plans/art-bible.md` | Technical Artist |
| Cross-GDD review | (review pass, not a file) | Game Designer |

### Primary Roles
- Game Designer (all GDDs)
- Creative Director (vision alignment reviews)
- Technical Artist (art bible)
- Producer (design sprint planning)

### Skills Used
- `ccgs-map-systems` — decompose concept into systems
- `ccgs-design-system` — author individual GDDs
- `ccgs-design-review` — review GDD completeness
- `ccgs-review-all-gdds` — cross-GDD consistency
- `ccgs-art-bible` — visual identity spec

---

## Phase 3: Pre-Production (Architecture)

### What Happens Here
GDDs are translated into a technical architecture. Key decisions are documented as ADRs. The codebase structure is planned before implementation begins.

### Entry Criteria
- All core GDDs exist and pass review

### Exit Criteria (Gate to Production)
- `docs/architecture/architecture.md` exists
- ADRs created for all significant technical decisions
- Control manifest exists (programmer rules sheet)
- Test framework is set up
- First sprint is planned

### Detection Signal
GDDs exist but no `docs/architecture/architecture.md`.

### Active Pipelines
- **DESIGN-ONLY** (architecture design)
- **PLANNING** (sprint planning for production)

### Key Deliverables
| Artifact | Location | Owner |
|----------|----------|-------|
| Architecture document | `docs/architecture/architecture.md` | Lead Programmer |
| ADRs | `docs/architecture/adr-*.md` | Lead Programmer |
| Control manifest | `docs/architecture/control-manifest.md` | Lead Programmer |
| Test framework | `Assets/_Game/Tests/` | QA Lead + Gameplay Programmer |
| Sprint 1 plan | `docs/production/sprint-*.md` | Producer |

### Primary Roles
- Lead Programmer (architecture, ADRs)
- QA Lead (test strategy)
- Producer (sprint planning)
- Game Designer (clarifying design questions)

### Skills Used
- `ccgs-create-architecture` — master architecture document
- `ccgs-architecture-decision` — individual ADRs
- `ccgs-create-control-manifest` — programmer rules
- `ccgs-test-setup` — test framework scaffolding
- `ccgs-sprint-plan` — first sprint planning

---

## Phase 4: Production

### What Happens Here
Features are implemented sprint by sprint. The full pipeline operates: design → architecture → implement → test. This is where most of the game gets built.

### Entry Criteria
- Architecture document exists
- Test framework is set up
- At least one sprint is planned

### Exit Criteria (Gate to Polish)
- All core features specified in GDDs are implemented
- All features have test coverage
- No CRITICAL or HIGH severity bugs open
- Game is playable end-to-end

### Detection Signal
Architecture exists and code in `Assets/_Game/Scripts/`.

### Active Pipelines
- **FEATURE** (primary — most work happens here)
- **BUGFIX** (as bugs are found)
- **BALANCE** (as systems come online)
- **PLANNING** (sprint management)
- **DESIGN-ONLY** (if new design needs arise)

### Key Deliverables
| Artifact | Location | Owner |
|----------|----------|-------|
| Feature code | `Assets/_Game/Scripts/` | Gameplay Programmer |
| Tests | `Assets/_Game/Tests/` | Gameplay Programmer + QA Tester |
| Sprint reports | `docs/production/sprint-*.md` | Producer |
| Bug reports | (tracked in pipeline gates) | QA Tester |

### Primary Roles
All 8 roles are active. The Gameplay Programmer and QA Tester do most of the hands-on work.

### Skills Used
All pipeline skills are active. Most-used:
- `ccgs-dev-story` + `test-driven-development` — implementation
- `ccgs-qa-tester` — verification
- `ccgs-sprint-plan` + `ccgs-sprint-status` — sprint management
- `ccgs-quick-design` — small feature specs
- `ccgs-code-review` — code quality

---

## Phase 5: Polish

### What Happens Here
The game works but needs refinement. Performance optimization, visual polish, balance tuning, bug fixing, and UX improvements.

### Entry Criteria
- All core features are implemented
- Game is playable end-to-end
- No CRITICAL bugs

### Exit Criteria (Gate to Release)
- Performance targets met (stable frame rate)
- Balance review passes (no degenerate strategies, fair difficulty curve)
- No HIGH or CRITICAL bugs
- UX review passes (clear feedback, readable UI)
- Regression test suite is comprehensive

### Detection Signal
All GDD features implemented (requires manual assessment or Producer judgment).

### Active Pipelines
- **POLISH** (primary)
- **BUGFIX** (bug squashing)
- **BALANCE** (final tuning)
- **PLANNING** (polish sprint planning)

### Key Deliverables
| Artifact | Location | Owner |
|----------|----------|-------|
| Performance report | (in pipeline gates) | Technical Artist |
| Balance report | (in pipeline gates) | Game Designer |
| Regression suite | `Assets/_Game/Tests/` | QA Lead + QA Tester |
| Polish checklist | `docs/production/polish-checklist.md` | Producer |

### Primary Roles
- Technical Artist (visual polish, performance)
- Game Designer (balance)
- QA Lead + QA Tester (bug squashing, regression testing)
- Gameplay Programmer (fixes and improvements)

### Skills Used
- `ccgs-perf-profile` — performance profiling
- `ccgs-balance-check` — balance analysis
- `ccgs-regression-suite` — test coverage
- `ccgs-smoke-check` — smoke test gates
- `ccgs-bug-triage` — bug prioritization
- `systematic-debugging` — bug fixing

---

## Phase 6: Release

### What Happens Here
Final validation, release checklist, and launch preparation.

### Entry Criteria
- Polish targets met
- No HIGH or CRITICAL bugs
- Regression suite passes

### Exit Criteria
- Release checklist complete
- Build is stable
- Launch artifacts prepared

### Detection Signal
Polish phase complete + release checklist passes (requires manual assessment).

### Active Pipelines
- **PLANNING** (release coordination)
- **BUGFIX** (critical-only fixes)

### Key Deliverables
| Artifact | Location | Owner |
|----------|----------|-------|
| Release checklist | `docs/release/release-checklist.md` | QA Lead |
| Final build | (Unity build output) | Lead Programmer |

### Primary Roles
- Producer (coordination)
- QA Lead (release readiness)
- Lead Programmer (build stability)

### Skills Used
- `ccgs-release-checklist` — release validation
- `ccgs-launch-checklist` — launch readiness
- `ccgs-smoke-check` — final smoke test

---

## Phase Transition Rules

1. **Phase transitions are mandatory gates.** The AI always stops and presents the phase gate to the user.
2. **Phases can overlap.** You might do polish work during production. The phase label represents the PRIMARY focus.
3. **Phase regression is allowed.** If production reveals a design flaw, the team can drop back to design work without formally changing phases.
4. **The Producer detects and reports the phase** at the start of every session using the detection signals above.
5. **When in doubt, ask the user.** Phase detection is heuristic. If artifacts don't clearly indicate a phase, the Producer asks.

---

## Quick Reference: What's Active When

| Pipeline | Concept | Pre-Prod (Design) | Pre-Prod (Arch) | Production | Polish | Release |
|----------|---------|-------------------|-----------------|------------|--------|---------|
| FEATURE | - | - | - | **Primary** | - | - |
| BUGFIX | - | - | - | Active | Active | Critical only |
| BALANCE | - | - | - | Active | Active | - |
| POLISH | - | - | - | - | **Primary** | - |
| PLANNING | Active | Active | Active | Active | Active | Active |
| DESIGN-ONLY | **Primary** | **Primary** | Active | Active | - | - |
