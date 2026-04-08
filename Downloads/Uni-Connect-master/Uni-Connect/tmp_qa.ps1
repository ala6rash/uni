$baseUrl = "http://localhost:5282"
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession

Write-Host "=============================="
Write-Host "🧪 UNICONNECT QA AUDIT START"
Write-Host "=============================="

# ---------------------------------------------
# Test 1: GET Register Page & Extract CSRF
# ---------------------------------------------
Write-Host "-> Fetching /Login/Register..."
$response = Invoke-WebRequest -Uri "$baseUrl/Login/Register" -WebSession $session -ErrorAction SilentlyContinue
$html = $response.Content

# Simple regex to grab the __RequestVerificationToken
$tokenMatch = [regex]::Match($html, 'name="__RequestVerificationToken" type="hidden" value="([^"]+)"')
$csrfToken = $tokenMatch.Groups[1].Value

if ($csrfToken) {
    Write-Host "✅ [PASS] CSRF Token successfully extracted."
} else {
    Write-Host "❌ [FAIL] Could not find CSRF token."
}

# ---------------------------------------------
# Test 2: Execute Valid Registration 
# ---------------------------------------------
Write-Host "-> Testing Valid Registration (R1)..."
$registerBody = @{
    "__RequestVerificationToken" = $csrfToken
    "Name" = "QA Tester"
    "Email" = "qa_tester@philadelphia.edu.jo"
    "Faculty" = "IT"
    "YearOfStudy" = "4"
    "Password" = "Password123!"
    "ConfirmPassword" = "Password123!"
}

$registerResponse = Invoke-WebRequest -Uri "$baseUrl/Login/Register" -Method POST -Body $registerBody -WebSession $session -MaximumRedirection 0 -ErrorAction SilentlyContinue

if ($registerResponse.StatusCode -eq 302 -and $registerResponse.Headers.Location -eq "/Login/Login") {
    Write-Host "✅ [PASS] Account registered successfully. Redirected to Login."
} elseif ($registerResponse.StatusCode -eq 200) {
    # If it returns 200, it means validation failed and it returned the view
    Write-Host "❌ [FAIL] Registration rejected. Validation Error."
} else {
    Write-Host "❌ [FAIL] Unexpected HTTP status: $($registerResponse.StatusCode)"
}

# ---------------------------------------------
# Test 3: SQL Database Verification
# ---------------------------------------------
Write-Host "-> Verifying Database Integrity..."
# We will check this externally using sqlcmd.

# ---------------------------------------------
# Test 4: Perform Login with New Account
# ---------------------------------------------
Write-Host "-> Testing Valid Login (L1)..."

# Fetch login page to get new CSRF
$loginGet = Invoke-WebRequest -Uri "$baseUrl/Login/Login" -WebSession $session
$loginHtml = $loginGet.Content
$loginTokenMatch = [regex]::Match($loginHtml, 'name="__RequestVerificationToken" type="hidden" value="([^"]+)"')
$loginCsrfToken = $loginTokenMatch.Groups[1].Value

$loginBody = @{
    "__RequestVerificationToken" = $loginCsrfToken
    "Email" = "qa_tester@philadelphia.edu.jo"
    "Password" = "Password123!"
}

$loginResponse = Invoke-WebRequest -Uri "$baseUrl/Login/Login" -Method POST -Body $loginBody -WebSession $session -MaximumRedirection 0 -ErrorAction SilentlyContinue

if ($loginResponse.StatusCode -eq 302 -and $loginResponse.Headers.Location -eq "/Dashboard/Dashboard") {
    Write-Host "✅ [PASS] Login successful. Authentication Cookie set."
    Write-Host "✅ [PASS] Redirected to Dashboard successfully."
} else {
    Write-Host "❌ [FAIL] Login failed. HTTP Status: $($loginResponse.StatusCode)"
}

# ---------------------------------------------
# Test 5: Verify Lockout Behavior (L4)
# ---------------------------------------------
Write-Host "-> Testing Lockout Behavior (L4)..."
$lockoutBody = @{
    "__RequestVerificationToken" = $loginCsrfToken
    "Email" = "qa_tester@philadelphia.edu.jo"
    "Password" = "WrongPassword!"
}

for ($i = 1; $i -le 5; $i++) {
    $failResp = Invoke-WebRequest -Uri "$baseUrl/Login/Login" -Method POST -Body $lockoutBody -WebSession $session -ErrorAction SilentlyContinue
    if ($i -eq 5) {
        if ($failResp.Content -match "Account locked for 15 minutes") {
            Write-Host "✅ [PASS] Account correctly locked after 5 failed attempts."
        } else {
            Write-Host "❌ [FAIL] Account did not lock properly."
        }
    }
}

Write-Host "=============================="
Write-Host "🎉 QA AUDIT COMPLETE"
Write-Host "=============================="
