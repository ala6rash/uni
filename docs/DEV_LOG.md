# Uni-Connect — Development Log (uni-staging)

Purpose: This file is a running log of **every meaningful change** made in this repository.
It helps the team, the documentation, and the final report by recording **what changed, why, and how**.

---

## 2026-04-15 — Documentation Framework Setup
**Area:** Docs  
**Owner:** github-copilot (system setup)  
**Type:** Docs

**What changed**
- Created `docs/COPILOT_RULES.md` with mandatory team ownership boundaries and workflow rules
- Created `docs/DEV_LOG.md` with entry template for tracking all meaningful changes
- Created `docs/DOCS_INDEX.md` as comprehensive documentation map
- Created `docs/reference/` folder for technical specifications
- Created placeholder files: `Doc1_Chapter5_Content.txt` and `Doc2_Research_Content.txt`
- Created `docs/design/` folder (ready for UI/UX design images)

**Why**
- Establish a mandatory documentation framework required by the development workflow
- Create boundaries for team ownership (ala6rash: Auth; Ahmad: Chat/Sessions)
- Set up structure for design reference materials and research documentation
- Ensure every code change is logged with reasoning, files, and testing steps

**How (implementation details)**
- Created directory structure: `docs/reference/` and `docs/design/`
- Wrote COPILOT_RULES.md with clear do-not-touch areas and security rules
- Created DEV_LOG.md with structured template for future entries
- Created DOCS_INDEX.md as comprehensive map of all documentation
- Added placeholder content files for future research/design content population

**Files changed**
- `docs/COPILOT_RULES.md` (new)
- `docs/DEV_LOG.md` (new)
- `docs/DOCS_INDEX.md` (new)
- `docs/reference/Doc1_Chapter5_Content.txt` (new)
- `docs/reference/Doc2_Research_Content.txt` (new)
- `docs/design/` (new folder)

**How to test**
1. Verify all files exist: `git ls-files docs/`
2. Check structure: `ls -la docs/` shows COPILOT_RULES.md, DEV_LOG.md, DOCS_INDEX.md, reference/, design/
3. Confirm rules are readable: `cat docs/COPILOT_RULES.md` (no corruption)
4. Verify DEV_LOG template loads: `cat docs/DEV_LOG.md` (shows entry template)
5. Build still works: `dotnet build` (no breaking changes)

**Notes / follow-ups**
- Reference folder files are placeholders; team to populate with actual SDD Chapter 5 and research
- Design folder awaits UI/UX mockup images from design phase
- This entry serves as proof-of-concept that DEV_LOG workflow is functioning
- Next: Ready to implement feature changes with full DEV_LOG documentation

---

## Entry Template (copy/paste for each change)

### YYYY-MM-DD — <short title>
**Area:** Auth / Dashboard / DB / UI / Other  
**Owner:** ala6rash / ahmad / team  
**Type:** Feature / Bugfix / Refactor / Security / Docs

**What changed**
- 

**Why**
- 

**How (implementation details)**
- 

**Files changed**
- `path/to/file1`
- `path/to/file2`

**How to test**
1. 
2. 
3. 

**Notes / follow-ups**
- 
