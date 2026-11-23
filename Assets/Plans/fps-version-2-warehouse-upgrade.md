# Project Overview
- Game Title: Robot Warehouse — FPS V2 (working title, editable in menu).
- High-Level Concept: A small first-person mission to clear three hostile robots from a custom warehouse. The player explores three connected combat spaces, uses cover, manages magazine/reserve ammunition, collects supplies, and earns points for kills.
- Players: Single player versus AI; offline.
- Inspiration / Reference Games: Existing Unity FPS Microgame as technical foundation; assignment's Explore → Fight → Mission Complete flow. Do not reproduce the original tutorial level layout.
- Tone / Art Direction: Readable stylized sci-fi warehouse using existing robot, weapon, pickup, material and sound assets, with simple custom static geometry. No new generated characters or external asset purchases.
- Target Platform: Existing StandaloneWindows64 configuration, Unity 6000.0.85f1. Source-project upgrade is agreed scope; executable export was skipped in consultation, so build delivery is not assumed approved.
- Screen Orientation / Resolution: Landscape; validate at 1920×1080 and 1280×720. Use scalable UI rather than force a fixed display resolution.
- Render Pipeline: Built-in; do not migrate rendering pipelines or input packages.

## Audit and decisions
Read actual scripts, prefabs, loaded scene components, build-scene list, input settings and dependency relationships. Current Console has no errors; it has duplicate legacy NavMesh editor menu warnings and an unrelated Invalid image warning.

| Requirement | Current evidence | V2 action |
|---|---|---|
| FPS movement/look/jump/gravity/collision | PlayerCharacterController, CharacterController and PlayerInputHandler connected; gravity 20, jump 9 | Reuse; validate controls and tune scene overrides for smaller arena if necessary |
| Shooting + damage + feedback | Blaster projectile, ShootSfx and MuzzleFlashPrefab assigned; Health and Damageable infrastructure | Reuse projectiles, verify actual hit/damage/death chain |
| Ammo depletion + R reload | Blaster AutomaticReload=true, HasPhysicalBullets=false, MaxAmmo=16, no WeaponAnimator; R input exists | NOT compliant currently. Implement finite magazine/reserve and timed manual reload independent of animation events |
| Player/enemy health | Player 100, hoverbot 100, turret 400; health UI configured | Reuse and balance V2 hoverbots; verify AI damage and player death |
| 2–3 enemies | Two active enemies: turret + hoverbot | Current count satisfies literal minimum; V2 uses three hoverbots, no advanced AI work |
| Distinct small level | MainScene uses original MainLvl/dungeon/room layout assets | Create clearly different warehouse scene, preserve V1 scene |
| HUD | Health bar, crosshair, ammo fill; numeric ammo hidden for nonphysical weapon | Display numeric magazine/reserve, score and kill progress; keep readable health bar |
| Win/lose/restart | GameFlowManager responds to objective/death; WinScene/LoseScene have Play Again → MainScene | Retarget V2 entry/restart, validate transitions, reset run state |
| Two V2 improvements | Sprint, health pickups, muzzle flash, enemy health bars, footsteps, main menu and damage flash already present | Do not claim existing tutorial features as newly authored improvements. Add working score system and ammo pickups |
| Submission | No verified Windows build or demo video | Add README/checklist/demo script; executable export only if separately requested/approved |

User chose **warehouse + score + ammo pickups**. Alternatives considered: score + completion timer (simpler but less resource-management gameplay), and interactive doors + ammo pickups (higher interaction/NavMesh risk). Warehouse approach selected: meaningful resource decisions plus immediate progress feedback (SDT competence; MDA combat mechanics produce planning and mastery). Do not add boss fights, waves, multiplayer, multiple weapons or new character models.

# Game Mechanics
## Core Gameplay Loop
1. Start from the main menu and enter the new warehouse mission.
2. Learn movement and reload in a safe loading bay; initial enemy must not attack on the first frame.
3. Move through cargo storage and a final service bay, using sightline breaks and cover.
4. Fire the existing projectile weapon. A shot consumes one magazine round; empty magazines cannot shoot or regenerate automatically.
5. Press R to reload from finite reserve ammo. Ammo pickups extend the available shots; existing health pickups support recovery.
6. Killing a robot awards 100 points once and advances the objective. Three kills give 300 points and Mission Complete.
7. Player health reaching zero gives GAME OVER; Restart starts a fresh V2 mission with original health, ammo, enemies and score.

