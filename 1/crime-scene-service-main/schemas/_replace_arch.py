from pathlib import Path

arch = Path(r"E:\diploma_maga\1\crime-scene-service-main\schemas\Architecture.drawio")
new = Path(r"E:\diploma_maga\1\crime-scene-service-main\schemas\SystemArchitecture.drawio")

text = arch.read_text(encoding="utf-8")
new_text = new.read_text(encoding="utf-8")

marker = '  <diagram id="oArSV0Q2MtY9UXLNaCbe" name="Architecture">'
start = text.index(marker)
end = text.index("  </diagram>", start) + len("  </diagram>")

new_start = new_text.index("<mxGraphModel")
new_end = new_text.index("</mxGraphModel>") + len("</mxGraphModel>")
model = new_text[new_start:new_end]

replacement = f'{marker}\n    {model}\n  </diagram>'
updated = text[:start] + replacement + text[end:]
arch.write_text(updated, encoding="utf-8")
print("OK")
