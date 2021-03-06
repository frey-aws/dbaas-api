﻿using System;
using System.Collections.Generic;

namespace OperationsApi.BusinessLogic.Model.Aws
{    
    public class Default
    {
        public int AllocatedStorage { get; set; }
        public int DatabasePort { get; set; }
        public int BackupRetentionPeriodInDays { get; set; }
        public bool PubliclyAccessible { get; set; }
        public bool EnableEncryption { get; set; }
        public bool MinorVersionUpgrade { get; set; }
        public bool MultiAzDeployment { get; set; }
        public bool? EnableEnhancedMonitor { get; set; }
        public int? EnhancedMonitorGranularitySeconds { get; set; }
        public string Version { get; set; }
        public string LicenseModel { get; set; }
    }

    public class RegularExpression
    {
        public string Parameter { get; set; }
        public string Regex { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ValidationList
    {
        public List<RegularExpression> RegularExpression { get; set; }
    }

    public class VersionList
    {
        public string Version { get; set; }

        public List<string> LicenseModel { get; set; }

        public string InstanceClassDefault { get; set; }

        public List<string> InstanceClassList { get; set; }
    }

    public class EngineList
    {
        public string Engine { get; set; }        
        public List<string> StorageTypeList { get; set; }
        public List<int> EnhancedMonitorList { get; set; }
        public int AllocatedStorageMinimumGb { get; set; }
        public int AllocatedStorageMaximumGb { get; set; }
        public int AllocatedStorageMaximumMultiAz { get; set; }
        public Default Default { get; set; }
        public List<ValidationList> ValidationList { get; set; }
        public List<VersionList> VersionList { get; set; }
        public bool? MultiAzDeployment { get; set; }
    }

    public class CreateRdsInstance
    {
        public List<EngineList> EngineList { get; set; }
    }
}