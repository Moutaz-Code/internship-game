from pathlib import Path

from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER, TA_LEFT
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle
from reportlab.lib.units import mm
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.pdfgen import canvas
from reportlab.platypus import Frame, KeepTogether, Paragraph, Spacer


WORKSPACE = Path(r"C:\Users\Mou\Documents\Codex\Internship Game")
OUTPUT = WORKSPACE / "output" / "pdf" / "Bi-Weekly_Intern_Report_Weeks_5_and_6_2026.pdf"
LOGO = WORKSPACE / "tmp" / "pdfs" / "biweekly_reference" / "adu_logo.png"
INTERN_SIGNATURE = WORKSPACE / "tmp" / "pdfs" / "signature_extract" / "intern_signature.png"
COMPANY_SIGNATURE = WORKSPACE / "tmp" / "pdfs" / "signature_extract" / "company_supervisor_signature.png"

PAGE_W, PAGE_H = A4
BLACK = colors.HexColor("#111111")
LIGHT_GREY = colors.HexColor("#D1D1D1")
FOOTER_GREY = colors.HexColor("#D9D9D9")
WORK_GREEN = colors.HexColor("#00B050")


pdfmetrics.registerFont(TTFont("Arial", r"C:\Windows\Fonts\arial.ttf"))
pdfmetrics.registerFont(TTFont("Arial-Bold", r"C:\Windows\Fonts\arialbd.ttf"))


assignments = [
    "Defined and implemented the v0.3.0.0 Readable Stealth and Contractor Counterplay prototype around one clear question: whether players could understand and deliberately manage the Owl's sight, sound, hiding, and distraction behavior without a stealth meter.",
    "Reworked Owl vision into three-sample Concealed, Noticeable, and Exposed evidence with graded confirmation rates instead of instant binary detection.",
    "Unified crouching, walking, sprinting, landing, and object impacts through a normalized sound-stimulus path with distance falloff and an explicit hearing threshold.",
    "Separated confirmed sight, accepted sound, last-seen position, last-heard position, search probes, and patrol targets so the Owl acted on credible information rather than a hidden live player position.",
    "Protected full visual cover followed by quiet relocation as a dependable stealth rule, with current confirmed sight taking priority over distracting sounds.",
    "Added nine portable metal distractions with single-impact gating, spatial audio, cooldown protection, and complete reset behavior.",
    "Added three maintenance-test panels that create localized machinery distractions, visible active states, cooldowns, and matching procedural audio.",
    "Expanded F1/F3 developer telemetry to expose visual samples, confirmation, movement noise, hearing decisions, knowledge source, target position, route-adaptation state, and panel status.",
    "Validated v0.3.0.0 with 22 focused Edit Mode tests, 133 total Edit Mode tests, six Play Mode tests, a clean Unity console, a Windows development build, and a launch smoke test.",
    "Designed the v0.4.0.0 Spatial Readability and Pursuit Topology experiment and locked an asymmetric three-lobed nuclear facility connected by North Pipe Gallery, Utility Spine, and South Maintenance Channel.",
    "Replaced the previous rectangular greybox with the new facility while preserving the completed objective, extraction, stealth, feedback, reset, and route-adaptation systems.",
    "Authored the facility's portals, objective sockets, room landmarks, cover grammars, maintenance panels, and portable distraction placements from deterministic layout data.",
    "Built an explicit 20-node route model with three supported route profiles and two graph-distinct approaches to every required objective location.",
    "Changed route evidence from world-position inference to authored connector zones and Owl nodes, improving the fairness and explainability of adaptive pursuit.",
    "Implemented deterministic objective distribution with exactly one live fuel rod per facility lobe and two inert decoys selected from unused sockets.",
    "Authored 18 Owl navigation and search nodes and expanded operator telemetry with spatial labels, connector identity, route profile, target node, navigation mode, and fallback status.",
    "Validated v0.4.0.0 with 28 focused Edit Mode tests, 141 total Edit Mode tests, seven Play Mode tests, a clean Windows build, a launch smoke test, and overhead and room-level evidence captures.",
    "Prepared scope, implementation, validation, risk, and agent-handoff documentation so the project could be transferred between development machines without losing authority or known limitations.",
]


