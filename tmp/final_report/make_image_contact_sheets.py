from __future__ import annotations

import argparse
import math
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("input_dir", type=Path)
    parser.add_argument("output_dir", type=Path)
    parser.add_argument("--pattern", default="*.png")
    parser.add_argument("--per-sheet", type=int, default=6)
    parser.add_argument("--columns", type=int, default=2)
    parser.add_argument("--thumb-width", type=int, default=900)
    args = parser.parse_args()

    images = sorted(args.input_dir.glob(args.pattern), key=lambda p: p.name.lower())
    if not images:
        raise SystemExit(f"No matching images in {args.input_dir}")

    args.output_dir.mkdir(parents=True, exist_ok=True)
    font = ImageFont.load_default()
    margin = 24
    label_height = 44

    for offset in range(0, len(images), args.per_sheet):
        group = images[offset : offset + args.per_sheet]
        thumbs: list[tuple[Path, Image.Image]] = []
        for path in group:
            image = Image.open(path).convert("RGB")
            scale = min(1.0, args.thumb_width / image.width)
            thumb = image.resize(
                (max(1, round(image.width * scale)), max(1, round(image.height * scale))),
                Image.Resampling.LANCZOS,
            )
            thumbs.append((path, thumb))

        cell_width = max(image.width for _, image in thumbs)
        cell_height = max(image.height for _, image in thumbs) + label_height
        rows = math.ceil(len(thumbs) / args.columns)
        sheet = Image.new(
            "RGB",
            (
                margin + args.columns * (cell_width + margin),
                margin + rows * (cell_height + margin),
            ),
            "#d7d7d7",
        )
        draw = ImageDraw.Draw(sheet)
        for index, (path, thumb) in enumerate(thumbs):
            row, column = divmod(index, args.columns)
            x = margin + column * (cell_width + margin)
            y = margin + row * (cell_height + margin)
            draw.rectangle((x, y, x + cell_width, y + cell_height), fill="white")
            draw.text((x + 8, y + 8), path.name, fill="black", font=font)
            image_x = x + (cell_width - thumb.width) // 2
            image_y = y + label_height + (cell_height - label_height - thumb.height) // 2
            sheet.paste(thumb, (image_x, image_y))

        end = offset + len(group)
        sheet.save(
            args.output_dir / f"images-{offset + 1:03d}-{end:03d}.jpg",
            quality=90,
            optimize=True,
        )


if __name__ == "__main__":
    main()
