"""Generate an Obsidian Canvas overview for a Markdown vault.

This script scans a vault for Markdown files and writes a generated Canvas board
with one file node per note. It is safe to rerun: the generated board is
overwritten, while the manual working board is created only if missing.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import os
import sys
from pathlib import Path


REPO_ROOT = Path(__file__).resolve().parents[1]
DEFAULT_VAULT = REPO_ROOT / "docs"
DEFAULT_OUTPUT_NAME = Path("_boards") / "all-docs-auto.canvas"
MANUAL_BOARD_NAME = Path("_boards") / "working-map.canvas"

DEFAULT_COLUMNS = 2
DEFAULT_CARD_WIDTH = 740
DEFAULT_CARD_HEIGHT = 1600
DEFAULT_GAP_X = 40
DEFAULT_GAP_Y = 80
METADATA_HEIGHT = 180

IGNORE_DIRS = {
    ".obsidian",
    "_boards",
    ".git",
    "node_modules",
    "dist",
    "build",
    ".cache",
    "__pycache__",
    ".venv",
    "venv",
    ".mypy_cache",
    ".pytest_cache",
    ".ruff_cache",
}

INDEX_CONTENT = """# Internship Game Docs

## Boards

- [[_boards/all-docs-auto.canvas]]
- [[_boards/working-map.canvas]]

## Notes

Use `all-docs-auto.canvas` for the generated full overview.
Use `working-map.canvas` for manual visual planning.
"""

METADATA_TEXT = """Auto-generated docs overview

