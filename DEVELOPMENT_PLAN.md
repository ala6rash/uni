# 🎯 Uni-Connect Development Plan
**Status**: 80% Complete | **Target**: 100% Feature Parity + Launch Ready  
**Last Updated**: April 14, 2026  
**Team**: Ahmad (Primary), Ahmad Teammate (Chat/Notifications), 2 Others

---

## 📊 Current Progress

| Component | Status | Completion |
|-----------|--------|-----------|
| **Authentication** | ✅ Complete | 100% |
| **Database Schema** | ✅ Complete | 100% |
| **Design System** | ✅ Complete | 100% |
| **Dashboard (Feed)** | ✅ Complete | 100% |
| **Leaderboard** | ✅ Complete | 100% |
| **Profile** | ✅ Complete | 100% |
| **Home Landing** | ✅ Complete | 100% |
| **Notifications** | 🟡 Structure Only | 10% |
| **Create Post (Wizard)** | ❌ Not Started | 0% |
| **Single Post View** | ❌ Not Started | 0% |
| **Private Sessions/Chat** | 🟡 Infrastructure Only | 10% |
| **Points & Rewards** | ❌ Not Started | 0% |
| **Settings Page** | ❌ Not Started | 0% |
| **Admin Dashboard** | ❌ Not Started | 0% |

**Overall**: 7/14 features complete = **80% DONE**

---

## 🔧 PHASE 1: CRITICAL FEATURES (Week 1)
*Target: 80% → 95% — Unblock all core Q&A flows*

### 1️⃣ Notifications Page (30 min)
**Current State**: View exists, no data binding  
**Deliverable**: Dynamic notification list with filters  
**Tasks**:
- [ ] Update DashboardController.Notifications() to fetch `User.Notifications`
- [ ] Update Notifications.cshtml with Razor loops
  - Display: Type, ReferenceID, CreatedAt, IsRead status
  - Filters: All, Answers, Upvotes, Sessions
  - Action: Mark single/all as read
- [ ] Add toast notification when new notification arrives
- [ ] Wire to navbar badge (count of unread)

**Files to Modify**:
- `Controllers/DashboardController.cs` (add Notifications action logic)
- `Views/Dashboard/Notifications.cshtml` (dynamic rendering)

**Acceptance Criteria**:
- [ ] Notifications appear when logged in
- [ ] Can filter by type
- [ ] Mark as read works
- [ ] Navbar badge shows unread count

---

### 2️⃣ Create Post Wizard (1.5 hrs)
**Current State**: CreatePost.cshtml exists, controller stub only  
**Deliverable**: 3-step form to create new question  
**Architecture**:
```
Step 1: Search Existing (AJAX)
  ├─ User types query
  ├─ Shows similar posts (client-side filter)
  └─ "Continue to Write" button

Step 2: Write Question
  ├─ Title input (required, min 10 chars)
  ├─ Content textarea (required, min 20 chars)
  ├─ Category dropdown (IT, Business, Engineering, Pharmacy)
  ├─ Tags input (multiselect, max 5)
  └─ "Preview" button

Step 3: Preview & Confirm
  ├─ Show formatted post preview
  ├─ Display: "-10 pts will be deducted"
  └─ Confirm button → POST to controller
```

**Tasks**:
- [ ] Create CreatePost.cshtml with 3-step form layout
- [ ] Add AJAX search in Step 1 (filter Dashboard.cshtml posts client-side)
- [ ] Add form validation (title min 10, content min 20)
- [ ] Create PostViewModel with validation
- [ ] Implement DashboardController.CreatePost(PostViewModel) to:
  - Create new Post entity
  - Reduce user Points by 10
  - Save to database
  - Redirect to SinglePost detail page
- [ ] Show toast: "Question posted! -10 pts deducted"

**Files to Modify/Create**:
- `Views/Dashboard/CreatePost.cshtml` (create 3-step form)
- `Controllers/DashboardController.cs` (add POST handler)
- `ViewModels/PostViewModel.cs` (already exists, may need updates)