skills = [
    "Unity 6 C# development with the Universal Render Pipeline, Unity AI Navigation, procedural spatial audio, Rigidbody interaction, and deterministic scene generation.",
    "Designing graded visual-confirmation systems, hearing thresholds, evidence priority, last-known-position tracking, and fair stealth counterplay.",
    "Creating reusable portable and fixed distraction systems with impact gating, cooldowns, spatial feedback, and complete reset semantics.",
    "Graph-based level design using explicit rooms, connectors, route profiles, portal dimensions, objective sockets, and authored AI nodes.",
    "Designing asymmetric greybox spaces around landmarks, sight breaks, recovery cover, alternative paths, and readable circulation.",
    "Separating gameplay semantics from compatibility identifiers so existing systems could be preserved while the level topology changed.",
    "Using deterministic seeds and constrained selection rules to guarantee meaningful objective distribution and reproducible test cases.",
    "Writing focused Edit Mode tests, integrated Play Mode tests, topology validation, NavMesh reachability checks, and reset regressions.",
    "Producing Windows development builds, launch smoke tests, console audits, runtime captures, and implementation evidence.",
    "Maintaining scope documents, technical handoffs, decision records, known-risk registers, and evidence-based cut lines.",
]


knowledge = [
    "Learned that readable stealth depends on causal feedback: the player should be able to distinguish current sight, accepted sound, and remembered evidence instead of treating every enemy reaction as arbitrary.",
    "Learned that current confirmed sight must outrank a remote distraction; otherwise the player can unrealistically redirect an enemy that is already watching them.",
    "Learned to evaluate sound after distance falloff and to separate the hearing threshold from the stronger evidence threshold used for long-term route adaptation.",
    "Learned that full cover is only useful when the AI also supports last-seen memory, quiet relocation, search behavior, and eventual disengagement.",
    "Gained experience designing procedural audio and local visual feedback so stealth information remains diegetic rather than dependent on a HUD meter.",
    "Learned that a map's topology should be represented explicitly. Inferring route choice from world coordinates can confuse destinations with the paths used to reach them.",
    "Learned that landmarks and room silhouettes support mental mapping, but connector identity and repeated circulation patterns are equally important for navigation.",
    "Learned to use constrained deterministic generation to ensure every run exercises the intended experiment, such as requiring one live objective in each lobe.",
    "Improved my understanding of pursuit design: patrol nodes, search probes, connector evidence, NavMesh movement, and graph fallback each solve different problems and should remain independently testable.",
    "Learned that automated reachability proves technical connectivity but cannot prove that a player understands the layout, feels safe carrying an object through it, or considers the Owl fair.",
    "Learned to preserve known limitations honestly. Desktop automation and launch smoke tests do not validate XR comfort, spatial audio in a headset, haptics, physical effects, or uninformed player comprehension.",
    "Learned that hard scope boundaries and ordered cut lists protect the prototype question when schedule pressure encourages unrelated feature expansion.",
    "Gained experience producing implementation handoffs that record authority, ownership, validation evidence, open risks, and exact restart procedures rather than relying on undocumented memory.",
]


def draw_logo(c, x, y, width=118):
    height = width * 43 / 156
    c.drawImage(str(LOGO), x, y, width=width, height=height, preserveAspectRatio=True, mask="auto")


def draw_page_shell(c, page_number, top_logo=False):
    c.setStrokeColor(BLACK)
    c.setLineWidth(0.8)
    c.rect(20, 20, PAGE_W - 40, PAGE_H - 40)

    if top_logo:
        draw_logo(c, 90, PAGE_H - 72, 118)

    draw_logo(c, 28, 34, 118)
    c.setFont("Arial", 10.5)
    c.setFillColor(BLACK)
    c.drawString(155, 54, "Career Development Department")
    c.setFont("Arial", 9.4)
    c.drawString(155, 42, "P.O. Box 59911, Abu Dhabi, United Arab Emirates Tel. 02-5015919 | internship@adu.ac.ae")

    c.setFillColor(FOOTER_GREY)
    c.rect(0, 0, 200, 13, stroke=0, fill=1)
    c.rect(PAGE_W - 100, 0, 100, 13, stroke=0, fill=1)
    c.setFillColor(BLACK)
    c.setFont("Arial", 6.8)
    c.drawString(25, 3.3, "Document Ref: ______________________________")
    c.drawRightString(PAGE_W - 16, 3.3, f"Page {page_number} of 5")


