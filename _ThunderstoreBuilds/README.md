# Advanced Structure Skins
~~Compatible with skins created for ButteredLilly's Skin Manager.~~

No longer compatible with existing texture skins as of RUMBLE v0.5 as structure meshes have changed.

Advanced Structure Skins allows users to create custom shaders for structures, providing creators with extreme flexibility, albeit also introducing additional complexity and potential incompatibilities.

This mod *may* remove game functionality, including but not limited to visible grounded effects, grounded vertex offsets and on-collision structure shaking, depending which shaders are used (as these features are implemented in the shader and must be rebuilt by skin creators.)

You can watch a video tutorial for User setup and for getting started as a developer here:
https://youtu.be/igGSTZQgDr0

# For Users
## Retexturing Structures
Skins are super easy to import and use! Here's the folder structure you'll want to create:
```
RUMBLE
| - UserData
|   | - Skins
|   |   | - StructureName
|   |   |   | - Mat.png
|   |   |   | - Main.png
|   |   |   | - Normal.png
|   |   |   | - Grounded.png
```

Valid structure names are `Disc`, `Pillar`, `Ball`, `Cube`, `Wall`, `SmallRock` and `LargeRock`.

If you want to use multiple textures, add your textures to a subfolder under the `StructureName`, and the game will automatically select between different skins randomly. If you want to disable this functionality without deleting the skin permanently, just add an Underscore (`_`) to the beginning of the folder name and the game will ignore it.

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

You can combine this with the previous example too, swapping between multiple skins with multiple textures.

## Using Custom Shaders
Shaders are created by other players and are available in the Rumble Modding Discord. To select the shader you want to use, load into the game and press F10 to view the game UI. From the Mods dropdown, select Advanced Structure Skins to access the following Mod settings:

`Use Global Skin`: When enabled, all structures will use the same shader (designated by Global Skin Path)

`Global Skin Path`: The path to the shader used by all structures when Use Global Skin is enabled. `myShader` will select the shader located at `UserData\Skins\myShader.bundle`.

`Disc Skin path`: The path to the shader used by discs when Use Global Skin is disabled. `Disc/myShader` will select the shader located at `UserData\Skins\Disc\myShader.bundle`.

`Pillar Skin path`: The path to the shader used by pillars when Use Global Skin is disabled. `Pillar/myShader` will select the shader located at `UserData\Skins\Pillar\myShader.bundle`.

`Ball Skin path`: The path to the shader used by balls when Use Global Skin is disabled. `Ball/myShader` will select the shader located at `UserData\Skins\Ball\myShader.bundle`.

`Cube Skin path`: The path to the shader used by cubes when Use Global Skin is disabled. `Cube/myShader` will select the shader located at `UserData\Skins\Cube\myShader.bundle`.

`Wall Skin path`: The path to the shader used by walls when Use Global Skin is disabled. `Wall/myShader` will select the shader located at `UserData\Skins\Wall\myShader.bundle`.

`Small Rock Skin path`: The path to the shader used by the small rocks found in the gym when Use Global Skin is disabled. `SmallRock/myShader` will select the shader located at `UserData\Skins\SmallRock\myShader.bundle`.

`Boulder Skin path`: The path to the shader used by boulders when Use Global Skin is disabled. `LargeRock/myShader` will select the shader located at `UserData\Skins\LargeRock\myShader.bundle`.

Once you've got your shaders selected in-game, they'll be applied to any newly spawned structure. To reload shaders for all structures, hit F5 on your keyboard. It's worth noting that shader files are cached to help with performance reloading materials for many structures, so they are unfortunately not hot-swappable; this means you'll need to restart your game if you make changes to a shader you've already loaded.

However, shaders you have not loaded yet can be loaded freshly at any time without a restart.

To reset a structure to the default game shader, you can simply set the respective `Skin Path` to `default`. This can be done either globally or per-structure. As such, you cannot name a structure skin `default.bundle`, as it will be replaced by the default shaders.

To select a random shader from your `UserData\Skins\` folder, set the `Skin Path` to `random`. This also supports subdirectories, so entering `Disc\random` will select shaders in your `UserData\Skins\Disc\` folder.

# For Developers
First of all, I've exposed the CustomShaders API in the mod to make it super easy and fast to get the shaders from existing bundles located in the users' Skins folder. At the moment, the [SDK](https://github.com/Cxntrxl/AdvancedStructureSkinsSDK) only supports overrides targeting structures, but in the future I can probably implement a system for overrides based on GameObject names.

Second of all, the meat and potatoes - How do you make your own shaders?

There's a bunch of useful tools and example shaders in the [SDK available on github](https://github.com/Cxntrxl/AdvancedStructureSkinsSDK). The most important of which is the **Shader Asset Bundle Builder**, which you can access via the Tools tab at the top of the window. It should be relatively straightforward to use if you already know your way around unity.

Next, creating shaders. To make them compatible with the game, you'll need to ensure some of your property names match the game's existing shaders. The most notable of which are:

`Texture2D_3812B1EC`: Albedo, Base Colour, whatever - this is your colour texture.

`Texture2D_2058E65A`: Normal map. NOTE: Using the default normal map causes stylized shading to appear on your structures. Removing this property will fix that!

`Texture2D_8F187FEF`: Mat.png - in most skins, green is used to add glossiness and white is used for metallic. This no longer exists on default shaders, but the mod still supports the Mat.png file for custom shaders!

`_FloorHeight`: The height (in worldspace) that the floor is at the last time a structure was grounded. Do note, this does **not** reset when a structure is ungrounded, so it must be used in tandem with `_Stable`.

`_Stable`: 0 for ungrounded, 1 for grounded.

`_shake`: 0 for steady, 1 for shaking.

`_shakeAmount`, `_shakeFrequency`: Somewhat self-explanatory, but these values determine how much the verts of the object are offset over time to cause the shake effect on collision with another structure. Expected values are `0.0075` and `75` respectfully.

The grounded effect is probably the most important part to implement in a shader, since it has some pretty solid gameplay consequences, so I've included a couple helper functions to make life easier when implementing it.

You can include these functions in your HLSL shaders by adding `#include "Assets/ExampleShaders/Shaders/AdditionalFunctions/GroundMask.hlsl"` to your includes, or in your Shader Graphs by adding a `Custom Function` node and selecting the `GroundMask.hlsl` file from the AdditionalFunctions folder. Your `Custom Function` node will require a `Vector3` input taking in the Worldspace Position, 3 `float` inputs taking in the `_FloorHeight`, `_Stable` and a `Floor Height Offset`, as well as 1 `float` output, returning white for any surface above the floor. You can name the function either `GroundMask` or `GroundMaskGradient` depending on which format suits your needs.

For vert offsets (such as shaking during hitstop or merging with the ground when grounded), you can implement the `StructureVertOffsets(float3 worldPos, float floorHeight, float stable, float offset, float floorBlendAmount, float shake, float shakeAmount, float shakeFrequency)` function by adding `#include "Assets/ExampleShaders/Shaders/AdditionalFunctions/StructureVertOffsets.hlsl"` to your includes, or in your Shader Graphs by adding a `Custom Function` node and selecting the `StructureVertOffsets.hlsl` file from the AdditionalFunctions folder. An example of implementation of this function is available in the Shaders folder.

There's also a couple other simple shader examples in the [SDK](https://github.com/Cxntrxl/AdvancedStructureSkinsSDK) project which should help give some idea on how other shader properties are implemented - it's up to you how you use the tools provided.