**DB Impact**:
- New Post record created
- User.Points decreased by 10
- Auto-timestamp CreatedAt

**Acceptance Criteria**:
- [ ] Step 1: Shows existing posts matching query (client-side)
- [ ] Step 2: Form validates title & content
- [ ] Step 3: Preview shows formatted post
- [ ] Confirm creates Post in database
- [ ] User points deducted by 10
- [ ] Redirects to SinglePost detail page
- [ ] Toasts show feedback

---

### 3️⃣ Single Post Detail View (1.5 hrs)
**Current State**: SinglePost.cshtml exists, no controller logic  
**Deliverable**: Full post view with answers & reply form  
**Layout**:
```
┌─────────────────────────────────────┐
│ Post Header (Author, Time, Category)│
├─────────────────────────────────────┤
│ Post Title (h1)                     │
│ Post Content (paragraph)            │
├─────────────────────────────────────┤
│ Stats: 👍 42 | 💬 7 answers | 👁️ 124 │
├─────────────────────────────────────┤
│ [Upvote] [Request Session] [Report] │
├─────────────────────────────────────┤
│ ANSWERS SECTION                     │
│  Answer 1 (Best Answer ✅ badge)    │
│  Answer 2                           │
│  ...                                │
├─────────────────────────────────────┤
│ POST ANSWER FORM                    │
│  Textarea: Write answer (min 20)    │
│  [Post Answer] button               │
└─────────────────────────────────────┘
```

**Tasks**:
- [ ] Create DashboardController.SinglePost(int postId) action:
  - Fetch Post with `.Include(p => p.User).Include(p => p.Answers).ThenInclude(a => a.User)`
  - Increment ViewsCount
  - Pass to view
- [ ] Create SinglePost.cshtml view:
  - Display post.Title, content, author, time, category
  - List all post.Answers in order (best first if IsAccepted=true)
  - Show answer author avatar, content, upvotes, "Mark Best Answer" button (if own post)
  - Answer form textarea with validation
- [ ] Implement DashboardController.PostAnswer(int postId, string content):
  - Validate content (min 20 chars)
  - Create Answer entity (IsAccepted=false initially)
  - Award user +5 points (answerer earns points)
  - Save & redirect back
  - Toast: "+5 pts earned for answering!"
- [ ] Implement "Mark Best Answer" button (only for post author):
  - Set Answers.IsAccepted = true for selected answer
  - Award answer author +15 points bonus
  - Hide answer form (post is now solved)

**Files to Modify/Create**:
- `Controllers/DashboardController.cs` (add SinglePost GET, PostAnswer POST, MarkBestAnswer POST)
- `Views/Dashboard/SinglePost.cshtml` (create detail view)

**DB Impact**:
- Post.ViewsCount incremented
- New Answer record created
- Answer author Points += 5
- Best answer author Points += 15
- Answer.IsAccepted = true when marked best

**Acceptance Criteria**:
- [ ] GET /Dashboard/SinglePost/1 displays post with all answers
- [ ] ViewsCount increments on page load
- [ ] Posting answer validates min 20 chars
- [ ] Answer creator gets +5 points
- [ ] Marking best answer: IsAccepted=true, +15 pts to author
- [ ] Best answer badge displays
- [ ] Answer form hides when post solved
- [ ] Upvote button works (increments Post.Upvotes)

---

## 🚀 PHASE 2: REAL-TIME FEATURES (Week 2)
*Target: 95% → 100%*

### 4️⃣ Private Sessions / Chat (2 hrs)
**Current State**: SignalR infrastructure exists, no UI/logic  
**Ownership**: 🤝 Coordinate with Ahmad Teammate  
**Deliverable**: Real-time private tutoring sessions with chat  

**Key Flows**:
```
1. User posts question → [Request Session] button
2. Button opens modal → Select helper + describe session
3. Helper accepts request → PrivateSession created
4. Chat interface loads → Real-time messages via SignalR
5. User marks session done → Session closes
```

