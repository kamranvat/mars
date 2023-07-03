# MARS

## Content
- What is this
- How to play
- Known Issues
- Features / Basic Game Mechanics
- UI
- Script functionality
- Github
- Resources I used
- Art and Assets
- Lore
- Note on late hand in and video link

  
### What is this?

MARS is a Unity project to be submitted for the Introduction to Unity course, summer semester of 2023. It is (rather, should be) a tower-defense like video game.

## How to play

Upgrade your turrets by clicking on an array -> turret -> upgrade. Continue when done.

## Known issues
I have had some issues with my UI that I had to fix over the weekend before the deadline, which lead to many game-systems being implemented in the code, but missing things like a single function, the appropriate mappings in the editor, and such. Known issues are:

- The level finishes while the last wave is being fought
     
- Only turrets can be upgraded, not other stats.
     
- The moon weapon attacks can not be triggered. Originally, there was supposed to be a button for each moon in the game state UI element, which when clicked would trigger the launch of the missiles. Since I did not manage to finish the launch function in time, I removed those buttons. I left the other nonfunctional UI elements in, because I decided that the part I am most happy with is how the UI looks, and I did not want to destroy that as well. Handing in with half the functions after a full week of coding every day was frustrating enough.
  
Things that are (mostly) in the code, but not in the game:

- EMP weapons that only damage shields
- Instantiation of explosions
- Different enemy types
- Different turrets
- Collecting a second resource type "intel"
- Shooting at the mouse cursor instead of the closest enemy, for the special attack (could be adapted to have the moon missiles launch at the cursor)

## Features / Basic game mechanics:
- Enemies spawn in increasingly difficult waves. Level difficulties are handled as int[] of hp values, and level amount, wave hp, as well as GameWon and such are entirely derived from those values. This means that making the game longer, or shorter, or changing the level difficulties is done simply by changing the values of that one list.
- The player has turrets that engage those enemies automatically.
- These turrets can be upgraded after each level.
- Enemies as well as the player have health bars. There is also a shield mechanic in place.
- Enemies drop resources when they die, which are collected by the player
- Buying upgrades reduces the resources the player has. If there are not enough resources to buy an upgrade, or if the item to be upgraded is at its maximum level, the upgrade button is not shown.
- At the beginning of the first level, an intro screen is displayed instead of the upgrade screen. It gives some story background on the game.
- At the end of the last level, another story screen is shown, ending the game. From this screen, the game can be quit.

## UI
A main focus for the game, and unfortunately, the element I struggled with the most. The UI is scripted to show different canvasses at different points in the game, which I have had to move out of the UI Manager class and into the GameControl element to fix some issues that I could not otherwise resolve over the past two days.
Ultimately (as explained in the note at the end of the readme), some of the Upgrade Dialogue (all the elements that are not turrets) do not do anything. I have opted to leave them in anyways, since the presentation and layout were a big part of the project, and I felt that deleting all those UI elements would have diminished the project. In place:

- The upgrade canvas also has a second panel that is displayed when the player decides to upgrade an element. The window shown depends on the type of item:
- If the player chooses one of the three arrays, which have three turret slots each, a window is displayed which lets the player choose which turret slot to upgrade.
- Clicking on one of the options, the player can then choose to upgrade or go back. Both options close the window, the upgrade button additionally increases the correct turret level by one and reduces the resources the player has.
- If the player instead chooses to upgrade one of the other stats, the appropriate window with a description of the asset is shown. In this case, the upgrade/back options are directly available.
- In both cases, upgrade button is only shown when an upgrade is possible.
- There is also a panel displaying the upgrade levels of different stats, as well as the current health and shield status of the player.
  