### Ammunition contract
- Player weapon: manual-fire Blaster variant; provisional 12-round magazine, 48 starting reserve, 96 maximum reserve, 1.5-second reload. Final tuning based on projectile damage and actual combat tests.
- Reload transfers min(missing magazine rounds, reserve), only upon completion. Reserve is not spent by shooting; magazine is.
- Repeated R while reloading does nothing; R on full magazine or zero reserve does nothing.
- No shots or repeated reloads while reload is active. Pause freezes reload elapsed time. Switching/deactivating a manual weapon cancels reload without transferring ammunition.
- Additive pickup capped to reserve maximum; pickup stays if no ammo was accepted. Partial acceptance may consume the pickup and must be documented consistently.
- HasPhysicalBullets remains a casing/visual option, not the switch that enables manual ammunition. Do not turn it on for Blaster: its casing/ejection assets are unassigned.
- Preserve existing automatic-recharge enemy weapons and other V1 weapon modes. The shared controller is used by enemies, so manual behavior must be gated by configuration.

### Score contract
- 100 points per unique enemy death, never damage or repeated death callbacks.
- New run starts at 0. Restart/menu entry cannot retain stale listeners or score.
- Capture a completed-run score snapshot before loading result scenes; no cloud, high-score persistence or networking required.
- Resolve simultaneous last-enemy death/player death with a single terminal transition; adopt deterministic loss precedence if both occur in the same frame.

## Controls and Input Methods
- Keep verified **Legacy Input Manager** (activeInputHandler=0).
- WASD/arrows: move; mouse: look; left mouse: fire; Space: jump; Left Shift: existing sprint; R: manual reload; Tab: existing pause/options.
- InputManager already contains Reload=r, Fire=mouse 0, Sprint=left shift and Jump=space. Inspect actual existing movement/pause bindings before writing help text. Do not add Input System or duplicate axes.
- Keep existing cursor locking for gameplay and unlocking for menus; verify pause/resume and result buttons.

# UI
Use existing Canvas/uGUI and TextMeshPro font assets, not a new UI framework. Preserve established fps.UI assembly boundaries. Reuse the current Roboto font; English short labels avoid introducing unsupported Vietnamese glyphs without a separate font/localization task.

## HUD wireframe
```text
┌──────────────────────────────────────────────────────────┐
│ [Tab] Pause        CLEAR THE WAREHOUSE       SCORE 000    │
│                    ROBOTS 0 / 3                         │
│                                                          │
│                          +                               │
│                                                          │
│ [HP bar] 100 / 100                 AMMO 12 / 48           │
│                                   [R] Reload / Reloading │
└──────────────────────────────────────────────────────────┘
```
- Health bottom-left, ammunition bottom-right, crosshair central. Score/progress top-right/top-center without overlapping compass or notification UI.
- Numeric ammo always visible for the manual V2 weapon regardless of shell casing configuration.
- Show Reloading while active; show R hint when magazine not full and reserve positive; show No reserve when depleted.
- New labels must not intercept gameplay/UI raycasts unnecessarily. CanvasScaler and anchors support both target test resolutions.

## Menu/result wireframes
```text
ROBOT WAREHOUSE — FPS V2
Clear 3 robots. Collect ammo. Earn points.
[ Start Mission ]  [ Controls ]

MISSION COMPLETE                 GAME OVER
Score: 300                       Score: <run score>
[ Restart Mission ] [ Menu ]      [ Restart Mission ] [ Menu ]
```
- Update WinScene/LoseScene text and route their restart buttons to V2, keeping menu links to IntroMenu.
- Clear controls text should match actual bindings; replace tutorial control image if it would misdescribe the new ammo rules.

