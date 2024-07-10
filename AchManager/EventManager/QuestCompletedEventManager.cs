using FFXIVClientStructs.FFXIV.Application.Network.WorkDefinitions;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AchManager.EventManager
{
  internal class QuestCompletedEventManager : AchievementUpdateEventManagerBase<QuestCompletedEventArgs>
  {
    #region Singleton

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

    private readonly CancellationToken _cancellationToken;

    private IEnumerable<LeveWork>? _leveQuests;
    private IEnumerable<QuestWork>? _normalQuests;
    private BeastReputationWork[]? _beastTribeQuests;
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
          FireOnEvent(new QuestCompletedEventArgs(completedQuestTypes.Value));
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
