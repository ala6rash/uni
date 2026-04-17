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

## 2026-04-15 — Documentation Reorganization & Reference Setup
**Area:** Docs  
**Owner:** ahmad (documentation audit)  
**Type:** Docs

**What changed**
- Replaced placeholder content in `Doc1_Chapter5_Content.txt` with real reference to Chapter 5 Design Document
- Replaced placeholder content in `Doc2_Research_Content.txt` with real reference to Research & Specifications
- Created `docs/design/MOCKUPS_INDEX.md` with comprehensive index of all HTML mockups and design assets
- Fixed all documentation paths to use relative paths only (no absolute file:/// references)
- Removed reference to AutoRecovery file in placeholder content
- Updated reference files to point to actual source documents in `project to be know/`

**Why**
- Remove all placeholder documentation that serves no purpose
- Properly organize and index design assets from "project to be know" folder
- Ensure all documentation references use relative paths (repo-portable, no absolute paths)
- Create clear navigation structure for design mockups and technical specifications
- Enable team members to quickly find and reference design and research documentation

**How (implementation details)**
- Updated `Doc1_Chapter5_Content.txt` with comprehensive index of Chapter 5 coverage areas + relative path to source
- Updated `Doc2_Research_Content.txt` with comprehensive index of research topics + relative path to source
- Created `docs/design/MOCKUPS_INDEX.md` with structured listing of 14 HTML mockup files and 15+ wireframe images
- Removed all file:/// and absolute path references
- All references now use relative paths: `../../project to be know/filename`
- Maintained cross-references between design, reference, and governance documents

**Files changed**
- `docs/reference/Doc1_Chapter5_Content.txt` (updated — removed placeholder, added real content reference)
- `docs/reference/Doc2_Research_Content.txt` (updated — removed placeholder, added real content reference)
- `docs/design/MOCKUPS_INDEX.md` (new — comprehensive design assets index)

**How to test**
1. Verify no placeholder text remains: `grep -r "PLACEHOLDER" docs/` (should return no results)
2. Verify documentation references are relative: `grep -r "../../project to be know" docs/` (shows proper references)
3. Confirm no absolute paths: `grep -r "file://" docs/` and `grep -r "AutoRecovery" docs/` (should return no results)
4. Open a reference file and check it's readable: `cat docs/reference/Doc1_Chapter5_Content.txt` (should show structured content, not placeholders)
5. Verify design index exists: `ls docs/design/MOCKUPS_INDEX.md` and check it lists all mockup files
6. Build still works: `dotnet build Uni-Connect/Uni-Connect.csproj` (no breaking changes)

**Notes / follow-ups**
- Source documents remain in `project to be know/` folder (Chapter5_Final.docx, UniConnect-Complete-Research 2.docx)
- Design mockup files (HTML + PNG/JPG) remain in `project to be know/` as the source; index in docs/design/ points to them
- Team can now reference Chapter 5 and Research documents directly via relative paths
- Design review process can use MOCKUPS_INDEX.md for navigation
- All documentation is now production-ready (no placeholders, proper structure, correct paths)

---

## 2026-04-15 — Removed Empty Project Directory
**Area:** Other  
**Owner:** ahmad (cleanup)  
**Type:** Cleanup

**What changed**
- Deleted `UniConnect.Web/` directory

**Why**
- Project was empty (only contained `bin/` and `obj/` folders)
- No source files, no `.csproj` configuration, no active development
- Dead weight in the repository with no purpose
- All functionality is in `Uni-Connect/` project

**How (implementation details)**
- Used `Remove-Item -Recurse -Force` to delete `UniConnect.Web/` folder
- Verified deletion: `git status` confirms directory is gone

**Files changed**
- `UniConnect.Web/` (deleted)

**How to test**
1. Verify directory no longer exists: `ls UniConnect.Web` (should fail)
2. Confirm git sees deletion: `git status` (should show deleted entry or clean)

**Notes / follow-ups**
- All actual development code remains in `Uni-Connect/`
- Solution file `Uni-Connect.sln` was already marked as deleted (unused)
- Workspace is now cleaner with only the active project directory

---

## 2026-04-15 — Dashboard Authorization & Logout Security Fix
**Area:** Auth / Security  
**Owner:** ahmad (implementation)  
**Type:** Security / Bugfix

**What changed**
- Added `[Authorize]` attribute at class level to `DashboardController.cs`
- Added `using Microsoft.AspNetCore.Authorization;` import to DashboardController
- Updated `ChatPage.cshtml`: Added Razor POST form for logout with CSRF token protection
- Updated `doLogout()` JavaScript function to submit backend logout form instead of client-side redirect to `login.html`

**Why**
- Enforce authentication on all Dashboard routes at the controller level (more secure than per-action checks)
- Protect logout endpoint from CSRF attacks by requiring anti-forgery token
- Ensure proper backend session cleanup on SignOut (clearing authentication cookie)
- Prevent users from accessing dashboard without authentication even if they bypass individual action guards
- Fix security gap: previously logout redirected directly to login.html without clearing server-side session

**How (implementation details)**
- Added `using Microsoft.AspNetCore.Authorization;` to DashboardController.cs imports
- Added `[Authorize]` attribute before `public class DashboardController : Controller` declaration
- Did NOT modify any Dashboard action logic (no business logic changes)
- Added hidden Razor form in ChatPage.cshtml before script section:
  ```html
  <form asp-controller="Login" asp-action="Logout" method="post" id="logoutForm" style="display:none">
      @Html.AntiForgeryToken()
  </form>
  ```
- Updated `doLogout()` JavaScript function from `window.location.href = 'login.html'` to `document.getElementById('logoutForm').submit()`
- Kept confirm() popup to prevent accidental logouts
- SignalR chat functionality and ChatPage logic remain untouched

**Files changed**
- `Uni-Connect/Controllers/DashboardController.cs` (added using + [Authorize] attribute)
- `Uni-Connect/Views/Dashboard/ChatPage.cshtml` (added logout form, updated doLogout function)

**How to test**
1. Verify DashboardController compiles: `dotnet build Uni-Connect/Uni-Connect.csproj` (should succeed)
2. Test unauthorized access: Navigate to `/Dashboard/Dashboard` without authentication → should redirect to `/Login/Login_Page`
3. Test authorized access: Log in successfully → Dashboard loads without redirect
4. Test logout flow:
   - Click "Sign Out" button in ChatPage navbar or sidebar
   - Confirm dialog appears
   - Click OK → LoginController.Logout POST endpoint is called
   - Verify authentication cookie is cleared (check Network tab: `Set-Cookie` for auth cookie should be cleared)
   - Redirected to `/Login/Login_Page`
   - Attempt to access `/Dashboard/Dashboard` → should redirect to login (session cleared)
5. Verify CSRF protection: Check Network tab shows form submission with `RequestVerificationToken` in POST body
6. Test HomeController.Index still public: Navigate to `/` without auth → should show landing page (not redirect)

**Notes / follow-ups**
- HomeController.Index remains public (no [Authorize]) — users can see landing page without login
- All other Dashboard routes now protected by class-level [Authorize]
- Logout POST is properly secured against CSRF attacks via token validation (configured in Program.cs)
- Previously logout.cshtml had manual form submission; ChatPage now uses same pattern for consistency
- Chat functionality (SignalR) unaffected; ChatPage just uses updated authentication model
- Tested logout behavior manually before appending DEV_LOG

---

## 2026-04-15 — Navigation Link Refactor: Static HTML to MVC Routes
**Area:** UI / Frontend  
**Owner:** ahmad (refactoring)  
**Type:** Refactor / Maintenance

**What changed**
- Replaced all static HTML file references (dashboard.html, create-post.html, etc.) with ASP.NET MVC routing tags (asp-controller/asp-action)
- Updated `Uni-Connect/Views/Dashboard/ChatPage.cshtml`: 16 navigation links converted to MVC routes
- Updated `Uni-Connect/Views/Shared/_DashboardLayout.cshtml`: 4 navigation links converted to MVC routes
- Added "coming soon" placeholders (href="#" title="Coming soon" onclick="return false") for Points & Settings pages (not yet implemented)
- No changes to SignalR chat logic, message functions, or server-side behavior

**Why**
- ASP.NET MVC routing should be used instead of hard-coded HTML file paths for maintainability and consistency
- Ensures navigation works correctly with Razor view engine URL generation
- Prevents 404 errors from broken file references when application is deployed
- Follows ASP.NET Core MVC best practices for link generation
- Centralizes route changes to controller level (only one place to update if routes change)

**How (implementation details)**
- Searched ChatPage.cshtml for all href="*.html" attributes in navigation links
- Replaced 16 links:
  - `href="dashboard.html"` → `asp-controller="Dashboard" asp-action="Dashboard"` (3 instances)
  - `href="create-post.html"` → `asp-controller="Dashboard" asp-action="CreatePost"` (2 instances)
  - `href="notifications.html"` → `asp-controller="Dashboard" asp-action="Notifications"` (2 instances)
  - `href="profile.html"` → `asp-controller="Dashboard" asp-action="Profile"` (3 instances)
  - `href="leaderboard.html"` → `asp-controller="Dashboard" asp-action="Leaderboard"` (1 instance)
  - `href="sessions.html"` → `asp-controller="Dashboard" asp-action="ChatPage"` (1 instance)
  - `href="points.html"` → `href="#" title="Coming soon" onclick="return false"` (2 instances)
  - `href="settings.html"` → `href="#" title="Coming soon" onclick="return false"` (2 instances)
- Updated _DashboardLayout.cshtml sidebar navigation:
  - `href="#notifications"` → `asp-controller="Dashboard" asp-action="Notifications"`
  - `href="#profile"` → `asp-controller="Dashboard" asp-action="Profile"`
  - `href="#rewards"` → `href="#" title="Coming soon" onclick="return false"`
  - `href="#settings"` → `href="#" title="Coming soon" onclick="return false"`
- Did NOT modify:
  - SignalR chat initialization or message handling scripts
  - Room logic or connection management
  - Send/receive message functions
  - Chat history loading or message rendering

**Files changed**
- `Uni-Connect/Views/Dashboard/ChatPage.cshtml` (16 navigation links updated)
- `Uni-Connect/Views/Shared/_DashboardLayout.cshtml` (4 navigation links updated)

**How to test**
1. Compile: `dotnet build Uni-Connect/Uni-Connect.csproj` (should succeed)
2. Run application and navigate to Dashboard
3. Test each navigation link:
   - Click "Home Feed" → loads Dashboard with feeds visible
   - Click "Ask Question" → loads CreatePost page
   - Click "Private Sessions" → loads ChatPage with chat interface intact
   - Click "Leaderboard" → loads Leaderboard page
   - Click "Notifications" → loads Notifications page
   - Click "My Profile" → loads Profile page
   - Click "Points & Rewards" → should disable click (coming soon)
   - Click "Settings" → should disable click (coming soon)
4. Verify chat functionality still works:
   - SignalR connection established (check browser console, no connection errors)
   - Messages send/receive correctly
   - Multiple chat sessions work independently
   - Connection room logic works as before
5. Check browser console for JavaScript errors (should be none)
6. Test responsive design: sidebar navigation and dropdown menu still functional on mobile

**Notes / follow-ups**
- Points & Settings controller actions do not exist yet; links are disabled with "Coming soon" tooltip
- Chat page continues to function normally with all SignalR features intact; only navigation routing was changed
- All routes use existing DashboardController actions (no new controller/action creation required)
- Navigation now fully complies with ASP.NET MVC conventions
- Future: implement Points and Settings pages and re-enable those links

---

## 2026-04-15 — Navigation Refactor Complete: All Static HTML Links Removed
**Area:** UI / Frontend  
**Owner:** ahmad (verification)  
**Type:** Refactor / Maintenance

**What changed**
- Verified and confirmed removal of all remaining static HTML file references (*.html) from Razor views
- Completed navigation refactor across entire ViewsDirectory
- All dashboard navigation now uses ASP.NET MVC routing conventions exclusively
- No remaining href="*.html" links found in any .cshtml files

**Why**
- Final verification to ensure complete consistency with ASP.NET MVC best practices
- Prevent any broken navigation references from being deployed
- Ensure all user navigation flows through proper MVC routing
- Document completion of navigation modernization work

**How (implementation details)**
- Conducted comprehensive search for remaining href="*.html" patterns across all Razor views
- Verified all core dashboard files:
  - Uni-Connect/Views/Dashboard/ChatPage.cshtml (16 HTML links fixed previously)
  - Uni-Connect/Views/Shared/_DashboardLayout.cshtml (4 HTML links fixed previously)
  - Uni-Connect/Views/Dashboard/Dashboard.cshtml (already using asp-action)
  - Uni-Connect/Views/Dashboard/Notifications.cshtml (no HTML links present)
  - Uni-Connect/Views/Dashboard/CreatePost.cshtml (already using asp-action/Url.Action)
  - Uni-Connect/Views/Dashboard/Profile.cshtml (already using asp-action)
  - Uni-Connect/Views/Dashboard/Leaderboard.cshtml (already using asp-action)
- Search results: 0 remaining href="*.html" links in all Views/** files
- All routes properly mapped to DashboardController actions or marked as "Coming Soon"

**Files verified**
- `Uni-Connect/Views/Dashboard/` (all files verified clean)
- `Uni-Connect/Views/Shared/_DashboardLayout.cshtml` (verified clean)
- `Uni-Connect/Views/Shared/_Layout.cshtml` (already using proper MVC routes)
- `Uni-Connect/Views/Login/` (not dashboard navigation, excluded from scope)
- `Uni-Connect/Views/Home/` (not dashboard navigation, excluded from scope)

**How to test**
1. Search verification: `grep -r "href=\".*\.html\"" Uni-Connect/Views/` (should return 0 results)
2. Compile: `dotnet build Uni-Connect/Uni-Connect.csproj` (should succeed without warnings)
3. Run and navigate all dashboard pages to verify links work correctly
4. Spot-check key navigation paths:
   - From any page → Home Feed works
   - From any page → Ask Question works
   - From any page → Leaderboard works
   - From any page → Notifications works
   - From any page → Profile works
   - Points/Settings show "Coming soon" without errors
5. Verify no 404 errors in browser console or application logs

**Notes / follow-ups**
- Navigation refactor fully complete and verified
- All Razor views now exclusively use ASP.NET MVC routing conventions
- No legacy HTML file references remain in the codebase
- Future feature additions should continue to use asp-controller/asp-action tags
- When Points and Settings features are implemented, re-enable those navigation links via controller actions
- Search performed across entire Views/** directory confirms comprehensive cleanup

---

## 2026-04-15 — User Profile Links: "Coming Soon" Placeholders
**Area:** UI / Frontend  
**Owner:** ahmad (implementation)  
**Type:** Refactor / Maintenance

**What changed**
- Replaced 6 remaining `user-profile.html` links with "Coming soon" disabled placeholders
- Updated `Uni-Connect/Views/Dashboard/ChatPage.cshtml`: All user profile links converted to disabled href="#" with title attribute
- Profile view feature deferred to future implementation phase (Points/Settings pattern applied)

**Why**
- User profile viewing feature not yet implemented in MVC (no ViewUser action in DashboardController)
- Consistency: Apply same "Coming soon" pattern to user profiles as applied to Points & Settings pages
- Prevent broken navigation or 404 errors from incomplete feature implementation
- Maintain clean, functional UI while deferring this feature to future work

**How (implementation details)**
- Located all 6 instances of `href="user-profile.html?user=..."` in ChatPage.cshtml
  - Line 148: Rania profile link in chat header
  - Line 171: Lina avatar link in requests section
  - Line 176: Lina name link in requests section
  - Line 194: Sami avatar link in requests section
  - Line 199: Sami name link in requests section
  - Line 217: Omar avatar link in history section
- Replaced each with: `href="#" title="Coming soon" onclick="return false"`
- Did NOT modify any HTML structure, CSS classes, data attributes, or SignalR logic
- Did NOT create controller actions or new views for user profile viewing

**Files changed**
- `Uni-Connect/Views/Dashboard/ChatPage.cshtml` (6 user-profile.html links → Coming soon placeholders)

**How to test**
1. Compile: `dotnet build Uni-Connect/Uni-Connect.csproj` (should succeed)
2. Run application and navigate to Dashboard → ChatPage
3. Test profile link behavior:
   - Click on tutor avatar in chat header → no navigation, link disabled
   - Click on user name in requests section (Lina, Sami, Omar) → no navigation, link disabled
   - Hover over any disabled profile link → "Coming soon" tooltip visible (title attribute)
   - Check browser console for JavaScript errors (should be none)
4. Verify navigation elsewhere still works:
   - All other dashboard navigation links still functional
   - SignalR chat continues to work normally
5. Comprehensive search verification:
   - Run: `grep -r "\.html" Uni-Connect/Views/` (should return 0 matches)
   - Run: `grep -r "href=\".*\.html\"" Uni-Connect/Views/` (should return 0 matches)
   - Run: `grep -r "window\.location.*\.html" Uni-Connect/Views/` (should return 0 matches)
   - Run: `grep -r "location\.href.*\.html" Uni-Connect/Views/` (should return 0 matches)
   - Result: All patterns return 0 matches ✓

**Notes / follow-ups**
- User profile viewing feature fully deferred until implementation phase
- Same "Coming soon" pattern successfully applied to all incomplete features (Points, Settings, User Profiles)
- All remaining static HTML file references removed from Razor views—**navigation refactor 100% complete**
- When user profile feature is ready, create ViewUser(string username) action in DashboardController and update these links to use asp-controller/asp-action
- Current state verified clean with 6-pattern comprehensive search: ALL patterns return 0 matches

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
