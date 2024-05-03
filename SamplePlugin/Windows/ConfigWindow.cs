using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.AccessControl;
using AchManager.AchievementTrigger;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using ImGuiNET;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;

namespace AchManager.Windows;

public class ConfigWindow : Window, IDisposable
{
  private readonly Configuration Configuration;
  private readonly IEnumerable<Achievement> _allAchievements;
  private IEnumerable<Achievement> _filteredAllAchievements;
  private string _allAchievementsSearchText = string.Empty;

  private static readonly string[] _triggerTypeStrings = GetTriggerTypeStrings();

  private static readonly ImGuiTableFlags _tableFlags = ImGuiTableFlags.Resizable | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Hideable
                    | ImGuiTableFlags.Sortable
                    | ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.NoBordersInBody
                    | ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY
                    | ImGuiTableFlags.SizingFixedFit;

  

  // We give this window a constant ID using ###
  // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
  // and the window ID will always be "###XYZ counter window" for ImGui
  public ConfigWindow(Plugin plugin) : base("A Wonderful Configuration Window###With a constant ID")
  {
    //Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
    //        ImGuiWindowFlags.NoScrollWithMouse;

    //Size = new Vector2(232, 75);
    //SizeCondition = ImGuiCond.Always;

    Configuration = plugin.Configuration;
    _allAchievements = Svc.Data.GetExcelSheet<Achievement>().Skip(1);
  }

  public void Dispose() { }

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
      _filteredAllAchievements = _allAchievements.Where(a => a.Name.RawString.ToLower().Contains(_allAchievementsSearchText) ||
                                                             a.Description.RawString.ToLower().Contains(_allAchievementsSearchText));
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
        ImGui.Text(ach.AchievementCategory.Value.AchievementKind.Value.Name);

        ImGui.TableNextColumn();
        bool watched = Configuration.WatchedAchievements.Keys.Contains(ach.RowId);
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
        ImGui.Text(achInfo.AchievementCategory.Value.AchievementKind.Value.Name);

        ImGui.TableNextColumn();
        int index = Array.IndexOf(_triggerTypeStrings, ach.Value.ToString());
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

  private static string[] GetTriggerTypeStrings()
  {
    var enumValues = Enum.GetValues(typeof(TriggerType));
    var strings = new string[enumValues.Length];
    for (int i = 0; i < enumValues.Length; i++)
      strings[i] = enumValues.GetValue(i).ToString();

    return strings;
  }
}
