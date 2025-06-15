using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Application.Network.WorkDefinitions;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AchManager.EventManager
{
  /// <summary>
  /// Event manager that informs about completed quests.
  /// </summary>
  internal sealed class QuestCompletedEventManager : AchievementUpdateEventManagerBase<QuestCompletedEventArgs>
  {
    #region Singleton

    /// <summary>
    /// The unique instance of this event manager.
    /// </summary>
    public static QuestCompletedEventManager Instance
    {
      get
      {
        _instance ??= new QuestCompletedEventManager();
        return _instance;
      }
    }
    private static QuestCompletedEventManager? _instance;

    #endregion Singleton

    #region Properties

    private readonly CancellationTokenSource _cancellationToken = new();

    private IEnumerable<LeveWork>? _leveQuests;
    private IEnumerable<QuestWork>? _normalQuests;
    private IEnumerable<DailyQuestWork>? _dailyQuests;

    #endregion Properties

    #region Construction

    private QuestCompletedEventManager()
    {
      Task.Factory.StartNew(async () =>
      {
        while (!_cancellationToken.IsCancellationRequested)
        {
          UpdateQuests();
          await Task.Delay(2000);
        }
      });
    }

    #endregion Construction

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Dispose()
    {
      _cancellationToken.Cancel();
    }

    private void UpdateQuests()
    {
      unsafe
      {
        var qm = QuestManager.Instance();
        CompletedQuestType? completedQuestTypes = null;

        // leves
        var leves = qm->LeveQuests.ToArray().Where(l => l.LeveId != 0);
        if (leves.Count() < _leveQuests?.Count())
          AddFlagToNullableEnum(ref completedQuestTypes, CompletedQuestType.Leve);
        _leveQuests = leves;

        // normal quests
        var normal = qm->NormalQuests.ToArray().Where(n => n.QuestId != 0);
        if (normal.Count() < _normalQuests?.Count())
          AddFlagToNullableEnum(ref completedQuestTypes, CompletedQuestType.Normal);
        _normalQuests = normal;

        // daily
        var dailies = qm->DailyQuests.ToArray().Where(d => d.QuestId != 0);
        if (dailies.Count() < _dailyQuests?.Count())
          AddFlagToNullableEnum(ref completedQuestTypes, CompletedQuestType.Daily);
        _dailyQuests = dailies;

        if (completedQuestTypes.HasValue)
        {
          Svc.Log.Debug($"{nameof(QuestCompletedEventManager)}: Fire ({completedQuestTypes.Value})");
          FireOnEvent(new QuestCompletedEventArgs(completedQuestTypes.Value));
        }
      }
    }

    private static void AddFlagToNullableEnum(ref CompletedQuestType? completedQuestTypes, CompletedQuestType flag)
    {
      if (completedQuestTypes == null)
        completedQuestTypes = flag;
      else
        completedQuestTypes |= flag;
    }
  }
}
