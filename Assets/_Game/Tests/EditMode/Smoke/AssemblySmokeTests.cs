using NUnit.Framework;
using UnityEditor;
using System.IO;

namespace TowerDefense.Game.Tests.EditMode.Smoke
{
public class AssemblySmokeTests
{
    [Test]
    public void GameplayAssemblyDefinition_HasExpectedName()
    {
        const string assetPath = "Assets/_Game/Scripts/TowerDefense.Game.asmdef";
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
        string assemblyDefinition = File.ReadAllText(fullPath);
        var asset = AssetDatabase.LoadMainAssetAtPath(assetPath);

        Assert.That(asset, Is.Not.Null);
        Assert.That(assemblyDefinition, Does.Contain("\"name\": \"TowerDefense.Game\""));
    }
}
}
