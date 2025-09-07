BoldDesk CLI

Quick start

- Configure: `bolddesk config set --domain <your-domain> --api-key <your-api-key>`
- List agents: `bolddesk agents list` (alias: `bolddesk list agents`)

Notes

- The CLI supports resource-first commands like `agents list`, `tickets list`, etc.
- For convenience, a top-level alias `list <resource>` is also available, so `bolddesk list agents` works. If you accidentally type `list agents list`, the extra `list` is ignored.
- Run `bolddesk --help` for the complete command overview, or `bolddesk <resource> help` for resource-specific help.

