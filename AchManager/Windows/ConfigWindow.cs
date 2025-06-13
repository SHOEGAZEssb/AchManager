using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AchManager.AchievementTrigger;
using AchManager.EventManager;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using ImGuiNET;
using Lumina.Excel.Sheets;

namespace AchManager.Windows;

public class ConfigWindow : Window
{
  private readonly IEnumerable<Achievement> _allAchievements = Svc.Data.GetExcelSheet<Achievement>()?.Skip(1)?
                                                               .Where(a => !string.IsNullOrEmpty(a.Name.ToString()) &&
                                                                           a.AchievementCategory.Value.AchievementKind.Value.Name != "Legacy")
                                                               ?? [];
  private IEnumerable<Achievement> _filteredAllAchievements = [];
  private string _allAchievementsSearchText = string.Empty;

  private IEnumerable<WatchedAchievement> _filteredWatchedAchievements = [];
  private string _watchedAchievementsSearchText = string.Empty;

  private static readonly string[] _triggerTypeStrings = GetTriggerTypeStrings();

  private static readonly ImGuiTableFlags _tableFlags = ImGuiTableFlags.Resizable | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Hideable
                    | ImGuiTableFlags.Sortable
                    | ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.NoBordersInBody
                    | ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY
                    | ImGuiTableFlags.SizingFixedFit;

  private static readonly ImGuiTableFlags _sessionStatsTableFlags = ImGuiTableFlags.Resizable
                    | ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.NoBordersInBody
                    | ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY
                    | ImGuiTableFlags.SizingFixedFit;

  private DefaultTriggerConfigWindow? _currentConfigWindow;

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

  public ConfigWindow()
    : base("AchManager Configuration###With a constant ID")
  {
    _filteredAllAchievements = _allAchievements;
    _filteredWatchedAchievements = Plugin.Configuration.Achievements;

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

      if (ImGui.BeginTabItem("Session Stats"))
      {
        DrawSessionStats();
        ImGui.EndTabItem();
      }

      if (ImGui.BeginTabItem("Settings"))
      {
        DrawSettings();
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
        _filteredAllAchievements = _allAchievements.Where(a => a.Name.ToString().Contains(_allAchievementsSearchText, StringComparison.CurrentCultureIgnoreCase) ||
                                                               a.Description.ToString().Contains(_allAchievementsSearchText, StringComparison.CurrentCultureIgnoreCase));
      _fullListNeedsSorting = true;
    }

    if (ImGui.BeginTable("##allAchievementsTable", 4, _tableFlags))
    {
      ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.DefaultSort | ImGuiTableColumnFlags.NoHide, 0.0f, (int)AchievementListColumns.Name);
      ImGui.TableSetupColumn("Description", ImGuiTableColumnFlags.DefaultSort | ImGuiTableColumnFlags.WidthStretch, 0.0f, (int)AchievementListColumns.Description);
      ImGui.TableSetupColumn("Category", ImGuiTableColumnFlags.DefaultSort, 0.0f, (int)AchievementListColumns.Category);
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
          ImGui.Text(ach.Name.ToString());

          ImGui.TableNextColumn();
          ImGui.Text(ach.Description.ToString());

          ImGui.TableNextColumn();
          ImGui.Text(ach.AchievementCategory.Value.AchievementKind.Value.Name.ToString());

          ImGui.TableNextColumn();
          bool watched = Plugin.Configuration.WatchedAchievements.ContainsKey(ach.RowId);
          if (ImGui.Checkbox($"##ach_{ach.RowId}_watch", ref watched))
          {
            if (watched)
              Plugin.Configuration.AddWatchedAchievement(ach.RowId);
            else
              Plugin.Configuration.RemoveWatchedAchievement(ach.RowId);
          }
        }