## Script Functionality
### Spawning enemies
- For each level, I only define the total health points that the enemies get. The goal is to have the rest happen automatically, so that I can create an arbitrary amount of levels quickly. By defining levels based on their total enemy hp, I will also have an idea of the damage the player has to be able to deal to keep things balanced. Additional things to keep it interesting - like spawning different enemies - will be added after I have this system in place.
### How it works:
- **totalHp** defines the difficulty of the level - it is the sum of the hp of all spawned enemies.
- This gets broken down into waves. The higher the level, the more waves there are - the formula for **waveAmount** is *currentLevel / 2 + 1*
- The waves are not created equal - **waveHealthDistribution**() returns an array of **waveAmount** values for each level. These are taken from a linear distribution, and always sum up to **totalHp**.
  
- _Example: Level 5 might have **totalHp** of 1200. **waveAmount**() returns 3. **waveHealthDistribution()** will now split the **totalHp** of 1200 into three **waveTotalHp** values, e.g. 200, 400, and 600 (not actual values), such that level 5 ends up having 3 increasingly difficult waves._
- Let's stick with this example and say the player is in a level with three waves. Now, the first wave will spawn. Enemies spawn at one of the predefined spawn points off screen and start flying towards the center.  They do not spawn one by one, but are instantiated in the **SpawnGroup()** function, which will spawn a small swarm (size is passed into the function) which then moves towards the player. At spawn, the hp of the spawned enemies is kept track of.
  
##### Ending a wave
Once the amount of hp that has been spawned reaches the waveTotalHp, it is checked whether there are still living enemies. As soon as they are all dead, or a timer runs out, the wave ends. Now, the next wave will spawn.

##### Ending a level
At the end of each wave, we check if it was the last one for this level. If it was, killing that wave will advance the game phase.
Once a level is over, the level counter gets incremented by one. 

##### _Summary: what does each function do?_
- total_hp - defines level difficulty
- levelSystem() - takes totalHp and currentLevel and spawns the appropriate amount of waves, passing waveHp into each wave. Only spawns a new wave when the current wave is dead. This means it needs to access isWaveAlive().
- waveSystem() - takes waveHp and spawns the appropriate amount of groups using spawnGroup() functionality. The groups are spawned with a delay, not when the previous one is dead. Later, it should be able to take a enemyType list and choose randomly between types of groups, but for now, only one type should be implemented.
- spawnGroup() - spawns the group at a random spawn point. Adds the hp it added to a public variable spawnedHp. waveHp gets accessed by waveSystem() as well - resetting it once the wave is over.

#### Problems I solved
- Because I started building this without a very good plan of what I wanted to do, I messed up some variable scopes and spent a lot of time wrestling with this script. Ultimately, I decided to rewrite the whole thing from scratch after I had seen the parts that did not work.

### Enemy behaviour
Enemies move towards the center of the screen. They handle their collision logic, destroying bullets that hit them, triggering the fuses of missiles nearby, and getting the damage tuple (more on that below) from things that damage them. They also have a damage tuple themselves, which decides how they damage the player on contact.

### Turrets
-  Each turret looks for the enemy closest to its position in its update function. It has a coroutine **ShootInArc()** which points the turret at the closest enemy (but only if it is in front of the turret, such that the turret does noot aim through the planet) and spawns a bullet with the same orientation as the turret. The bullet itself is scripted to move forward. Ideally, this will cause the bullet to hit its target.
- I also added a function to shoot at the mouse cursor. I only used it for finding a weird bug that caused my bullets to move at a slight angle. However, playing around with it was fun, so I left it, and decided to use it for the moon attack game mechanic (Note: this is one of the things I had to leave out).
#### Problems I solved
- I tried to do this using LookAt(), which sadly does not work (and has no equivalent in) 2D unity games. It does rotate the front of the turret towards the target, but the _front_ is the _camera-facing side_ of a sprite - which means it gets rotated such that it goes invisible in 2D. Instead, I got surprise visited by Pythagoras and had to implement a little geometry (which, luckily, is a problem that hasy been solved by many others as well). Now, the turret always points its local z-axis at its target.
- I then also faced the problem of the turret not hitting the target. The shots were always off-center, with accuracy decreasing the further the angle was off the vertical axis. Turns out some error in my math caused 360 degrees of turret rotation to be squeezed into a 180 degree arc, meaning that if I wanted it to aim 10 degrees to the right, it would actually do 20. I managed to correct that quickly once I saw the problem, but finding it took me quite a while.
- While debugging the previous thing, I added a function to aim at the mouse cursor. Playing around with this in the editor was kind of fun, so I decided to add some of that into the game if I would have the time.
  
