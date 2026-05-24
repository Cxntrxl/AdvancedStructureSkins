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
                writer.Write(manifest.material?.name ?? "");
                writer.Write(manifest.previewTexture?.name ?? "");
                writer.Write(manifest.allowedInComp);
                writer.Write(manifest.overrides?.Count ?? 0);

                if (manifest.overrides != null)
                    foreach (var o in manifest.overrides)
                        WriteOverride(writer, o);

                writer.Write(manifest.textures?.Count ?? 0);

                if (manifest.textures != null)
                    foreach (var t in manifest.textures)
                        WriteTextureSet(writer, t);

                return ms.ToArray();
            }
        }
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
        SkinManifestBinary manifest = new SkinManifestBinary();

        using (MemoryStream ms = new MemoryStream(bytes))
        {
            using (BinaryReader reader = new BinaryReader(ms))
            {
                manifest.version = reader.ReadInt32();
                manifest.skinName = reader.ReadString();
                manifest.materialName = reader.ReadString();
                manifest.previewTextureName = reader.ReadString();
                manifest.allowedInComp = reader.ReadBoolean();

                int overrideCount = reader.ReadInt32();
                manifest.overrides = new MaterialOverrideBinary[overrideCount];

                for (int i = 0; i < overrideCount; i++) manifest.overrides[i] = ReadOverride(reader);

                int textureCount = reader.ReadInt32();
                manifest.textures = new TextureSetBinary[textureCount];

                for (int i = 0; i < textureCount; i++) manifest.textures[i] = ReadTextureSet(reader);

                return manifest;
            }
        }
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

