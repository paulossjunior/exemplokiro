# Offline Handling - Manual Testing Guide

## Prerequisites

1. Start the back-end API server
2. Start the front-end development server: `npm run dev`
3. Open the application in a browser
4. Open browser DevTools (F12)

## Test Scenarios

### Test 1: Browser Offline Event Detection

**Steps:**
1. Open DevTools → Network tab
2. Select "Offline" from the throttling dropdown
3. Observe the offline indicator banner appears at the top
4. Select "No throttling" to go back online
5. Observe the banner disappears

**Expected Results:**
- ✓ Red banner appears immediately when going offline
- ✓ Banner shows "You are currently offline"
- ✓ Banner shows "Attempting to reconnect..." with spinner
- ✓ Banner disappears when going back online
- ✓ Smooth slide-down animation

### Test 2: API Request Failure Detection

**Steps:**
1. Keep the browser online
2. Stop the back-end API server
3. Navigate to a page that makes API requests (e.g., Projects list)
4. Wait for 2-3 failed requests
5. Observe the offline indicator appears

**Expected Results:**
- ✓ After 2 consecutive failed requests, offline banner appears
- ✓ Banner shows last successful connection time
- ✓ Console shows retry attempts with delays

### Test 3: Automatic Retry with Exponential Backoff

**Steps:**
1. Stop the back-end API server
2. Open browser console
3. Try to load a page that makes API requests
4. Observe console logs for retry attempts

**Expected Results:**
- ✓ Console shows: "[Retry] Attempt 1/3 failed. Retrying in 1000ms..."
- ✓ Console shows: "[Retry] Attempt 2/3 failed. Retrying in 2000ms..."
- ✓ Console shows: "[Retry] Attempt 3/3 failed. Retrying in 4000ms..."
- ✓ After 3 attempts, request fails with error
- ✓ Delays increase exponentially (1s → 2s → 4s)

### Test 4: Last Successful Fetch Timestamp

**Steps:**
1. Load a page successfully (e.g., Projects list)
2. Note the current time
3. Stop the back-end API server
4. Wait 2-3 minutes
5. Try to navigate to another page
6. Observe the offline banner

**Expected Results:**
- ✓ Banner shows "Last successful connection: X minutes ago"
- ✓ Time updates correctly based on when last request succeeded
- ✓ Human-readable format (seconds, minutes, hours)

### Test 5: Auto-Resume on Connectivity Restoration

**Steps:**
1. Go offline (stop API server or use DevTools)
2. Verify offline banner appears
3. Go back online (start API server or disable offline mode)
4. Wait a few seconds
5. Try to make an API request

**Expected Results:**
- ✓ Offline banner disappears automatically
- ✓ API requests succeed immediately
- ✓ No manual refresh required
- ✓ Failed request count resets to 0

### Test 6: Retry Limit (3 Attempts)

**Steps:**
1. Stop the back-end API server
2. Open browser console
3. Try to create a new project or transaction
4. Count the retry attempts in console

**Expected Results:**
- ✓ Exactly 3 retry attempts are made
- ✓ After 3rd attempt, error is shown to user
- ✓ No infinite retry loop
- ✓ Error message is clear and actionable

### Test 7: Non-Retryable Errors

**Steps:**
1. Ensure API server is running
2. Try to access a non-existent resource (404)
3. Try to access without authentication (401)
4. Try to submit invalid data (400)

**Expected Results:**
- ✓ 404 errors are NOT retried
- ✓ 401 errors are NOT retried (redirects to login)
- ✓ 400 validation errors are NOT retried
- ✓ Only network errors and 5xx errors are retried

### Test 8: Multiple Concurrent Requests

**Steps:**
1. Stop the back-end API server
2. Navigate to dashboard (makes multiple API calls)
3. Observe retry behavior

**Expected Results:**
- ✓ Each request retries independently
- ✓ Offline banner appears after 2 failed requests
- ✓ All requests eventually fail after 3 attempts
- ✓ No request flooding or race conditions

### Test 9: Accessibility

**Steps:**
1. Enable screen reader (NVDA, JAWS, or VoiceOver)
2. Go offline
3. Listen for announcements

