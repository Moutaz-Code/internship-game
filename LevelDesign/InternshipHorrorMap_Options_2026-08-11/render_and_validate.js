const fs = require('fs');
const path = require('path');
const crypto = require('crypto');
const sharp = require('sharp');

const root = __dirname;
const source = 'C:/Users/Mou/Pictures/Krita Drawings/InternshipHorrorMap.png';
const lockedBase = path.join(root, 'InternshipHorrorMap_locked.png');
const options = [
  ['option_1_operational_clarity_overlay.svg', 'option_1_operational_clarity_composite.png'],
  ['option_2_stealth_grammars_overlay.svg', 'option_2_stealth_grammars_composite.png'],
  ['option_3_experimental_contract_overlay.svg', 'option_3_experimental_contract_composite.png'],
  ['artistic_room_themes_overlay.svg', 'artistic_room_themes_composite.png'],
];

function sha256(buffer) {
  return crypto.createHash('sha256').update(buffer).digest('hex');
}

async function main() {
  const sourceBytes = fs.readFileSync(source);
  const lockedBytes = fs.readFileSync(lockedBase);
  if (!sourceBytes.equals(lockedBytes)) {
    throw new Error('Locked base is not byte-for-byte identical to the source.');
  }

  const baseMeta = await sharp(lockedBytes).metadata();
  if (baseMeta.width !== 2048 || baseMeta.height !== 2048) {
    throw new Error(`Unexpected locked-base dimensions: ${baseMeta.width}x${baseMeta.height}`);
  }

  const rendered = [];
  for (const [overlayName, compositeName] of options) {
    const overlayPath = path.join(root, overlayName);
    const compositePath = path.join(root, compositeName);
    const svg = fs.readFileSync(overlayPath, 'utf8');

    if (!svg.includes('width="2048"') || !svg.includes('height="2048"') || !svg.includes('viewBox="0 0 2048 2048"')) {
      throw new Error(`${overlayName} does not use the locked 2048x2048 coordinate system.`);
    }
    if (/<image\b/i.test(svg)) {
      throw new Error(`${overlayName} embeds or redraws a raster base image.`);
    }

    const overlayPng = await sharp(Buffer.from(svg)).png().toBuffer();
    await sharp(lockedBytes)
      .composite([{ input: overlayPng, top: 0, left: 0 }])
      .png()
      .toFile(compositePath);

    const compositeBytes = fs.readFileSync(compositePath);
    const compositeMeta = await sharp(compositeBytes).metadata();
    if (compositeMeta.width !== 2048 || compositeMeta.height !== 2048) {
      throw new Error(`${compositeName} changed canvas dimensions.`);
    }

    rendered.push({
      overlay: overlayName,
      overlaySha256: sha256(Buffer.from(svg)),
      composite: compositeName,
      compositeSha256: sha256(compositeBytes),
      width: compositeMeta.width,
      height: compositeMeta.height,
      base: path.basename(lockedBase),
    });
  }

  const manifest = {
    generatedAt: new Date().toISOString(),
    source,
    sourceSha256: sha256(sourceBytes),
    lockedBase: path.basename(lockedBase),
    lockedBaseSha256: sha256(lockedBytes),
    byteForByteIdentical: sourceBytes.equals(lockedBytes),
    canvas: { width: baseMeta.width, height: baseMeta.height },
    overlaysEmbedBase: false,
    options: rendered,
  };

  fs.writeFileSync(path.join(root, 'validation_manifest.json'), JSON.stringify(manifest, null, 2) + '\n', 'utf8');
  process.stdout.write(JSON.stringify(manifest, null, 2) + '\n');
}

main().catch((error) => {
  process.stderr.write(`${error.stack || error}\n`);
  process.exitCode = 1;
});
