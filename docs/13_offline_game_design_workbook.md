# Offline Game Design Workbook

Use this when you have no Wi-Fi and want to keep making useful progress.

Rule of thumb: do not try to answer perfectly. Write rough answers, circle uncertain areas, and mark anything that needs validation later.

## 1. One-Sentence Game Definition

Fill this in 3 different ways.
1. The game is about: Go in. Do the mission. Leave alive. 

2. The player fantasy is: Being a good "soldier", following orders, getting things done

3. A first-time player should describe the game as: Direct, no playing around, clear in direction.

Best version:

> "I like this game, and I want to play more. Exploring the rooms/routes/facility was fun, and the mixture of trying to do the objective while trying to avoid the *AI* added some nice tension, and I felt really immersed and as if I am actually in the game because of all the haptics"

## 2. Design Pillars

Pick 3-5 pillars. Each pillar should affect real decisions.

| Pillar                                          | What It Means                                                                                                                               | What It Forbids                                                                                                    |
| ----------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------ |
| Haptics are not gimmicks                        | Haptics are designed as an augmentation to the experience. I.e. Haptic X serves the core of Y game function, not just a fun addition to it. | E.g. Making a proximity sensor that has a strong haptic cue AND a strong visual cue, make only                     |
| Each haptic is unique in function               | Each haptic device must correspond to one (or at most two, if an exception is needed) gameplay mechanics. This avoids player confusion.     | Allowing a haptic device (e.g. a haptic suit) be triggerable by multiple gameplay elements.                        |
| High Interactability                            | Immersion requires being, if partially, part of the game's world. Gameplay must involve the player actively.                                | Creating gameplay elements that boil down to "spam this button really quickly"                                     |
| A Balance between explicit and implicit systems | Player interaction is not a binary choice between "gameplay" and "not gameplay." Instead, it is a fluid spectrum.                           | Do not rely strictly on explicit instructions when an implicit nudge can convey the same emotion more dynamically. |
| Communicate using mechanics, not "word"         | Visual cues, auditory clues, scene composition, lighting, etc can do the same guidance a textbox can.                                       | Avoid heavy reliance on explicit player instructions, especially when a game mechanic can do the job.              |

Example format:

| Pillar | What It Means | What It Forbids |
|---|---|---|
| Venue-only immersion | Haptics/physical effects make the threat feel present. | Features that feel identical on a home headset. |

## 3. Player Experience Targets

**At minute 0-2, the player should feel:** 

- If first time: "Ooh I like the grim atmosphere". If not the first: "I hope this run goes well, i couldn't outsmart the *AI* last time"

**At minute 3-7, the player should feel:** 

- "OK gotta plan the route so i don't get screwed over. ooh shiny thing."

**At minute 8-15, the player should feel:** 

- "Shit i almost fucked up"

**At extraction, the player should feel:** 

- "I'm almost done!!!!"

**After removing the headset, the player should say:** 

> "I wanna do that again."

## 4. Core Loop Fill-Out

Write the simplest possible version first.

```text
Enter facility
    -> Start exploration/route planning
Find/understand objective
    -> Bring prep material? like phasmophobia?
Move through danger
    -> Avoid/mitigate AI threat
Recover signal core
    -> Protect and move to extraction point/exit
Threat escalates
    -> Outsmart and adapt
Return to extraction
    -> Final Stand
Escape/fail
```

Now customize it:

1. **What does the player physically do most often?** Distract/stun AI, hide from it; Physical objective work (repair gen, collect items, etc).

2. **What creates tension every 30-60 seconds?** Never tiring AI, always chasing. The occasional intentional near-miss encounter.

3. **What tells the player they are making progress?** Progress could be localized to the objective repair/collection etc?

