from __future__ import annotations

import argparse
import re
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont


def natural_key(path: Path) -> tuple[int, str]:
    match = re.search(r"(\d+)(?=\.png$)", path.name)
    return (int(match.group(1)) if match else 0, path.name)


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("input_dir", type=Path)
    parser.add_argument("output_dir", type=Path)
    parser.add_argument("--per-sheet", type=int, default=4)
    args = parser.parse_args()

    pages = sorted(args.input_dir.glob("page-*.png"), key=natural_key)
    if not pages:
        raise SystemExit(f"No page PNGs in {args.input_dir}")

    args.output_dir.mkdir(parents=True, exist_ok=True)
    font = ImageFont.load_default()
    margin = 24
    label_height = 32
    columns = 2

    for offset in range(0, len(pages), args.per_sheet):
        group = pages[offset : offset + args.per_sheet]
        loaded = [Image.open(path).convert("RGB") for path in group]
        cell_width = max(image.width for image in loaded)
        cell_height = max(image.height for image in loaded) + label_height
        rows = (len(loaded) + columns - 1) // columns
        sheet = Image.new(
            "RGB",
            (margin + columns * (cell_width + margin), margin + rows * (cell_height + margin)),
            "#d7d7d7",
        )
        draw = ImageDraw.Draw(sheet)
        for index, (path, page) in enumerate(zip(group, loaded)):
            row, column = divmod(index, columns)
            x = margin + column * (cell_width + margin)
            y = margin + row * (cell_height + margin)
            draw.rectangle((x, y, x + cell_width, y + cell_height), fill="white")
            draw.text((x + 8, y + 8), path.stem, fill="black", font=font)
            image_x = x + (cell_width - page.width) // 2
            sheet.paste(page, (image_x, y + label_height))
        start = natural_key(group[0])[0]
        end = natural_key(group[-1])[0]
        sheet.save(args.output_dir / f"pages-{start:02d}-{end:02d}.jpg", quality=90, optimize=True)


if __name__ == "__main__":
    main()