### Turret slots
- There are three bases on mars which can hold up to three turrets each. There is also one special turret on Deimos and Phobos, respectively. To keep the placement and upgrading of all the turrets in their slots somewhat simple, there is the TurretManager.cs script handling all of it. It stores for the Martian turrets the position, type, upgrade level, and the value of the turret. For the special turrets on the moons, it stores only the type, upgrade level, and value.
					- The attachment slots themselves are handled in this script as well, to keep things tidy. In the Unity Editor, Empty GameObjects are attached as children to the celestial bodies and added to the script.
					- At the beginning of each level, the AttachTurrets() script populates all the slots with the appropriate turrets. SetAllUpgrades() allows the Upgrade Manager to instantiate the upgrades the player buys.
					- The script also provides a GetTurrets() function, which is important for the UI for saving the state of the game at the end of each level.

### Moons and Missiles
- I decided to have the two martian moons rotate around the planet, and to add special turrets onto them that launch missiles. These missiles steer into the nearest enemy, then instantiate an explosion which causes damage on collision.

### Damage model
- Damage is not passed as a simple value, but as a tuple of damage, bypass, and emp.
- damage is the damage value here, as you might expect
- bypass is a float between 0 and 1, determining how much of the damage is not blocked by the shield, but added directly.
- emp is a boolean, which says whether the damage is caused by an EMP weapon. If it is true, the damage gets applied to the shield 100%, but does not cause any damage to health values.
- Note: I implemented it this way to allow for different bullet and missile types, as well as different enemy types. I did not get around to implementing EMP weapons, but the different bullet leves have different bypass values.
  
### Shield
The player, as well as enemies, can have a shield. As usual in video games, this absorbs the damage just like a health value, but recharges. Shields have a delay implemented, which resets every time damage is taken. Only if the delay is at zero the shield recharges at its shield recharge rate.
The maximum hp cap for the shield as well as the regeneration rate are exposed to the Upgrade Manager and their respective UI elements.



## GitHub
As I am working alone and in a very linear fashion, I commited to main all the time
Also, again because I am on my own, I tracked my todos and such in a file synced between my local devices, instead of using GitHub issues.
My own files are in a markdown format, and are the basis for this readme. I have copied them into here (and slightly reformatted them for readability)
 
## Resources I used
Since 2d tower defense is not the most common Unity project, I had to piece together content from a lot of tutorials. Here are links to resources I found helpful:
- Enemy waves:
					 https://levelup.gitconnected.com/how-to-create-an-enemy-wave-system-in-unity-49c5328564e7
						 https://micha-l-davis.medium.com/spawning-waves-of-enemies-in-unity-fc42caf6df56
- Tower defense:
						 https://www.kodeco.com/269-how-to-create-a-tower-defense-game-in-unity-part-1
- Turret things:
          https://forum.unity.com/threads/how-to-make-a-turret-ai-in-unity.314027/
- The answers by Raimi for turrets, and ABerlemont for shooting at the mouse cursor: https://forum.unity.com/threads/2d-lookat.99708/ (This was essential in resolving my turret aiming issues)
						 https://answers.unity.com/questions/989146/how-to-attach-an-object-onto-another-object.html
- Percistence, game controller:
						 https://learn.unity.com/tutorial/persistence-saving-and-loading-data# (starts a few minutes in, really good tutorial)
				
- UI:
- This video (and the playlist it is in, for the modal windows as well) https://www.youtube.com/watch?v=HwdweCX5aMI
- Panels https://youtu.be/CGsEJToeXmA?t=316
- https://flatuicolors.com/palette/us for colours
- Progress bars (only implemented a basic version) https://www.youtube.com/watch?v=J1ng1zA3-Pk&list=PLXD0wONGOSCKcUJHc4-7LIkEgvFvJ-nl1&index=4
- To show a simple window https://www.youtube.com/watch?v=msCGC22vKQM

