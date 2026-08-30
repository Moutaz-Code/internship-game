from __future__ import annotations

from pathlib import Path
from typing import Iterable

from PIL import Image, ImageDraw, ImageFont
from docx import Document
from docx.enum.section import WD_ORIENT, WD_SECTION
from docx.enum.style import WD_STYLE_TYPE
from docx.enum.table import WD_ALIGN_VERTICAL, WD_CELL_VERTICAL_ALIGNMENT, WD_TABLE_ALIGNMENT
from docx.enum.text import WD_ALIGN_PARAGRAPH, WD_BREAK, WD_LINE_SPACING
from docx.oxml import OxmlElement
from docx.oxml.ns import qn
from docx.shared import Inches, Pt, RGBColor


ROOT = Path(r"C:\Users\Mou\Documents\Codex\Internship Game")
OUT = ROOT / "output" / "final_report"
TMP = ROOT / "tmp" / "final_report"
OUT.mkdir(parents=True, exist_ok=True)
TMP.mkdir(parents=True, exist_ok=True)

DOCX_PATH = OUT / "Motaz_Pharaon_Final_Internship_Report.docx"
DIAGRAM_PATH = TMP / "gameplay_loop_diagram.png"
BLENDER_PLACEHOLDER_PATH = TMP / "blender_custom_pieces_placeholder.png"

PLAN_IMAGE = ROOT / "LevelDesign" / "InternshipHorrorMap_Options_2026-08-11" / "InternshipHorrorMap_locked.png"
V040_OVERHEAD = ROOT / "UnityProject" / "InternshipGameSynced" / "Assets" / "_Project" / "Captures" / "V040" / "V040_7C_Overhead_Open.png"
V040_RUNTIME = ROOT / "UnityProject" / "InternshipGameSynced" / "Assets" / "_Project" / "Captures" / "V040" / "V040_Runtime_Spawn.png"
P02_PROGRESS = ROOT / "UnityProject" / "InternshipGameSynced" / "Assets" / "_Project" / "Captures" / "P02WallCompletion" / "Room12_Overhead.png"


def set_cell_shading(cell, fill: str) -> None:
    tc_pr = cell._tc.get_or_add_tcPr()
    shd = tc_pr.find(qn("w:shd"))
    if shd is None:
        shd = OxmlElement("w:shd")
        tc_pr.append(shd)
    shd.set(qn("w:fill"), fill)


def set_cell_margins(cell, top=90, start=100, bottom=90, end=100) -> None:
    tc = cell._tc
    tc_pr = tc.get_or_add_tcPr()
    tc_mar = tc_pr.first_child_found_in("w:tcMar")
    if tc_mar is None:
        tc_mar = OxmlElement("w:tcMar")
        tc_pr.append(tc_mar)
    for margin, value in (("top", top), ("start", start), ("bottom", bottom), ("end", end)):
        node = tc_mar.find(qn(f"w:{margin}"))
        if node is None:
            node = OxmlElement(f"w:{margin}")
            tc_mar.append(node)
        node.set(qn("w:w"), str(value))
        node.set(qn("w:type"), "dxa")


def set_table_borders(table, color="808080", size="6") -> None:
    tbl_pr = table._tbl.tblPr
    borders = tbl_pr.first_child_found_in("w:tblBorders")
    if borders is None:
        borders = OxmlElement("w:tblBorders")
        tbl_pr.append(borders)
    for edge in ("top", "left", "bottom", "right", "insideH", "insideV"):
        el = borders.find(qn(f"w:{edge}"))
        if el is None:
            el = OxmlElement(f"w:{edge}")
            borders.append(el)
        el.set(qn("w:val"), "single")
        el.set(qn("w:sz"), size)
        el.set(qn("w:space"), "0")
        el.set(qn("w:color"), color)


def set_table_widths(table, widths_in: list[float]) -> None:
    table.autofit = False
    total = sum(widths_in)
    tbl_pr = table._tbl.tblPr
    tbl_w = tbl_pr.find(qn("w:tblW"))
    if tbl_w is None:
        tbl_w = OxmlElement("w:tblW")
        tbl_pr.append(tbl_w)
    tbl_w.set(qn("w:w"), str(int(total * 1440)))
    tbl_w.set(qn("w:type"), "dxa")
    tbl_ind = tbl_pr.find(qn("w:tblInd"))
    if tbl_ind is None:
        tbl_ind = OxmlElement("w:tblInd")
        tbl_pr.append(tbl_ind)
    tbl_ind.set(qn("w:w"), "0")
    tbl_ind.set(qn("w:type"), "dxa")
    grid = table._tbl.tblGrid
    for child in list(grid):
        grid.remove(child)
    for width in widths_in:
        col = OxmlElement("w:gridCol")
        col.set(qn("w:w"), str(int(width * 1440)))
        grid.append(col)
    for row in table.rows:
        for idx, cell in enumerate(row.cells):
            width = int(widths_in[idx] * 1440)
            tc_pr = cell._tc.get_or_add_tcPr()
            tc_w = tc_pr.find(qn("w:tcW"))
            if tc_w is None:
                tc_w = OxmlElement("w:tcW")
                tc_pr.append(tc_w)
            tc_w.set(qn("w:w"), str(width))
            tc_w.set(qn("w:type"), "dxa")
            cell.width = Inches(widths_in[idx])


def repeat_table_header(row) -> None:
    tr_pr = row._tr.get_or_add_trPr()
    tbl_header = OxmlElement("w:tblHeader")
    tbl_header.set(qn("w:val"), "true")
    tr_pr.append(tbl_header)


def keep_row_together(row) -> None:
    tr_pr = row._tr.get_or_add_trPr()
    cant_split = OxmlElement("w:cantSplit")
    tr_pr.append(cant_split)


def set_repeat_header_and_keep(table) -> None:
    if table.rows:
        repeat_table_header(table.rows[0])
    for row in table.rows:
        keep_row_together(row)


def set_cell_text(cell, text: str, bold=False, size=9.5, align=WD_ALIGN_PARAGRAPH.LEFT) -> None:
    cell.text = ""
    p = cell.paragraphs[0]
    p.alignment = align
    p.paragraph_format.space_after = Pt(0)
    p.paragraph_format.line_spacing = 1.0
    r = p.add_run(text)
    r.bold = bold
    r.font.name = "Times New Roman"
    r._element.rPr.rFonts.set(qn("w:eastAsia"), "Times New Roman")
    r.font.size = Pt(size)
    cell.vertical_alignment = WD_CELL_VERTICAL_ALIGNMENT.CENTER
    set_cell_margins(cell)


def add_field(paragraph, instruction: str, display: str = ""):
    run = paragraph.add_run()
    fld_char = OxmlElement("w:fldChar")
    fld_char.set(qn("w:fldCharType"), "begin")
    instr = OxmlElement("w:instrText")
    instr.set(qn("xml:space"), "preserve")
    instr.text = instruction
    sep = OxmlElement("w:fldChar")
    sep.set(qn("w:fldCharType"), "separate")
    text = OxmlElement("w:t")
    text.text = display
    end = OxmlElement("w:fldChar")
    end.set(qn("w:fldCharType"), "end")
    run._r.extend([fld_char, instr, sep, text, end])
    return run


def set_page_number_format(section, fmt: str, start: int) -> None:
    sect_pr = section._sectPr
    pg_num = sect_pr.find(qn("w:pgNumType"))
    if pg_num is None:
        pg_num = OxmlElement("w:pgNumType")
        sect_pr.append(pg_num)
    pg_num.set(qn("w:fmt"), fmt)
    pg_num.set(qn("w:start"), str(start))


def add_page_number_footer(section) -> None:
    section.footer.is_linked_to_previous = False
    footer = section.footer
    p = footer.paragraphs[0]
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.paragraph_format.space_after = Pt(0)
    run = add_field(p, " PAGE ", "1")
    run.font.name = "Times New Roman"
    run.font.size = Pt(10)


def remove_footer_content(section) -> None:
    section.footer.is_linked_to_previous = False
    for p in section.footer.paragraphs:
        p._element.getparent().remove(p._element)
    section.footer.add_paragraph("")


def set_doc_defaults(doc: Document) -> None:
    styles = doc.styles
    normal = styles["Normal"]
    normal.font.name = "Times New Roman"
    normal._element.rPr.rFonts.set(qn("w:eastAsia"), "Times New Roman")
    normal.font.size = Pt(12)
    normal.paragraph_format.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
    normal.paragraph_format.line_spacing = 1.1
    normal.paragraph_format.space_after = Pt(4)
    normal.paragraph_format.widow_control = True

    for name, size, before, after in (
        ("Heading 1", 16, 12, 8),
        ("Heading 2", 13, 10, 5),
        ("Heading 3", 12, 8, 4),
    ):
        st = styles[name]
        st.font.name = "Times New Roman"
        st._element.rPr.rFonts.set(qn("w:eastAsia"), "Times New Roman")
        st.font.size = Pt(size)
        st.font.bold = True
        st.font.color.rgb = RGBColor(0, 0, 0)
        st.paragraph_format.space_before = Pt(before)
        st.paragraph_format.space_after = Pt(after)
        st.paragraph_format.keep_with_next = True
        st.paragraph_format.keep_together = True
    styles["Heading 3"].font.italic = True

    if "Caption" in styles:
        cap = styles["Caption"]
    else:
        cap = styles.add_style("Caption", WD_STYLE_TYPE.PARAGRAPH)
    cap.font.name = "Times New Roman"
    cap._element.rPr.rFonts.set(qn("w:eastAsia"), "Times New Roman")
    cap.font.size = Pt(10)
    cap.font.italic = True
    cap.font.color.rgb = RGBColor(0, 0, 0)
    cap.paragraph_format.alignment = WD_ALIGN_PARAGRAPH.CENTER
    cap.paragraph_format.space_before = Pt(3)
    cap.paragraph_format.space_after = Pt(8)
    cap.paragraph_format.keep_together = True

    for list_name in ("List Bullet", "List Number"):
        if list_name in styles:
            st = styles[list_name]
            st.font.name = "Times New Roman"
            st._element.rPr.rFonts.set(qn("w:eastAsia"), "Times New Roman")
            st.font.size = Pt(12)
            st.paragraph_format.space_after = Pt(3)
            st.paragraph_format.line_spacing = 1.1

    settings = doc.settings._element
    update_fields = settings.find(qn("w:updateFields"))
    if update_fields is None:
        update_fields = OxmlElement("w:updateFields")
        settings.append(update_fields)
    update_fields.set(qn("w:val"), "true")

    compat = settings.find(qn("w:compat"))
    if compat is None:
        compat = OxmlElement("w:compat")
        settings.append(compat)

    for st in styles:
        if st.type in (WD_STYLE_TYPE.PARAGRAPH, WD_STYLE_TYPE.CHARACTER):
            rpr = st._element.get_or_add_rPr()
            lang = rpr.find(qn("w:lang"))
            if lang is None:
                lang = OxmlElement("w:lang")
                rpr.append(lang)
            lang.set(qn("w:val"), "en-US")


def configure_section(section, top=1.0, bottom=1.0, left=0.75, right=0.75) -> None:
    section.orientation = WD_ORIENT.PORTRAIT
    section.page_width = Inches(8.5)
    section.page_height = Inches(11)
    section.top_margin = Inches(top)
    section.bottom_margin = Inches(bottom)
    section.left_margin = Inches(left)
    section.right_margin = Inches(right)
    section.header_distance = Inches(0.4)
    section.footer_distance = Inches(0.45)


