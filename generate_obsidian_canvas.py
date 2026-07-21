"""Compatibility wrapper for the Obsidian Canvas generator.

The maintained implementation lives in scripts/generate_obsidian_canvas.py.
You can still run this file directly from the project root.
"""

import sys

sys.dont_write_bytecode = True

from scripts.generate_obsidian_canvas import main


if __name__ == "__main__":
    raise SystemExit(main())
