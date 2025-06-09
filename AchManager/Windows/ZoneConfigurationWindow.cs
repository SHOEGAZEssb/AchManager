using AchManager.AchievementTrigger;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ImGuiNET;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AchManager.Windows
{
  internal class ZoneConfigurationWindow(TriggerConfig triggerConfig, string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false) : Window(name, flags, forceMainWindow)
  {
    private readonly TriggerConfig _triggerConfig = triggerConfig;
    private static readonly Dictionary<uint, string> _territoryData;
    private bool _showIDs = true;
    private string _searchText = string.Empty;
    private IEnumerable<KeyValuePair<uint, string>> _filteredAvailableZones = _territoryData;

    static ZoneConfigurationWindow()
    {
      _territoryData = [];
      var validTerritories = Svc.Data.Excel.GetSheet<TerritoryType>().Where(t => t.PlaceName.IsValid && !t.PlaceName.Value.Name.IsEmpty);
      foreach (var territory in validTerritories)
        _territoryData.Add(territory.RowId, territory.PlaceName.Value.Name.ToString());
    }

    public override void Draw()
    {
      var treatAsWhitelist = _triggerConfig.TreatRequiredTerritoriesAsWhitelist;
      if (ImGui.Checkbox("Treat configured zones as whitelist", ref treatAsWhitelist))
      {
        _triggerConfig.TreatRequiredTerritoriesAsWhitelist = treatAsWhitelist;
        Plugin.Configuration.Save();
      }
      if (ImGui.IsItemHovered())
      {
        ImGui.BeginTooltip();
        ImGui.SetTooltip("If checked, configured zones are treated as a whitelist, meaning the trigger will fire when the character is in one of those zones.\r\n" +
                         "If unchecked, configured zones are treated as a blacklist, meaning the trigger will NOT fire when the character is in one of those zones.");
        ImGui.EndTooltip();
      }

      ImGui.Checkbox("Show zone IDs", ref _showIDs);

      ImGui.Separator();

      if (ImGui.Button("Add current zone") && !_triggerConfig.RequiredTerritories.Contains(Player.Territory))
      {
        _triggerConfig.RequiredTerritories.Add(Player.Territory);
        Plugin.Configuration.Save();
      }

      ImGui.Text("Search:");
      ImGui.SameLine();
      if (ImGui.InputText("", ref _searchText, 128))
      {
          _filteredAvailableZones = string.IsNullOrEmpty(_searchText) ? _territoryData :
                                                                        _territoryData.Where(a => a.Value.Contains(_searchText, StringComparison.CurrentCultureIgnoreCase)).ToArray();
      }

      float spacing = 10f;
      var avail = ImGui.GetContentRegionAvail();
      float listBoxWidth = (avail.X - spacing) / 2f;
      float listBoxHeight = avail.Y - 20f;

      // configured zones
      ImGui.BeginGroup();
      ImGui.Text($"Configured Zones ({(_triggerConfig.TreatRequiredTerritoriesAsWhitelist ? "Whitelist" : "BlackList")})");
      if (ImGui.BeginListBox("###ConfiguredZones", new Vector2(listBoxWidth, listBoxHeight)))
      {
        for (int i = 0; i < _triggerConfig.RequiredTerritories.Count; i++)
        {
          ImGui.Selectable(GetZoneName(_triggerConfig.RequiredTerritories[i], _showIDs));

          if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
          {
            _triggerConfig.RequiredTerritories.RemoveAt(i);
            Plugin.Configuration.Save();
          }
        }

        ImGui.EndListBox();
      }
      ImGui.EndGroup();

      // add horizontal spacing
      ImGui.SameLine();
      ImGui.Dummy(new Vector2(spacing, 0));
      ImGui.SameLine();

      // configured zones
      var filteredTerritories = _filteredAvailableZones.Where(t => !_triggerConfig.RequiredTerritories.Contains(t.Key));
      ImGui.BeginGroup();
      ImGui.Text("Available Zones");
      if (ImGui.BeginListBox("###AvailableZones", new Vector2(listBoxWidth, listBoxHeight)))
      {
        for (int i = 0; i < filteredTerritories.Count(); i++)
        {
          ImGui.Selectable(GetZoneName(filteredTerritories.ElementAt(i).Key, _showIDs));

          if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
          {
            _triggerConfig.RequiredTerritories.Add(filteredTerritories.ElementAt(i).Key);
            Plugin.Configuration.Save();
          }
        }

        ImGui.EndListBox();
      }
      ImGui.EndGroup();
    }

    private static string GetZoneName(uint zoneID, bool showID)
    {
      string name = _territoryData[zoneID];
      return showID ? $"{name} ({zoneID})" : name;
    }
  }
}