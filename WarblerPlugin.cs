using System.Runtime.CompilerServices;
using NPlug;
namespace Warbler;
public static class SimpleDelayPlugin
{
    public static AudioPluginFactory GetFactory()
    {
        var factory = new AudioPluginFactory(new("Warbler", "https://github.com/xoofx/NPlug", "no_reply@nplug.org"));
        factory.RegisterPlugin<WarblerProcessor>(new(WarblerProcessor.ClassId, "Warbler", AudioProcessorCategory.Effect));
        factory.RegisterPlugin<WarblerController>(new(WarblerController.ClassId, "Warbler Controller"));
        return factory;
    }

    [ModuleInitializer]
    internal static void ExportThisPlugin()
    {
        AudioPluginFactoryExporter.Instance = GetFactory();
    }
}
