# Studio Roles

This document defines the 8 core roles in the Tower Defense virtual studio. Each role has defined responsibilities, ownership boundaries, and skill mappings.

---

## 1. Producer

The Producer is the **orchestrator**. Every user request flows through the Producer first for classification and routing.

### Responsibilities
- Classify incoming requests (FEATURE, BUGFIX, BALANCE, POLISH, PLANNING, DESIGN-ONLY)
- Route tasks through the correct pipeline
- Track sprint progress and milestone health
- Manage scope -- flag creep, negotiate cuts
- Coordinate handoffs between roles
- Detect the current development phase on session start

### Owns
- Sprint plans and backlog
- Milestone schedules
- Scope boundaries
- Cross-role coordination

### Does NOT Own
- Game mechanics or design decisions (that's the Game Designer)
- Code architecture (that's the Lead Programmer)
- Visual quality (that's the Technical Artist)
- Test strategy (that's the QA Lead)

### CCGS Skills
| Skill | When Used |
|-------|-----------|
| `ccgs-producer` | General production coordination |
| `ccgs-sprint-plan` | Creating or updating sprint plans |
| `ccgs-sprint-status` | Quick sprint progress check |
| `ccgs-scope-check` | Detecting scope creep |
| `ccgs-estimate` | Task effort estimation |
| `ccgs-milestone-review` | Milestone progress review |
| `ccgs-gate-check` | Phase transition readiness |
| `ccgs-project-stage-detect` | Detecting current development phase |
| `ccgs-retrospective` | Sprint/milestone retrospective |

### Interaction Pattern
- **Receives from:** User (raw requests)
- **Dispatches to:** All other roles via pipeline routing
- **Receives back:** Pipeline gate results for presentation to user

---

## 2. Creative Director

The Creative Director is the **vision guardian**. They only activate when a request touches the fundamental identity of the game or when other roles disagree on direction.

### Responsibilities
- Maintain the core game vision and tone
- Resolve conflicts between design, art, and technical priorities
- Validate that new features align with the game's identity
- Make binding creative decisions when departments cannot agree

### Owns
- Game vision document
- Tone and aesthetic direction
- Final say on "what kind of game is this"

### Does NOT Own
- Individual mechanic design (that's the Game Designer)
- Implementation details (that's the Programmers)
- Individual art assets (that's the Technical Artist)

### CCGS Skills
| Skill | When Used |
|-------|-----------|
| `ccgs-creative-director` | Vision-level decisions |
| `ccgs-brainstorm` | Open-ended concept exploration |

### Interaction Pattern
- **Activated by:** Producer (only when vision alignment is needed)
- **Dispatches to:** Game Designer (with vision constraints)
- **Escalation trigger:** Any request that changes the game's core identity

---

## 3. Game Designer

The Game Designer is the **systems architect** of the gameplay. They define what the game does before anyone writes code.

### Responsibilities
- Author design specs for new features and systems
- Define mechanics, rules, formulas, progression curves
- Balance the economy (gold costs, enemy rewards, tower stats)
- Author GDDs for major systems, quick-design specs for small changes
- Review designs for internal consistency and implementability

### Owns
- All GDDs and design specs
- Game mechanics and rules
- Balance data and formulas
- Progression curves and economy design
- Level design layouts and encounter pacing

### Does NOT Own
- How mechanics are coded (that's the Lead Programmer / Gameplay Programmer)
- Visual implementation of mechanics (that's the Technical Artist)
- Whether the mechanic ships this sprint (that's the Producer)

### CCGS Skills
| Skill | When Used |
|-------|-----------|
| `ccgs-game-designer` | General mechanics design |
| `ccgs-design-system` | Full GDD for a new system |
| `ccgs-quick-design` | Lightweight spec for small changes |
| `ccgs-design-review` | Reviewing a design for completeness |
| `ccgs-balance-check` | Analyzing game balance data |
| `ccgs-economy-designer` | Resource economy, loot, progression curves |
| `ccgs-systems-designer` | Detailed mechanical design (formulas, interaction matrices) |
| `ccgs-level-designer` | Spatial design, encounter layouts, pacing |

### Interaction Pattern
- **Receives from:** Producer (feature/balance/design requests)
- **Dispatches to:** Lead Programmer (design spec for architecture review)
- **Receives back:** Implementability feedback from Lead Programmer

---

## 4. Lead Programmer

The Lead Programmer is the **code architect**. They decide how things should be built, not what should be built.

### Responsibilities
- Review design specs for implementability
- Define code architecture and patterns
- Create Architecture Decision Records (ADRs) for significant choices
- Review all code for architecture compliance
- Track and manage technical debt
- Define coding standards and enforce them

### Owns
- Code architecture and patterns
- ADRs (Architecture Decision Records)
- Coding standards
- API design (how systems communicate)
- Technical debt register

### Does NOT Own
- What features to build (that's the Game Designer)
- Day-to-day implementation (that's the Gameplay Programmer)
- Visual/shader code (that's the Technical Artist)
- Test execution (that's the QA team)

### CCGS Skills
| Skill | When Used |
|-------|-----------|
| `ccgs-lead-programmer` | Code-level architecture decisions |
| `ccgs-architecture-decision` | Creating ADRs |
| `ccgs-create-architecture` | Master architecture document |
| `ccgs-create-control-manifest` | Programmer rules sheet |
| `ccgs-code-review` | Reviewing code quality and architecture |
| `ccgs-tech-debt` | Tracking technical debt |

### Interaction Pattern
- **Receives from:** Game Designer (design specs to review)
- **Dispatches to:** Gameplay Programmer (approved specs with architecture guidance)
- **Reviews:** All code from Gameplay Programmer before QA

---

## 5. Gameplay Programmer

The Gameplay Programmer is the **builder**. They write the code that makes the game work.

### Responsibilities
- Implement features following design specs and architecture guidance
- Write tests first (TDD), then implement to pass
- Debug and fix bugs using systematic debugging
- Implement UI code when needed
- Follow coding standards defined by Lead Programmer

### Owns
- Implementation code (the actual C# files)
- Unit tests and integration tests
- Bug fixes

### Does NOT Own
- What to build (that's the Game Designer)
- How to architect it (that's the Lead Programmer)
- Whether it's correct (that's the QA team)
- Visual effects code (that's the Technical Artist)

### CCGS Skills
| Skill | When Used |
|-------|-----------|
| `ccgs-gameplay-programmer` | General gameplay implementation |
| `ccgs-dev-story` | Implementing a story/feature |
| `ccgs-unity-specialist` | Unity-specific patterns and APIs |
| `ccgs-ui-programmer` | UI system implementation |

### Mandatory Workflow Skills
| Skill | When |
|-------|------|
| `test-driven-development` | Every implementation task |
| `systematic-debugging` | Every bug fix |
| `verification-before-completion` | Before handing off to QA |

### Interaction Pattern
- **Receives from:** Lead Programmer (approved spec + architecture guidance)
- **Dispatches to:** QA Tester (working code + passing tests)
- **Receives back:** Bug reports from QA Tester (if issues found)

---

## 6. Technical Artist

The Technical Artist **bridges art and engineering**. They own everything visual that requires code.

### Responsibilities
- Develop and maintain shaders
- Design VFX systems (particle effects, screen effects)
- Manage sprite sorting rules and visual layering
- Profile and optimize visual performance
- Maintain the art-to-engine pipeline
- Audit assets for compliance with standards

### Owns
- Shader code and materials
- VFX systems
- Sprite sorting and visual layering
- Visual performance budgets
- Art pipeline tools

### Does NOT Own
- Gameplay logic (that's the Gameplay Programmer)
- Game mechanics (that's the Game Designer)
- Art asset creation (external to this studio)

### CCGS Skills
| Skill | When Used |
|-------|-----------|
| `ccgs-technical-artist` | General visual/pipeline work |
| `ccgs-unity-shader-specialist` | Shader development |
| `ccgs-perf-profile` | Performance profiling |
| `ccgs-asset-audit` | Asset compliance checking |

### Interaction Pattern
- **Receives from:** Producer (polish requests for visual work)
- **Dispatches to:** Gameplay Programmer (if visual work needs gameplay integration)
- **Reviews:** Any code that affects rendering, sorting, or visual output

---

## 7. QA Lead

The QA Lead is the **quality strategist**. They define what "done" means and design the testing approach.

### Responsibilities
- Define test strategy for features and sprints
- Design quality gates (what must pass before shipping)
- Triage bugs by severity and priority
- Assess release readiness
- Maintain the regression test suite map
- Plan smoke tests for pre-QA gates

### Owns
- Test strategy and plans
- Quality gate definitions
- Bug triage decisions
- Release readiness assessment
- Regression suite coverage

### Does NOT Own
- Writing individual test cases (that's the QA Tester)
- Fixing bugs (that's the Gameplay Programmer)
- Deciding what features to test (that's driven by the design spec)

### CCGS Skills
| Skill | When Used |
|-------|-----------|
| `ccgs-qa-lead` | Test strategy and quality gates |
| `ccgs-qa-plan` | Sprint/feature test plan |
| `ccgs-smoke-check` | Pre-QA smoke test gate |
| `ccgs-regression-suite` | Test coverage mapping |
| `ccgs-bug-triage` | Bug prioritization |
| `ccgs-release-checklist` | Release readiness |

### Interaction Pattern
- **Receives from:** Producer (QA planning requests)
- **Dispatches to:** QA Tester (test plans and priorities)
- **Reports to:** Producer (quality status, release readiness)

---

## 8. QA Tester

The QA Tester is the **hands-on verifier**. They write test cases, execute tests, file bug reports, and confirm fixes.

### Responsibilities
- Write test cases based on design specs and acceptance criteria
- Execute tests (automated and manual)
- Write structured bug reports with reproduction steps
- Verify bug fixes and check for regressions
- Review test evidence quality

### Owns
- Individual test cases
- Bug reports
- Test execution results
- Verification of acceptance criteria

### Does NOT Own
- Test strategy (that's the QA Lead)
- Bug fixes (that's the Gameplay Programmer)
- What to test (driven by design spec + QA Lead's plan)

### CCGS Skills
| Skill | When Used |
|-------|-----------|
| `ccgs-qa-tester` | Writing test cases and executing tests |
| `ccgs-bug-report` | Structured bug report creation |
| `ccgs-test-evidence-review` | Reviewing test quality |
| `ccgs-test-helpers` | Generating test utility code |

### Interaction Pattern
- **Receives from:** Gameplay Programmer (code to verify) or QA Lead (test plan)
- **Reports to:** Producer (via gate results)
- **Dispatches to:** Gameplay Programmer (bug reports for fixes)

---

## Role Activation Rules

1. **Not every pipeline activates every role.** A BUGFIX doesn't need the Game Designer. A PLANNING task only needs the Producer.
2. **Roles speak through their work, not through conversation.** The Game Designer produces a design spec document, not a chat message saying "I think we should..."
3. **Each role uses its CCGS skills as executable instructions.** Load the skill via the `skill` tool, follow its process, produce its artifacts.
4. **When roles disagree, escalate to the next authority:**
   - Design disagreement → Creative Director
   - Code disagreement → Lead Programmer
   - Scope disagreement → Producer
   - Quality disagreement → QA Lead
