# Verify the actual tab and transport

## What happened

During F02 browser verification, a Playwright comparison using `p !== page`
selected the originating tab through a differently wrapped page handle.
Separately, observing the first WebSocket captured Vite's HMR connection rather
than SignalR. Initial observations therefore did not establish cross-tab
notification delivery.

## Impact and evidence

The harness assumptions caused failed checks and misleading intermediate
observations. An HMR frame contained `{"type":"connected"}`, not
`CharacterChanged`. In-page markers subsequently identified distinct primary
and secondary tabs. Filtering the WebSocket URL to the Character notification
endpoint established two real revision frames and an 8 ms secondary-tab update
after the primary tab's save response.

## Candidate improvements

- For integrated-browser multi-tab checks, establish distinct page identity
  with an in-page marker or another explicit runtime identifier; do not assume
  JavaScript handle identity proves distinct tabs.
- Match the intended WebSocket endpoint and wait for its connection before
  issuing the edit. Exclude development-server transport events.
- Require the notification frame and the other tab's authoritative reload,
  rather than treating a fast heading change alone as evidence of SignalR.

These are verification proposals, not new repository instructions.
