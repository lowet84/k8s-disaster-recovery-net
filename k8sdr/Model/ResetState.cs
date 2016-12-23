using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8sdr.Model
{
    public enum ResetState
    {
        ReadyToRestoreMaster = 0,
        RestoringMaster = 1,
        ReadyToRestoreNodes = 2,
        RestoringNodes = 3,
        ReadyToSetLabels = 4,
        SettingLabels = 5,
        ReadyToStartCoreServices = 6,
        StartingCoreServices = 7,
        ReadyToStartLizardfs = 8,
        StartingLizardfs = 9,
        ReadyToStartChunks = 10,
        StartingChunks = 11,
        ReadyToRepairFiles = 12,
        RepairingFiles = 13,
        ReadyToSetupNamespaces = 14,
        SettingUpNamespaces = 15,
        ReadyToSetupStorage = 16,
        SettingUpStorage = 17,
        ReadyToStartApps = 18,
        StartingApps = 19,
        Finished = 20
    }
}