**Tasks**:
- [ ] Create Sessions.cshtml template
- [ ] Implement DashboardController.Sessions() to list user's PrivateSessions
- [ ] Create RequestSessionModal (AJAX) for single post
- [ ] Implement RequestSession POST (create Request record)
- [ ] Create ChatPage.cshtml with message rendering
- [ ] Wire ChatHub.SendMessage() to broadcast messages
- [ ] Add message persistence to database
- [ ] Award points: +20 pts to tutor when session closes

**Files to Modify/Create**:
- `Views/Dashboard/Sessions.cshtml` (list sessions)
- `Views/Dashboard/ChatPage.cshtml` (message UI)
- `Controllers/DashboardController.cs` (session logic)
- `Hubs/ChatHub.cs` (already exists, may need updates)

**DB Impact**:
- Request.Status: "Open" → "Accepted"
- PrivateSession created (StudentID, HelperID, RequestID, CreatedAt)
- Message records inserted (MessageText, SenderID, SentAt)
- Tutor Points += 20 on session close

**Acceptance Criteria** (defer to coordinator with Ahmad Teammate):
- [ ] User can request tutoring session from post
- [ ] Helper notified of new request
- [ ] Chat opens after acceptance
- [ ] Messages persist to database
- [ ] Real-time message delivery via SignalR
- [ ] Session closure triggers tutor reward

---

### 5️⃣ Points & Rewards (45 min)
**Current State**: User.Points field exists, no display/redemption  
**Deliverable**: Points breakdown + campus venue redemptions  

**Layout**:
```
┌─────────────────────────────────────┐
│ 🪙 Your Points: 840 pts             │
├─────────────────────────────────────┤
│ EARNED                              │
│  ✅ Post Answer      +5 pts (30x)   │
│  ✅ Best Answer      +15 pts (2x)   │
│  ✅ Upvote Received  +10 pts (5x)   │
│  ✅ Tutor Session    +20 pts (4x)   │
│  ✅ Daily Login      +2 pts (10x)   │
│  TOTAL EARNED: +830 pts             │
│                                     │
│ SPENT                               │
│  ❌ Post Question    -10 pts (8x)   │
│  ❌ Request Session  -5 pts (1x)    │
│  TOTAL SPENT: -85 pts               │
│                                     │
│ NET: 830 - 85 = 745 pts             │
├─────────────────────────────────────┤
│ CAMPUS REWARDS                      │
│  🍴 Cafeteria (100 pts off 10%)     │
│    [200 pts] [Redeem]               │
│                                     │
│  📚 Library (5 JD off books)         │
│    [400 pts] [Redeem]               │
│                                     │
│  📖 Bookshop (15% discount)          │
│    [350 pts] [Redeem]               │
│                                     │
│  🏪 Gift Card (150 pts = 1 JD)      │
│    [150 pts] [Redeem]               │
├─────────────────────────────────────┤
│ (QR Code appears after redemption)  │
└─────────────────────────────────────┘
```

**Tasks**:
- [ ] Create Points.cshtml with breakdown table
- [ ] Implement DashboardController.Points() to calculate earned/spent stats
- [ ] Add redemption buttons:
  - Check if user.Points >= venue.Cost
  - Create Redemption record
  - Deduct points
  - Generate QR code for venue scanning
- [ ] Add modal to display QR code after redemption
- [ ] Create RedemptionViewModel with venue info

**Files to Modify/Create**:
- `Views/Dashboard/Points.cshtml` (create points display)
- `Controllers/DashboardController.cs` (add Points GET, Redeem POST)
- `ViewModels/RedemptionViewModel.cs` (new file)
- `Models/Redemption.cs` (new model if not exists)

**DB Impact**:
- New Redemption record (UserID, VenueID, RedeemedAt, QRCode)
- User.Points -= venue.Cost