def add_body(doc: Document, text: str, *, first_indent=True) -> None:
    for block in [b.strip() for b in text.strip().split("\n\n") if b.strip()]:
        p = doc.add_paragraph()
        p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
        p.paragraph_format.line_spacing = 1.1
        p.paragraph_format.space_after = Pt(4)
        if first_indent:
            p.paragraph_format.first_line_indent = Inches(0.25)
        p.add_run(block)


def add_bullets(doc: Document, items: Iterable[str]) -> None:
    for item in items:
        p = doc.add_paragraph(style="List Bullet")
        p.paragraph_format.left_indent = Inches(0.35)
        p.paragraph_format.first_line_indent = Inches(-0.18)
        p.add_run(item)


def add_numbered(doc: Document, items: Iterable[str]) -> None:
    for item in items:
        p = doc.add_paragraph(style="List Number")
        p.paragraph_format.left_indent = Inches(0.35)
        p.paragraph_format.first_line_indent = Inches(-0.18)
        p.add_run(item)


def add_heading(doc: Document, text: str, level: int, *, page_break=False) -> None:
    p = doc.add_paragraph(text, style=f"Heading {level}")
    if page_break:
        p.paragraph_format.page_break_before = True


def set_alt_text(inline_shape, title: str, description: str) -> None:
    doc_pr = inline_shape._inline.docPr
    doc_pr.set("title", title)
    doc_pr.set("descr", description)


def add_figure(doc: Document, path: Path, number: int, caption: str, source: str, width=6.3, alt="") -> None:
    if not path.exists():
        raise FileNotFoundError(path)
    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.paragraph_format.keep_with_next = True
    p.paragraph_format.space_before = Pt(5)
    shape = p.add_run().add_picture(str(path), width=Inches(width))
    set_alt_text(shape, f"Figure {number}", alt or caption)
    cap = doc.add_paragraph(style="Caption")
    cap.add_run(f"Figure {number}. {caption}\n").bold = True
    cap.add_run(f"Source/credit: {source}")


def add_captioned_table_title(doc: Document, number: int, title: str) -> None:
    p = doc.add_paragraph(style="Caption")
    p.paragraph_format.keep_with_next = True
    p.add_run(f"Table {number}. {title}").bold = True