**Expected Results:**
- ✓ Screen reader announces "You are currently offline"
- ✓ Announcement is immediate (assertive)
- ✓ Banner is keyboard accessible
- ✓ No focus traps

### Test 10: Visual Design

**Steps:**
1. Go offline
2. Observe the banner design
3. Test on different screen sizes

**Expected Results:**
- ✓ Red background with white text (high contrast)
- ✓ Clear icon indicating offline status
- ✓ Spinner animation is smooth
- ✓ Responsive on mobile, tablet, desktop
- ✓ Banner doesn't overlap content

## Performance Testing

### Test 11: Network Status Check Performance

**Steps:**
1. Open browser DevTools → Performance tab
2. Start recording
3. Go offline and online multiple times
4. Stop recording and analyze

**Expected Results:**
- ✓ Minimal CPU usage for status checks
- ✓ No memory leaks
- ✓ Event listeners properly attached/detached

### Test 12: Retry Delay Accuracy

**Steps:**
1. Stop API server
2. Make an API request
3. Use browser console to measure actual delays
4. Compare with expected delays (1s, 2s, 4s)

**Expected Results:**
- ✓ Delays are accurate (±100ms tolerance)
- ✓ Exponential backoff works correctly
- ✓ Max delay cap (10s) is respected

## Edge Cases

### Test 13: Rapid Online/Offline Switching

**Steps:**
1. Rapidly toggle offline mode on/off in DevTools
2. Observe banner behavior

**Expected Results:**
- ✓ Banner appears/disappears correctly
- ✓ No flickering or race conditions
- ✓ State remains consistent

### Test 14: Long Offline Period

**Steps:**
1. Go offline
2. Wait 30+ minutes
3. Go back online

**Expected Results:**
- ✓ Timestamp shows correct duration (e.g., "30 minutes ago")
- ✓ Application resumes normally
- ✓ No stale data issues

### Test 15: Offline During Form Submission

**Steps:**
1. Fill out a form (e.g., create project)
2. Stop API server
3. Submit the form
4. Observe retry behavior

**Expected Results:**
- ✓ Form shows loading state
- ✓ Retry attempts are made
- ✓ After 3 attempts, error is shown
- ✓ Form data is preserved (not lost)
- ✓ User can retry submission

## Browser Compatibility

Test on the following browsers:
- ✓ Chrome/Edge (Chromium)
- ✓ Firefox
- ✓ Safari
- ✓ Mobile browsers (iOS Safari, Chrome Mobile)

## Debugging Tips

### Enable Detailed Logging

Set environment variable in `.env.development`:
```
VITE_ENABLE_API_LOGGING=true
```

### Check Network Store State

In browser console:
```javascript
// Access network store
const networkStore = useNetworkStore();
console.log('Is Online:', networkStore.isOnline);
console.log('Failed Count:', networkStore.failedRequestCount);
console.log('Last Fetch:', networkStore.lastFetchTimestamp);
```

### Simulate Specific Scenarios

```javascript
// Force offline
const networkStore = useNetworkStore();
networkStore.setOnline(false);

// Force online
networkStore.setOnline(true);

// Reset failed count
networkStore.resetFailedCount();
```

## Common Issues

### Issue: Banner doesn't appear when offline
**Solution:** Check if browser supports `navigator.onLine` API

### Issue: Retry attempts not working
**Solution:** Verify API server is actually stopped, check console for errors

### Issue: Banner doesn't disappear when online
**Solution:** Check if successful API request was made, verify event listeners

### Issue: Timestamp not updating
**Solution:** Verify `recordSuccessfulFetch()` is called on successful responses

## Success Criteria

All tests should pass with the following results:
- ✓ Offline detection works via browser events and API failures
- ✓ Retry logic uses exponential backoff (1s, 2s, 4s)
- ✓ Maximum 3 retry attempts
- ✓ Last fetch timestamp displays correctly
- ✓ Auto-resume works when connectivity restored
- ✓ Accessible with screen readers
- ✓ Responsive design on all screen sizes
- ✓ No performance issues or memory leaks
