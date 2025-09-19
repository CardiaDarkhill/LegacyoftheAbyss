using System.Reflection;
using System.Runtime.Versioning;
using LegacyoftheAbyss;

[assembly: AssemblyCompany("LegacyoftheAbyss")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyProduct("LegacyoftheAbyss")]
[assembly: AssemblyTitle("LegacyoftheAbyss")]
[assembly: AssemblyInformationalVersion(PluginInfo.PLUGIN_VERSION)]
[assembly: AssemblyFileVersion("0.1.0.0")]
[assembly: AssemblyVersion("0.1.0.0")]
[assembly: TargetFramework(".NETStandard,Version=v2.1", FrameworkDisplayName = ".NET Standard 2.1")]