# Key Asset & Context
## Existing assets to preserve or reuse
- `Assets/FPS/Scenes/MainScene.unity`: keep original V1 level, do not overwrite its geometry.
- `Assets/FPS/Scenes/IntroMenu.unity`, `WinScene.unity`, `LoseScene.unity`: shared presentation and V2 routing edits.
- `Assets/FPS/Prefabs/Player.prefab`: reuse through V2 scene instance overrides or a V2 variant; avoid globally changing starting weapons.
- `Assets/FPS/Prefabs/Weapons/Weapon_Blaster.prefab`: retain original; create manual-reload variant for V2.
- `Assets/FPS/Prefabs/Enemies/Enemy_HoverBot.prefab`: three instances, with scene overrides for health/detection/patrol as needed.
- `Assets/FPS/Prefabs/UI/GameHUD.prefab`, `AmmoCounter.prefab`, `GameManager.prefab`: existing HUD/manager foundations. Prefer V2 variants/scene overrides for added presentation.
- `Assets/FPS/Prefabs/Pickups/Pickup_Health.prefab`: place two existing health pickups; no need to rebuild.
- `Assets/FPS/Prefabs/Projectiles/Projectile_Blaster.prefab`: inspect damage and references before balancing, retain projectile mechanics.

## Existing scripts likely modified
- `Assets/FPS/Scripts/Game/Shared/WeaponController.cs`: manual ammo initialization, reserve properties, timed reload, shoot gates, safe optional animation; retain public compatibility wrappers where needed. Existing StartReloadAnimation calls GetComponent<Animator>().SetTrigger("Reload") unconditionally and Reload relies on physical carried bullets. Existing carried bullet arithmetic conflates magazine/total and requires correction for manual path.
- `Assets/FPS/Scripts/Gameplay/AmmoPickup.cs`: use successful reserve addition on the matched runtime weapon and only consume pickup when accepted. Current script has no prefab/scene dependents and is not placed in MainScene.
- `Assets/FPS/Scripts/UI/AmmoCounter.cs`: numeric manual ammo and hints; rename erroneous `Destroy()` callback to `OnDestroy()` for event unsubscription, null-safe initialization.
- `Assets/FPS/Scripts/Gameplay/Objectives/ObjectiveKillEnemies.cs`: ensure initial counter displays the actual three enemies after registration, not stale serialized six. Do not accidentally require fps.AI from fps.Gameplay and create an assembly cycle; use configured scene target plus existing events or an appropriate manager-initialization contract.
- `Assets/FPS/Scripts/Game/Managers/GameFlowManager.cs`: only if necessary for single-ending guard/snapshot and deterministic simultaneous outcome. Preserve existing event and scene-loading architecture.
- `Assets/FPS/Scripts/Game/Events.cs`: add score update event if used, following existing EventManager patterns.
- `PlayerWeaponsManager.cs`: inspect reload gate/cancellation; modify only when necessary to support new contract without changing enemy behavior.

## Proposed new assets/scripts
- `Assets/FPS/Scenes/Warehouse_V2.unity` and a distinct baked NavMesh asset beside it.
- `Assets/FPS/Prefabs/Weapons/Weapon_Blaster_V2.prefab` and `Assets/FPS/Prefabs/Pickups/Pickup_Ammo_V2.prefab`.
- V2 UI prefab variants only where shared HUD changes would otherwise affect V1 presentation.
- `Assets/FPS/Scripts/Game/Managers/RunScoreManager.cs`: run score, unique kill handling, result snapshot and reset.
- `Assets/FPS/Scripts/UI/ScoreHUD.cs`, `RunResultUI.cs`: UI presentation, subscribing/unsubscribing correctly.
- Simple warehouse materials under `Assets/FPS/Art/Materials/V2/` only if existing art cannot provide readable surfaces.
- `Assets/FPS/Documentation/V2-README.md`, `V2-Demo-Checklist.md`: feature/rubric mapping, controls, asset provenance, restart flow and demo script.
- Temporary verification scripts/tests created only during approved implementation; remove disposable artifacts afterwards.

