using AchManager.Windows;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ECommons;
using System.IO;

namespace AchManager;

public sealed class Plugin : IDalamudPlugin
{
  public static ITextureProvider TextureProvider { get; private set; }

  private DalamudPluginInterface PluginInterface { get; init; }
  private ICommandManager CommandManager { get; init; }
  public static Configuration Configuration { get; private set; }

  public readonly WindowSystem WindowSystem = new("AchManager");
  private ConfigWindow ConfigWindow { get; init; }

  public Plugin(
      [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
      [RequiredVersion("1.0")] ICommandManager commandManager,
      [RequiredVersion("1.0")] ITextureProvider textureProvider)
  {
    PluginInterface = pluginInterface;
    CommandManager = commandManager;
    TextureProvider = textureProvider;

    ECommonsMain.Init(pluginInterface, this);

    Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
    Configuration.Initialize(PluginInterface);

    ConfigWindow = new ConfigWindow(WindowSystem);

    WindowSystem.AddWindow(ConfigWindow);

    CommandManager.AddHandler("/ach", new CommandInfo(OnAchCommand)
    {
      HelpMessage = "Open the AchManager UI"
    });

    PluginInterface.UiBuilder.Draw += DrawUI;

    // This adds a button to the plugin installer entry of this plugin which allows
    // to toggle the display status of the configuration ui
    PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
  }

  public void Dispose()
  {
    WindowSystem.RemoveAllWindows();

    CommandManager.RemoveHandler("/ach");
    ECommonsMain.Dispose();
  }

  private void OnAchCommand(string command, string args)
  {
    // in response to the slash command, just toggle the display status of our main ui
    ToggleMainUI();
  }

  private void DrawUI() => WindowSystem.Draw();

  public void ToggleMainUI() => ConfigWindow.Toggle();
}
