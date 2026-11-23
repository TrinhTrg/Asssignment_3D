# Version 2 demo and acceptance checklist

Record Game View or a playable build. Do not show editor-only debug kills as gameplay proof.

## Suggested 2–4 minute video
- 0:00–0:25: START MISSION, show the custom warehouse, move forward/backward/sideways, mouse look, jump and sprint.
- 0:25–1:10: shoot a robot; show enemy damage, muzzle flash, ammo count dropping, empty magazine cannot fire, press R and show RELOADING followed by magazine refill/reserve decrease.
- 1:10–2:00: collect amber ammo supply, show reserve increasing, collect health after taking damage, use crates as cover.
- 2:00–3:00: show score rising 100 per kill, finish three kills, MISSION COMPLETE / SCORE 300. Restart and show reset to three enemies, score 0, 12/48 ammo and 100 HP.
- 3:00–3:40: demonstrate robot detection/attack and player damage, GAME OVER, Restart/Menu.

## Rubric evidence
| Requirement | Evidence to show |
|---|---|
| FPS control | WASD/arrows, mouse, Space, gravity and collision |
| Shooting/reload | Actual projectile damage, finite 12-round magazine and R reload |
| Health/enemy | Player and robot health, robot attack, death/destruction |
| Different level | Warehouse three-bay layout and cover, not V1 tutorial map |
| HUD/state | HP, AMMO magazine/reserve, crosshair, robot counter, both results/restart |
| New feature 1 | SCORE +100 for a kill, final result score |
| New feature 2 | Amber ammo box increases reserve and disappears only when accepted |
| Organization | V2 scene/variants and documentation; original level retained |

## Human test before submission
- [ ] Finish one mission without debug methods, including a reload and an ammo pickup.
- [ ] Lose to enemy damage and restart successfully.
- [ ] All movement, looking, firing, reload and pause bindings work with real input.
- [ ] Pause freezes reload/AI; resume works; cursor behaves correctly.
- [ ] HUD readable at 1920×1080 and 1280×720.
- [ ] No errors in Console during complete playthrough.
- [ ] Source archive includes Packages and ProjectSettings.
- [ ] If a Windows build is requested, build and launch that executable separately; Editor success is not proof of executable success.
- [ ] Record and submit actual video; this document is not a demo video.
