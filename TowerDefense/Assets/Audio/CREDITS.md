# Audio Credits — TP3 IFT-2103

Tous les assets audio utilisés sont sous licences libres (CC0 ou CC-BY 3.0). À inclure dans le PDF de remise.

## Musique

| Fichier dans le projet | Source | Auteur | Licence |
|---|---|---|---|
| `Music/music_menu.wav` | freesound.org #197360 — *Music from the Middle Ages — Douce Dame Jolie* | Kyster | CC-BY 3.0 |
| `Music/music_prep.ogg` | freesound.org #608913 — *Afternoon Stroll* | HenryOfSkalitz | CC-BY 3.0 |
| `Music/music_defense_light.wav` | freesound.org #414214 — *Anime Fight Music Loop 1* | Sirkoto51 | CC-BY 3.0 |
| `Music/music_defense_intense.wav` | freesound.org #443128 — *Boss Battle Loop 3* | Sirkoto51 | CC-BY 3.0 |
| `Music/music_victory.wav` | freesound.org #843046 — *Victory Fanfare 8-bit Thunder 4* | Silverillusionist | CC-BY 3.0 |
| `Music/music_defeat.ogg` | freesound.org #627916 — *Time Draft Hiss and Crackle* | Sonically Sound | CC-BY 3.0 |

## Ambiance

| Fichier | Source | Auteur | Licence |
|---|---|---|---|
| `Ambient/ambient_wind.wav` | freesound.org #609035 — *Winter Fantasy — Witch, Dark Forest, River, Coyote, Rain, Snow, Wind, Birds, Dogs, Nature Atmosphere Ambient Surround* | Szegvari | CC-BY 3.0 |

## Effets sonores

Tous les SFX proviennent des packs Kenney, licence **CC0 1.0 Universal (domaine public)**.

### Tower SFX (Kenney RPG Audio + Impact Sounds)
- `tower_shoot_1/2/3.ogg` ← `drawKnife1/2/3.ogg` (RPG Audio)
- `tower_impact_1/2.ogg` ← `impactMetal_light_000/001.ogg` (Impact Sounds)
- `tower_place.ogg` ← `metalLatch.ogg` (RPG Audio)
- `tower_upgrade.ogg` ← `impactBell_heavy_000.ogg` (Impact Sounds)

### Enemy SFX (Kenney Impact Sounds)
- `enemy_hit_1/2.ogg` ← `impactWood_heavy_000/001.ogg`
- `enemy_death_1/2.ogg` ← `impactPunch_heavy_000/001.ogg`
- `enemy_footstep_1..4.ogg` ← `footstep_grass_000..003.ogg`

### Base SFX
- `base_hit.ogg` ← `impactPlate_heavy_000.ogg`

### Player SFX
- `player_footstep_1..4.ogg` ← `footstep_wood_000..003.ogg`

### UI SFX (Kenney UI Audio + Impact Sounds)
- `ui_hover.ogg` ← `rollover1.ogg` (UI Audio)
- `ui_click.ogg` ← `click1.ogg` (UI Audio)
- `ui_back.ogg` ← `switch2.ogg` (UI Audio)
- `ui_open.ogg` ← `switch14.ogg` (UI Audio)
- `ui_wave_start.ogg` ← `impactBell_heavy_002.ogg` (Impact Sounds)

## Sources des packs Kenney

- **Kenney Impact Sounds** — https://kenney.nl/assets/impact-sounds — CC0
- **Kenney RPG Audio** — https://kenney.nl/assets/rpg-audio — CC0
- **Kenney UI Audio** — https://kenney.nl/assets/ui-audio — CC0

## Note : foleys ambiance

Le dossier `Ambient/` est vide. Pour couvrir le critère « foleys » de la grille Ambiance sonore
(6 points : foley environnementaux + spatialisation), ajouter au moins :
- Un `ambient_wind` loop (2D, sur l'AudioManager au démarrage du Game)
- Un `ambient_torch` crackle (3D, spatialisé, attaché aux torches pour le critère spatialisation)
