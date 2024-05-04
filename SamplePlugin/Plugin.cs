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
  private const string CommandName = "/pmycommand";

  private DalamudPluginInterface PluginInterface { get; init; }
  private ICommandManager CommandManager { get; init; }
  public Configuration Configuration { get; init; }

  public readonly WindowSystem WindowSystem = new("AchManager");
  private ConfigWindow ConfigWindow { get; init; }
  private MainWindow MainWindow { get; init; }

  public Plugin(
      [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
      [RequiredVersion("1.0")] ICommandManager commandManager,
      [RequiredVersion("1.0")] ITextureProvider textureProvider)
  {
    PluginInterface = pluginInterface;
    CommandManager = commandManager;

    ECommonsMain.Init(pluginInterface, this);

    Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
    Configuration.Initialize(PluginInterface);

    // you might normally want to embed resources and load them from the manifest stream
    var file = new FileInfo(Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png"));

    // ITextureProvider takes care of the image caching and dispose
    var goatImage = textureProvider.GetTextureFromFile(file);

    ConfigWindow = new ConfigWindow(this);
    MainWindow = new MainWindow(this, goatImage);

    WindowSystem.AddWindow(ConfigWindow);
    WindowSystem.AddWindow(MainWindow);

    CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
    {
      HelpMessage = "A useful message to display in /xlhelp"
    });

    PluginInterface.UiBuilder.Draw += DrawUI;

    // This adds a button to the plugin installer entry of this plugin which allows
    // to toggle the display status of the configuration ui
    PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

    // Adds another button that is doing the same but for the main ui of the plugin
    PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
  }

  public void Dispose()
  {
    WindowSystem.RemoveAllWindows();

    CommandManager.RemoveHandler(CommandName);
    ECommonsMain.Dispose();
  }

  private void OnCommand(string command, string args)
  {
    // in response to the slash command, just toggle the display status of our main ui
    ToggleMainUI();
  }

  private void DrawUI() => WindowSystem.Draw();

  public void ToggleConfigUI() => ConfigWindow.Toggle();
  public void ToggleMainUI() => MainWindow.Toggle();
}
