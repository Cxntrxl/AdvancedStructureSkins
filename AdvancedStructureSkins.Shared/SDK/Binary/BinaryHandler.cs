using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.Shared.SDK.Binary
{
    public class BinaryHandler
{
    public static byte[] Write(SkinManifest manifest)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write(1);
                writer.Write(string.IsNullOrEmpty(manifest.skinName) ? manifest.name : manifest.skinName);
                
                writer.Write(manifest.previewTexture?.name ?? "");
                writer.Write(manifest.allowedInComp);
                
                writer.Write(manifest.shaders?.Count ?? 0);
                if (manifest.shaders != null)
                    foreach (var m in manifest.shaders)
                        WriteShaderManifest(writer, m);

                writer.Write(manifest.textures?.Count ?? 0);
                if (manifest.textures != null)
                    foreach (var t in manifest.textures)
                        WriteTextureSet(writer, t);

                return ms.ToArray();
            }
        }
    }

    public static void WriteShaderManifest(BinaryWriter writer, ShaderManifest manifest)
    {
        writer.Write(manifest.material?.name ?? "");
        
        writer.Write(manifest.overrides?.Count ?? 0);
        if (manifest.overrides != null)
            foreach (var o in manifest.overrides)
                WriteOverride(writer, o);
    }

    private static void WriteOverride(BinaryWriter writer, MaterialPropertyOverride o)
    {
        writer.Write(o.propertyName ?? "");
        writer.Write((byte)o.propertyType);
        
        writer.Write(o.colorValue.r);
        writer.Write(o.colorValue.g);
        writer.Write(o.colorValue.b);
        writer.Write(o.colorValue.a);
        
        writer.Write(o.floatValue);
        writer.Write(o.intValue);
        
        writer.Write(o.vectorValue.x);
        writer.Write(o.vectorValue.y);
        writer.Write(o.vectorValue.z);
        writer.Write(o.vectorValue.w);

        writer.Write(o.textureValue == null ? string.Empty : o.textureValue.name);
        
        writer.Write((byte)o.targetStructures);
    }

    private static void WriteTextureSet(BinaryWriter writer, TextureSetManifest t)
    {
        writer.Write(t.name);
        writer.Write(t.previewTexture != null ? t.previewTexture.name : "");
        writer.Write(t.textures?.Count ?? 0);
        
        if (t.textures != null)
            foreach (TextureEntry e in t.textures) WriteTextureEntry(writer, e);
        
        writer.Write((byte)t.targets);
    }

    private static void WriteTextureEntry(BinaryWriter writer, TextureEntry e)
    {
        writer.Write((byte)e.type);
        writer.Write(e.textures?.Count ?? 0);
        
        if (e.textures != null)
            foreach (Texture t in e.textures)  writer.Write(t != null ? t.name : string.Empty);
    }

    public static SkinManifestBinary Read(byte[] bytes)
    {
        using (MemoryStream ms = new MemoryStream(bytes))
        {
            using (BinaryReader reader = new BinaryReader(ms))
            {
                switch (reader.ReadInt32())
                {
                    case 1: return ReadV1Manifest(reader);
                }
            }
        }
        
        return null;
    }

    private static SkinManifestBinary ReadV1Manifest(BinaryReader reader)
    {
        SkinManifestBinary manifest = new SkinManifestBinary();
        
        manifest.skinName = reader.ReadString();
        manifest.previewTextureName = reader.ReadString();
        manifest.allowedInComp = reader.ReadBoolean();
                
        int shaderCount = reader.ReadInt32();
        manifest.shaders = new ShaderManifestBinary[shaderCount];
        for (int i = 0; i < shaderCount; i++) manifest.shaders[i] = ReadShaderManifest(reader);

        int textureCount = reader.ReadInt32();
        manifest.textures = new TextureSetBinary[textureCount];

        for (int i = 0; i < textureCount; i++) manifest.textures[i] = ReadTextureSet(reader);

        return manifest;
    }

    private static ShaderManifestBinary ReadShaderManifest(BinaryReader reader)
    {
        ShaderManifestBinary manifest = new ShaderManifestBinary();
        manifest.materialName = reader.ReadString();
        
        int overridesCount = reader.ReadInt32();
        manifest.overrides = new MaterialOverrideBinary[overridesCount];
        
        for (int i = 0; i < overridesCount; i++) manifest.overrides[i] = ReadOverride(reader);

        return manifest;
    }

    private static MaterialOverrideBinary ReadOverride(BinaryReader reader)
    {
        MaterialOverrideBinary o = new MaterialOverrideBinary();
        
        o.propertyName = reader.ReadString();
        o.type = (ShaderPropertyType)reader.ReadByte();

        float r = reader.ReadSingle();
        float g = reader.ReadSingle();
        float b = reader.ReadSingle();
        float a = reader.ReadSingle();
        
        o.colorValue = new Color(r, g, b, a);

        o.floatValue = reader.ReadSingle();
        o.intValue = reader.ReadInt32();
        
        float x = reader.ReadSingle();
        float y = reader.ReadSingle();
        float z = reader.ReadSingle();
        float w = reader.ReadSingle();
        
        o.vectorValue = new Vector4(x, y, z, w);

        o.textureValue = reader.ReadString();
        
        o.targetStructures = (StructureFlags)reader.ReadByte();

        return o;
    }

    private static TextureSetBinary ReadTextureSet(BinaryReader reader)
    {
        TextureSetBinary t = new TextureSetBinary();

        t.name = reader.ReadString();
        t.previewTextureName = reader.ReadString();
        int textureCount = reader.ReadInt32();

        t.textures = new TextureEntryBinary[textureCount];

        for (int i = 0; i < textureCount; i++)
            t.textures[i] = ReadTextureEntry(reader);

        t.targets = (StructureFlags)reader.ReadByte();

        return t;
    }

    private static TextureEntryBinary ReadTextureEntry(BinaryReader reader)
    {
        TextureEntryBinary e = new TextureEntryBinary();

        e.type = (TextureType)reader.ReadByte();
        int textureCount = reader.ReadInt32();

        e.textures = new string[textureCount];

        for (int i = 0; i < textureCount; i++)
            e.textures[i] = reader.ReadString();

        return e;
    }
}
}