**Acceptance Criteria**:
- [ ] Points page shows all transactions
- [ ] Breakdown accurately reflects posts/answers/upvotes
- [ ] Redemption buttons check user balance
- [ ] Deduction works correctly
- [ ] QR code displays after redemption
- [ ] Venue can scan QR to verify

---

### 6️⃣ Settings Page (30 min)
**Current State**: Not created  
**Deliverable**: User settings (password, preferences)  

**Minimum Viable Version**:
```
┌─────────────────────────────────────┐
│ ⚙️ ACCOUNT SETTINGS                 │
├─────────────────────────────────────┤
│ Email: ahmad@philadelphia.edu.jo    │
│ Faculty: IT                         │
│ Year: 3                             │
├─────────────────────────────────────┤
│ CHANGE PASSWORD                     │
│ Current Password: [___________]     │
│ New Password: [___________]         │
│ Confirm Password: [___________]     │
│ [Update Password]                   │
├─────────────────────────────────────┤
│ PREFERENCES                         │
│ ☑ Email notifications               │
│ ☑ Show profile publicly             │
│ [Save Preferences]                  │
├─────────────────────────────────────┤
│ DANGER ZONE                         │
│ [Delete Account] (irreversible)     │
└─────────────────────────────────────┘
```

**Tasks**:
- [ ] Create Settings.cshtml
- [ ] Implement DashboardController.Settings() GET
- [ ] Implement DashboardController.ChangePassword() POST:
  - Verify current password
  - Hash new password
  - Update database
  - Toast success
- [ ] Implement DashboardController.UpdatePreferences() POST

**Files to Modify/Create**:
- `Views/Dashboard/Settings.cshtml` (new)
- `Controllers/DashboardController.cs` (add Settings actions)

---

## 📋 PHASE 3: POLISH & DEPLOYMENT
*Get to production ready*

### 7️⃣ Admin Dashboard (Optional for MVP - Defer)
**Deliverable**: Moderation & analytics  
**Can add post-launch if needed**

---

## 📅 EXECUTION TIMELINE

### **WEEK 1 (THIS WEEK)**
```
Day 1-2 (Mon-Tue):
  ✳️ Notifications (30 min)
  ✳️ CreatePost Wizard (1.5 hrs)
  = 2 hrs coding

Day 3-4 (Wed-Thu):
  ✳️ SinglePost Page (1.5 hrs)
  = 1.5 hrs coding

Day 5 (Fri):
  ✳️ Testing & Code Review
  ✳️ Fix bugs
  = 1 hr QA

CUMULATIVE: ~4.5 hours
TARGET: Reach 95% (7/14 → 11/14 pages done)
```

### **WEEK 2 (FOLLOWING WEEK)**
```
Day 1-2 (Mon-Tue):
  🤝 Sessions/Chat with Ahmad Teammate (2 hrs)

Day 3 (Wed):
  ✳️ Points & Rewards (45 min)
  ✳️ Settings (30 min)

Day 4-5 (Thu-Fri):
  ✳️ Full End-to-End Testing
  ✳️ Deploy to Live Server
  ✳️ Post-Deployment Testing

CUMULATIVE: ~5 hours
TARGET: Reach 100% (12/14 → 14/14) + LIVE
```

---

## 🎮 HOW TO USE THIS PLAN

### As You Work:
1. **At start of each task**: Mark checkbox `[x]`
2. **As you complete sub-tasks**: Check them off
3. **When stuck**: Reference the "Files to Modify" section
4. **Before moving on**: Verify "Acceptance Criteria" pass

### To Track Progress:
- Update `[x] / [ ]` checkboxes as you go
- Update "Overall Progress" section at top after each phase
- Commit to git with phase completion (e.g., "✅ Phase 1 Complete: Notifications + CreatePost + SinglePost")

### For Code Questions:
- Refer to "Files to Modify" for exact locations
- Check "DB Impact" to understand data changes
- Review "Acceptance Criteria" to test your work

---

## 🤝 TEAM ASSIGNMENTS

