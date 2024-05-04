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

public class ConfigWindow(Plugin plugin) : Window("AchManager Configuration###With a constant ID")
{
  private readonly Configuration Configuration = plugin.Configuration;
  private readonly IEnumerable<Achievement> _allAchievements = Svc.Data.GetExcelSheet<Achievement>()?.Skip(1) ?? [];
  private IEnumerable<Achievement> _filteredAllAchievements = [];
  private string _allAchievementsSearchText = string.Empty;

  private static readonly string[] _triggerTypeStrings = GetTriggerTypeStrings();

  private static readonly ImGuiTableFlags _tableFlags = ImGuiTableFlags.Resizable | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Hideable
                    | ImGuiTableFlags.Sortable
                    | ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.NoBordersInBody
                    | ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY
                    | ImGuiTableFlags.SizingFixedFit;

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
    ImGui.Text("Search");
    ImGui.SameLine();
    if (ImGui.InputText("##allAchievementsSearchText", ref _allAchievementsSearchText, 128))
    {
      var searchTextToLower = _allAchievementsSearchText.ToLower();
      _filteredAllAchievements = _allAchievements.Where(a => a.Name.RawString.ToLower().Contains(searchTextToLower) ||
                                                             a.Description.RawString.ToLower().Contains(searchTextToLower));
    }

    if (string.IsNullOrEmpty(_allAchievementsSearchText))
      _filteredAllAchievements = _allAchievements;

    if (ImGui.BeginTable("##allAchievementsTable", 4, _tableFlags))
    {
      ImGui.TableSetupColumn("Ach Name");
      ImGui.TableSetupColumn("Ach Description");
      ImGui.TableSetupColumn("Ach Category");
      ImGui.TableSetupColumn("Watch");
      ImGui.TableHeadersRow();

      foreach (var ach in _filteredAllAchievements)
      {
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

  private void DrawWatchedAchievementList()
  {
    if (ImGui.BeginTable("##watchedAchievementsTable", 5, _tableFlags))
    {
      ImGui.TableSetupColumn("Ach Name");
      ImGui.TableSetupColumn("Ach Description");
      ImGui.TableSetupColumn("Ach Category");
      ImGui.TableSetupColumn("Update Trigger");
      ImGui.TableSetupColumn("Remove");
      ImGui.TableHeadersRow();

      foreach (var ach in Configuration.WatchedAchievements)
      {
        var achInfo = _allAchievements.FirstOrDefault(a => a.RowId == ach.Key);
        if (achInfo == null)
          continue;

        ImGui.TableNextRow();

        ImGui.TableNextColumn();
        ImGui.Text(achInfo.Name);

        ImGui.TableNextColumn();
        ImGui.Text(achInfo.Description);

        ImGui.TableNextColumn();
        ImGui.Text(achInfo.AchievementCategory.Value?.AchievementKind.Value?.Name ?? "");

        ImGui.TableNextColumn();
        int index = Array.IndexOf(_triggerTypeStrings, GetStringForTrigger(ach.Value));
        if (ImGui.Combo($"##ach_{ach.Key}_triggerTypeCombo", ref index, _triggerTypeStrings, _triggerTypeStrings.Length))
        {
          Configuration.ChangeTriggerTypeForAchievement(ach.Key, (TriggerType)Enum.Parse(typeof(TriggerType), _triggerTypeStrings[index]));
        }

        ImGui.TableNextColumn();
        if (ImGui.Button($"Remove##ach_{ach.Key}_removeWatched"))
        {
          Configuration.RemoveWatchedAchievement(ach.Key);
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
}
