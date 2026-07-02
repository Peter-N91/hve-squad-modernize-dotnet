---
description: "Squad roster: roles and the deployed HVE Core agents that fill them"
---

# Squad Roster

## Members

| Role          | Member Name | Agent Name (Primary)          | Alternate Agents                                          | Invocation         | Model Tier |
|---------------|-------------|-------------------------------|-----------------------------------------------------------|--------------------|------------|
| researcher    | Alpha       | Squad Modernization Planner   | Task Researcher, Researcher Subagent, Codebase Profiler, Meeting Analyst | runSubagent / task | default    |
| lead          | Beta        | Task Planner                  | RPI Agent, Phase Implementor, Task Challenger             | runSubagent / task | default    |
| developer     | Gamma       | Task Implementor              | Phase Implementor                                         | runSubagent / task | default    |
| tester        | Delta       | Task Reviewer                 | Code Review Full, Code Review Standards, Code Review Functional, PR Review, Implementation Validator, Plan Validator, RPI Validator | runSubagent / task | fast       |
| modernizer    | Epsilon     | Squad Modernization Planner   | —                                                         | runSubagent / task | default    |
| architect     | Zeta        | System Architecture Reviewer  | Arch Diagram Builder, ADR Creator, Network ISA-95 Planner | runSubagent / task | default    |
| security      | Eta         | Security Planner              | Security Reviewer, SSSC Planner, Skill Assessor, Finding Deep Verifier, Report Generator, Dependency Reviewer, Codebase Profiler | runSubagent / task | default    |
| rai           | Theta       | RAI Planner                   | —                                                         | runSubagent / task | default    |
| scribe        |             | Squad Scribe                  | Memory                                                    | runSubagent / task | fast       |
