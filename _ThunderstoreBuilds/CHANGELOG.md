Version 1.3.3
### Fixes
Fixed an incompatibility with DieHarder by TacoSlayer

Version 1.3.2
### Fixes
Fixed error in loading persistent settings where the mod would fail to initialize if I attempted to load a setting that doesn't exist.

Version 1.3.1 - [BROKEN]
### Changes
Implemented support for OptimizedGraphicsFix to improve mod compatibility

Version 1.3.0
### Fixes
Shaders now display lighting correctly again. Existing shaders will need to be rebuilt to ignore normals, as the default normal map is now stylized.

# Version 1.2.1 - [EXPERIMENTAL]
### Fixes
Fixes texture tinting for structure textures on default shaders

# Version 1.2.0 - [EXPERIMENTAL]
### Fixes
Basic mod functionality for RUMBLE 0.5.0.1

This version is experimental! Existing texture skins will not work as structure models have changed, and existing shaders may behave strangely.

# Version 1.1.0
### Additions
Added a warning the first time you enter the Gym with Simple Visuals enabled. This will only appear once, and won't prevent the mod from working (as some shaders do work with simple shading!) Thanks to Orangenal for the idea!

Added persistent settings! This should be the last time your selected skins reset when you update the mod - I've implemented a custom save file system which should hopefully reduce friction on future updates.

# Version 1.0.6
### Fixes
Fixed a bug in texture loading which caused skin application to fail under certain conditions.

Thanks to Orangenal For the report!

# Version 1.0.5
### Fixes
Fixed bug in texture loading which caused the mod's initialization to fail if the user is missing certain skin directories.

Thanks to Huntersgaminggrounds for the report!

# Version 1.0.4
### Additions
Added random texture selection! If you want to use multiple textures, add your textures to a subfolder under the `StructureName`, and the game will automatically select between different skins randomly. If you want to disable this functionality without deleting the skin permanently, just add an Underscore (`_`) to the beginning of the folder name and the game will ignore it.

Example folder structure:
```
RUMBLE
| - UserData
|   | - Skins
|   |   | - StructureName
|   |   |   | - SkinName (can be anything)
|   |   |   |   | - Main.png
|   |   |   |   | - Normal.png
|   |   |   |   | - Mat.png
|   |   |   |   | - Grounded.png
|   |   |   | - SkinName2
|   |   |   |   | - Main.png
|   |   |   |   | - Normal.png
|   |   |   |   | - Mat.png
|   |   |   |   | - Grounded.png
```

If you want to use multiple textures of the same type for a single skin (like if you wanted a playing card which changed the `Main` texture randomly, but kept the same `Normal`, simply add a folder for that type of texture to your `SkinName` folder.
Example:
```
RUMBLE
| - UserData
|   | - Skins
|   |   | - StructureName
|   |   |   | - SkinName (can be anything)
|   |   |   |   | - Main
|   |   |   |   |   | - tex1.png (can be anything)
|   |   |   |   |   | - tex2.png (can be anything)
|   |   |   |   |   | - tex3.png (can be anything)
|   |   |   |   | - Normal.png
|   |   |   |   | - Mat.png
|   |   |   |   | - Grounded.png
```

Also, textures can now be hot-reloaded by holding `F6` while in game. This means you can swap in and out textures from your game files and reload the ones you're using on the fly, without having to restart your game.

# Version 1.0.3
### Additions
Added random shader selection! Enter `random` as the ModUI shader settings to select a random shader from your `/RUMBLE/UserData/Skins/` folder.
Enter `StructureType/random` to select from the shaders in `/RUMBLE/UserData/Skins/StructureType/` folder, allowing for a different random pool per structure.

### Fixes
Fixed issues in material caching which caused balls to revert to default shading after entering a multiplayer scene.

Thanks to Pompyy and Dr.Rock for the bug report!

# Version 1.0.2
### Additions
Added a "Clear Cache" bind (Hold `F6` for 3 seconds) to allow for hot-reload of structure shaders, allowing developers to iterrate on shaders faster without having to restart their games.

### Fixes
Fixed issues in material application logic which caused structures to take the wrong material properties when reloaded using `F5`.

# Version 1.0.1
### Fixes
ModUI shader settings now accept either `myShader` or `myShader.bundle` as valid input
###### Note: `myShader` should be replaced with the name of the bundle you'd like to use.