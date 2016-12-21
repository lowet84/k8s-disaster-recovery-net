using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8sdr.Model
{
    public enum ResetState
    {
        Unarmed,
        NotReady,
        ReadyToRestoreMaster,
        RestoringMaster,
        ReadyToRestoreNodes,
        RestoringNodes,
        ReadyToSetLabels,
        SettingLabels,
        ReadyToStartCoreServices,
        StartingCoreServices,
        ReadyToStartLizardfs,
        StartingLizardfs,
        ReadyToStartChunks,
        StartingChunks,
        ReadyToSetupStorage,
        SettingUpStorage,
        ReadyToStartApps,
        StartingApps,
        Error,
        Finished
    }
}
