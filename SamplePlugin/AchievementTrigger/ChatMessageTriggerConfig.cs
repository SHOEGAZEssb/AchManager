using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchManager.AchievementTrigger
{
  [Serializable]
  internal class ChatMessageTriggerConfig() : ITriggerConfig
  {
    #region Properties

    public string RequiredMessageContent { get; set; } = string.Empty;

    #endregion Properties
  }
}
