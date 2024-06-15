using System;
using System.Collections.Generic;
using System.Linq;
using AchManager.AchievementTrigger;
using AchManager.EventManager;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace AchManager.Windows;

public class ConfigWindow : Window
{
  private readonly WindowSystem _windowSystem;
  private readonly Configuration Configuration = Plugin.Configuration;
  private readonly IEnumerable<Achievement> _allAchievements = Svc.Data.GetExcelSheet<Achievement>()?.Skip(1)?
                                                               .Where(a => !string.IsNullOrEmpty(a.Name) &&
                                                                           (a.AchievementCategory.Value?.AchievementKind.Value?.Name ?? string.Empty) != "Legacy")
                                                               ?? [];
  private IEnumerable<Achievement> _filteredAllAchievements = [];
  private string _allAchievementsSearchText = string.Empty;

  private IEnumerable<WatchedAchievement> _watchedAchievements = [];

  private static readonly string[] _triggerTypeStrings = GetTriggerTypeStrings();

  private static readonly ImGuiTableFlags _tableFlags = ImGuiTableFlags.Resizable | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Hideable
                    | ImGuiTableFlags.Sortable
                    | ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.NoBordersInBody
                    | ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY
                    | ImGuiTableFlags.SizingFixedFit;

  private DefaultConfigWindow? _currentConfigWindow;

  private bool _fullListNeedsSorting = true;
  private bool _watchedListNeedsSorting = true;

  private enum AchievementListColumns
  {
    Name = 0,
    Description = 1,
    Category = 2,
    Watch = 3,
    Progress = 4
  }

  public ConfigWindow(WindowSystem windowSystem)
    : base("AchManager Configuration###With a constant ID")
  {
    _windowSystem = windowSystem;
    _filteredAllAchievements = _allAchievements;
    _watchedAchievements = Configuration.Achievements;

    SizeConstraints = new()
    {
      MinimumSize = new(300, 400),
      MaximumSize = new(float.MaxValue, float.MaxValue)
    };
    Size = new(300, 400);
    SizeCondition = ImGuiCond.FirstUseEver;
  }

  public override void Draw()
  {
    if (ImGui.BeginTabBar("##configTabBar"))
    {
      if (ImGui.BeginTabItem("Achievements"))
      {
        DrawFullAchievementList();
        ImGui.EndTabItem();
      }

      if (ImGui.BeginTabItem("Watched Achievements"))
      {
        DrawWatchedAchievementList();
        ImGui.EndTabItem();
      }
    }
  }

  private void DrawFullAchievementList()
  {
    ImGui.Text("Open your achievements window once to filter out completed achievements.");
    ImGui.Separator();

    ImGui.Text("Search");
    ImGui.SameLine();
    if (ImGui.InputText("##allAchievementsSearchText", ref _allAchievementsSearchText, 128))
    {
      if (string.IsNullOrEmpty(_allAchievementsSearchText))
        _filteredAllAchievements = _allAchievements;
      else
        _filteredAllAchievements = _allAchievements.Where(a => a.Name.RawString.Contains(_allAchievementsSearchText, StringComparison.CurrentCultureIgnoreCase) ||
                                                               a.Description.RawString.Contains(_allAchievementsSearchText, StringComparison.CurrentCultureIgnoreCase));
      _fullListNeedsSorting = true;
    }

    if (ImGui.BeginTable("##allAchievementsTable", 4, _tableFlags))
    {
      ImGui.TableSetupColumn("Ach Name", ImGuiTableColumnFlags.DefaultSort | ImGuiTableColumnFlags.NoHide, 0.0f, (int)AchievementListColumns.Name);
      ImGui.TableSetupColumn("Ach Description", ImGuiTableColumnFlags.DefaultSort | ImGuiTableColumnFlags.WidthStretch, 0.0f, (int)AchievementListColumns.Description);
      ImGui.TableSetupColumn("Ach Category", ImGuiTableColumnFlags.DefaultSort, 0.0f, (int)AchievementListColumns.Category);
      ImGui.TableSetupColumn("Watch", ImGuiTableColumnFlags.DefaultSort | ImGuiTableColumnFlags.NoHide, 0.0f, (int)AchievementListColumns.Watch);
      ImGui.TableSetupScrollFreeze(0, 1);
      ImGui.TableHeadersRow();

      var sortSpecs = ImGui.TableGetSortSpecs();
      if ((sortSpecs.SpecsDirty || _fullListNeedsSorting) && _filteredAllAchievements.Any())
      {
        _filteredAllAchievements = SortAchievementList(_filteredAllAchievements, sortSpecs);
        sortSpecs.SpecsDirty = false;
        _fullListNeedsSorting = false;
      }  

      unsafe
      {
        var achInstance = FFXIVClientStructs.FFXIV.Client.Game.UI.Achievement.Instance();
        foreach (var ach in _filteredAllAchievements)
        {
          if (achInstance->IsComplete((int)ach.RowId))
            continue;

          ImGui.TableNextRow();

          ImGui.TableNextColumn();
          ImGui.Text(ach.Name);

          ImGui.TableNextColumn();
          ImGui.Text(ach.Description);

          ImGui.TableNextColumn();
          ImGui.Text(ach.AchievementCategory.Value?.AchievementKind.Value?.Name ?? "");

          ImGui.TableNextColumn();
          bool watched = Configuration.WatchedAchievements.ContainsKey(ach.RowId);
          if (ImGui.Checkbox($"##ach_{ach.RowId}_watch", ref watched))
          {
            if (watched)
              Configuration.AddWatchedAchievement(ach.RowId);
            else
              Configuration.RemoveWatchedAchievement(ach.RowId);
          }
        }

        ImGui.EndTable();
      }
    }
  }