## Dependency and compatibility context
- Actual direct dependents of WeaponController include Blaster, Shotgun, Launcher, both enemy prefabs and Pickup_Blaster. Every shared behavior change needs a V1/automatic-enemy regression check.
- AmmoCounter.cs → AmmoCounter.prefab → GameHUD.prefab; changing it can affect both gameplay scenes.
- GameFlowManager → GameManager.prefab; LoadSceneButton → IntroMenu/WinScene/LoseScene.
- Existing assemblies: fps.Game, fps.Gameplay, fps.AI, fps.UI. Keep logic in their current layers; UI can depend on game/gameplay; do not introduce circular references or modify packages.
- AI Navigation 2.0.15 is installed. Current scene has one package NavMeshSurface (agent type 0), two NavMeshAgents (radius 0.5, height 2, type 0), two pickup NavMeshModifiers; no detected NavMeshObstacle/NavMeshLink. V2 uses same agent type and existing EnemyMobile AI. Static crates/walls are baked geometry, no dynamic carving/links needed.
- Duplicate editor menu warnings originate from legacy Assets/NavMeshComponents plus installed navigation package. Do not delete legacy assets blindly or edit package source; treat as nonblocking unless independently traced and a safe migration is justified.
- Before approved edits, refresh dependency/dependent impact analysis for every affected existing asset; include new scene references after creation. Current tools allowed inspection using AssetDatabase.GetDependencies and reverse asset scans where no dedicated dependency tool is available.

# Implementation Steps
1. **Baseline and safety.** Dependencies: approval. Recheck Console, scene/prefab configuration, package versions, input backend and working compilation. Inspect projectile damage and referenced gameplay components. Record V1 scene and prefab values; preserve GUIDs and existing user assets. Ensure new paths are unused. Read dependency impact for all actual edits.
2. **Manual ammo model.** Dependencies: Step 1. Update WeaponController with separate magazine/reserve contract in the manual-reload branch, positive validated capacities, timed reload independent of Animator, pause/cancel behavior, shooting lock and read APIs. Retain current automatic recharge behavior. Create Weapon_Blaster_V2 variant (AutomaticReload=false, HasPhysicalBullets=false, manual shoot); assign provisional balance. Verify empty/full/partial reload and enemy automatic weapon regression before scene wiring.
3. **Ammo pickup.** Dependencies: Step 2. Update AmmoPickup to add reserve to matched runtime V2 weapon via SourcePrefab/HasWeapon contract. Create distinctive ammo-box pickup with collider, kinematic Rigidbody, existing feedback sound/visuals and no blocking NavMesh contribution. Verify full reserve rejection, capped addition and single consumption.
4. **Score and result data.** Dependencies: Steps 1–2. Implement RunScoreManager in fps.Game; subscribe to EnemyKillEvent early, award 100 once per enemy, broadcast changes, reset per new mission and preserve only latest result snapshot. Integrate terminal guard/snapshot with GameFlowManager only as required. Verify death events cannot double-score or replace an already scheduled ending.
5. **Warehouse level.** Dependencies: Steps 2–4. Create Warehouse_V2, retaining required managers, EventSystem and player HUD but replacing original map geometry with custom loading bay → storage room → service bay layout (roughly 30×36 m, adjustable to assets). Add exterior boundaries, internal walls, open passages at least 3 m wide, varied crate cover and a short route around cover. Spawn player in safe bay, three robots in staged combat positions, two health and two reachable ammo pickups. Use static colliders and readable floor/wall contrast. Prefer level geometry prefab hierarchy for organization. Do not retain off-map original geometry, stale patrol nodes, lava or old NavMesh data.
6. **Navigation and objective wiring.** Dependencies: Step 5. Configure package NavMeshSurface for type 0, current level hierarchy/layer collection and suitable geometry; exclude pickups/actors. Bake new mesh and save a separate data asset. Verify all three agents sample valid NavMesh positions, can reach player spaces and do not have nonkinematic Rigidbody/active obstacle conflicts. Replace patrol routes to match the warehouse. Configure kill-all objective and initial `0 / 3` display consistently.
7. **HUD and menu/result flow.** Dependencies: Steps 2–6. Update AmmoCounter and V2 UI variant, attach ScoreHUD/RunResultUI, and wire numeric magazine/reserve and score display. Keep health/crosshair/feedback, add numeric HP if practical. Retarget Start/Restart to Warehouse_V2 in IntroMenu/WinScene/LoseScene; revise result text. Add new scene to enabled build scenes after IntroMenu; retain V1 scene for comparison and valid result/menu scenes. Verify event cleanup and cursor/timeScale reset across pause, menu and repeated restart.
8. **Balance and regression.** Dependencies: Steps 5–7. Test actual projectile damage, AI detection/attack and pickup placement. Tune health, projectile fire cadence, scene movement speed and finite ammo so a reasonable-accuracy player can win while still showing reload/pickups. Maintain recoverable safe access to ammo and avoid enemy spawn shooting through cover. Verify automatic enemy weapons and V1 scene still initialize without errors.
9. **Submission preparation.** Dependencies: Step 8. Create README and 2–4 minute demo checklist documenting new map, manual reload, score and ammo pickups as new work. Include source attribution for Microgame assets and explain how to run V1/V2. Do not claim an executable/video exists unless actually produced. Windows build/export is an optional follow-up requiring the user's delivery decision; do not alter target platforms for this task.