def half_hour_labels():
    values = []
    minutes = 7 * 60
    while minutes <= 20 * 60 + 30:
        hour_24, minute = divmod(minutes, 60)
        suffix = "AM" if hour_24 < 12 else "PM"
        hour_12 = hour_24 % 12 or 12
        values.append(f"{hour_12}:{minute:02d}{suffix}")
        minutes += 30
    return values


def draw_timesheet(c, top, week_number, beginning, ending):
    x = 34
    width = PAGE_W - 68
    day_col = 30
    times = half_hour_labels()
    slot = (width - day_col) / len(times)
    header_h = 40
    row_h = 17
    days = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"]

    c.setFillColor(BLACK)
    c.setFont("Arial", 11.5)
    c.drawString(44, top, f"Week # {week_number}")
    c.drawString(44, top - 15, f"Week Beginning: {beginning}")
    c.drawString(270, top - 15, f"Week Ending: {ending}")

    table_top = top - 32
    table_bottom = table_top - header_h - row_h * len(days)
    c.setFillColor(colors.HexColor("#E8E8E8"))
    c.rect(x, table_top - header_h, width, header_h, stroke=0, fill=1)

    for row, day in enumerate(days):
        y = table_top - header_h - (row + 1) * row_h
        if day in {"Mon", "Tue", "Wed", "Thu", "Fri"}:
            for idx, label in enumerate(times):
                hour_text = label[:-2]
                suffix = label[-2:]
                h, m = hour_text.split(":")
                hour = int(h) % 12 + (12 if suffix == "PM" else 0)
                minute = int(m)
                value = hour + minute / 60
                if 10 <= value < 14 or 15 <= value < 19:
                    c.setFillColor(WORK_GREEN)
                    c.rect(x + day_col + idx * slot, y, slot, row_h, stroke=0, fill=1)

    c.setStrokeColor(colors.HexColor("#555555"))
    c.setLineWidth(0.35)
    c.rect(x, table_bottom, width, header_h + row_h * len(days), stroke=1, fill=0)
    c.line(x + day_col, table_bottom, x + day_col, table_top)
    for idx in range(len(times) + 1):
        xx = x + day_col + idx * slot
        c.line(xx, table_bottom, xx, table_top)
    c.line(x, table_top - header_h, x + width, table_top - header_h)
    for row in range(len(days) + 1):
        yy = table_top - header_h - row * row_h
        c.line(x, yy, x + width, yy)

    c.setFillColor(BLACK)
    c.setFont("Arial-Bold", 6.4)
    for idx, label in enumerate(times):
        xx = x + day_col + idx * slot + slot * 0.66
        c.saveState()
        c.translate(xx, table_top - header_h + 3)
        c.rotate(90)
        c.drawString(0, 0, label)
        c.restoreState()
    for row, day in enumerate(days):
        yy = table_top - header_h - (row + 0.72) * row_h
        c.saveState()
        c.translate(x + 20, yy)
        c.rotate(90)
        c.drawString(0, 0, day)
        c.restoreState()

    c.setFont("Arial", 11.5)
    c.drawString(44, table_bottom - 26, "Total Working Hours: ______40______")
    return table_bottom - 58


def draw_cover_page(c):
    draw_page_shell(c, 1, top_logo=True)
    c.setFillColor(LIGHT_GREY)
    c.rect(45, PAGE_H - 178, 235, 37, stroke=0, fill=1)
    c.setFillColor(BLACK)
    c.setFont("Arial-Bold", 17)
    c.drawString(53, PAGE_H - 165, "Bi-weekly Intern Report")

    c.setFont("Arial", 12)
    c.drawString(45, PAGE_H - 201, "Name of Intern: Motaz Pharaon")
    c.drawString(45, PAGE_H - 222, "Abu Dhabi University ID #: 1086106")
    c.drawString(45, PAGE_H - 243, "Name of Organization: Pixoul Gaming")

    next_top = draw_timesheet(c, PAGE_H - 270, 5, "July 27th 2026", "July 31st 2026")
    draw_timesheet(c, next_top, 6, "August 3rd 2026", "August 7th 2026")
    c.showPage()