## Art and assets
My vision for the project was to build a rather simple game, but fill in as much art and lore as I could. This has not entirely worked out, since I got stuck on the programming bits a few times, and many things were more time-consuming than I thought. Still, I found time to generate a lot of assets with different text to image models. Originally, I had planned to run NASA images through Stable Diffusion img2img locally on my PC, but I had some installation issues. I then tried DALL-E, which was not very good at generating scifi-things, especially when "mars" was part of the prompt. I used Midjourney instead, which generated all the images that are visible in the UI after surprisingly few tries at getting the prompts right.
I also generated the enemy sprite (actually, different ones, but I ended up adding only one enemy) by running a prompt for a top-down, 2d fighter spacecraft with a perfectly black background, then using a website that removes png backgrounds on that. The resulting png can then just be imported as a Unity asset.
I used that same workflow on the missile sprite as well.
For the Moons and the image of Mars, I removed the background from images that I found on the following NASA websites (the license allows this use):
https://solarsystem.nasa.gov/moons/mars-moons/deimos/in-depth/
https://www.nasa.gov/multimedia/imagegallery/image_feature_1199.html
https://www.nasa.gov/mission_pages/mars/images/index.html
The [starry background image](https://unsplash.com/photos/VZ_GDBK98FQ?utm_source=unsplash&utm_medium=referral&utm_content=creditShareLink) is by <a href="https://unsplash.com/@yongchuan?utm_source=unsplash&utm_medium=referral&utm_content=creditCopyText">Yong Chuan Tan</a>

## Lore
The game was originally designed to include a dialogue-system based prologue, that would set some variables which would then define which ending screen gets shown. This is another thing I had to scrap, even though it is already half-done (well, the script is missing, but the text exists). I left the Intro scene in the project though, if you are interested in reading about the background I came up with. To get that, select the Prologue1 object, then click Dialogue Editor (next to Scene and Game).
The Lore was sketched out to allow four different endings, based on two booleans that the Dialogue system asset I imported can set based on the options you select. I thought it would be fitting to have AI-generated content in the game, just like with the images. I tried to get ChatGPT to output the story for each branch in dialogue-element length paragraphs, but I could not get it to stop mixing up the order in which it is supposed to write the things in. Therefore, I just gave it a bullet-point version, let it generate some text, then I manually sorted it all, removed all unnecessary bits, and formatted it for dialogue.
Ultimately, I scrapped the dialogue, but decided that since I had already fed ChatGPT all the background of my game universe, I could use it to generate the description texts for my UI. This worked very well after a few tries at getting the prompt right, and I only had to touch up the generated text a little bit before I could copy it into the game.

## Note on late hand-in and video link
As explained in the opening paragraph, I had some serious time-issues when finishing this project. This was not due to me not putting in enough time, but is at least in part because I underestimated how difficult it would be to build the systems I had envisioned. My choice of 2d tower defense was to have a simple construct I could build UI and Dialogue around. However, I learned the hard way that auto-aiming turrets in 2d are harder than in 3d (there is no 2d equivalent of LookAt()), tower defense is much less common as a project than I thought, and my choice of having _groups_ within _waves_, _waves_ within _levels_, and all _levels_ within a _scene_, with _hp_ values passed between all of these, was objectively bad. I also struggled with sharing variables and functions between scripts. This got better when I learned that GameControl or GameManager objects can be used.
On top of my poor planning, I had an issue where the canvasses I instantiated would either not display (if I followed online advice on how to fix the issue), or display with the buttons non-clickable (which was my original issue), which I wrangled for a day and a half before I took a step of deleting what I had done and re-doing all canvasses. Somehow, the issue disappeared. However, I had originally intended to use those days to finish up the systems that were almost done - which means that now, there are a lot of unfinished things in the game.

Video link to an explanation of my game: https://drive.google.com/file/d/1BfV-dusavZ-FWPS6Pl8xIIsvs-Y5PEEB/view?usp=sharing