4. **What can go wrong without feeling unfair?** AI always adapts, but at times may adapt more aggressively, or have more aggressive behavior 

	1. **What changes between runs?** AI by design is changing, but room layouts, room contents, maybe main objective, overall mission objective (that's a big scope tho)

## 5. First Playable Scope

### Must Have

These are required for the first playable.

- [ ] VR player movement works comfortably.
- [ ] Player starts in a safe entry zone.
- [ ] Player can understand the objective.
- [ ] Player can find or activate one signal core.
- [ ] Player can extract.
- [ ] At least one threat cue creates tension.
- [ ] Run can be reset quickly.

Add your own:

- [ ] 
- [ ] 
- [ ] 

### Should Have

Useful if time allows.

- [ ] 2-3 possible signal core locations.
- [ ] One route/hazard variation.
- [ ] Simple threat state debug display.
- [ ] Simulated haptic cue.

Add your own:

- [ ] 
- [ ] 
- [ ] 

### Could Have

Nice, but not necessary.

- [ ] Heart-rate simulation.
- [ ] More detailed threat behavior.
- [ ] More rooms.
- [ ] Environmental storytelling props.

Add your own:

- [ ] 
- [ ] 
- [ ] 

### Won't Have Yet

Protect the prototype by listing what is not included.

- [ ] Multiplayer.
- [ ] Persistent progression.
- [ ] Full procedural level generation.
- [ ] Multiple enemies.
- [ ] Final art pass.

Add your own:

- [ ] 
- [ ] 
- [ ] 

## 6. Objective Design Form

Objective name: Retrieve Fuel Rods

What the player is told: Collect live fuel rods

What the player actually needs to do: Reach objective room, locate live fuel rods, collect them, and return back/go to extraction point.

How the player finds it: It won't be a free highlight. Maybe the correct fuel rods respond to a Geiger counter.

What physical VR action is involved: haptic suit for threat proximity (maybe), heart rate sensor for stealth. Maybe player, instead of Geiger counter, can be blasted by heat lamp if touching a valid fuel rod. Other effects may be used, or delegated to other objective types.

What can interrupt or complicate it: need 3s to determine a fuel rod, AI passing can maybe interrupt, or AI can spike gieger counter which gives false readings.

How the game confirms success: 2 stages, fuel rods dropped off at extraction point, and player leaves successfully, before 20 minute mark.

How the game avoids confusion: confusion of..?

Failure cases: player does not get any fuel rods, or collects more than 25% wrong fuel rods. Player dies to AI, player does not make it on time to exit.

- 

Fallback simpler version: collect rods and escape with them. Fail if neither happen.

- 

## 7. Threat Design Form

Threat name or codename: The Owl Thing

What the threat represents: Radioactive Owl-based Entity

What the player knows at first: Entity is interested in hunting them.

What the player learns over time: Entity has sharp vision, and learns from the player's tactics.

How the threat finds the player:

- [ ] Sound/noise
- [x] Line of sight
- [ ] Proximity
- [ ] Objective progress
- [ ] Time pressure
- [x] Heart-rate/stress signal
- [ ] Other:

Threat states:

| State         | What The Player Sees/Hears/Feels | What The Threat Does |
| ------------- | -------------------------------- | -------------------- |
| Dormant       |                                  |                      |
| Investigating |                                  |                      |
| Hunting       |                                  |                      |
| Disengaging   |                                  |                      |

Warning cues before danger:

- 

What makes it scary:

- 

What would make it unfair:

- 

Simplest prototype version:

- 

## 8. Adaptive Behavior Form

The threat should appear to adapt to:

- [ ] Player noise
- [ ] Repeated hiding spots
- [ ] Time spent standing still
- [ ] Sprinting or fast movement
- [ ] Objective progress
- [ ] Stress/heart rate
- [ ] Other:

For each chosen signal, fill this:

| Player Signal | How We Detect It | How Threat Responds | How Player Notices |
|---|---|---|---|
|  |  |  |  |
|  |  |  |  |
|  |  |  |  |

Important: What should adaptive behavior never do?

- 

## 9. Haptics And Physical Effects Form

Candidate effect:

In-world meaning:

Player body location:

Intensity range:

When it triggers:

Cooldown:

Can the player predict/understand it?

Safety or comfort risk:

Fallback if real hardware is unavailable:

- [ ] Controller rumble
- [ ] Audio cue
- [ ] Visual cue
- [ ] Simulated debug mode
- [ ] Other:

Test question:

> Does this physical cue make the moment clearer, scarier, or more memorable?

## 10. VR Comfort Checklist

Mark any risk that applies.

- [ ] Forced smooth camera movement.
- [ ] Fast artificial acceleration.
- [ ] Required rapid turning.
- [ ] Required crouching or repeated bending.
- [ ] Required frantic physical movement.
- [ ] Low-light navigation confusion.
- [ ] Jump scare too close to player's face.
- [ ] Haptics too intense or surprising.
- [ ] Player could collide with real-world boundaries.
- [ ] Player has no clear stop/pause signal.

Comfort rules for this game:

1. 
2. 
3. 
4. 
5. 

## 11. Room / Level Form

Room name:

Purpose:

Player enters from:

Player exits to:

Main object or interaction:

Threat opportunity:

Haptic/audio/lighting opportunity:

Navigation cue:

Replay variation:

What can go wrong:

Simplest greybox version:

## 12. Run Variation Form

List run variables that can change without breaking the critical path.

| Variable | Options | Risk | Prototype Version |
|---|---|---|---|
| Signal core location |  |  |  |
| Hazard state |  |  |  |
| Route state |  |  |  |
| Threat aggression |  |  |  |
| Lighting/audio event |  |  |  |

The 3 variables most worth prototyping first:

1. 
2. 
3. 

## 13. Failure And Recovery

Player can fail by:

- 

Failure should feel:

- [ ] Scary
- [ ] Fair
- [ ] Learnable
- [ ] Fast to recover from
- [ ] Other:

What happens after failure?

How does the operator reset the session?

What should never happen in a public demo?

- 

## 14. Tutorial / Onboarding

The player must learn:

- [ ] How to move.
- [ ] How to interact.
- [ ] What a signal core is.
- [ ] Where extraction is.
- [ ] What threat cues mean.
- [ ] What haptics mean.

How to teach without a long tutorial:

| Thing To Teach | Diegetic Teaching Idea | Backup UI/Instruction |
|---|---|---|
| Movement |  |  |
| Interaction |  |  |
| Objective |  |  |
| Extraction |  |  |
| Threat cue |  |  |
| Haptic cue |  |  |

## 15. Feature Pitch Cards

Use one card per idea. Keep it brutal and practical.

### Feature:

Player value:

Prototype cost:

Risk:

Dependencies:

Fallback:

Cut if:

Decision:

- [ ] Keep
- [ ] Simplify
- [ ] Delay
- [ ] Cut

### Feature:

Player value:

Prototype cost:

Risk:

Dependencies:

Fallback:

Cut if:

Decision:

- [ ] Keep
- [ ] Simplify
- [ ] Delay
- [ ] Cut

## 16. Open Questions Parking Lot

Write questions freely. We can sort them later.

- 
- 
- 
- 
- 

## 17. End-Of-Offline Summary

When Wi-Fi is back, fill this before sending notes to Codex.

Most important decisions I made:

1. 
2. 
3. 

Biggest uncertainties:

1. 
2. 
3. 

Things I want Codex to turn into docs/tasks:

1. 
2. 
3. 

Things I think should be cut or delayed:

1. 
2. 
3. 