body_style = ParagraphStyle(
    "body",
    fontName="Arial",
    fontSize=10.5,
    leading=12.8,
    textColor=BLACK,
    alignment=TA_LEFT,
    spaceAfter=2.0,
    allowWidows=0,
    allowOrphans=0,
)
heading_style = ParagraphStyle(
    "heading",
    parent=body_style,
    fontName="Arial-Bold",
    fontSize=12.3,
    leading=14.5,
    spaceBefore=3,
    spaceAfter=6,
)
title_style = ParagraphStyle(
    "title",
    parent=body_style,
    fontSize=12.5,
    leading=15,
    alignment=TA_LEFT,
    spaceAfter=13,
)


def bullet(text):
    return Paragraph("- " + text, body_style)


def draw_text_page(c, page_number, story, top_logo=False):
    draw_page_shell(c, page_number, top_logo=top_logo)
    top = PAGE_H - (95 if top_logo else 42)
    frame = Frame(45, 79, PAGE_W - 90, top - 79, leftPadding=0, rightPadding=0, topPadding=0, bottomPadding=0, showBoundary=0)
    frame.addFromList(story, c)
    if story:
        raise RuntimeError(f"Page {page_number} overflowed with {len(story)} flowables remaining")
    c.showPage()


def draw_signature_page(c):
    draw_page_shell(c, 5, top_logo=True)
    c.setFillColor(BLACK)
    c.setFont("Arial", 12.5)
    c.drawString(45, PAGE_H - 125, "Intern Signature:")
    c.drawString(45, PAGE_H - 150, "Company Supervisor:")
    c.drawString(45, PAGE_H - 175, "ADU Faculty Supervisor:")
    c.setLineWidth(0.7)
    c.line(140, PAGE_H - 128, 355, PAGE_H - 128)
    c.line(166, PAGE_H - 153, 355, PAGE_H - 153)
    c.line(181, PAGE_H - 178, 355, PAGE_H - 178)

    # The source report contains transparent signature overlays. Reuse only the
    # intern and company-supervisor marks authorized by the user; the faculty line
    # deliberately remains unsigned.
    c.drawImage(str(INTERN_SIGNATURE), 148, PAGE_H - 130, width=154, height=23.8,
                preserveAspectRatio=True, mask="auto")
    c.drawImage(str(COMPANY_SIGNATURE), 173, PAGE_H - 174, width=112, height=42.7,
                preserveAspectRatio=True, mask="auto")
    c.setFont("Arial-Bold", 12.5)
    c.drawCentredString(PAGE_W / 2, PAGE_H - 220, "Please forward this form to your faculty supervisor")
    c.showPage()


def build():
    OUTPUT.parent.mkdir(parents=True, exist_ok=True)
    c = canvas.Canvas(str(OUTPUT), pagesize=A4, pageCompression=1)
    c.setTitle("Bi-Weekly Intern Report - Weeks 5 and 6")
    c.setAuthor("Motaz Pharaon")
    c.setSubject("Pixoul Gaming internship report covering 27 July through 7 August 2026")

    draw_cover_page(c)

    page2 = [
        Paragraph("Bi-weekly Report: 27 July - 7 August 2026", title_style),
        Paragraph("Assignments engaged in:", heading_style),
    ] + [bullet(item) for item in assignments[:10]]
    draw_text_page(c, 2, page2)

    page3 = [bullet(item) for item in assignments[10:]]
    page3 += [Spacer(1, 5), Paragraph("Skills applied/learned:", heading_style)]
    page3 += [bullet(item) for item in skills]
    draw_text_page(c, 3, page3, top_logo=False)

    page4 = [Paragraph("Knowledge gained:", heading_style)]
    page4 += [bullet(item) for item in knowledge]
    draw_text_page(c, 4, page4, top_logo=False)

    draw_signature_page(c)
    c.save()
    print(OUTPUT)


if __name__ == "__main__":
    build()
