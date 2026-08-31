from pathlib import Path

import pdfplumber
from pptx import Presentation


ROOT = Path(__file__).resolve().parent


def pptx_text(path: Path, titles_only: bool = False) -> None:
    presentation = Presentation(path)
    print(f"\n===== {path.name} ({len(presentation.slides)} slides) =====")
    for number, slide in enumerate(presentation.slides, 1):
        texts = []
        for shape in slide.shapes:
            if getattr(shape, "has_text_frame", False):
                text = " ".join(shape.text.split())
                if text:
                    texts.append(text)
            if getattr(shape, "has_table", False):
                for row in shape.table.rows:
                    row_text = " | ".join(" ".join(cell.text.split()) for cell in row.cells)
                    if row_text.strip(" |"): 
                        texts.append(row_text)
        if titles_only:
            title = ""
            if slide.shapes.title is not None:
                title = " ".join(slide.shapes.title.text.split())
            if not title and texts:
                title = texts[0]
            print(f"{number:02}: {title}")
        else:
            print(f"\n--- Slide {number} ---")
            for text in dict.fromkeys(texts):
                print(text)


def pdf_text(path: Path) -> None:
    print(f"\n===== {path.name} =====")
    with pdfplumber.open(path) as pdf:
        print(f"Pages: {len(pdf.pages)}")
        for number, page in enumerate(pdf.pages, 1):
            text = "\n".join(line.rstrip() for line in (page.extract_text() or "").splitlines())
            print(f"\n--- Page {number} ---\n{text}")


pptx_text(ROOT / "Internship Presentation Sample.pptx")
pptx_text(ROOT / "Summer2223_Internship_Presentation.pptx", titles_only=True)
pdf_text(ROOT / "Winter2324_Internship_Presentation.pdf")