Generated from Markdown files in this vault.
Do not manually curate this board; changes may be overwritten.
Use _boards/working-map.canvas for manual planning.
"""


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description="Generate an Obsidian Canvas grid from Markdown files.",
    )
    parser.add_argument(
        "--vault",
        type=Path,
        default=DEFAULT_VAULT,
        help=f"Obsidian vault path. Default: {DEFAULT_VAULT}",
    )
    parser.add_argument(
        "--output",
        type=Path,
        default=None,
        help="Output .canvas path. Default: <vault>/_boards/all-docs-auto.canvas",
    )
    parser.add_argument(
        "--columns",
        type=positive_int,
        default=DEFAULT_COLUMNS,
        help=f"Number of columns in the grid. Default: {DEFAULT_COLUMNS}",
    )
    parser.add_argument(
        "--card-width",
        type=positive_int,
        default=DEFAULT_CARD_WIDTH,
        help=f"File node width. Default: {DEFAULT_CARD_WIDTH}",
    )
    parser.add_argument(
        "--card-height",
        type=positive_int,
        default=DEFAULT_CARD_HEIGHT,
        help=f"File node height. Default: {DEFAULT_CARD_HEIGHT}",
    )
    parser.add_argument(
        "--gap-x",
        type=non_negative_int,
        default=DEFAULT_GAP_X,
        help=f"Horizontal gap between file nodes. Default: {DEFAULT_GAP_X}",
    )
    parser.add_argument(
        "--gap-y",
        type=non_negative_int,
        default=DEFAULT_GAP_Y,
        help=f"Vertical gap between file nodes. Default: {DEFAULT_GAP_Y}",
    )
    return parser.parse_args()


def positive_int(value: str) -> int:
    parsed = int(value)
    if parsed <= 0:
        raise argparse.ArgumentTypeError("value must be greater than 0")
    return parsed


def non_negative_int(value: str) -> int:
    parsed = int(value)
    if parsed < 0:
        raise argparse.ArgumentTypeError("value must be 0 or greater")
    return parsed


def normalize_path(path: Path) -> Path:
    return path.expanduser().resolve()


def ensure_index(vault: Path) -> bool:
    index_path = vault / "00-index.md"
    if index_path.exists():
        return False

    index_path.write_text(INDEX_CONTENT, encoding="utf-8")
    return True


def ensure_manual_board(vault: Path) -> bool:
    manual_board = vault / MANUAL_BOARD_NAME
    if manual_board.exists():
        return False

    manual_board.parent.mkdir(parents=True, exist_ok=True)
    manual_board.write_text(
        json.dumps({"nodes": [], "edges": []}, indent=2),
        encoding="utf-8",
    )
    return True


def should_ignore_dir(dirname: str) -> bool:
    return dirname in IGNORE_DIRS


def find_markdown_files(vault: Path) -> list[Path]:
    md_files: list[Path] = []

    for root, dirnames, filenames in os.walk(vault):
        dirnames[:] = sorted(
            [name for name in dirnames if not should_ignore_dir(name)],
            key=str.casefold,
        )

        root_path = Path(root)
        for filename in sorted(filenames, key=str.casefold):
            if filename.casefold().endswith(".md"):
                md_files.append(root_path / filename)

    return sorted(
        md_files,
        key=lambda path: (
            path.relative_to(vault).as_posix().casefold(),
            path.relative_to(vault).as_posix(),
        ),
    )


def stable_file_id(relative_path: str) -> str:
    digest = hashlib.sha1(relative_path.casefold().encode("utf-8")).hexdigest()[:12]
    return f"file-{digest}"


def build_canvas(
    vault: Path,
    md_files: list[Path],
    *,
    columns: int,
    card_width: int,
    card_height: int,
    gap_x: int,
    gap_y: int,
) -> dict[str, list[dict[str, object]]]:
    metadata_width = max(
        900,
        columns * card_width + max(0, columns - 1) * gap_x,
    )
    grid_y = METADATA_HEIGHT + gap_y

    nodes: list[dict[str, object]] = [
        {
            "id": "text-generated-overview",
            "type": "text",
            "text": METADATA_TEXT,
            "x": 0,
            "y": 0,
            "width": metadata_width,
            "height": METADATA_HEIGHT,
        }
    ]

    for index, file_path in enumerate(md_files):
        relative_path = file_path.relative_to(vault).as_posix()
        col = index % columns
        row = index // columns

        nodes.append(
            {
                "id": stable_file_id(relative_path),
                "type": "file",
                "file": relative_path,
                "x": col * (card_width + gap_x),
                "y": grid_y + row * (card_height + gap_y),
                "width": card_width,
                "height": card_height,
            }
        )

    return {"nodes": nodes, "edges": []}


def write_canvas(output: Path, canvas: dict[str, list[dict[str, object]]]) -> None:
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(
        json.dumps(canvas, indent=2, ensure_ascii=False),
        encoding="utf-8",
    )


def main() -> int:
    args = parse_args()
    vault = normalize_path(args.vault)
    output = normalize_path(args.output) if args.output else vault / DEFAULT_OUTPUT_NAME

    if not vault.exists():
        print(f"Error: vault path does not exist: {vault}", file=sys.stderr)
        return 1

    if not vault.is_dir():
        print(f"Error: vault path is not a directory: {vault}", file=sys.stderr)
        return 1

    try:
        output.relative_to(vault)
    except ValueError:
        print(
            "Warning: output is outside the vault. Obsidian can only open boards "
            "inside the vault cleanly.",
            file=sys.stderr,
        )

    index_created = ensure_index(vault)
    manual_board_created = ensure_manual_board(vault)
    md_files = find_markdown_files(vault)
    canvas = build_canvas(
        vault,
        md_files,
        columns=args.columns,
        card_width=args.card_width,
        card_height=args.card_height,
        gap_x=args.gap_x,
        gap_y=args.gap_y,
    )
    write_canvas(output, canvas)

    print(f"Created: {output}")
    print(f"Added {len(md_files)} Markdown files.")
    if index_created:
        print(f"Created index: {vault / '00-index.md'}")
    else:
        print(f"Index already exists: {vault / '00-index.md'}")
    if manual_board_created:
        print(f"Created manual board: {vault / MANUAL_BOARD_NAME}")
    else:
        print(f"Manual board preserved: {vault / MANUAL_BOARD_NAME}")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
