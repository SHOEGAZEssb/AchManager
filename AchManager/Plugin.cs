using AchManager.EventManager;
using AchManager.Windows;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ECommons;
using System.IO;

namespace AchManager;

/// <summary>
/// The main dalamud plugin class.
/// </summary>
public sealed class Plugin : IDalamudPlugin
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  /// <summary>
  /// Provides access to game textures.
  /// </summary>
  public static ITextureProvider TextureProvider { get; private set; }

  /// <summary>
  /// The current configuration of the plugin.
  /// </summary>
  public static Configuration Configuration { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

  /// <summary>
  /// ImGui window manager.
  /// </summary>
  public readonly WindowSystem WindowSystem = new("AchManager");

  private IDalamudPluginInterface PluginInterface { get; init; }
  private ICommandManager CommandManager { get; init; }

  private ConfigWindow ConfigWindow { get; init; }

  /// <summary>
  /// Constructor.
  /// </summary>
  /// <param name="pluginInterface">Interface object between dalamud and the game.</param>
  /// <param name="commandManager">Object for managing slash commands.</param>
  /// <param name="textureProvider">Object for providing access to game textures.</param>
  public Plugin(IDalamudPluginInterface pluginInterface,
                ICommandManager commandManager,
                ITextureProvider textureProvider)
  {
    PluginInterface = pluginInterface;
    CommandManager = commandManager;
    TextureProvider = textureProvider;

    ECommonsMain.Init(pluginInterface, this);

    Configuration = Configuration.Load(Path.Combine(PluginInterface!.ConfigDirectory.FullName, "AchManager.json")) ?? new Configuration();
    Configuration.Initialize(PluginInterface);

    ConfigWindow = new ConfigWindow(WindowSystem);

    WindowSystem.AddWindow(ConfigWindow);

    CommandManager.AddHandler("/ach", new CommandInfo(OnAchCommand)
    {
      HelpMessage = "Open the AchManager UI"
    });

    CommandManager.AddHandler("/achfetch", new CommandInfo(OnAchFetchCommand)
    {
      HelpMessage = "Manually fetches the progress of all watched achievements"
    });

    PluginInterface.UiBuilder.Draw += DrawUI;
    PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
    PluginInterface.UiBuilder.OpenConfigUi += ToggleMainUI;
  }

  /// <summary>
  /// Disposes the plugin.
  /// </summary>
  public void Dispose()
  {
    WindowSystem.RemoveAllWindows();
    CommandManager.RemoveHandler("/ach");
    CommandManager.RemoveHandler("/achfetch");
    DisposeEventManagers();
    ECommonsMain.Dispose();
  }

  /// <summary>
  /// Disposes all event managers.
  /// </summary>
  private static void DisposeEventManagers()
  {
    ChatMessageEventManager.Instance.Dispose();
    DutyCompletedEventManager.Instance.Dispose();
    FateCompletedEventManager.Instance.Dispose();
    MarkKilledEventManager.Instance.Dispose();
    QuestCompletedEventManager.Instance.Dispose();
  }

  private void OnAchCommand(string command, string args)
  {
    // in response to the slash command, just toggle the display status of our main ui
    ToggleMainUI();
  }

  private void OnAchFetchCommand(string command, string args)
  {
    Configuration.FetchProgress();
  }

  private void DrawUI() => WindowSystem.Draw();

  /// <summary>
  /// Shows the main UI.
  /// </summary>
  public void ToggleMainUI() => ConfigWindow.Toggle();
}
