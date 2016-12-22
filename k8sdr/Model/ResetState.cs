using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8sdr.Model
{
    public enum ResetState
    {
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
        ReadyToSetupNamespaces,
        SettingUpNamespaces,
        ReadyToSetupStorage,
        SettingUpStorage,
        ReadyToSetupClaims,
        SettingUpClaims,
        ReadyToStartApps,
        StartingApps,
        Error,
        Finished,
        Unarmed
    }
}