        ImGui.EndTable();
      }
    }
  }

  private void DrawWatchedAchievementList()
  {
    ImGui.Text("Search");
    ImGui.SameLine();
    if (ImGui.InputText("##watchedAchievementsSearchText", ref _watchedAchievementsSearchText, 128))
    {
      if (string.IsNullOrEmpty(_watchedAchievementsSearchText))
        _filteredWatchedAchievements = Plugin.Configuration.Achievements;
      else
        _filteredWatchedAchievements = Plugin.Configuration.Achievements.Where(a => a.AchievementInfo.Name.ToString().Contains(_watchedAchievementsSearchText, StringComparison.CurrentCultureIgnoreCase) ||
                                                               a.AchievementInfo.Description.ToString().Contains(_watchedAchievementsSearchText, StringComparison.CurrentCultureIgnoreCase));
      _watchedListNeedsSorting = true;
    }

    ImGui.Separator();

    if (ImGui.BeginTable("##watchedAchievementsTable", 7, _tableFlags))
    {
      ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.DefaultSort | ImGuiTableColumnFlags.NoHide, 0.0f, (int)AchievementListColumns.Name);
      ImGui.TableSetupColumn("Description", ImGuiTableColumnFlags.DefaultSort | ImGuiTableColumnFlags.WidthStretch, 0.0f, (int)AchievementListColumns.Description);
      ImGui.TableSetupColumn("Category", ImGuiTableColumnFlags.DefaultSort, 0.0f, (int)AchievementListColumns.Category);
      ImGui.TableSetupColumn("Progress", ImGuiTableColumnFlags.DefaultSort, 0.0f, (int)AchievementListColumns.Progress);
      ImGui.TableSetupColumn("Update Trigger", ImGuiTableColumnFlags.NoSort | ImGuiTableColumnFlags.NoHide);
      ImGui.TableSetupColumn("Config", ImGuiTableColumnFlags.NoSort | ImGuiTableColumnFlags.NoHide);
      ImGui.TableSetupColumn("Remove", ImGuiTableColumnFlags.NoSort | ImGuiTableColumnFlags.NoHide);
      ImGui.TableSetupScrollFreeze(0, 1);
      ImGui.TableHeadersRow();

      var sortSpecs = ImGui.TableGetSortSpecs();
      if ((sortSpecs.SpecsDirty || _watchedListNeedsSorting) && Plugin.Configuration.Achievements.Any())
      {
        _filteredWatchedAchievements = SortWatchedAchievementList(_filteredWatchedAchievements, sortSpecs);
        sortSpecs.SpecsDirty = false;
        _watchedListNeedsSorting = false;
      }

      foreach (var ach in _filteredWatchedAchievements)
      {
        ImGui.TableNextRow();

        ImGui.TableNextColumn();
        ImGui.Text(ach.AchievementInfo.Name.ToString());

        ImGui.TableNextColumn();
        ImGui.Text(ach.AchievementInfo.Description.ToString());

        ImGui.TableNextColumn();
        ImGui.Text(ach.AchievementInfo.AchievementCategory.Value.AchievementKind.Value.Name.ToString());

        ImGui.TableNextColumn();
        ImGui.Text($"{ach.Progress} / {ach.ProgressMax}");

        ImGui.TableNextColumn();
        int index = Array.IndexOf(_triggerTypeStrings, GetStringForTrigger(ach.Trigger));
        if (ImGui.Combo($"##ach_{ach.WatchedID}_triggerTypeCombo", ref index, _triggerTypeStrings, _triggerTypeStrings.Length))
          Plugin.Configuration.ChangeTriggerTypeForAchievement(ach.WatchedID, Enum.Parse<TriggerType>(_triggerTypeStrings[index]));

        ImGui.TableNextColumn();
        if (ach.Trigger != null && ImGui.Button($"Config##ach_{ach.WatchedID}_openConfig"))
        {
          if (_currentConfigWindow != null)
            Plugin.WindowSystem.RemoveWindow(_currentConfigWindow);

          _currentConfigWindow = GetConfigWindowForTrigger(ach.Trigger);
          Plugin.WindowSystem.AddWindow(_currentConfigWindow);
          _currentConfigWindow.Toggle();
        }

        ImGui.TableNextColumn();
        if (ImGui.Button($"Remove##ach_{ach.WatchedID}_removeWatched"))
          Plugin.Configuration.RemoveWatchedAchievement(ach.WatchedID);
      }

      ImGui.EndTable();
    }
  }

  private void DrawSessionStats()
  {
    if (Configuration.SessionStats.Count == 0)
    {
      string text = "No progress so far :(";
      Vector2 contentSize = ImGui.GetContentRegionAvail();
      Vector2 textSize = ImGui.CalcTextSize(text);

      // Center X and Y
      float textX = (contentSize.X - textSize.X) * 0.5f;
      float textY = (contentSize.Y - textSize.Y) * 0.5f;

      // Move cursor to centered position
      ImGui.SetCursorPos(new Vector2(textX, textY));
      ImGui.Text(text);
    }
    else
    {
      var text = "Here's your progress today! Keep up the great work!!";
      Vector2 contentSize = ImGui.GetContentRegionAvail();
      Vector2 textSize = ImGui.CalcTextSize(text);
      float textX = (contentSize.X - textSize.X) * 0.5f;
      ImGui.SetCursorPosX(textX);
      ImGui.Text(text);

      if (ImGui.BeginTable("##sessionStatsTable", 3, _sessionStatsTableFlags))
      {
        ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.DefaultSort | ImGuiTableColumnFlags.NoHide, 0.0f, (int)AchievementListColumns.Name);
        ImGui.TableSetupColumn("Description", ImGuiTableColumnFlags.DefaultSort | ImGuiTableColumnFlags.WidthStretch, 0.0f, (int)AchievementListColumns.Description);
        ImGui.TableSetupColumn("Progress", ImGuiTableColumnFlags.DefaultSort, 0.0f, (int)AchievementListColumns.Progress);
        ImGui.TableSetupScrollFreeze(0, 1);
        ImGui.TableHeadersRow();

        foreach (var ach in Configuration.SessionStats)
        {
          var achInfo = _allAchievements.First(a => a.RowId == ach.Key);

          ImGui.TableNextRow();

          ImGui.TableNextColumn();
          ImGui.Text(achInfo.Name.ToString());

          ImGui.TableNextColumn();
          ImGui.Text(achInfo.Description.ToString());

          ImGui.TableNextColumn();
          ImGui.Text($"{ach.Value.initial} -> {ach.Value.current} / {ach.Value.max} ");
          ImGui.SameLine();
          var progress = ach.Value.current - ach.Value.initial;
          double percentage = progress * 100.0 / ach.Value.max;
          ImGui.TextColored(new Vector4(0.6f, 1.0f, 0.6f, 1.0f), $"( +{progress} / +{percentage:F2}%% )");
        }

        ImGui.EndTable();
      }
    }
  }

  private static void DrawSettings()
  {
    bool preventLogSpam = Plugin.Configuration.PreventChatEventManagerLogSpam;
    if (ImGui.Checkbox("Prevent ChatEventManager Log Spam", ref preventLogSpam))
    {
      Plugin.Configuration.PreventChatEventManagerLogSpam = preventLogSpam;
      Plugin.Configuration.Save();
    }
    if (ImGui.IsItemHovered())
    {
      ImGui.BeginTooltip();
      ImGui.SetTooltip("If checked, the ChatEventManager will not print log messages to the Dalamud log");
      ImGui.EndTooltip();
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
    else if (trigger is QuestCompletedTrigger)
      return TriggerType.QuestCompleded.ToString();
    else if (trigger is BannerShownTrigger)
      return TriggerType.BannerShown.ToString();
    else
      throw new ArgumentException("unknown trigger type");
  }

  private static string[] GetTriggerTypeStrings()
  {
    var enumValues = Enum.GetValues<TriggerType>();
    var strings = new string[enumValues.Length];
    for (int i = 0; i < enumValues.Length; i++)
      strings[i] = enumValues.GetValue(i)?.ToString() ?? string.Empty;

    return strings;
  }

  private static DefaultTriggerConfigWindow GetConfigWindowForTrigger(AchievementUpdateTriggerBase trigger)
  {
    if (trigger.Config is MarkKilledTriggerConfig mktc)
      return new MarkKilledTriggerConfigWindow(mktc, "Mark Killed Trigger Config");
    else if (trigger.Config is DutyCompletedTriggerConfig dctc)
      return new DutyCompletedTriggerConfigWindow(dctc, "Duty Completed Trigger Config");
    else if (trigger.Config is ChatMessageTriggerConfig cmtc)
      return new ChatMessageTriggerConfigWindow(cmtc, "Chat Message Trigger Config");
    else if (trigger.Config is QuestCompletedTriggerConfig qctc)
      return new QuestCompletedTriggerConfigWindow(qctc, "Quest Completed Trigger Config");
    else if (trigger.Config is BannerShownTriggerConfig bstc)
      return new BannerShownTriggerConfigWindow(bstc, "Banner Shown Trigger Config");
    else
      return new DefaultTriggerConfigWindow(trigger.Config, $"{trigger.TriggerIdentifier} Config");
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
          sortedAchievementList = columnSpecs.SortDirection == ImGuiSortDirection.Ascending ? sortedAchievementList.OrderBy(a => a.Name.ToString())
                                                                                            : sortedAchievementList.OrderByDescending(a => a.Name.ToString());
          break;
        case AchievementListColumns.Description:
          sortedAchievementList = columnSpecs.SortDirection == ImGuiSortDirection.Ascending ? sortedAchievementList.OrderBy(a => a.Description.ToString())
                                                                                            : sortedAchievementList.OrderByDescending(a => a.Description.ToString());
          break;
        case AchievementListColumns.Category:
          sortedAchievementList = columnSpecs.SortDirection == ImGuiSortDirection.Ascending ? sortedAchievementList.OrderBy(a => a.AchievementCategory.Value.AchievementKind.Value.Name.ToString())
                                                                                            : sortedAchievementList.OrderByDescending(a => a.AchievementCategory.Value.AchievementKind.Value.Name.ToString());
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
          sortedAchievementList = columnSpecs.SortDirection == ImGuiSortDirection.Ascending ? sortedAchievementList.OrderBy(a => a.AchievementInfo.Name.ToString())
                                                                                            : sortedAchievementList.OrderByDescending(a => a.AchievementInfo.Name.ToString());
          break;
        case AchievementListColumns.Description:
          sortedAchievementList = columnSpecs.SortDirection == ImGuiSortDirection.Ascending ? sortedAchievementList.OrderBy(a => a.AchievementInfo.Description.ToString())
                                                                                            : sortedAchievementList.OrderByDescending(a => a.AchievementInfo.Description.ToString());
          break;
        case AchievementListColumns.Category:
          sortedAchievementList = columnSpecs.SortDirection == ImGuiSortDirection.Ascending ? sortedAchievementList.OrderBy(a => a.AchievementInfo.AchievementCategory.ToString())
                                                                                            : sortedAchievementList.OrderByDescending(a => a.AchievementInfo.AchievementCategory.ToString());
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