| Team Member | Responsibility | Timeline |
|-------------|------------------|----------|
| **Ahmad (You)** | Phase 1 (Notifications, CreatePost, SinglePost) | Week 1 |
| **Ahmad (You)** | Points & Rewards | Week 2 Wed |
| **Ahmad Teammate** | Sessions/Chat (SignalR) | Week 2 Mon-Tue |
| **Ahmad (You)** | Settings | Week 2 Wed |
| **Ahmad (You)** | Final testing + Deployment | Week 2 Thu-Fri |
| **Team** | Post-launch bug fixes | Ongoing |

---

## ⚠️ OPEN QUESTIONS

**Clarify before starting Phase 1**:

1. **Points System - CONFIRM RULES**:
   - [ ] Creating post: -10 pts? ✅ (confirmed)
   - [ ] Posting answer: +5 pts earned, -10 to ask? (CLARIFY)
   - [ ] Best answer: +15 pts? (CLARIFY)
   - [ ] Upvote received: +10 pts? (CLARIFY)
   - [ ] Requesting session: -5 pts? (CLARIFY)
   - [ ] Tutoring session: +20 to helper? (CLARIFY)
   - [ ] Daily login bonus: +2 pts? (CLARIFY)

2. **Notification Types - CONFIRM**:
   - [ ] What triggers "upvote" notification? (post upvoted)
   - [ ] What triggers "answer" notification? (new answer posted)
   - [ ] What triggers "session" notification? (request received)
   - [ ] What triggers "best answer" notification? (post marked as best)

3. **MVP Scope**:
   - [ ] Do we launch with Admin Dashboard? (Recommend: NO, post-launch)
   - [ ] Do we need Settings for MVP? (Recommend: YES, password change minimum)
   - [ ] Do we deploy all features or in phases? (Recommend: All Phase 1+2 before launch)

4. **QR Code Library**:
   - [ ] Which QR code library to use for C#? (Recommend: QRCoder NuGet)
   - [ ] Should QR codes be stored in database or generated on-the-fly? (Recommend: Generated)

---

## 📊 SUCCESS METRICS

After Week 2, you should have:

✅ **14/14 Pages Implemented**:
- Dashboard ✓
- Leaderboard ✓
- Profile ✓
- Home ✓
- Login/Register ✓
- **Notifications** (NEW)
- **CreatePost** (NEW)
- **SinglePost** (NEW)
- **Sessions/Chat** (NEW)
- **Points & Rewards** (NEW)
- **Settings** (NEW)
- Admin (Deferred)

✅ **100% Feature Parity** with live site design

✅ **Real Database Integration**:
- All pages fetch live data
- No mock data anywhere
- Points system working end-to-end
- Real-time chat via SignalR

✅ **Live Deployment**: Accessible at http://allahawani-001-site1.rtempurl.com/

✅ **Git History**: Clean commits documenting each phase

---

## 🚀 LAUNCH CHECKLIST

Before deploying to live server:

- [ ] All 14 pages implemented
- [ ] All acceptance criteria pass
- [ ] No console errors in browser DevTools
- [ ] Database migrations run successfully
- [ ] SignalR connections tested
- [ ] QR codes generate correctly
- [ ] Points deductions/earnings work
- [ ] Authentication flows tested (login, register, logout)
- [ ] Mobile responsive (test on phone)
- [ ] Performance: Page load < 2 seconds
- [ ] All git commits pushed
- [ ] Live server database updated
- [ ] Redirect from old URLs working
- [ ] SSL certificate valid
- [ ] Monitoring alerts configured

---

## 📞 ESCALATION PATH

If stuck:
1. Check "Acceptance Criteria" for what's expected
2. Review "Files to Modify" section
3. Test via browser DevTools (F12)
4. Check git diff for recent changes
5. Ask team members about clarifications

---

**Created**: April 14, 2026  
**Last Updated**: April 14, 2026  
**Status**: Ready to Execute Phase 1 ✅

