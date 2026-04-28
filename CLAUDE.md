# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a **Godot 4.6.1 game jam template** with C# support, designed for rapid development and automated publishing to itch.io and Steam. The active game project lives in `src/archetype-poc/`.

## Common Commands

**Initial project setup:**
```bash
./scripts/setup_jam.sh      # Configure itch.io username, game name, Godot version
```

**Release a new version:**
```bash
./scripts/push_release.sh   # Increments version tag and pushes to trigger CI/CD
```

**Create a new game from this template:**
```bash
./scripts/new_game.sh       # Uses GitHub CLI to create a new repo from this template
```

**Open the project:**
- Open `src/archetype-poc/` in Godot 4.6.1

There is no build/test CLI — building and running happens through the Godot editor or CI/CD.

## Architecture

### Autoload Singletons (Global Services)
The game uses Godot's autoload system for global services, registered in `project.godot`:
- `Utility` — helper functions (color, random, serialization)
- `Events` (EventBus) — signal-based decoupled event system
- `Create` (Factory) — object instantiation utility
- `Audio` — audio playback manager
- `NodeEffects` — visual effects (e.g., sheen)
- `Settings` — persistent game configuration

### C# / Archetype Framework
The project uses `Godot.NET.Sdk/4.6.1` with these NuGet packages:
- `Archetype.Core`, `Archetype.Engine`, `Archetype.Text`, `Archetype.Build` (all v0.1.0)

The `archetype-backend/` subdirectory is a placeholder for backend/library code.

### CI/CD Workflows
- **`build-and-publish.yml`** — triggered by `v*` tags; exports for Windows/macOS/Linux/Web and publishes to itch.io via Butler. Requires `BUTLER_API_KEY` secret.
- **`deploy-to-steam.yml`** — triggered by `steam*` tags; exports and deploys Windows build to Steam. Requires `STEAM_USERNAME`, `STEAM_CONFIG_VDF`, `STEAM_APP_ID` secrets.

Both workflows use:
- `ITCHIO_USERNAME`: thewarlock
- `GAME_NAME`: archetype-poc
- `GODOT_VERSION`: 4.6.1-stable
- `PROJECT_PATH`: `./src/`

### Versioning
Tags follow `v1.2` or `v1.2.3` format. `push_release.sh` auto-increments the minor version by default.
