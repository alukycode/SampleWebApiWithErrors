# Architecture Flow Diagrams

Presentation-ready architecture diagrams for the Logs-Triage / Sample Web API
system, covering the end-to-end loop from production telemetry to a human-
approved Azure DevOps work item or Claude-Routine-driven GitHub pull request.

## Files

| File | Content |
| ---- | ------- |
| `01-scheduled-scan-flow.mmd` / `.png` / `.svg` | **Flow A** — scheduled scan: Web API telemetry → `triage-scan-job` → LangGraph → MCP tools → proposal → frontend / MongoDB. |
| `02-decision-and-pr-flow.mmd` / `.png` / `.svg` | **Flows B & C** — reviewer decision → Service Bus → `triage-resume-job` → Azure DevOps ticket (B) or Claude Routine → GitHub draft PR with `AB#<id>` (C). |

The `.mmd` files are the [Mermaid](https://mermaid.js.org/) sources; the `.png`
and `.svg` files are rendered for slide decks. Re-render with:

```bash
npm install -g @mermaid-js/mermaid-cli
mmdc -i docs/architecture/01-scheduled-scan-flow.mmd \
     -o docs/architecture/01-scheduled-scan-flow.png \
     -w 2400 -H 2000 --backgroundColor white
```

## Reading the diagrams

* **Solid arrows** carry data or control flow.
* **Dashed arrows** indicate a "uses tool / API" relationship (a component
  calling an MCP server, the LLM, a read-only search tool, etc.).
* **The yellow/orange dashed boundary** marks the **Human Approval Boundary**:
  no external side effects (ticket creation, Claude Routine, GitHub changes)
  occur upstream of it without a reviewer decision.
* Swimlanes correspond to: **Production Telemetry**, **Deployment Runtime**,
  **Python Triage Worker**, **MCP Integrations**, **LLM Analysis Boundary**,
  **Persistence & Secrets**, **Human Review Frontend**, **Messaging / Resume
  Flow**, **Approved Side Effects**, and **Claude Code Routine**.

## Key invariants the diagrams encode

* The LLM **drafts** proposals only — it does not create Azure DevOps tickets,
  publish Service Bus messages, trigger Claude Routine, or modify GitHub.
* The triage worker **does not** create GitHub branches, commits, or PRs.
  Those side effects belong to Claude Code Routine, which operates inside the
  connected GitHub repository.
* Every approved side effect (work-item creation, Claude Routine invocation,
  persisted decision) happens **only after a human reviewer decision** crosses
  the approval boundary via the Azure Service Bus queue.
* Idempotency is enforced by `message_id` on `proposal_decisions` and
  `pr_routine_runs`, and by fingerprint dedupe on the scan side.
