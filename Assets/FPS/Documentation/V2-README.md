# Robot Warehouse — FPS Version 2

Open `Assets/FPS/Scenes/IntroMenu.unity` and press Play → START MISSION.
For direct testing open `Assets/FPS/Scenes/Warehouse_V2.unity`.
The original tutorial level is preserved as `Assets/FPS/Scenes/MainScene.unity`.

## Controls
- WASD / arrow keys: move; mouse: look.
- Left mouse: shoot; Space: jump; Left Shift: sprint.
- R: reload from reserve; Tab: pause/options; Escape: release mouse.
- Existing right-mouse aiming and crouching remain available.

## New work for Version 2
1. **Custom warehouse level:** three connected bays, alternating wide doorways, nine cargo obstacles, boundary walls and marked routes. Three robot enemies with their own patrol routes. A separate NavMesh asset belongs to the new scene.
2. **Finite magazine + manual reload:** 12 magazine rounds, 48 starting reserve, maximum reserve 96; reload takes 1.5 seconds. No automatic player recharge. Reload transfers only the rounds missing from the magazine; cannot fire during reload. Pause freezes it; stowing the weapon cancels it without spending reserve. No weapon animator or shell assets are required.
3. **Ammo pickups (new improvement):** two amber supply boxes each offer 24 reserve rounds. A full reserve rejects the pickup; partial acceptance consumes the box, discarding excess. Walk into the box to collect it.
4. **Score system (new improvement):** +100 per unique robot death, shown with kills in the HUD; all three kills = 300 points. Result scenes display the last run's score. Restart resets score, health, ammo and robots.

Existing Microgame sprint, health pickups, enemy health bars, muzzle flash, footsteps, main menu and damage feedback are retained, **not claimed as newly authored improvements**.

## Game flow
Start → Loading Bay → Cargo Storage → Service Bay → defeat 3 robots → MISSION COMPLETE → Restart/Menu.
Player health reaching zero → GAME OVER → Restart/Menu.
If both terminal events occur in the same frame, defeat takes precedence; an already scheduled result cannot be overwritten.

## Balancing
- Player: 100 health, ground speed 6 m/s, sprint multiplier 1.5.
- Robots: 75 health each; player projectile damage 15 → 5 successful body hits per robot (before any damage multipliers).
- Starting magazine + reserve = 60 rounds; total possible with both boxes = 108 rounds, subject to reserve cap.
- Two existing health pickups restore up to 40 health each. Robots can also drop existing health loot.
- Boundary walls are 7 m high, internal dividers 6 m, passages 4 m wide, static crate cover 2.8 m high.

## Implementation layout
- `Game/Shared/WeaponController.cs`: manual reserve/reload branch; automatic enemy and original V1 recharge behavior retained.
- `Game/Managers/RunScoreManager.cs`: unique-kill scoring and result snapshot.
- `Gameplay/AmmoPickup.cs`: accepted reserve supply and pickup feedback.
- `UI/AmmoCounter.cs`, `ScoreHUD.cs`, `RunResultUI.cs`: magazine/reserve, reload hint, health/score and results.
- V2 weapon and ammo HUD use prefab variants. Original Player and Blaster prefabs retain their baseline configuration.
- Keep the Built-in render pipeline and Legacy Input Manager. No new packages required.

## Assets and attribution
Robot meshes/animations, Blaster/projectiles, fonts, VFX, skybox, health pickups and sounds are reused from the existing Unity FPS Microgame project. Warehouse layout/static geometry, four V2 materials, ammo supply box, V2 configurations and added gameplay/UI integration are the new work. Preserve the original project's licence/attribution files when submitting. No external generated assets were used.

## Submission
Submit the **full Unity project**, including Assets, Packages and ProjectSettings. Do not submit only the scripts. Library/Temp are generally regenerated and not needed in a source archive.
Windows build is a separate delivery task, not produced by this upgrade unless explicitly confirmed later. Existing build-scene list starts with IntroMenu and contains Warehouse_V2 plus results and the preserved V1 scene.
Demo video instructions: see `V2-Demo-Checklist.md`.

## Verification notes
Completed automated multi-frame Play Mode verification in Unity 6000.0.85f1:
- Menu Start button invokes the warehouse scene; three robots start on NavMesh; player grounded, HP100, score0, ammo12/48.
- Twelve actual shots drain the magazine only; empty firing rejected; no automatic recovery.
- Timed reload works without an Animator; firing during reload rejected; pause freezes reload.
- Reload transfers tested: 0/48 → 12/36; 5/3 → 8/0; full-magazine/zero-reserve requests rejected.
- Reserve caps at96; real trigger contact rejects a full-reserve pickup and consumes it once when ammunition is accepted.
- Health pickup physically heals40 up to the cap. Diagnostic 80 → 40 → 80 confirmed this; an initial test incorrectly expected100 after existing enemy damage and was corrected (not a game bug).
- Enemy AI/projectiles damage the player. Actual player projectiles kill all three robots, produce score300 and enter WinScene.
- A player deliberately wounded by the test was finished by a real enemy attack and entered LoseScene.
- Restart restores health100, ammo12/48, score0 and three robots; three further reload/restart cycles passed.
- No runtime errors captured in the accepted full test.
- Actual PNG source dimensions verified: HUD1280×720; menu and result screens1920×1080. Screenshot importer initially resized them to powers of two; that was corrected for inspection.
- Final HUD checked at both1920×1080 and1280×720 after moving score below compass: no compass/score overlap, readable ammo/health/results.
- Stowing a reloading manual weapon cancels without moving/spending ammo; duplicate enemy event awards100 once.
- Same-frame win/death selects LoseScene; later win events cannot overwrite an already scheduled defeat.
- Original MainScene still starts with two enemies; V1 Blaster retains automatic recharge and replenishes after depletion.
- Final visual/edge-case/V1 regression captured no runtime errors.

The test invoked existing weapon/button APIs and repositioned actors to isolate combat and pickups; it was **not a fair manual walkthrough**. Input simulation is not supported by this project's Legacy Input Manager. Real keyboard/mouse movement, sprint, jump, reload key and a complete human playthrough still need the checklist test before submission. Executable and video are not produced.

Existing duplicate legacy NavMesh editor menu warnings are nonblocking and were not hidden by removing packages or assets.
