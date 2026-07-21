# Obsidian Canvas Workflow

Use this workflow when you want a visual overview of the project docs.

## Setup

1. Open this folder as an Obsidian vault:

```text
C:\Users\Mou\Documents\Internship Game\docs
```

2. In Obsidian, enable the Canvas core plugin if it is not already enabled.

## Generate The Auto Board

From the project root, run the helper:

```powershell
scripts\generate_canvas.bat
```

Or run the Python script directly with the installed Python path:

```powershell
& "C:\Users\Mou\AppData\Local\Programs\Python\Python314\python.exe" scripts\generate_obsidian_canvas.py --vault "C:\Users\Mou\Documents\Internship Game\docs"
```

The generated board will be written to:

```text
docs\_boards\all-docs-auto.canvas
```

## How To Use The Boards

- Use `_boards/all-docs-auto.canvas` as the generated full overview.
- Use `_boards/working-map.canvas` for manual planning, grouping, and thinking.
- Do not rely on manual edits inside `all-docs-auto.canvas`; the generator overwrites it each time.
- The generator does not overwrite `working-map.canvas`.

## Normal Loop

```text
Add/edit Markdown docs
        ↓
Run canvas generator
        ↓
Open Obsidian
        ↓
Use all-docs-auto.canvas for overview
        ↓
Use working-map.canvas for manual thinking/planning
```

## Tweaking Layout

The generator supports:

```powershell
scripts\generate_canvas.bat --columns 4 --card-width 520 --card-height 360 --gap-x 80 --gap-y 80
```

The script uses only the Python standard library.
