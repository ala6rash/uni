# Uni-Connect (Staging Mirror)

This repository (`ala6rash/uni`) is the **staging mirror** of:
- Final/production repository: `https://github.com/Ahmad-Allahawani/Uni-Connect`

## Mirror policy (important)
- `uni` must keep the **same project structure, authentication approach, and database approach** as `Uni-Connect`.
- Do development in feature branches in `uni` first.
- Move code to `Uni-Connect` only after testing.
- Keep these reference docs and **never delete them**:
  - `docs/TO_BE_KNOWN.md`
  - `docs/Chapter5_Final.md`

## One-time safety backup before mirror updates
Before replacing/syncing `master`, create a backup branch or tag:

```bash
git checkout master
git pull origin master
git branch backup-before-mirror
git tag backup-before-mirror-$(date +%Y%m%d-%H%M%S)
```

Then mirror from final repo:

```bash
git remote add upstream https://github.com/Ahmad-Allahawani/Uni-Connect.git  # one time
git fetch upstream
git reset --hard upstream/master
```

## Beginner workflow (daily)
1. Clone staging repo:
   ```bash
   git clone https://github.com/ala6rash/uni.git
   cd uni
   ```
2. Create your feature branch:
   ```bash
   git checkout master
   git pull origin master
   git checkout -b feature/your-task-name
   ```
3. Do your code changes.
4. Run and test locally:
   ```bash
   dotnet build Uni-Connect.sln
   dotnet test Uni-Connect.sln
   ```
5. Commit and push your branch:
   ```bash
   git add .
   git commit -m "feat: short clear message"
   git push -u origin feature/your-task-name
   ```
6. Open a PR in `ala6rash/uni` to review staging changes.
7. After staging is tested, open a PR from the same commit set to:
   - `Ahmad-Allahawani/Uni-Connect` (final repo)
8. Merge to final repo only when verified by the team.

## Current task board from final repo

## What we need to work on

### Home / Login
0. Fixing the shared layout (nav-bar, side-bar, footer)
1. Login authentication
2. Registration
3. Forgot password authentication

### Dashboard
4. User state must be working
5. Like, comment, view must be working
6. Request session
7. Trending topics and adding topics
8. Search bar
9. Sign-out
10. Search banner fix
11. Displaying the questions
12. Interacting with posts
13. Trending tab in the dashboard page
14. Faculty-specific questions

### Profile
15. Everything in the profile page

### Leaderboard
16. Faculty filtering
17. Time filtering
18. Rank calculations (top and bottom boards)

### Notifications
19. Notification system for the website

### Other (Not done yet)
20. Ask question, sessions, points & rewards, settings, admin dashboard

---

## ⚠️ Important
**Please, if you want to take a task, tell the team first.
When you finish it, remove it from this list.**