# Verification & Testing
## Baseline/Edit Mode
- No C# compile errors and no shader errors in affected assets.
- Warehouse scene has exactly three active EnemyController instances, one player, valid references, one objective and no missing scripts.
- All enabled build scenes exist and menu/result targets match their names.
- New NavMesh data is not V1's NavMesh asset; agent type 0 matches the surface.
- Unit tests (where compatible existing Test Framework 1.6.0 permits): reload transfer arithmetic, capacity clamps, duplicate score handling/reset. Do not test only formulas when actual components still need runtime coverage.

## Runtime functional tests
- Movement in all directions, mouse look, jump/gravity/landing and collision with walls/cover; no escaping the level. Compare normal/sprint movement with input and check pause/resume.
- Fire N shots at legal cadence: magazine decrements N, reserve unchanged; zero magazine produces no new projectile or damage and does not recover after waiting.
- R reload completes after expected duration: e.g. 0/48 → 12/36; 5/3 → 8/0; full magazine/zero reserve/repeated R leave state correct; cannot fire during reload. No Animator NullReferenceException.
- Pause freezes reload and resuming completes once; cancel/reactivate leaves ammo consistent.
- Actual projectile hitting robot decreases Health; blocked shots hit cover, not the player/robot behind it. Robots detect/approach/attack the player; damage decreases player health and HUD.
- Ammo pickup adds capped reserve once, matches the runtime player's V2 weapon and updates HUD immediately; at maximum reserve it is not wasted. Health pickup restores only up to maximum health.
- Unique enemy death awards 100 and increments progress once. Final death gives 300, Mission Complete and correct result score; repeated callbacks do not duplicate points.
- Player death gives GAME OVER, and no later enemy death overwrites it. Simultaneous player/final-enemy death follows documented deterministic policy.
- Restart from both results: health 100, initial ammo, score 0, three live enemies, correct cursor/timeScale and no doubled listeners. Repeat restart/menu cycle at least three times.
- All three agents on NavMesh with complete paths to intended combat spaces and no navigation errors.

## Visual and manual verification
- Capture and inspect actual Play Mode Game View screenshots of warehouse, ammo/reload HUD and results; static scene reads do not establish runtime visual correctness.
- Check HUD at 1920×1080 and 1280×720: no overlap, clipping, hidden reserve count or unreadable text; crosshair and pickups distinguishable; no magenta materials.
- Play a complete fair run without debug kill methods to verify level navigation, ammo balance and win reachability. Separate scripted damage/death tests from authentic combat verification in the report.
- Check Console for runtime exceptions during startup/combat/reload/results/restart; explain any remaining nonblocking Editor warnings instead of claiming all warnings eliminated.

## Demo script (2–4 minutes)
- 0:00–0:25: menu, custom warehouse overview, movement/look/jump and sprint.
- 0:25–1:10: shooting robot, health/damage HUD, empty magazine, blocked firing and R reload.
- 1:10–2:00: ammo pickup/reserve increase, cover use and score increase.
- 2:00–3:00: third robot kill → Mission Complete/300 points → Restart resets state.
- 3:00–3:40: enemy damages player → GAME OVER → Restart/Menu.

## Acceptance
All minimum A–H requirements verified in the V2 scene; an obviously new level layout; score and ammo pickup demonstrated as two newly implemented improvements; no gameplay-blocking errors; source and verification/demo documentation available. Keep unverified executable/video delivery explicitly separate.