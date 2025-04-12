using AchManager.AchievementTrigger;
using Dalamud.Interface.Windowing;
using ECommons;
using ImGuiNET;
using System;
using System.Linq;

namespace AchManager.Windows
{
  internal class DefaultTriggerConfigWindow(TriggerConfig triggerConfig, Configuration pluginConfig, string name, ImGuiWindowFlags flags = ImGuiWindowFlags.AlwaysAutoResize, bool forceMainWindow = false)
    : Window(name, flags, forceMainWindow)
  {
    private readonly TriggerConfig _triggerConfig = triggerConfig;
    protected readonly Configuration _pluginConfig = pluginConfig;

    /// <summary>
    /// Draws the config.
    /// Inheriting config windows should not override
    /// this, but override <see cref="DrawChildContent"/>
    /// instead.
    /// </summary>
    public override void Draw()
    {
      if (ImGui.TreeNodeEx("##notificationConfig", ImGuiTreeNodeFlags.CollapsingHeader | ImGuiTreeNodeFlags.DefaultOpen, "Progress Notifications"))
      {
        ImGui.Indent();

        var chat = _triggerConfig.ShowChatMessage;
        if (ImGui.Checkbox("Chat##showChatMessage", ref chat))
        {
          _triggerConfig.ShowChatMessage = chat;
          _pluginConfig.Save();
        }
        if (ImGui.IsItemHovered())
        {
          ImGui.BeginTooltip();
          ImGui.SetTooltip("If checked, achievement progress will be posted in the chat.");
          ImGui.EndTooltip();
        }

        var notif = _triggerConfig.ShowNotification;
        if (ImGui.Checkbox("Dalamud Notification##showNotification", ref notif))
        {
          _triggerConfig.ShowNotification = notif;
          _pluginConfig.Save();
        }
        if (ImGui.IsItemHovered())
        {
          ImGui.BeginTooltip();
          ImGui.SetTooltip("If checked, achievement progress will be shown as a dalamud notification.");
          ImGui.EndTooltip();
        }

        ImGui.BeginGroup();
        var notifyEveryXTimes = _triggerConfig.TriggerEveryXTimes;
        if (ImGui.Checkbox("Notify every##notifyEveryXTimesCB", ref notifyEveryXTimes))
        {
          _triggerConfig.TriggerEveryXTimes = notifyEveryXTimes;
          _pluginConfig.Save();
        }
        ImGui.SameLine();
        ImGui.BeginDisabled(!notifyEveryXTimes);
        var notifyEveryCount = _triggerConfig.TriggerEveryCount;
        if (ImGui.SliderInt("##notifyEverySlider", ref notifyEveryCount, 2, 100))
        {
          _triggerConfig.TriggerEveryCount = notifyEveryCount;
          _pluginConfig.Save();
        }
        ImGui.EndDisabled();
        ImGui.SameLine();
        ImGui.Text("progress steps");
        ImGui.EndGroup();
        if (ImGui.IsItemHovered())
        {
          ImGui.BeginTooltip();
          ImGui.SetTooltip("Defines how often you want progress notifications.");
          ImGui.EndTooltip();
        }

        ImGui.Unindent();
      }

      if (ImGui.TreeNodeEx("##delayConfig", ImGuiTreeNodeFlags.CollapsingHeader | ImGuiTreeNodeFlags.DefaultOpen, "Progress Check Delay"))
      {
        ImGui.Indent();

        ImGui.BeginGroup();
        ImGui.Text("Delay (ms)");
        ImGui.SameLine();
        var delay = _triggerConfig.DelayMS;
        if (ImGui.InputInt("##delayMS", ref delay) && delay >= 0)
        {
          _triggerConfig.DelayMS = delay;
          _pluginConfig.Save();
        }
        ImGui.EndGroup();
        if (ImGui.IsItemHovered())
        {
          ImGui.BeginTooltip();
          ImGui.SetTooltip("How long to wait after the trigger fired before checking achievement progress.");
          ImGui.EndTooltip();
        }

        ImGui.Unindent();
      }

      ImGui.Separator();
      if (ImGui.TreeNodeEx("##conditionConfig", ImGuiTreeNodeFlags.CollapsingHeader | ImGuiTreeNodeFlags.DefaultOpen, "Conditions"))
      {
        ImGui.Indent();

        ImGui.BeginGroup();
        ImGui.Text("Required Job:");
        ImGui.SameLine();
        var jobStrings = Enum.GetValues<Job>().Select(j => j.ToString()).ToArray();
        var index = jobStrings.IndexOf(j => j == _triggerConfig.RequiredJob.ToString());
        if (ImGui.Combo("##requiredJobCB", ref index, jobStrings, jobStrings.Length))
        {
          _triggerConfig.RequiredJob = Enum.Parse<Job>(jobStrings[index]);
          _pluginConfig.Save();
        }
        ImGui.EndGroup();
        if (ImGui.IsItemHovered())
        {
          ImGui.BeginTooltip();
          ImGui.SetTooltip("Defines which job the player must be on in order for the trigger to fire.");
          ImGui.EndTooltip();
        }

        ImGui.Unindent();
      }

      ImGui.Separator();
      if (ImGui.TreeNodeEx("##triggerSpecificConfig", ImGuiTreeNodeFlags.CollapsingHeader | ImGuiTreeNodeFlags.DefaultOpen, "Trigger Specific Options"))
      {
        ImGui.Indent();
        DrawChildContent();
        ImGui.Unindent();
      }
    }

    protected virtual void DrawChildContent() { }
  }
}