  private void DrawWatchedAchievementList()
  {
    if (ImGui.BeginTable("##watchedAchievementsTable", 7, _tableFlags))
    {
      ImGui.TableSetupColumn("Ach Name", ImGuiTableColumnFlags.DefaultSort | ImGuiTableColumnFlags.NoHide, 0.0f, (int)AchievementListColumns.Name);
      ImGui.TableSetupColumn("Ach Description", ImGuiTableColumnFlags.DefaultSort | ImGuiTableColumnFlags.WidthStretch, 0.0f, (int)AchievementListColumns.Description);
      ImGui.TableSetupColumn("Ach Category", ImGuiTableColumnFlags.DefaultSort, 0.0f, (int)AchievementListColumns.Category);
      ImGui.TableSetupColumn("Progress", ImGuiTableColumnFlags.DefaultSort, 0.0f, (int)AchievementListColumns.Progress);
      ImGui.TableSetupColumn("Update Trigger", ImGuiTableColumnFlags.NoSort | ImGuiTableColumnFlags.NoHide);
      ImGui.TableSetupColumn("Config", ImGuiTableColumnFlags.NoSort | ImGuiTableColumnFlags.NoHide);
      ImGui.TableSetupColumn("Remove", ImGuiTableColumnFlags.NoSort | ImGuiTableColumnFlags.NoHide);
      ImGui.TableSetupScrollFreeze(0, 1);
      ImGui.TableHeadersRow();

      var sortSpecs = ImGui.TableGetSortSpecs();
      if ((sortSpecs.SpecsDirty || _watchedListNeedsSorting) && Configuration.Achievements.Any())
      {
        _watchedAchievements = SortWatchedAchievementList(Configuration.Achievements, sortSpecs);
        sortSpecs.SpecsDirty = false;
        _watchedListNeedsSorting = false;
      }

      foreach (var ach in _watchedAchievements)
      {
        ImGui.TableNextRow();

        ImGui.TableNextColumn();
        ImGui.Text(ach.AchievementInfo.Name);

        ImGui.TableNextColumn();
        ImGui.Text(ach.AchievementInfo.Description);

        ImGui.TableNextColumn();
        ImGui.Text(ach.AchievementInfo.AchievementCategory.Value?.AchievementKind.Value?.Name ?? "");

        ImGui.TableNextColumn();
        ImGui.Text($"{ach.Progress} / {ach.ProgressMax}");

        ImGui.TableNextColumn();
        int index = Array.IndexOf(_triggerTypeStrings, GetStringForTrigger(ach.Trigger));
        if (ImGui.Combo($"##ach_{ach.WatchedID}_triggerTypeCombo", ref index, _triggerTypeStrings, _triggerTypeStrings.Length))
        {
          Configuration.ChangeTriggerTypeForAchievement(ach.WatchedID, (TriggerType)Enum.Parse(typeof(TriggerType), _triggerTypeStrings[index]));
        }

        ImGui.TableNextColumn();
        if (ach.Trigger != null && ImGui.Button($"Config##ach_{ach.WatchedID}_openConfig"))
        {
          if (_currentConfigWindow != null)
            _windowSystem.RemoveWindow(_currentConfigWindow);

          _currentConfigWindow = GetConfigWindowForTrigger(ach.Trigger, Configuration);
          _windowSystem.AddWindow(_currentConfigWindow);
          _currentConfigWindow.Toggle();
        }

        ImGui.TableNextColumn();
        if (ImGui.Button($"Remove##ach_{ach.WatchedID}_removeWatched"))
        {
          Configuration.RemoveWatchedAchievement(ach.WatchedID);
        }
      }

      ImGui.EndTable();
    }
  }

  private static string GetStringForTrigger(AchievementUpdateTriggerBase? trigger)
  {
    if (trigger == null)
      return TriggerType.None.ToString();
    else if (trigger is DutyCompletedTrigger)
      return TriggerType.DutyCompleted.ToString();
    else if (trigger is FateCompletedTrigger)
      return TriggerType.FateCompleted.ToString();
    else if (trigger is MarkKilledTrigger)
      return TriggerType.MarkKilled.ToString();
    else if (trigger is ChatMessageTrigger)
      return TriggerType.ChatMessage.ToString();
    else
      throw new ArgumentException("unknown trigger type");
  }

