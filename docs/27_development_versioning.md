# Development Versioning

Last updated: 2026-08-10

## Format

Builds use `v{x}.{y}.{z}.{w}`.

During early active development (`x = 0`):

- `y` is the weekly prototype number. The project plans one new prototype per week, so Prototype 3 begins at `v0.3.z.w`.
- `z` is a major change within the current weekly prototype.
- `w` is a minor change within the current weekly prototype.

`x` reaches `1` only at full release. The meanings and reset rules for `y`, `z`, and `w` after full release must be chosen before that milestone; the early-development convention does not define them.

## Current Mapping

- `v0.1.0.0` = legacy Prototype 1.0 baseline.
- `v0.2.2.0` = legacy Prototype 2.2 environment and presentation baseline.
- `v0.2.2.1` = fullscreen and versioning maintenance change on top of that baseline.
- `v0.2.2.2` = high-refresh desktop locomotion regression fix.
- `v0.2.3.0` = major Prototype 2 desktop embodiment, portal-accuracy, pickup, and Owl-difficulty pass.
- `v0.3.0.0` = Readable Stealth and Contractor Counterplay. Implemented, tracked, and automated delivery-validated; human validation remains open.
- `v0.4.0.0` = Spatial Readability and Pursuit Topology using the 7C three-lobed facility. Implemented, tracked, automated regression-validated, built, and smoke-launched; uninformed desktop, XR, and physical-hardware validation remains open.

## Repository and Build Rules

- Start release-facing commit subjects with the exact version.
- Set Unity `PlayerSettings.bundleVersion` to the four numeric parts without the `v` prefix.
- Show the `v`-prefixed version in runtime operator/pause information.
- Put Windows builds under `Builds/v{x}.{y}.{z}.{w}/` and include the version in the executable name.
- Do not reuse a published version for materially different content.