def make_loop_diagram(path: Path) -> None:
    width, height = 1800, 780
    img = Image.new("RGB", (width, height), "white")
    draw = ImageDraw.Draw(img)
    font_path = Path(r"C:\Windows\Fonts\times.ttf")
    bold_path = Path(r"C:\Windows\Fonts\timesbd.ttf")
    font = ImageFont.truetype(str(font_path), 42)
    bold = ImageFont.truetype(str(bold_path), 48)
    small = ImageFont.truetype(str(font_path), 31)
    labels = [
        ("1", "Accept contract", "Enter the facility"),
        ("2", "Search and assess", "Use routes and the Geiger counter"),
        ("3", "Recover rods", "Carry verified objectives"),
        ("4", "Adapt under pressure", "Avoid and misdirect the Owl"),
        ("5", "Cycle extraction", "Survive the timed holdout"),
        ("6", "Leave and replay", "New seed, routes, and threat bias"),
    ]
    boxes = []
    margin_x, gap_x = 85, 45
    box_w = (width - 2 * margin_x - 2 * gap_x) // 3
    box_h = 230
    ys = [90, 455]
    for idx, (num, title, subtitle) in enumerate(labels):
        row = 0 if idx < 3 else 1
        col = idx if idx < 3 else 5 - idx
        x = margin_x + col * (box_w + gap_x)
        y = ys[row]
        boxes.append((x, y, x + box_w, y + box_h))
        draw.rounded_rectangle(boxes[-1], radius=26, fill="#F2F2F2", outline="#333333", width=5)
        draw.ellipse((x + 24, y + 24, x + 92, y + 92), fill="#222222")
        tw = draw.textbbox((0, 0), num, font=bold)[2]
        draw.text((x + 58 - tw / 2, y + 28), num, font=bold, fill="white")
        draw.text((x + 120, y + 32), title, font=font, fill="#111111")
        bbox = draw.multiline_textbbox((0, 0), subtitle, font=small, spacing=8, align="center")
        sw = bbox[2] - bbox[0]
        draw.multiline_text((x + box_w / 2 - sw / 2, y + 130), subtitle, font=small, fill="#333333", spacing=8, align="center")

    def arrow(a, b):
        x1, y1 = a
        x2, y2 = b
        draw.line((x1, y1, x2, y2), fill="#555555", width=8)
        if abs(x2 - x1) > abs(y2 - y1):
            sign = 1 if x2 > x1 else -1
            pts = [(x2, y2), (x2 - sign * 28, y2 - 17), (x2 - sign * 28, y2 + 17)]
        else:
            sign = 1 if y2 > y1 else -1
            pts = [(x2, y2), (x2 - 17, y2 - sign * 28), (x2 + 17, y2 - sign * 28)]
        draw.polygon(pts, fill="#555555")

    arrow((boxes[0][2], (boxes[0][1] + boxes[0][3]) // 2), (boxes[1][0] - 10, (boxes[1][1] + boxes[1][3]) // 2))
    arrow((boxes[1][2], (boxes[1][1] + boxes[1][3]) // 2), (boxes[2][0] - 10, (boxes[2][1] + boxes[2][3]) // 2))
    arrow(((boxes[2][0] + boxes[2][2]) // 2, boxes[2][3]), ((boxes[3][0] + boxes[3][2]) // 2, boxes[3][1] - 10))
    arrow((boxes[3][0], (boxes[3][1] + boxes[3][3]) // 2), (boxes[4][2] + 10, (boxes[4][1] + boxes[4][3]) // 2))
    arrow((boxes[4][0], (boxes[4][1] + boxes[4][3]) // 2), (boxes[5][2] + 10, (boxes[5][1] + boxes[5][3]) // 2))
    # Close the loop along the outside-left margin so the return path never
    # crosses a label or a process box.
    start_y = (boxes[5][1] + boxes[5][3]) // 2
    end_y = (boxes[0][1] + boxes[0][3]) // 2
    outside_x = 32
    draw.line((boxes[5][0], start_y, outside_x, start_y, outside_x, end_y, boxes[0][0] - 12, end_y), fill="#666666", width=7)
    draw.polygon(
        [(boxes[0][0], end_y), (boxes[0][0] - 30, end_y - 18), (boxes[0][0] - 30, end_y + 18)],
        fill="#666666",
    )
    img.save(path, dpi=(220, 220))


def make_blender_placeholder(path: Path) -> None:
    width, height = 1800, 720
    img = Image.new("RGB", (width, height), "#F4F4F4")
    draw = ImageDraw.Draw(img)
    font_path = Path(r"C:\Windows\Fonts\times.ttf")
    bold_path = Path(r"C:\Windows\Fonts\timesbd.ttf")
    title = ImageFont.truetype(str(bold_path), 62)
    body = ImageFont.truetype(str(font_path), 42)
    small = ImageFont.truetype(str(font_path), 34)
    draw.rounded_rectangle((45, 45, width - 45, height - 45), radius=28, outline="#555555", width=6, fill="#FAFAFA")
    heading = "IMAGE PLACEHOLDER - REPLACE BEFORE SUBMISSION"
    hb = draw.textbbox((0, 0), heading, font=title)
    draw.text(((width - (hb[2] - hb[0])) / 2, 135), heading, font=title, fill="#222222")
    lines = [
        "Suggested screenshot: Blender comparison of the Creepy Cat base piece",
        "and Motaz Pharaon's custom 22.5-degree and 5-degree corner variants.",
    ]
    y = 300
    for line in lines:
        bb = draw.textbbox((0, 0), line, font=body)
        draw.text(((width - (bb[2] - bb[0])) / 2, y), line, font=body, fill="#333333")
        y += 72
    note = "Keep the Blender UI, object names, and corner geometry readable; hide unrelated/private panels."
    nb = draw.textbbox((0, 0), note, font=small)
    draw.text(((width - (nb[2] - nb[0])) / 2, 540), note, font=small, fill="#555555")
    img.save(path, dpi=(220, 220))


def add_cover(doc: Document) -> None:
    for _ in range(3):
        doc.add_paragraph("")
    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.paragraph_format.space_after = Pt(34)
    r = p.add_run("Internship Report - Game Development Intern")
    r.font.name = "Times New Roman"
    r.font.size = Pt(18)
    r.bold = True

    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.add_run("by").italic = True
    p.paragraph_format.space_after = Pt(22)

    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    r = p.add_run("Motaz Pharaon")
    r.bold = True
    r.font.size = Pt(16)
    p.paragraph_format.space_after = Pt(6)

    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.add_run("Student ID: 1086106")
    p.paragraph_format.space_after = Pt(28)

    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.add_run("Bachelor in Information Technology")
    p.paragraph_format.space_after = Pt(6)

    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.add_run("ITE399B | Internship in IT")
    p.paragraph_format.space_after = Pt(6)

    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.add_run("Abu Dhabi University")
    p.paragraph_format.space_after = Pt(6)

    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.add_run("Placement Organization: Pixoul Gaming")
    p.paragraph_format.space_after = Pt(70)

    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.add_run("Abu Dhabi, United Arab Emirates")
    p.paragraph_format.space_after = Pt(6)

    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.add_run("August 2026")


def build_report() -> None:
    make_loop_diagram(DIAGRAM_PATH)
    make_blender_placeholder(BLENDER_PLACEHOLDER_PATH)
    doc = Document()
    set_doc_defaults(doc)
    configure_section(doc.sections[0])
    remove_footer_content(doc.sections[0])
    add_cover(doc)

    front = doc.add_section(WD_SECTION.NEW_PAGE)
    configure_section(front)
    set_page_number_format(front, "lowerRoman", 1)
    add_page_number_footer(front)

    p = doc.add_paragraph("Acknowledgements", style="Heading 1")
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    add_body(doc, """
I would like to thank Pixoul Gaming for giving me the opportunity to complete an internship centered on a project that combined game design, software architecture, level design, Unity development, testing, and technical documentation. The placement allowed me to study how an entertainment venue connects custom interactive software with VR equipment and physical effects, and then to translate that inspiration into an original prototype.

I am especially grateful to my supervisor, Harout Boyajian, Tech Lead, for being supportive throughout the internship and for trusting me with near-complete freedom over the creative and technical direction of the project. That freedom made the work challenging, but it also allowed me to take responsibility for scope decisions, investigate problems, evaluate alternatives, and learn from the consequences of each iteration.

I also appreciate the support of Abu Dhabi University and the knowledge gained through the Bachelor in Information Technology program. Software Architecture and Design and IT Project Management were particularly valuable because they helped me structure an ambitious idea, defend it through a business case, manage risks, and treat replayability as a quality attribute that needed to influence the whole design rather than one isolated feature.
""", first_indent=False)

    doc.add_page_break()
    p = doc.add_paragraph("Summary/Abstract", style="Heading 1")
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    add_body(doc, """
This report documents an eight-week, 320-hour game-development internship at Pixoul Gaming from June 29 to August 21, 2026. The main project was a Unity prototype for a short-session science-fiction extraction-horror game set in a nuclear facility. The concept was shaped by Pixoul's arcade and VR context: a player should be able to understand the contract quickly, complete meaningful physical work under pressure, leave within a controlled session, and still want to return because the next run changes in visible and understandable ways.

The internship progressed from venue research, concept development, a business case, architecture, risk planning, and backlog preparation into a desktop-first Unity vertical slice. The strongest completed and tracked result was P01 version 0.4.0.0. It combined a three-lobed facility, a fuel-rod recovery and extraction loop, deterministic run variation, and an adversary called the Owl that used bounded, explainable, run-local adaptation rather than machine learning. Recorded delivery evidence for v0.4 included 28 focused Edit Mode tests, 141 total Edit Mode tests, seven Play Mode tests, a Windows development build, and a 12-second launch smoke check without a recorded project-code exception.

The final two internship weeks began P02, a separate approved-map workstream. I personally drew its floor plan and manually constructed the modular wall shell with prefabs adapted from Creepy Cat's 3D Scifi Kit Starter Kit, including custom Blender corner variants for angles required by the plan. By August 21 most rooms were built, while the tracked completion and last wall correction occurred on August 22 and are therefore identified as post-internship continuation. AI agents assisted with planning, analysis, partial code and editor tooling, automated checks, and documentation under my direction and review; they did not author my entire manual P02 shell. The project established strong automated and desktop evidence, but human readability, fairness, XR comfort, headset operation, and physical hardware remained untested. The internship strengthened my commitment to game development and showed me how architecture, project management, evidence discipline, and honest limitation reporting support creative work.
""")

    doc.add_page_break()
    p = doc.add_paragraph("Table of Contents", style="Heading 1")
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    toc_p = doc.add_paragraph()
    toc_p.paragraph_format.space_after = Pt(0)
    add_field(toc_p, ' TOC \\o "1-3" \\h \\z \\u ', "Right-click and select Update Field")

    doc.add_page_break()
    p = doc.add_paragraph("Weekly Timetable", style="Heading 1")
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    add_captioned_table_title(doc, 1, "Eight-week internship chronology and hours")
    weekly = [
        ("1", "Jun 29-Jul 3", "40", "Venue onboarding; research into PC and VR zones, custom experiences, and physical effects; brainstorming a venue-suited VR horror concept; early haptic and heart-rate ideas."),
        ("2", "Jul 6-10", "40", "Business case and proposal; vision, game flow and feel; replayability goal; architecture boundaries; project hub, sprint plan, backlog, risk register, QA template, and ClickUp-ready tasks."),
        ("3", "Jul 13-17", "40", "Unity vertical-slice implementation ramp; desktop player, facility greybox, fuel-rod objective, threat pressure, run state, interaction seams, and early verification."),
        ("4", "Jul 20-24", "40", "Prototype 1 delivery; readable-route adaptation; desktop interaction and environment polish; movement and OpenXR troubleshooting; fullscreen/versioning and regression work."),
        ("5", "Jul 27-31", "40", "v0.3 readable stealth and contractor counterplay; graded sight/sound evidence, distractions, bounded Owl responses, focused/full Edit Mode and Play Mode validation, build and smoke launch."),
        ("6", "Aug 3-7", "40", "v0.4 spatial readability; three unequal lobes, central hub, three connectors, declarative topology, regression/build evidence, captures, human-validation sheet, and implementation handoff."),
        ("7", "Aug 10-14", "40", "Approved-map development; personally drawn floor plan; facility-kit and asset import/adaptation; translation from pixels to modular dimensions; manual Entrance, Room 2, and Room 3 wall construction."),
        ("8", "Aug 17-21", "40", "Manual modular wall-shell expansion; room, hierarchy, angle, thickness, and connection corrections; most rooms completed by August 21; late integration preparation and documentation."),
    ]
    table = doc.add_table(rows=1, cols=4)
    table.alignment = WD_TABLE_ALIGNMENT.LEFT
    table.style = "Table Grid"
    set_table_widths(table, [0.55, 1.15, 0.55, 5.0])
    headers = ["Week", "Dates", "Hours", "Main activities and outcomes"]
    for cell, text in zip(table.rows[0].cells, headers):
        set_cell_text(cell, text, bold=True, size=9, align=WD_ALIGN_PARAGRAPH.CENTER)
        set_cell_shading(cell, "D9D9D9")
    for week, dates, hours, activities in weekly:
        cells = table.add_row().cells
        set_cell_text(cells[0], week, size=9, align=WD_ALIGN_PARAGRAPH.CENTER)
        set_cell_text(cells[1], dates, size=9, align=WD_ALIGN_PARAGRAPH.CENTER)
        set_cell_text(cells[2], hours, size=9, align=WD_ALIGN_PARAGRAPH.CENTER)
        set_cell_text(cells[3], activities, size=9)
    set_table_borders(table)
    set_repeat_header_and_keep(table)
    p = doc.add_paragraph()
    p.paragraph_format.space_before = Pt(5)
    p.add_run("Total: 320 hours. No absences or holidays. Work dated August 22 or later is excluded from the internship hours.").italic = True

    body = doc.add_section(WD_SECTION.NEW_PAGE)
    configure_section(body)
    set_page_number_format(body, "decimal", 1)
    add_page_number_footer(body)

    add_heading(doc, "1. Introduction", 1)
    add_body(doc, """
Internships are valuable because they require students to convert academic knowledge into decisions that have real constraints, stakeholders, deadlines, and consequences. My placement at Pixoul Gaming placed me in a particularly interesting environment: the organization operates a public gaming and immersive-entertainment venue, while the project I proposed required the mindset of a small game-development team. I had to think about the attraction of a concept, the realities of building it in Unity, the operational need for a repeatable arcade session, the safety and comfort of VR, and the evidence needed before calling an experience successful.

The internship ran for eight weeks from June 29 to August 21, 2026, with 40 hours completed each week for a total of 320 hours. I worked under Harout Boyajian, Tech Lead. I did not have a formally confirmed departmental title, so this report describes the placement accurately as game-development work under technical leadership and alongside an R&D context rather than assigning an unsupported organizational label.

The main outcome was a desktop-first Unity extraction-horror prototype. The game's near-future nuclear facility provided a focused setting for a short contract: enter, identify dangerous fuel rods, transport them to the intake, manage an intelligent threat, survive extraction, and leave. The intended product direction was VR and venue-specific physical immersion, but actual headset and hardware access was not established during the recorded period. Therefore, desktop implementation and automated tests served as engineering evidence while XR comfort, physical haptics, and customer experience remained explicit future validation gates.
""")

    add_heading(doc, "1.1 Learning Goals", 2)
    add_body(doc, """
My first learning goal was to experience the full path from an early creative idea to a structured, testable prototype. I wanted to avoid treating game design as a list of attractive features. Instead, I aimed to define a central player promise, identify the risks that could prevent it from working, choose one question for each prototype milestone, and use evidence to decide what to keep, simplify, or defer.

My second goal was to improve my Unity and C# development practice. This included learning how scenes, prefabs, state machines, deterministic seeds, navigation representations, test assemblies, build configurations, and debugging tools fit together. A prefab is a reusable Unity asset that can be instantiated many times with consistent structure [5]. This was especially relevant when I later used a modular environment kit to construct the P02 facility walls.

The third goal was to connect architecture to a commercial quality need. Replayability mattered because Pixoul operates in an arcade-like venue: a one-time experience may be memorable, but a sustainable game should offer a reason to return. I therefore treated replayability as a quality attribute that affected objectives, layout states, route choices, threat behavior, reset reliability, testing, and future content structure. The aim was not unlimited procedural generation. It was controlled variation that remained understandable, fair, and testable.

Finally, I wanted to learn how to work responsibly with AI-assisted tools. My supervisor encouraged their use, and I used agents for design drafts, analysis, partial code and editor tooling, test support, and documentation. My learning goal was to remain the project owner: I set direction, made scope decisions, operated Unity, reviewed and corrected outputs, manually constructed the later P02 walls, and distinguished an agent suggestion from verified implementation.
""")

    add_heading(doc, "1.2 Learning Activities", 2)
    add_body(doc, """
The learning activities combined observation, design, implementation, testing, and reflection. I began by touring the venue and researching how custom games could connect Unity software to specialized VR spaces and physical effects. I then produced a living set of Obsidian-compatible project documents: a project hub, vision brief, sprint plan, backlog, risk register, decision log, prototype log, QA template, architecture outline, and implementation handoffs. These documents were not a separate academic exercise. They acted as control panels for active decisions and helped prevent later proposals from being mistaken for completed features.

Development proceeded through question-driven milestones. Each milestone protected a baseline, introduced a bounded change, recorded automated evidence, and left human questions open when no participant observation existed. I also reviewed screenshots, scene hierarchies, test results, build behavior, and Git history. In the final two weeks, learning became strongly spatial and manual: I translated my own two-dimensional floor plan into a three-dimensional modular wall shell, corrected scale and connection problems, and learned that a plan can be geometrically clear while still requiring many small construction decisions.
""")

    add_heading(doc, "1.3 Assigned Tasks and Responsibilities", 2)
    add_body(doc, """
My assignment was unusual because I was not handed one isolated programming ticket or a finished game-design document. With my supervisor's approval, I proposed the project and became responsible for shaping a feasible internship outcome from a much larger product idea. The work therefore included six connected responsibilities: researching the venue context, defining the game concept, planning and controlling scope, developing and reviewing the Unity prototype, constructing the later level-design workstream, and recording evidence clearly enough for another person to understand what had and had not been completed.

At the design level, my tasks were to define the player contract, the extraction loop, the role of the Owl, replayable variation, and the relationship between a desktop prototype and the intended venue experience. At the technical level, I worked with Unity scenes, prefabs, C# systems, test suites, build settings, editor tools, debugging information, and versioned project state. I did not personally type every line produced during an AI-assisted workflow, but I selected the direction, operated and reviewed the project, rejected unsuitable suggestions, investigated failures, corrected integration problems, and accepted responsibility for the final scope and report claims.

The later P02 assignment added manual environment work. I drew the approved floor plan, translated it to a reusable modular scale, adapted third-party components, created Blender variations where the source kit did not match the required angles, and placed the wall shell room by room. This work was different from the earlier systems prototypes: success depended on patient spatial judgment, clean hierarchy, consistent wall thickness, practical doorway clearance, and honest separation between imported art, customized pieces, automated support tools, and my manual scene work.

Across all of these tasks, reporting was also a responsibility. I maintained planning and handoff documents, used Git history and captures to preserve chronology, attached validation counts to the correct milestone, and avoided treating an attractive proposal as an implemented result. The final report follows the same principle: it describes assigned work comprehensively while retaining the limits of the available human, XR, and hardware evidence.
""")

    add_heading(doc, "2. Company Description", 1, page_break=True)
    add_heading(doc, "2.1 About the Company", 2)
    add_body(doc, """
Pixoul Gaming is located at Al Qana in Abu Dhabi. Its official public description presents it as a 4,500-square-meter virtual-reality and esports hub with five zones, ten VR experiences, an esports academy, gaming broadcast studios, an esports bar, and an esports arena [1], [2]. This combination of public entertainment, competitive gaming, training, and custom immersive experiences made it an appropriate setting for an internship project concerned with both software and physical experience.

During onboarding I was introduced to PC gaming rooms, VR spaces, racing simulators, and custom in-house experiences. I was particularly interested in two open VR areas and in the way an elevator-themed thriller experience used environmental effects such as floor vibration, airflow, and heat to reinforce events inside the game. Other experiences used more specialized physical controls, such as a parachute mechanism that required the player to pull on its sides. These observations made the difference between venue entertainment and a home game concrete: the room itself can become part of the interface.

The official website describes Pixoul's broader offering as covering VR, retro, console, mobile, and esports zones and connects the venue with game development and education as well as public entertainment [2]. The venue opened at Al Qana in November 2022 [3]. My report uses those public facts only for general context. Internal company structure, client lists, and development processes are not inferred beyond my own placement experience.
""")

    add_heading(doc, "2.2 Organizational Layout and Internship Context", 2)
    add_body(doc, """
I worked under Harout Boyajian, Tech Lead, and sat near a member of R&D. I was not given a formal departmental designation that I can verify, and I did not work across the wider corporate hierarchy. The most accurate description is therefore that I completed an individual game-development project under technical supervision, with exposure to the relationship between research, software development, venue hardware, and operations.

The project structure was deliberately intern-led. My supervisor gave me near-complete freedom over the creative and technical direction, subject to the project needing to make sense for Pixoul. This freedom was important because it required me to justify the idea, define a practical slice, report progress, and accept when a promising feature was too risky for the available evidence. The working relationship was less about receiving a fixed list of programming tasks and more about proposing, structuring, and delivering a coherent prototype while keeping the supervisor informed.
""")

    add_heading(doc, "2.2.1 Visitors and Stakeholders", 3)
    add_body(doc, """
Pixoul serves professional and amateur gamers and offers experiences that range from individual VR attractions to multiplayer, console, training, and esports activities [1], [2]. For my project, this created several implied stakeholders: the player, the venue operator who must start and reset a session reliably, the technical team responsible for software and hardware integration, and management evaluating whether the concept offers repeat value. No customer study was performed during the internship, so these stakeholder needs were design assumptions and architecture drivers rather than measured customer findings.
""")

    add_heading(doc, "2.3 Company History and Venue Relevance", 2)
    add_body(doc, """
Pixoul announced the venue in 2022 as an integrated VR and esports destination and opened at Al Qana on November 4 of that year [3]. By the time of my 2026 placement, its public offering combined commercial attractions with custom immersive technology. The venue context influenced my central design decision: the prototype should not be a long home-style campaign that depends on many hours of progression. It should be a self-contained contract that can be explained, played, reset, and replayed within a controlled session.

This arcade requirement made retention and replayability important without justifying an oversized feature list. A returning player should encounter a different combination of routes, objective positions, hazards, or threat pressures while still understanding the rules. This became the bridge between company relevance and my academic work in architecture: replayability was treated as a quality attribute with system-wide consequences.
""")

    add_heading(doc, "3. Internship Activities", 1, page_break=True)
    add_heading(doc, "3.1 Training and Week 1 Concept Development", 2)
    add_body(doc, """
The onboarding period was short but enjoyable. I toured the venue, learned about the different gaming areas, and researched how the custom experiences combined software with physical equipment. The most important outcome was not a list of devices; it was a clearer understanding that haptics and room effects should communicate an in-world meaning. Floor vibration, wind, heat, or a physical control can be memorable, but if several effects mean the same thing—or one effect means too many things—the player may become confused.

My early brainstorming moved through several ideas before reaching a VR horror extraction roguelike direction. I was attracted to horror because physical effects could reinforce tension, and to extraction play because it creates a clear job, rising risk, and a satisfying exit. I also explored a heart-rate sensor as a possible input to a game mechanic. Later risk analysis showed why raw heart rate should not directly determine detection or failure: sensor noise, individual anxiety, calibration, accessibility, consent, and positive feedback loops could make the game unfair. The idea was therefore retained as future research with a simulated fallback rather than claimed as an implemented feature.

The Week 1 concept was intentionally broad. Details such as the final three-rod objective, bounded Owl adaptation, three-lobed map, and P02 floor plan were developed later. This chronology matters because it shows genuine iteration rather than presenting the final design as if it existed from the first day.
""")

    add_heading(doc, "3.2 Week 2 Planning, Business Case, and Architecture", 2)
    add_body(doc, """
During Week 2 I converted the concept into a planning spine. I used a business-case and proposal approach from IT Project Management to convince my supervisor that the project was ambitious but manageable if it was divided into prototypes. The Sprint 01 plan defined a single question: could a small VR facility feel tense and understandable while the player located an objective, sensed a threat, and extracted? The plan also created explicit must-have, should-have, could-have, and won't-have boundaries [8].

The Obsidian vault became the center of the planning workflow. The project hub recorded the current phase, assumptions, and open questions. The vision brief connected Pixoul's venue context to a 15-20 minute contract structure. The backlog separated production, core gameplay, threat, run variation, hardware, QA, and communication. The risk register documented failure conditions and fallbacks. A ClickUp-ready task pack translated the plan into actionable work. A first-person design workbook captured rough ideas about player fantasy, haptic meanings, objectives, the Owl, and the desired reaction after a run: the player should want to play again.

Software architecture helped turn replayability into design constraints. The game would not generate arbitrary runtime geometry. Instead, a deterministic seed would select among authored sockets, route states, lighting conditions, and bounded threat profiles. A deterministic seed is an input that causes the same choices to be produced again, which helps debugging and testing. The architecture separated the player, interaction, session state, objective, threat, run configuration, and hardware seams [9]. This meant the desktop prototype could use simulated devices while preserving interfaces for later XR or venue hardware.
""")

    add_heading(doc, "3.2.1 Planning Deliverables and Scope Control", 3)
    add_body(doc, """
The planning documents divided the vision into work that could be evaluated. The project hub recorded the current objective, assumptions, known evidence, and next decision. The vision brief described the intended player experience and venue relevance. The sprint plan converted that vision into a bounded prototype question, while the backlog separated must-have work from should-have, could-have, and explicitly deferred ideas. The risk register then linked major uncertainties to triggers and fallback actions rather than treating risk as a general feeling.

This structure was important because the concept could easily expand. Physical heat, wind, floor vibration, heart-rate input, equipment combinations, procedural contracts, multiple sites, advanced recovery mechanics, and a sophisticated adaptive enemy were all interesting, but they could not all be validated in eight weeks. I used cut lines to protect a complete desktop loop before pursuing venue hardware. When a feature lacked the required device, participant, or integration evidence, the plan preserved an interface or future note instead of presenting the feature as finished.

Architecture work supported the same restraint. Systems were separated according to their responsibilities so that the session state did not need to know every detail of the player, the Owl, a physical device, or a particular scene. Run configuration held repeatable choices. Objective components held rod and extraction rules. Threat behavior consumed prioritized evidence. Hardware-facing functions were treated as replaceable seams. This allowed the project to make progress with simulated or desktop inputs while keeping later XR and venue integration conceptually possible.

The most useful outcome of Week 2 was therefore not the quantity of documents. It was a decision process. Every planned feature had to answer four questions: what player problem does it address, what is the smallest useful version, what evidence would show that version works, and what fallback protects the prototype if it fails? That process guided the later milestones and prevented replayability from becoming an excuse for unlimited procedural complexity.
""")

    add_figure(doc, DIAGRAM_PATH, 1,
               "Simplified extraction-horror gameplay loop and the replay cycle.",
               "Report synthesis based on the verified project vision, first-playable specification, and implemented P01 loop [8]-[10].",
               width=6.5,
               alt="Six-step loop: accept contract, search, recover rods, adapt under pressure, cycle extraction, then leave and replay with a new seed.")

    add_heading(doc, "3.3 Project Definition and Design Objectives", 2)
    add_body(doc, """
The resulting game direction was a near-future science-fiction extraction-horror experience in which a contractor accepts dangerous short jobs. The internship prototype focused on one contract: enter a damaged nuclear facility, identify three live fuel rods, transport them to an intake, survive the Owl, complete a timed extraction cycle, and cross the open exit. The contractor fantasy gave the player agency and a clear reason to be in the facility; the horror came from incomplete information and an intelligent pursuer rather than from graphic violence.

The current slice used three active rods because repeated trips created route decisions and escalating exposure. The Geiger counter supported searching, while scan/acceptance rules distinguished correct rods. Run variation came from seeded objective placements, route states, threat configuration, and other authored choices. This produced procedural feeling without the risk of generating broken geometry. The design also protected rapid reset because an arcade operator needs a reliable way to start the next session.

The design pillars were venue-only immersion, dangerous contract fantasy, replayable uncertainty, an adaptive adversary, and comfort/reliability. These were ideals rather than completed validation results. For example, the vision targeted a physical effect, but real haptic hardware was unavailable in the recorded evidence. The report therefore treats the hardware ambition as a design driver and limitation, not as a delivered feature.
""")

    add_heading(doc, "3.3.1 Player Requirements and Acceptance Criteria", 3)
    add_body(doc, """
The project definition was converted into requirements that could guide implementation. The first requirement was contract clarity. A player entering the facility should understand that the job involves finding live rods, verifying them, carrying them to the intake, surviving pressure, and leaving through extraction. The interface, objective feedback, Geiger response, rod state, and extraction sequence therefore had to agree. If one system reported progress differently from another, the game would create confusion rather than horror.

The second requirement was a complete critical path. Every supported run configuration needed at least one valid route from entry to the required objectives, then to the intake and exit. This requirement influenced level topology, objective placement, door or route states, reset behavior, and automated tests. Variation was acceptable only when the contract remained completable. This was one reason the project used curated combinations and declarative constraints instead of unrestricted procedural geometry.

The third requirement was readable threat pressure. The Owl needed to respond to credible information such as confirmed sight, sound, and last-known position without appearing omniscient. Weaker stimuli could influence investigation but should not erase stronger evidence arbitrarily. Adaptation had to remain bounded and run-local so the player could change tactics and so the developer could reproduce the result. Automated criteria could check evidence priority, state transitions, caps, and reset behavior; human criteria were still needed to determine whether the same behavior felt fair.

The fourth requirement was replayable but explainable variation. A different seed could alter authored objective positions, route states, lighting, or threat configuration, yet the game still needed recognizable rules and a reliable reset. The purpose was not randomness for its own sake. Variation should change a decision, expose a different risk, or create a new route story while preserving the player's ability to learn from the previous run.

The fifth requirement concerned venue operation. The prototype should start predictably, complete within a controlled session, recover from a finished or failed run, and expose enough diagnostic state for an operator or developer to understand a problem. Windows build and smoke checks supported startup evidence, while version metadata and an enabled build scene reduced ambiguity about what was being delivered. Full operational reliability, however, would require longer runs and repeated reset testing on the target installation.

The final requirement was safe extensibility toward XR and physical effects. Desktop controls and simulated device seams allowed the core contract to be developed without target hardware, but they did not count as headset or haptic validation. Any later heat, airflow, vibration, or heart-rate feature would need an understandable in-world meaning, a safe intensity, a reliable fallback, and evidence from the actual device and representative participants. This requirement allowed the internship prototype to preserve the product direction while being honest about the evidence it could deliver.
""")

    add_heading(doc, "3.4 Game and Systems Design", 2)
    add_body(doc, """
The Owl was designed to appear intelligent without depending on machine learning. Its behavior used a state machine: a controlled set of modes such as patrolling, investigating, pursuing, and recovering, with explicit conditions for movement between them. It also stored bounded run-local evidence about player behavior. For example, repeated route tendencies could bias later patrol choices. The adaptation was deterministic, explainable, capped, and cleared when the run reset. It did not train on customers, remember players across sessions, or autonomously invent behavior.

This distinction supported both fairness and engineering. A venue operator and developer need to be able to explain why the entity chose a route, reproduce a problematic run, and disable adaptation for comparison. The project therefore kept an operator overlay and seed controls. The player-facing ambition was that the Owl would pressure repeated tactics while leaving credible counterplay: break sight, change routes, manage sound, use cover, and create distractions. Automated rules could verify priorities and thresholds, but only human observation could establish whether these responses felt fair.

Replayability also influenced content architecture. The broader product direction allowed curated combinations of sites, objectives, entities, and conditions, but the internship protected one nuclear contract. Compatibility would be authored rather than assuming every module could combine with every other module. This reduced combinatorial testing and allowed each physical effect to keep a narrow meaning. It also demonstrated a key project-management lesson: a scalable concept begins by proving one complete example, not by constructing a generalized system before its player value is known.
""")

    add_heading(doc, "3.5 Weeks 3-4: Unity Vertical Slice and Prototype 1", 2)
    add_body(doc, """
Weeks 3 and 4 moved from planning into Unity implementation. The initial vertical slice established a desktop player, facility greybox, interaction system, fuel-rod objective, intake and extraction states, threat behavior, run configuration, debugging support, and automated tests. Desktop controls were used because target XR hardware and runtime were not confirmed. This was a deliberate evidence layer: it allowed rapid testing of contract logic and navigation without pretending to validate headset comfort or physical interaction.

The scene was built as a greybox, meaning a simplified environment used to test size, routes, interactions, and pacing before final art. Early versions emphasized functional completeness. Later Week 4 work improved route readability, interactions, environment presentation, fullscreen behavior, and versioning. A run-local adaptation system was introduced without allowing it to override more credible information. Sight, sound, route knowledge, and adaptation needed clear priority rules so the Owl would not appear omniscient.

Several technical problems became useful learning moments. Movement sometimes disappeared at high refresh rates because individual movement steps fell below `CharacterController.minMoveDistance`; setting the threshold to zero removed that loss. OpenXR startup without a headset required build-time configuration handling rather than a claim that XR itself had been tested. Changes around XR initialization also needed to preserve whether a rod was currently held. Procedural footsteps initially sounded too harsh and were softened, but their final perceptual quality was not established by a later human test.

By the end of Week 4, the project had a functioning prototype family and a stronger process for protecting previous behavior through tests. It was still not a finished game. Visual fidelity, first-time comprehension, pacing, fairness, XR comfort, haptics, and real customer desire to replay remained open.
""")

    add_heading(doc, "3.5.1 Implementation Workflow and Technical Work", 3)
    add_body(doc, """
Implementation followed a vertical-slice strategy. Instead of building every system to its final depth, I aimed to connect the smallest versions of movement, interaction, objectives, threat pressure, feedback, and extraction into one runnable contract. This exposed integration problems earlier than separate feature demos would have. A fuel rod was not useful evidence merely because it existed as an object; it needed to be detected, accepted or rejected correctly, carried through state changes, delivered to the intake, counted toward the objective, and preserved through the transition into extraction.

The Unity work also required attention to scene and data ownership. Reusable prefabs held consistent object structures, while scene instances supplied location-specific context. Scriptable or inspectable configuration represented run choices without scattering constants across unrelated components. Debug displays made otherwise invisible state visible during development, including the current run seed, objective progress, threat evidence, and adaptation state. These facilities shortened debugging because a failure could be classified as a rule problem, a scene-reference problem, a configuration problem, or an interaction problem before editing code.

Each technical change was considered against the current baseline. A new behavior could not be allowed to erase a stronger threat signal, strand the objective, break the reset path, or make the standalone build fail at startup. Focused tests protected the narrow milestone, while the broader suite checked that earlier behavior survived. Play Mode tests then exercised selected runtime relationships that Edit Mode alone could not establish. This layered approach was especially important when changes involved XR initialization or scene lifecycle, because a script could appear correct in isolation while failing after Unity created, enabled, or reconfigured objects.

Build preparation was treated as technical work rather than an administrative final step. Fullscreen behavior, product/version metadata, build-scene identity, OpenXR feature configuration, and startup diagnostics could all change the result outside the Editor. Producing a Windows development build and launching it through a smoke window provided evidence that the packaged application started, but I kept that claim narrow. The build process did not replace a complete session, human usability study, or target-headset test.

This workflow taught me that game development involves the relationships between content, state, tooling, configuration, and validation. A visible mechanic may be the part the player notices, but reliable delivery also depends on reset behavior, reproducible configuration, clear ownership, and evidence that older systems were not silently damaged.
""")

    add_heading(doc, "3.6 Week 5: v0.3 Readable Stealth and Counterplay", 2)
    add_body(doc, """
Version 0.3.0.0 narrowed the development question to readable stealth and contractor counterplay. The milestone strengthened how the Owl interpreted confirmed sight, sound, and last-known information, and it introduced portable or fixed distractions without allowing a weak stimulus to erase a more credible target. Graded footsteps and restrained investigation cues were intended to expose cause and effect through the world rather than requiring a permanent stealth meter [12].

The delivery was supported by exact recorded evidence: 22 of 22 focused Edit Mode tests, 133 of 133 total Edit Mode tests, and six of six Play Mode tests passed. Unity's Edit Mode tests run in the Editor and can exercise game and editor code, while Play Mode tests exercise behavior in the runtime context [4]. The v0.3 Windows development build also completed and launched through its recorded smoke window. A smoke test is a short check that the application starts and remains alive; it is not a complete playthrough or proof of experience quality.

These results showed that documented rules and integration cases behaved as expected at delivery. They did not show that an uninformed player understood noise, cover, distractions, or the Owl's adaptation. The human-validation sheet remained open, which is why the report uses “recorded automated validation” rather than “fully tested.”
""")

    add_heading(doc, "3.7 Week 6: v0.4 Spatial Readability", 2)
    add_body(doc, """
Version 0.4.0.0 protected the v0.3 gameplay baseline while replacing the map with a stronger spatial experiment. The selected 7C layout used three unequal lobes around an inaccessible central void, joined by three pairwise connectors. Each lobe had a different identity and one objective pocket, while the connectors offered different risk and visibility patterns. The goal was to help the player form a mental map and make deliberate route choices under pursuit [13], [14].

The layout was represented declaratively before the scene was generated. In this context, declarative topology means that rooms, nodes, links, objective relationships, and route requirements were expressed as data or inspectable rules rather than existing only as manually placed geometry. Automated tests could therefore ask whether every objective had the required graph-distinct routes, whether connectors were used correctly, and whether the previous gameplay systems remained integrated.
""")

    add_figure(doc, V040_OVERHEAD, 2,
               "P01 v0.4 overhead view showing the three-lobed greybox and central void.",
               "Project capture from the tracked v0.4.0.0 delivery; geometry is implemented greybox evidence, not final art.",
               width=6.4,
               alt="Overhead Unity scene view of a greybox facility arranged as three lobes around a central open void.")

    add_body(doc, """
The recorded v0.4 delivery passed 28 of 28 focused Edit Mode tests, 141 of 141 total Edit Mode tests, and seven of seven Play Mode tests. A Windows development build was produced, and the application launched without a recorded project-code exception during a 12-second smoke window. The build record noted two expected rendering warnings. The main P01 scene remained the enabled Build Settings identity, which makes v0.4 the strongest completed, tracked, and build-validated prototype in the repository.

The milestone still had an important limitation. Automated topology tests could prove graph relationships, object presence, and documented integration cases, but they could not prove that a new player recognized the lobes, understood the connectors, felt the Owl was fair, or remained comfortable in VR. These outcomes required uninformed human and headset testing that was not completed.
""")

    add_heading(doc, "3.7.1 Spatial Design Decisions and Trade-offs", 3)
    add_body(doc, """
The three-lobed layout was chosen as a controlled experiment in mental-map formation. A perfectly symmetrical plan would be easy to generate but difficult to remember because every branch could look equivalent. The v0.4 plan instead gave each lobe a different size, shape, visual identity, and objective pocket. Pairwise connectors created alternative movement paths without turning the level into an unrestricted maze. The central void acted as a stable reference, while the different connectors were intended to carry different visibility and threat costs.

Declarative topology made these intentions inspectable. The project could verify that every required lobe existed, that connectors linked the intended pairs, that objective positions respected the documented rules, and that graph-distinct routes were available. Deterministic seeds then selected among authored states so a failing configuration could be reproduced. This approach offered variation while avoiding a major risk of procedural geometry: a mathematically generated map may technically connect while producing confusing spaces, unsuitable scale, or combinations that have never been designed together.

The work also demonstrated the difference between structural and experiential evidence. A route graph can prove that two locations are connected, but it does not prove that the route is visible, memorable, comfortable, or strategically interesting. A NavMesh can report a navigable surface, but it does not prove that the Owl will move naturally around corners or that the player will understand an escape path under pressure. Automated validation therefore protected the authored contract, while the human-validation sheet deliberately left perception questions open.

This distinction influenced my later P02 decisions. The approved hand-drawn plan was more detailed and visually specific than the generated P01 greybox, but it still had to be translated into wall thickness, doorway width, usable corners, and navigable occupied space. P01 taught me to preserve topological intent; P02 taught me that physical construction adds another layer of constraints that a diagram alone cannot resolve.
""")

    add_figure(doc, V040_RUNTIME, 3,
               "Representative P01 v0.4 runtime view at the player spawn.",
               "Project capture from the tracked v0.4.0.0 prototype.",
               width=6.4,
               alt="Dark first-person runtime view inside the P01 greybox facility near the player spawn.")

    add_heading(doc, "3.8 Weeks 7-8: Approved Map and Manual P02 Construction", 2)
    add_body(doc, """
The final two internship weeks started P02, a separate approved-map workstream. I personally drew the floor plan and used it as the locked topology reference. Unlike P01's generated greybox, P02 translated a hand-drawn plan into a modular environment assembled from reusable wall, corner, doorway, floor, and structural prefabs. The exercise shifted the project from systems-first proof toward detailed level construction.
""")

    add_figure(doc, PLAN_IMAGE, 4,
               "Student-authored locked floor plan used as the P02 topology authority.",
               "Drawn by Motaz Pharaon. The plan is a design source; generated concept artwork was not used as proof of geometry.",
               width=6.1,
               alt="Hand-drawn overhead floor plan of the approved P02 nuclear facility, with rooms, corridors, objectives, and extraction markers.")

    add_body(doc, """
The environment used Creepy Cat's 3D Scifi Kit Starter Kit, distributed through the Unity Asset Store under the Standard Unity Asset Store EULA [6], [7]. I did not model the imported kit from scratch. My work was to select and import the pack, understand its modular dimensions, adapt it to the plan, place the wall pieces manually, and create or adjust custom transition pieces where the available modules could not express the required angles cleanly.

Translation required a consistent scale. The facility-kit documentation recorded a working conversion of 0.025 meters per plan pixel, a 0.30-meter wall thickness, and a 3.60-meter wall height [15]. Those numbers helped align the drawing with reusable prefabs, but they did not eliminate manual judgment. Corners that looked simple in two dimensions could produce overlaps or gaps in Unity. Mirrored pieces changed collider and orientation behavior. Small angle errors accumulated across long walls. Door openings needed enough clearance, and hierarchy organization had to remain understandable as the shell grew.

Week 7 produced the facility-kit groundwork and the first manually authored rooms: the Entrance, Room 2, and Room 3. Week 8 expanded the shell through most of the plan and involved repeated corrections to wall angle, thickness, connection, and hierarchy. The Git history records “a few left to go” on August 21. The commits that recorded “rooms done” and a forgotten-wall correction were made on August 22, so the report does not move them backward into the official 320 hours.
""")

    add_heading(doc, "3.8.1 Blender Customization and Modular-Fit Work", 3)
    add_body(doc, """
A substantial part of the P02 work was performed in Blender before the pieces were usable in the Unity scene. Creepy Cat's kit provided the visual language and source models, but the approved floor plan contained turns and transitions that the standard modules could not express cleanly. I spent many hours adapting those imported build pieces to suit the plan, including producing 22.5-degree and 5-degree corner variants from the Creepy Cat models as a base. These were custom adaptations of licensed source assets, not a claim that I created the original kit or its art direction from scratch.

The need for these variants came from the relationship between the drawing and the modular grid. A floor-plan line can turn by any angle, while a commercial kit is normally optimized around a limited set of repeatable dimensions and common corners. Forcing every part of the plan into a right angle would have damaged the approved topology and changed how rooms and corridors connected. Placing ordinary straight modules at small rotations, on the other hand, could create gaps, overlaps, inconsistent thickness, and a visibly broken surface. Custom corner pieces allowed the manual shell to respect more of the drawing while remaining based on reusable assets.

The Blender work required a different type of reasoning from C# development. I had to judge how the source geometry should meet at a corner, how a modified piece would align with neighbouring modules, how its scale and orientation would appear after import, and whether it could be reused instead of solving only one local gap. A change that looked correct in isolation could still fail after import because the pivot, forward direction, wall depth, or neighbouring connection did not match the Unity placement convention. The practical workflow was therefore iterative: adapt a source piece, import or refresh it, test it against the plan and adjacent walls, identify the mismatch, and revise.

These small-angle variants also increased the importance of hierarchy and naming. When many similar straight, mirrored, transition, and corner pieces exist, an unclear label can make later correction unnecessarily slow. Consistent organization allowed me to locate the relevant wall, compare its orientation with the drawing, and replace or adjust it without disturbing a completed room. This became more important as the shell grew and the cost of an early angular error accumulated across several connected segments.

The customization work strengthened the distinction between asset use and asset authorship. Creepy Cat deserves credit for the original modular models and materials. My contribution was selecting the pack, understanding how it was constructed, adapting selected models for angles required by my floor plan, integrating the resulting variants into the facility kit, and manually using those pieces to build the P02 shell. This is a meaningful technical and level-design contribution, but it remains an adaptation of third-party work under the Standard Unity Asset Store EULA.

The experience also changed how I evaluate environment packs. A pack is not automatically a finished level-design solution. Its dimensions, pivots, available angles, visual seams, collider expectations, and import behavior have to be understood in relation to the project's own plan. The hours spent in Blender and Unity were therefore not separate cosmetic polishing; they were part of making the approved topology physically constructible with a coherent modular language.
""")

    add_figure(doc, BLENDER_PLACEHOLDER_PATH, 5,
               "Placeholder for Blender comparison of the licensed base piece and the student-customized 22.5-degree and 5-degree corner variants.",
               "Placeholder requested by the student; replace with a readable Blender screenshot before submission. Creepy Cat source models must remain credited.",
               width=6.4,
               alt="Clearly labelled replacement placeholder requesting a Blender screenshot of the Creepy Cat base model and student-customized corner variants.")

    add_figure(doc, P02_PROGRESS, 6,
               "Manual P02 modular-wall progress captured during the approved-map workstream.",
               "Student wall placement by Motaz Pharaon using Creepy Cat modular assets; project capture from the tracked P02 wall-completion set.",
               width=6.4,
               alt="Overhead Unity scene view showing manually placed modular walls aligned over part of the student-drawn P02 plan.")

    add_body(doc, """
AI-assisted tools contributed to the surrounding workflow. Agents helped draft implementation prompts, analyze map fidelity, suggest editor tooling and tests, and automate disposable facility-kit, collider, or integration layers. The manual P02 wall shell itself was my work. This distinction is important because the presence of a generator or prompt in the repository does not mean it authored all visible geometry. The report separates manual wall construction, imported art, custom transition pieces, and later automated integration by ownership and date.
""")

    add_heading(doc, "3.9 Technical Implementation in Unity", 2)
    add_body(doc, """
The Unity project used C# components for the player, objectives, interactions, threat behavior, run configuration, feedback, testing, and editor automation. The architecture favored narrow responsibilities and interfaces. Session state controlled the current run; seeded configuration selected authored variations; objective components handled rod and extraction rules; the Owl used a state machine plus bounded evidence; and feedback or hardware-facing systems sat behind replaceable seams. This reduced direct dependencies and made it possible to test rules without loading every scene.

The project used both Unity's NavMesh and a fallback route-graph concept. A NavMesh is a surface representation used to calculate where an agent can walk. A route graph represents important locations and connections more abstractly. Structural checks could confirm that navigation data or graph routes existed, but they could not guarantee that the Owl traversed them naturally in every encounter. This distinction became more important in P02, where a large manual shell, generated colliders, and later population work increased the chance that a technically connected route might still be awkward.

Editor tooling was used when repetitive operations needed to be reproducible and disposable. A generator could create a known hierarchy, while validation code checked ownership and counts. The current P02 wall-collider generator, for example, was designed to create separate collider children rather than editing imported prefab assets. However, its present source and related P02 tests are modified or untracked, and no current passing test run was recorded during this report audit. Those source assertions are therefore not presented as completed validation.
""")

    add_heading(doc, "3.9.1 State, Interfaces, and Reproducibility", 3)
    add_body(doc, """
One of the most important technical ideas was explicit state ownership. A game run moves through several phases: preparation, active search, objective progress, extraction, success or failure, and reset. If multiple objects independently decide which phase is active, defects become difficult to reproduce. The project instead treated session state as an authority and allowed player, objective, threat, feedback, and interface systems to respond through defined responsibilities. This made it clearer where a transition should occur and which component should preserve or clear information.

Interfaces were also used to protect future hardware choices. The intended venue version could eventually receive input from XR controllers, environmental effects, or a heart-rate device, but those devices were not available for validated integration. Separating the game rule from the device-specific signal allowed the desktop prototype to use simulated input without pretending that target hardware had been tested. It also made safety-related fallbacks possible: a missing or noisy sensor should not destroy the core contract.

Deterministic configuration connected architecture with debugging. When a seed selected the same authored objective positions, route states, and threat parameters, a problematic run could be repeated rather than described vaguely. The value of determinism was not that every run became predictable to the player; it was that variation remained accountable to the developer. A test, screenshot, or bug report could identify the configuration that produced an outcome, while a new seed still changed the next run.

The project therefore treated debugging support as part of implementation. Operator overlays, inspectable configuration, logs, test fixtures, scene hierarchy, and version identifiers helped answer what the software believed was happening. These tools were especially useful in an AI-assisted workflow because generated code or editor operations still required a human-readable way to confirm state, ownership, and scope. Automation reduced repetition, but reproducibility and inspection were what made the results trustworthy enough to keep.
""")

    add_heading(doc, "3.10 Testing, Debugging, and Build Evidence", 2)
    add_body(doc, """
Testing was organized in layers. Focused Edit Mode suites checked the current milestone's narrow rules. The full Edit Mode suite protected broader systems and editor-generation invariants. Play Mode tests exercised runtime integration in Unity's play context. Windows development builds then checked that the standalone player could be produced, and launch smoke checks looked for immediate startup failure. Each layer answered a different question, so passing one did not replace the others.
""")

    add_captioned_table_title(doc, 2, "Recorded milestone validation evidence")
    table = doc.add_table(rows=1, cols=6)
    table.alignment = WD_TABLE_ALIGNMENT.LEFT
    table.style = "Table Grid"
    set_table_widths(table, [0.7, 1.05, 1.0, 0.85, 1.25, 2.4])
    for cell, text in zip(table.rows[0].cells, ["Milestone", "Focused Edit", "Full Edit", "Play Mode", "Build/smoke", "Qualification"]):
        set_cell_text(cell, text, bold=True, size=8.5, align=WD_ALIGN_PARAGRAPH.CENTER)
        set_cell_shading(cell, "D9D9D9")
    rows = [
        ("v0.3", "22/22", "133/133", "6/6", "Windows build and launch smoke", "Recorded delivery evidence; human stealth readability/fairness remained untested."),
        ("v0.4", "28/28", "141/141", "7/7", "Windows build; 12-second smoke without recorded project-code exception", "Two expected rendering warnings; human spatial readability and XR remained untested."),
        ("Current P02", "Not run", "Not run", "Not run", "No recorded current release", "Modified/untracked tests and builders contain assertions, not a passing result."),
    ]
    for row in rows:
        cells = table.add_row().cells
        for idx, text in enumerate(row):
            set_cell_text(cells[idx], text, size=8.5, align=WD_ALIGN_PARAGRAPH.CENTER if idx < 5 else WD_ALIGN_PARAGRAPH.LEFT)
    set_table_borders(table)
    set_repeat_header_and_keep(table)

    add_body(doc, """
Debugging depended on reproducibility. Seeds allowed a run to be repeated, operator telemetry exposed state and evidence, and tests recorded the expected priority of sight, sound, adaptation, and reset behavior. Build smoke evidence was described narrowly because a 12-second stable launch does not show that a 15-minute session is fun, readable, or free from later defects.

Human evidence remained the largest gap. No recorded uninformed-player results established mental-map formation, counterplay comprehension, fairness, fear, fun, or desire to replay. No headset evidence established VR scale, locomotion comfort, reach, throwing, crouch fatigue, spatial audio, or safety. No physical hardware evidence established heat, airflow, floor vibration, haptics, or heart-rate behavior. Maintaining these “Not Tested” labels was part of the quality process rather than an admission that the automated work had no value.
""")

    add_heading(doc, "3.10.1 Challenges, Diagnosis, and Responses", 3)
    add_body(doc, """
Several challenges showed why diagnosis must begin with evidence instead of the most dramatic explanation. The high-refresh movement problem appeared as if the controller sometimes stopped responding. Inspection showed that the movement calculated for a very short frame could fall below Unity's minimum movement threshold. Setting `CharacterController.minMoveDistance` to zero addressed that threshold loss. The lesson was broader than the individual setting: a defect that appears random may be a deterministic interaction between frame timing and an engine safeguard.

OpenXR startup created a different boundary problem. A desktop build without an attached headset could fail or behave differently because XR feature configuration is evaluated during startup. The response focused on build-time configuration and sanitisation, with recorded restoration of the expected feature set, rather than declaring that VR itself worked. Changes around XR initialization also had to preserve gameplay state such as whether a fuel rod was already held. This showed that platform configuration and gameplay lifecycle can interact even when the visible feature appears unrelated.

Audio polish produced a more subjective challenge. Procedural footsteps initially sounded harsh, so their synthesis was softened. That edit was implemented and observed by the operator, but no later participant study established the final perceptual result. The appropriate response was to keep the change while preserving the open validation question. A technically quieter waveform is not automatically a more readable, frightening, or comfortable player experience.

The P02 wall shell created repeated geometric challenges. Small plan angles did not match the source kit, ordinary rotated pieces produced poor joins, mirrored models changed orientation behavior, and an early error could travel across a long wall. I responded by checking the drawing as the authority, adapting Creepy Cat pieces in Blender, creating 22.5-degree and 5-degree corner variants, and revisiting wall thickness, hierarchy, and connections in Unity. This was slow work, but it protected the approved topology instead of forcing the design to fit only the easiest modules.

Editor instability also required procedural discipline. When Unity produced a lock-message flood, the safe response was to preserve the current state and restart rather than continue making uncertain edits. Similarly, later collider and navigation work had to distinguish structural generation from actual gameplay quality. A collider count or NavMesh path can show that supporting data exists, but it cannot prove comfortable movement, natural pursuit, or a fair encounter. These cases reinforced a consistent lesson: define the observed symptom, isolate the responsible layer, apply the smallest supported response, and qualify what still remains untested.
""")

    add_captioned_table_title(doc, 3, "Validation layers and their evidence boundaries")
    table = doc.add_table(rows=1, cols=3)
    table.alignment = WD_TABLE_ALIGNMENT.LEFT
    table.style = "Table Grid"
    set_table_widths(table, [1.65, 2.55, 3.05])
    for cell, text in zip(table.rows[0].cells, ["Evidence layer", "What it can establish", "What it cannot establish by itself"]):
        set_cell_text(cell, text, bold=True, size=9, align=WD_ALIGN_PARAGRAPH.CENTER)
        set_cell_shading(cell, "D9D9D9")
    evidence_rows = [
        ("Edit Mode tests", "Rules, data, editor tooling, generation invariants, and regression cases", "Runtime feel, player understanding, XR comfort, or complete build behavior"),
        ("Play Mode tests", "Selected runtime integration behavior inside Unity", "A complete human play session or target-headset operation"),
        ("Build and smoke", "Standalone production and short startup stability", "Full-run reliability, fairness, fun, or usability"),
        ("Recorded desktop observation", "Specific operator-observed desktop behavior", "Uninformed participant outcomes or XR/hardware behavior"),
        ("XR and physical hardware", "No completed evidence in this report", "Must remain Not Tested until target devices and participants are used"),
    ]
    for row in evidence_rows:
        cells = table.add_row().cells
        for idx, text in enumerate(row):
            set_cell_text(cells[idx], text, size=9)
    set_table_borders(table)
    set_repeat_header_and_keep(table)

    add_heading(doc, "3.11 Documentation and Project Management", 2)
    add_body(doc, """
The project used a weekly, question-driven agile rhythm. Planning identified one prototype goal and the risks that could invalidate it. Implementation and local checks occurred during the week. Evidence, captures, decision logs, and handoffs were then updated so the next iteration started from a known baseline. This reduced the chance that a future agent—or I myself—would act on an attractive but superseded idea.

Git history provided a dated record of deliverables and manual P02 progress. It also exposed limits. P01 v0.4 was tracked and build-validated, while P02 remained a mix of tracked manual work, modified scene/settings, and untracked later tools and captures. For the report audit I treated all working-tree changes as user-owned and did not reset, clean, stage, commit, push, run Unity, or produce a new game build.

Project documentation also supported handoff. The current implementation state, predecessor work, known risks, facility-kit rules, v0.3 and v0.4 scopes, and human-validation sheets gave later work a clear evidence boundary. The documents did not make every proposal true; they made it easier to state what was approved, implemented, automatically checked, observed, or still unknown.
""")

    add_heading(doc, "3.11.1 Communication, Handoff, and Evidence Discipline", 3)
    add_body(doc, """
Documentation had three audiences. For my supervisor, it summarized the purpose, scope, progress, and risks of an intern-led project. For implementation work, it recorded exact rules, dependencies, acceptance checks, and known limitations. For future continuation, it explained which scene or milestone was authoritative and which proposals had been superseded. Writing for these audiences improved my ability to move between a design explanation and an actionable technical instruction.

The decision log was particularly useful when several reasonable alternatives existed. Recording why a choice was made prevented the project from repeatedly reopening the same debate without new evidence. The prototype log connected those decisions to dated results. Handoff documents then stated the baseline that later work had to preserve, including the exact test counts and the human questions that were still open. Together, these records made changes traceable without suggesting that documentation itself was proof of implementation.

This discipline also improved the final report. Git history confirmed when P02 rooms were added and showed that completion crossed the internship boundary. Captures distinguished the tracked P01 greybox from the later manual P02 shell. Tests established specific automated behavior, while user testimony established my personal actions, learning, and Blender work. Separating these source types allowed the narrative to be comprehensive without becoming inflated.
""")

    add_heading(doc, "3.12 Contribution and Attribution Boundaries", 2)
    add_captioned_table_title(doc, 4, "Contribution boundaries used in this report")
    table = doc.add_table(rows=1, cols=3)
    table.alignment = WD_TABLE_ALIGNMENT.LEFT
    table.style = "Table Grid"
    set_table_widths(table, [1.45, 3.45, 2.35])
    for cell, text in zip(table.rows[0].cells, ["Contributor", "Verified contribution", "Required qualification"]):
        set_cell_text(cell, text, bold=True, size=9, align=WD_ALIGN_PARAGRAPH.CENTER)
        set_cell_shading(cell, "D9D9D9")
    contrib_rows = [
        ("Motaz Pharaon", "Project direction, business case, final design/scope decisions, Unity operation, review/correction, debugging, asset selection/adaptation, Blender corner variants, student-drawn P02 plan, and manual P02 wall shell", "Do not imply every line of code or every generated object was manually authored."),
        ("AI agents/tools", "Design alternatives, analysis, partial code/editor tooling, automated operations, tests, documentation, and report assistance under student direction and review", "Suggestions and source assertions are not automatically implemented or validated facts."),
        ("Creepy Cat", "3D Scifi Kit Starter Kit modular environment assets and source models", "Imported third-party art; student contribution was selection, Blender adaptation including 22.5-degree and 5-degree variants, placement, and integration."),
        ("Joint iteration", "Systems, scopes, validation language, and tool-assisted workflows refined through student decisions and agent-supported analysis", "Final acceptance, rejection, operation, and manual scene decisions remained student-owned."),
    ]
    for row in contrib_rows:
        cells = table.add_row().cells
        for idx, text in enumerate(row):
            set_cell_text(cells[idx], text, size=9)
    set_table_borders(table)
    set_repeat_header_and_keep(table)

    add_body(doc, """
After the internship boundary, I continued development. The tracked P02 completion and forgotten-wall correction on August 22, followed by later collider, gameplay-integration, NavMesh, and population work, are described only as continuation. They are not included in the 320 internship hours and are not presented as a finished P02 release.
""")

    add_heading(doc, "4. Overall Internship Experience", 1, page_break=True)
    add_heading(doc, "4.1 Technical Skills", 2)
    add_body(doc, """
The internship expanded my technical understanding beyond writing individual scripts. I learned to think about how gameplay state, scene construction, reusable prefabs, run configuration, navigation, tests, and build settings interact. Problems often appeared at the boundary between systems: a movement threshold depended on frame timing; an XR initialization change could affect held-object state; a wall prefab's orientation affected collider ownership; and a graph that was structurally connected could still produce poor traversal.

I also learned the value of deterministic and explainable systems. The Owl could have been described vaguely as “learning the player,” but a bounded run-local model was better for the prototype. It gave the impression of adaptation while remaining testable and debuggable. The seed system served a similar purpose by allowing variation without losing reproducibility. These choices made the technical implementation support the intended player experience instead of becoming complexity for its own sake.

Manual P02 construction developed a different technical skill. I had to reason about scale, pivots, angles, wall thickness, doorway dimensions, hierarchy, and modular reuse. The floor plan remained an authority, but implementing it required interpreting how two-dimensional lines become three-dimensional occupied space. This work made level design feel less like drawing rooms and more like managing spatial constraints.
""")

    add_heading(doc, "4.1.1 Specific Technical Skills Gained", 3)
    add_body(doc, """
In Unity, I improved at reading a project as a connected system rather than a collection of scripts. I became more comfortable tracing a behavior from configuration to component state, scene reference, runtime event, test, and build result. I learned to use prefabs as controlled reusable structures, to treat scene hierarchy as technical information, and to recognize when an apparently visual problem originated in ownership, lifecycle, or build configuration.

My C# learning focused on boundaries and testability. State machines made threat behavior explicit. Seeded configuration made variable runs reproducible. Interfaces separated gameplay rules from future hardware. Focused and full Edit Mode suites taught me to distinguish a narrow acceptance check from a regression baseline, while Play Mode tests showed why runtime lifecycle sometimes needs a different evidence layer. Build smoke checks then demonstrated that Editor success is not identical to standalone startup.

Level construction and Blender adaptation expanded my skills beyond code. I learned how modular wall dimensions, pivots, local orientation, thickness, doorway clearance, and small angular differences affect a complete environment. Creating 22.5-degree and 5-degree corner variants from Creepy Cat's source pieces required me to adapt imported geometry while preserving a coherent modular language. Repeatedly comparing Blender output, Unity placement, neighbouring walls, and the approved plan taught me an iterative asset workflow grounded in spatial evidence.

I also gained evidence and debugging skills. A reproducible seed, visible operator state, named capture, commit date, and milestone-specific test count are not paperwork added after development; they help determine what changed and whether an explanation is credible. Learning to separate verified fact, automated validation, operator observation, user testimony, and an unknown result made my technical communication more precise and reduced the risk of exaggerating the prototype.
""")

    add_heading(doc, "4.2 Professional Skills", 2)
    add_body(doc, """
The strongest professional lesson was ownership. My supervisor's trust meant that I could not wait for every task to be prescribed. I needed to propose the work, explain its value, identify risks, choose a feasible milestone, and report honestly when evidence was incomplete. The business case and planning spine helped establish this responsibility at the start of the internship.

I improved at maintaining traceable decisions. Scope changes were recorded, test counts were attached to the correct version, and P01 was kept separate from the unfinished P02 workstream. This discipline matters when a project has many documents and AI-assisted outputs, because polished wording can otherwise hide that an idea was only proposed. I learned to ask: what is the primary evidence, when was it produced, who performed the work, and what does it actually prove?

Another professional skill was accepting limited evidence. It would have been easy to describe passing tests as proof that the level was readable or the game was complete. Instead, I learned that reporting an open human-validation gate protects the project. It tells the next developer or stakeholder what kind of work is required rather than encouraging them to trust the wrong result.
""")

    add_heading(doc, "4.3 Soft Skills", 2)
    add_body(doc, """
The project improved my communication because the idea had to make sense to both technical and non-technical readers. I practiced explaining an extraction loop, state machine, deterministic seed, NavMesh, prefab workflow, and test evidence without relying only on class names. Handoff documents also required me to state boundaries precisely so another person or agent could continue without reopening every decision.

Creative freedom strengthened decision-making. I considered many interesting mechanics—heart rate, physical heat, wind, floor vibration, broader contract combinations, recovery systems, equipment, and more—but learned that saying “not yet” can protect the core idea. The risk register gave those decisions a professional form: a delayed feature was not forgotten; it had a reason, trigger, and fallback.

The internship also improved patience and problem solving. Manual modular construction involved repeated corrections that were not glamorous but were necessary for a coherent shell. Technical debugging sometimes required preserving state, restarting after editor instability, or separating a configuration symptom from a gameplay defect. These experiences made the final outcome feel earned through iteration rather than produced by one successful attempt.
""")

    add_heading(doc, "4.4 University Applied Knowledge", 2)
    add_body(doc, """
Software Architecture and Design was the most useful university course for this project. It taught me how quality attributes influence architecture and how a niche attribute can still be analyzed systematically even if it is not a standard textbook example. I applied that approach to replayability. For Pixoul, replayability was connected to customer return and arcade value, so it could not be treated as a random-number feature added at the end.

Replayability influenced the architecture through deterministic seeds, authored variable states, isolated run configuration, reusable contract concepts, reset behavior, and bounded adaptation. It also influenced quality assurance: variations needed to preserve a valid critical path and be reproducible when a defect appeared. At the design level, it encouraged route choice and meaningful uncertainty. At the project level, it warned against fully procedural geometry and unrestricted module combinations that could create too much content and testing scope.

IT Project Management was the second major connection. I used a business case and proposal to obtain support for the project, then used sprint goals, a backlog, risk responses, priorities, acceptance criteria, cut lines, stakeholder updates, and versioned evidence to pace the work. These tools helped me start a large project without pretending that the whole vision could be delivered in eight weeks.
""")

    add_heading(doc, "4.5 Career Influence", 2)
    add_body(doc, """
The internship made me more certain that I want to continue in game development. I enjoyed the combination of creative direction and technical structure: deciding what the player should feel, translating that into systems and spaces, debugging when reality disagreed with the idea, and then using evidence to decide what changed next.

The most difficult challenge was replayability and my concern about player retention. It was not solved by one mechanic, and the internship did not include a customer-retention study. The useful outcome was learning how to turn the concern into practical architecture and testable decisions. A repeatable arcade game needs a clear first experience, a reliable reset, visible variation, understandable rules, and enough uncertainty to make a second attempt meaningful. Future human testing must determine whether the current prototype actually produces that desire.

The experience also showed me the type of developer I want to become. I want to be capable of owning a design problem across code, architecture, level layout, validation, and communication while remaining honest about work assisted by tools or created by third parties. The project is unfinished as a product, but it gave me a strong foundation for continuing in technical game development and design.
""")

    add_heading(doc, "4.6 Challenges, Learning Outcomes, and Overall Assessment", 2)
    add_body(doc, """
The main design challenge was turning replayability from a broad ambition into decisions that could fit an internship. At first, replayability could have meant more maps, more enemies, more objectives, more equipment, procedural geometry, adaptive behavior, or long-term progression. That definition would have made the project impossible to control. I learned to define it as reliable reset plus authored variation that changes decisions while preserving understandable rules. Deterministic seeds, route states, objective placement, bounded Owl adaptation, and compatibility limits were practical outcomes of that learning.

Player retention remained related but unproven. I worried about whether a customer would want to return after one session, which is especially relevant for an arcade-like venue. The internship taught me that this concern cannot be solved only by adding content. A returning experience also needs a clear first session, a reasonable session length, fast reset, visible differences, fair counterplay, and enough uncertainty to create a new story. The prototype created mechanisms intended to support these qualities, but only observation with representative players can establish whether they produce real repeat interest.

The most demanding production challenge was moving between different kinds of work. Early weeks emphasized research, writing, scope, and architecture. The middle weeks emphasized Unity systems, debugging, automated tests, and builds. The final weeks emphasized floor-plan reasoning, Blender customization, imported-asset adaptation, and manual placement. Each phase used a different definition of progress. A design document needed a clear decision; a script needed correct behavior; a build needed to launch; a wall shell needed consistent physical connections. I learned to adapt my working method without losing the common evidence trail.

Working with AI-assisted tools was another learning outcome. The tools could propose designs, draft code, generate editor operations, or summarize technical state quickly, but speed did not transfer responsibility. I still had to decide whether a suggestion matched the approved scope, inspect what it changed, operate Unity, correct failures, and separate a source assertion from a recorded result. The most valuable skill was not producing more text or code; it was learning to supervise the workflow and preserve authorship accurately. My manual P02 plan, Blender adaptations, and wall placement remain distinct from agent-supported analysis and tooling.

The placement also improved confidence and professional independence. My supervisor's support and near-complete creative freedom allowed me to pursue an ambitious idea, but it also meant that I had to create structure for myself. I learned to justify priorities, communicate uncertainty, accept that some desired features were not ready, and continue after technical setbacks without hiding them. The absence of a rigid task list made ownership one of the most important outcomes of the internship.

Overall, the experience was successful because it changed both what I could produce and how I evaluate production. I completed a validated P01 prototype milestone, substantially constructed a separate approved P02 environment during the internship, developed custom Blender pieces for its geometry, and left a documented path for future work. At the same time, I learned not to confuse a prototype with a product or automated evidence with player experience. The remaining XR, hardware, human-readability, fairness, and retention questions are not failures to conceal; they are a responsible definition of the next stage.

The internship confirmed that game development is the career direction I want to pursue. It combines the parts of information technology that I find most engaging: software structure, interaction, technical problem solving, visual-spatial reasoning, testing, and communication. It also showed me that good game development requires humility. A creative intention matters only when it can be translated into a working experience, evaluated with the right evidence, and revised when the evidence disagrees.
""")

    add_heading(doc, "5. Conclusion", 1, page_break=True)
    add_body(doc, """
The eight-week placement at Pixoul Gaming transformed an early interest in venue haptics and VR horror into a structured Unity prototype and a documented development process. I began with onboarding and research, used a business case and architecture planning to control an ambitious concept, implemented and validated successive P01 milestones, and ended the official internship by translating my own approved floor plan into a mostly constructed manual P02 wall shell.

P01 v0.4.0.0 is the strongest completed outcome. It preserved the extraction loop and readable-stealth systems inside a three-lobed facility and has recorded evidence for 28 focused Edit Mode tests, 141 total Edit Mode tests, seven Play Mode tests, a Windows development build, and a 12-second launch smoke check. P02 is separate: it demonstrates manual level-design and modular-construction learning, but its tracked completion occurred on August 22 and its later integration remains unfinished.

The central lesson was that creative ambition benefits from evidence discipline. Tests, builds, logs, Git history, captures, and handoffs made progress traceable, but none replaced human or headset validation. The project did not establish player readability, fairness, comfort, physical haptic quality, heart-rate behavior, or customer retention. These are the correct next questions, together with completing P02 integration and testing on target venue hardware.

The internship strengthened my intention to work in game development. It allowed me to connect Software Architecture and Design, IT Project Management, Unity development, level construction, AI-assisted workflows, and professional reporting in one project. Most importantly, it taught me to protect authorship and truth: credit imported assets, describe AI assistance accurately, preserve my own manual work, attach claims to the right milestone, and state “not tested” when the necessary evidence does not exist.
""")

    add_heading(doc, "References", 1, page_break=True)
    refs = [
        "[1] Pixoul Gaming, “What We’re All About,” Pixoul Gaming. https://pixoulgaming.com/vr-about-us/ (accessed Aug. 26, 2026).",
        "[2] Pixoul Gaming, “All You Need to Know About Pixoul,” Pixoul Gaming. https://pixoulgaming.com/vr-faq/ (accessed Aug. 26, 2026).",
        "[3] Pixoul Gaming, “Pixoul Gaming Is Set to Open in Al Qana on Friday, Nov 4,” Oct. 25, 2022. https://pixoulgaming.com/vr_news/pixoul-gaming-is-set-to-open-in-al-qana-on-friday-nov-4/ (accessed Aug. 26, 2026).",
        "[4] Unity Technologies, “Edit Mode vs. Play Mode Tests,” Unity Test Framework 2.0. https://docs.unity3d.com/Packages/com.unity.test-framework@2.0/manual/edit-mode-vs-play-mode-tests.html (accessed Aug. 26, 2026).",
        "[5] Unity Technologies, “Prefabs,” Unity 6 Manual. https://docs.unity3d.com/6000.0/Documentation/Manual/Prefabs.html (accessed Aug. 26, 2026).",
        "[6] Creepy Cat, “3D Scifi Kit Starter Kit,” Unity Asset Store. https://assetstore.unity.com/packages/3d/environments/3d-scifi-kit-starter-kit-92152 (accessed Aug. 26, 2026).",
        "[7] Unity Technologies, “Asset Store Terms of Service and EULA,” updated Dec. 4, 2024. https://unity.com/legal/as-terms (accessed Aug. 26, 2026).",
        "[8] Motaz Pharaon, “Vision Brief” and “Sprint 01 Plan,” internal internship project artifacts, July 2026.",
        "[9] Motaz Pharaon and project agents, “Technical Architecture Outline,” internal internship project artifact, July 2026.",
        "[10] Motaz Pharaon and project agents, “First Playable Specification,” internal internship project artifact, July 2026.",
        "[11] Motaz Pharaon and project agents, “Prototype Log,” internal living project artifact, July-Aug. 2026.",
        "[12] Motaz Pharaon and project agents, “v0.3.0.0 Readable Stealth Scope,” internal project artifact, July 2026.",
        "[13] Motaz Pharaon and project agents, “v0.4.0.0 Spatial Readability Scope,” internal project artifact, Aug. 2026.",
        "[14] Motaz Pharaon and project agents, “v0.4.0.0 7C Level Plan” and “Implementation Handoff,” internal project artifacts, Aug. 2026.",
        "[15] Motaz Pharaon and project agents, “Facility Kit and P02 Authoring,” internal implementation record, Aug. 2026.",
        "[16] Motaz Pharaon, “Bi-Weekly Internship Report, Weeks 3 and 4,” signed internship record, July 2026.",
        "[17] Motaz Pharaon, “Bi-Weekly Internship Report, Weeks 5 and 6,” signed internship record, Aug. 2026.",
    ]
    for ref in refs:
        p = doc.add_paragraph()
        p.alignment = WD_ALIGN_PARAGRAPH.LEFT
        p.paragraph_format.left_indent = Inches(0.3)
        p.paragraph_format.first_line_indent = Inches(-0.3)
        p.paragraph_format.space_after = Pt(5)
        p.paragraph_format.line_spacing = 1.0
        p.add_run(ref)

    add_heading(doc, "Appendix A. Technical Terms Used in the Report", 1, page_break=True)
    glossary = [
        ("Build smoke test", "A short startup check used to confirm that a standalone build launches and does not immediately fail. It is not a complete playthrough."),
        ("Declarative topology", "A representation of rooms, nodes, links, and constraints as inspectable data or rules rather than only as scene geometry."),
        ("Deterministic seed", "An input that reproduces the same authored selections, allowing a variable run to be debugged and tested again."),
        ("Edit Mode test", "A Unity test executed in the Editor context, suitable for rules, data, editor tooling, and many generation checks."),
        ("Greybox", "A simplified environment used to validate scale, layout, interactions, navigation, and pacing before final art."),
        ("NavMesh", "A navigable-surface representation used by game agents to calculate movement through a scene."),
        ("Play Mode test", "A Unity test that exercises selected behavior in the runtime/play context."),
        ("Prefab", "A reusable Unity asset whose instances share a defined structure, useful for modular objects such as walls and doors."),
        ("Raycast", "A query projected through the scene to detect what a player or system is pointing at or can see, usually through colliders."),
        ("State machine", "A controlled set of behavior states and explicit rules for moving between them."),
    ]
    table = doc.add_table(rows=1, cols=2)
    table.alignment = WD_TABLE_ALIGNMENT.LEFT
    table.style = "Table Grid"
    set_table_widths(table, [1.55, 5.7])
    for cell, text in zip(table.rows[0].cells, ["Term", "Plain-language meaning in this project"]):
        set_cell_text(cell, text, bold=True, size=9.5, align=WD_ALIGN_PARAGRAPH.CENTER)
        set_cell_shading(cell, "D9D9D9")
    for term, meaning in glossary:
        cells = table.add_row().cells
        set_cell_text(cells[0], term, bold=True, size=9.5)
        set_cell_text(cells[1], meaning, size=9.5)
    set_table_borders(table)
    set_repeat_header_and_keep(table)

    add_heading(doc, "Appendix B. Post-Internship Continuation Boundary", 1, page_break=True)
    add_body(doc, """
The official internship ended on August 21, 2026. Git history records that most P02 rooms had been constructed by that date, with a few still left to complete. On August 22, the wall shell was recorded as complete and received one additional forgotten-wall correction. Later work included collider, gameplay-integration, NavMesh, hierarchy-audit, and scene-population activity.

This continuation is relevant because it shows how the internship project developed, but it is not included in the 320 internship hours. The current P02 workstream is still not presented as a release: its scene is not the enabled Build Settings identity, its current working state includes modified and untracked files, and no current full P02 test/build result was recorded during the report audit.
""")

    core = doc.core_properties
    core.title = "Final Internship Report - Game Development at Pixoul Gaming"
    core.subject = "ITE399B Internship in IT"
    core.author = "Motaz Pharaon"
    core.keywords = "internship, Pixoul Gaming, Unity, game development, extraction horror, Abu Dhabi University"
    core.comments = "Prepared from verified project evidence; original source files preserved."

    doc.save(DOCX_PATH)
    print(DOCX_PATH)


if __name__ == "__main__":
    build_report()