  private static string[] GetTriggerTypeStrings()
  {
    var enumValues = Enum.GetValues(typeof(TriggerType));
    var strings = new string[enumValues.Length];
    for (int i = 0; i < enumValues.Length; i++)
      strings[i] = enumValues.GetValue(i)?.ToString() ?? string.Empty;

    return strings;
  }

  private static DefaultConfigWindow GetConfigWindowForTrigger(AchievementUpdateTriggerBase trigger, Configuration pluginConfig)
  {
    if (trigger.Config is MarkKilledTriggerConfig mktc)
      return new MarkKilledTriggerConfigWindow(mktc, pluginConfig, "Mark Killed Trigger Config");
    else if (trigger.Config is ChatMessageTriggerConfig cmtc)
      return new ChatMessageTriggerConfigWindow(cmtc, pluginConfig, "Chat Message Trigger Config");
    else
      return new DefaultConfigWindow(trigger.Config, pluginConfig, $"{trigger.TriggerIdentifier} Config");
  }

  private static IEnumerable<Achievement> SortAchievementList(IEnumerable<Achievement> achievements, ImGuiTableSortSpecsPtr sortSpecs)
  {
    var sortedAchievementList = achievements;

    for (int i = 0; i < sortSpecs.SpecsCount; i++)
    {
      var columnSpecs = sortSpecs.Specs;

      switch ((AchievementListColumns)columnSpecs.ColumnUserID)
      {
        case AchievementListColumns.Name:
          sortedAchievementList = columnSpecs.SortDirection == ImGuiSortDirection.Ascending ? sortedAchievementList.OrderBy(a => a.Name.RawString)
                                                                                            : sortedAchievementList.OrderByDescending(a => a.Name.RawString);
          break;
        case AchievementListColumns.Description:
          sortedAchievementList = columnSpecs.SortDirection == ImGuiSortDirection.Ascending ? sortedAchievementList.OrderBy(a => a.Description.RawString)
                                                                                            : sortedAchievementList.OrderByDescending(a => a.Description.RawString);
          break;
        case AchievementListColumns.Category:
          sortedAchievementList = columnSpecs.SortDirection == ImGuiSortDirection.Ascending ? sortedAchievementList.OrderBy(a => a.AchievementCategory.Value?.AchievementKind.Value?.Name.RawString ?? "")
                                                                                            : sortedAchievementList.OrderByDescending(a => a.AchievementCategory.Value?.AchievementKind.Value?.Name.RawString ?? "");
          break;
        case AchievementListColumns.Watch:
          sortedAchievementList = columnSpecs.SortDirection == ImGuiSortDirection.Ascending ? sortedAchievementList.OrderBy(a => Plugin.Configuration.WatchedAchievements.ContainsKey(a.RowId)) 
                                                                                            : sortedAchievementList.OrderByDescending(a => Plugin.Configuration.WatchedAchievements.ContainsKey(a.RowId));
          break;
        default:
          break;
      }
    }

    return sortedAchievementList;
  }

  private static IEnumerable<WatchedAchievement> SortWatchedAchievementList(IEnumerable<WatchedAchievement> achievements, ImGuiTableSortSpecsPtr sortSpecs)
  {
    var sortedAchievementList = achievements;

    for (int i = 0; i < sortSpecs.SpecsCount; i++)
    {
      var columnSpecs = sortSpecs.Specs;

      switch ((AchievementListColumns)columnSpecs.ColumnUserID)
      {
        case AchievementListColumns.Name:
          sortedAchievementList = columnSpecs.SortDirection == ImGuiSortDirection.Ascending ? sortedAchievementList.OrderBy(a => a.AchievementInfo.Name.RawString)
                                                                                            : sortedAchievementList.OrderByDescending(a => a.AchievementInfo.Name.RawString);
          break;
        case AchievementListColumns.Description:
          sortedAchievementList = columnSpecs.SortDirection == ImGuiSortDirection.Ascending ? sortedAchievementList.OrderBy(a => a.AchievementInfo.Description.RawString)
                                                                                            : sortedAchievementList.OrderByDescending(a => a.AchievementInfo.Description.RawString);
          break;
        case AchievementListColumns.Category:
          sortedAchievementList = columnSpecs.SortDirection == ImGuiSortDirection.Ascending ? sortedAchievementList.OrderBy(a => a.AchievementInfo.AchievementCategory.Value?.AchievementKind.Value?.Name.RawString ?? "")
                                                                                            : sortedAchievementList.OrderByDescending(a => a.AchievementInfo.AchievementCategory.Value?.AchievementKind.Value?.Name.RawString ?? "");
          break;
        case AchievementListColumns.Progress:
          sortedAchievementList = columnSpecs.SortDirection == ImGuiSortDirection.Ascending ? sortedAchievementList.OrderBy(a => a.Progress)
                                                                                            : sortedAchievementList.OrderByDescending(a => a.Progress);
          break;
        default:
          break;
      }
    }

    return sortedAchievementList;
  }
}
