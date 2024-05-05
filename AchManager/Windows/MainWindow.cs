﻿using System;
using System.Numerics;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace AchManager.Windows;

public class MainWindow : Window
{
  private readonly IDalamudTextureWrap? GoatImage;
  private readonly Plugin Plugin;

  // We give this window a hidden ID using ##
  // So that the user will see "My Amazing Window" as window title,
  // but for ImGui the ID is "My Amazing Window##With a hidden ID"
  public MainWindow(Plugin plugin, IDalamudTextureWrap? goatImage)
      : base("My Amazing Window##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
  {
    SizeConstraints = new WindowSizeConstraints
    {
      MinimumSize = new Vector2(375, 330),
      MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
    };

    GoatImage = goatImage;
    Plugin = plugin;
  }

  public override void Draw()
  {
    if (ImGui.Button("Show Settings"))
    {
      Plugin.ToggleConfigUI();
    }

    ImGui.Spacing();

    ImGui.Text("Have a goat:");
    if (GoatImage != null)
    {
      ImGuiHelpers.ScaledIndent(55f);
      ImGui.Image(GoatImage.ImGuiHandle, new Vector2(GoatImage.Width, GoatImage.Height));
      ImGuiHelpers.ScaledIndent(-55f);
    }
    else
    {
      ImGui.Text("Image not found.");
    }
  }
}