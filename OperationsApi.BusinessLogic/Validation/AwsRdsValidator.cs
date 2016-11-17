using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using OperationsApi.BusinessLogic.Model.Aws;

using Amazon.EC2;
using Amazon.RDS;
using Amazon.RDS.Model;

namespace OperationsApi.BusinessLogic.Validation
{
    public class AwsRdsValidator : ValidationBase
    {                
        private AmazonEC2Client _ec2Client;
        private AmazonEC2Client ec2Client
        {
            get
            {
                if(null == _ec2Client)
                {
                    _ec2Client = new AmazonEC2Client();            // TODO: use default configuration, may need to add in the ability to regionalize
                }

                return _ec2Client;
            }
        }

        private AmazonRDSClient _rdsClient;
        private AmazonRDSClient rdsClient
        {
            get
            {
                if (null == _rdsClient)
                {
                    _rdsClient = new AmazonRDSClient();            // TODO: use default configuration, may need to add in the ability to regionalize
                }

                return _rdsClient;
            }
        }

        #region AWS Reference

        private IList<DBEngineVersion> _engineList;
        private IList<DBEngineVersion> engineList
        {
            get
            {
                if(null == _engineList)
                {
                    _engineList = rdsClient.DescribeDBEngineVersions().DBEngineVersions;
                }

                return _engineList;
            }
        }

        private IList<OrderableDBInstanceOption> instanceOptionList;
        private void GetInstanceOptionList(string engine)
        {
            var request = new DescribeOrderableDBInstanceOptionsRequest
            {
                Engine = engine
            };

            instanceOptionList = rdsClient.DescribeOrderableDBInstanceOptions(request).OrderableDBInstanceOptions;            
        }        

        #endregion

        public AwsRdsValidator()
        {
                        
        }

        private CreateRdsInstance createRdsInstance;

        public IValidRequest ValidateRdsCreate(CreateDBInstanceRequest request)
        {
            createRdsInstance = SerializeHelper.GetObject<CreateRdsInstance>
                                                   (File.ReadAllText(AppSetting.VALIDATION_JSON_DIRECTORY + AppSetting.VALIDATION_AWS_CREATE_RDS));
            
            // first validate the engine, version, instanceclass - most likely to have issues
            ValidateEngineVersionInstance(request.Engine, request.EngineVersion, request.DBInstanceClass, request.LicenseModel);

            // if the engine, version, or instance class are not correct, bail out of continued validation until corrected
            if(ValidRequest.IsValid)
            {
                // next if they chose DbSubnetGroupName, validate it
                if (!string.IsNullOrEmpty(request.DBSubnetGroupName))
                {
                    ValidateDbSubnetGroup(request.DBSubnetGroupName);
                }

                // next if they chose AvailabilityZone, validate it
                if (!string.IsNullOrEmpty(request.AvailabilityZone))
                {
                    ValidateAvailabilityZone(request.AvailabilityZone);
                }

                // next if they chose Port, validate it
                if (request.Port > 0)
                {
                    ValidateIngressPort(request.Port);
                }

                // next if they chose DBParameterGroupName, validate it
                if (!string.IsNullOrEmpty(request.DBParameterGroupName))
                {
                    ValidateRdsParameterGroup(request.DBParameterGroupName, request.Engine);
                }

                // next if they chose OptionGroupName, validate it
                if (!string.IsNullOrEmpty(request.OptionGroupName))
                {
                    ValidateRdsOptionGroup(request.OptionGroupName, request.Engine);
                }
            }
            
            return ValidRequest;
        }

        public IValidRequest ValidateRdsModify(ModifyDBInstanceRequest request)
        {
            //TODO: Determine if TRY/CATCH is warranted here if configuration is missing ... probably ...
            createRdsInstance = SerializeHelper.GetObject<CreateRdsInstance>
                                                    (File.ReadAllText(AppSetting.VALIDATION_JSON_DIRECTORY + AppSetting.VALIDATION_AWS_CREATE_RDS));           

            return ValidRequest;
        }

        #region AWS Validation

        /// <summary>
        /// ValidateEngineVersionInstance:  Multiple combinations are valid here.  See:  OperationsApi.Configs/valid-create-rds-instance.json for details
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="version"></param>
        /// <param name="instanceType"></param>
        private void ValidateEngineVersionInstance(string engine, string version, string instanceType, string licenseModel)
        {            
            if(!string.IsNullOrEmpty(engine))
            {
                engine = engine.ToLowerInvariant();

                var validEngine = engineList.Where(p => p.Engine == engine).FirstOrDefault();
                
                if (null == validEngine)
                {
                    ValidRequest.AddError(engine + " is not a valid engine.");        // TODO: Potentially move to a configuration as well
                }
                else
                {
                    GetInstanceOptionList(engine);

                    // apply defaults if version, instanceType, or licenseModel are null and defaults are available ...
                    var defaultEngine = createRdsInstance.EngineList.Where(p => p.Engine == engine).FirstOrDefault();

                    if(null != defaultEngine)
                    {
                        version = string.IsNullOrEmpty(version) ? defaultEngine.Default.Version : version;
                        licenseModel = string.IsNullOrEmpty(licenseModel) ? defaultEngine.Default.LicenseModel : licenseModel;

                        var defaultVersion = defaultEngine.VersionList.Where(p => p.Version == version).SingleOrDefault();

                        if(null != defaultVersion)
                        {
                            instanceType = string.IsNullOrEmpty(instanceType) ? defaultVersion.InstanceClassDefault : instanceType;
                        }                        
                    }
                
                    var validVersion = engineList.Where(p => p.Engine == engine && p.EngineVersion == version).SingleOrDefault();

                    if (null == validVersion)
                    {
                        ValidRequest.AddError(version + " is not a valid version for [Engine: " + engine + "].");
                    }
                    else
                    {
                        if(!instanceOptionList.Any(p => p.Engine == engine && p.EngineVersion == version && p.LicenseModel == licenseModel))
                        {
                            ValidRequest.AddError(licenseModel + " is not a valid license model for [Engine: " + engine + ", Version: " + version + "].");
                        }
                        else
                        {
                            if (!instanceOptionList.Any(p => p.Engine == engine && p.EngineVersion == version && p.LicenseModel == licenseModel && p.DBInstanceClass == instanceType))
                            {
                                ValidRequest.AddError(instanceType + " is not a valid instance for [Engine: " + engine + ", Version: " + version + "].");
                            }
                        }                        
                    }
                }
            }
            else
            {
                ValidRequest.AddError(engine + " is required to provision an RDS instance.");
            }     
        }

        /// <summary>
        /// ValidateVpc:  Make sure the VPC is valid
        /// </summary>
        /// <param name="vpcId"></param>
        /// <returns></returns>
        internal void ValidateDbSubnetGroup(string dBSubnetGroupName)
        {
            try
            {                
                var result = rdsClient.DescribeDBSubnetGroups();
                if (!result.DBSubnetGroups.Any(p => p.DBSubnetGroupName == dBSubnetGroupName))
                {
                    validRequest.AddError(dBSubnetGroupName + " is not a valid DB Subnet Group selection");        // TODO:  Determine how we avoid DEFAULT VPC if possible
                };
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>
        /// ValidateAvailabilityZone:  Make sure the availability zone is valid
        /// </summary>
        /// <param name="zoneName"></param>
        /// <returns></returns>
        internal bool ValidateAvailabilityZone(string zoneName)
        {
            bool valid = false;

            try
            {
                var result = ec2Client.DescribeAvailabilityZones();
                if (!result.AvailabilityZones.Any(p => p.ZoneName == zoneName))
                {
                    validRequest.AddError(zoneName + " is not a valid Availability Zone");        // TODO:  Determine how we avoid DEFAULT VPC if possible
                };                
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return valid;
        }

        // TODO:  Create VPC?  Unlikely but noted ...
        // TODO:  Create DB Subnet Group?  Unlikely but noted ...

        /// <summary>
        /// ValidateSecurityGroup:  Make sure the security group selected is valid
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        internal void ValidateSecurityGroup(string groupId)
        {            
            try
            {
                var result = ec2Client.DescribeSecurityGroups();
                if (!result.SecurityGroups.Any(p => p.GroupId == groupId))
                {
                    ValidRequest.AddError(groupId + " is not a valid Security Group");        // TODO:  Determine how we avoid DEFAULT VPC if possible
                };                
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }            
        }

        /// <summary>
        /// ValidateIngressPort:  Validate the ingress port is between 1150 and 65535
        /// </summary>        
        /// <param name="port"></param>
        /// <returns></returns>
        internal void ValidateIngressPort(int port)
        {
            try
            {
                if (port < 1150 || port > 65535)
                {
                    ValidRequest.AddError(port + " is not a valid port (allowed between 1150 and 65535)");        // TODO:  Determine how we avoid DEFAULT VPC if possible
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>
        ///
        /// !!!!! NOT IMPLEMENTED !!!!!!!! AS THIS REALLY DOESN"T NEED TO BE A FATAL ERROR TO THE AWS API ...
        ///
        /// ValidateSecurityGroupPort:  Make sure that the port where the user selected has proper ingress for security group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        internal void ValidateSecurityGroupPort(string groupId, int port)
        {
            try
            {
                var result = ec2Client.DescribeSecurityGroups();
                var group = result.SecurityGroups.Where(p => p.GroupId == groupId).SingleOrDefault();

                if (!group.IpPermissions.Any(x => x.ToPort == port))
                {
                    ValidRequest.AddError(groupId + " is not allowing Ingress on " + port);        // TODO:  Determine how we avoid DEFAULT VPC if possible
                };                
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>
        /// ValidateRdsGroupName: Confirm parameter group name is valid
        /// </summary>
        /// <param name="parameterGroupName"></param>
        /// <returns></returns>
        internal void ValidateRdsParameterGroup(string parameterGroupName, string engine)
        {
            try
            {
                var result = rdsClient.DescribeDBParameterGroups();
                if (!result.DBParameterGroups.Any(p => p.DBParameterGroupName == parameterGroupName))
                {
                    ValidRequest.AddError(parameterGroupName + " is not a valid Parameter Group Name.");        // TODO:  Determine if we should return valid values
                };

                // TODO: This might be overkill ... but think it could happen
                if (!result.DBParameterGroups.Any(p => p.DBParameterGroupName == parameterGroupName && p.DBParameterGroupFamily.IndexOf(engine) < 0))
                {
                    ValidRequest.AddError(parameterGroupName + " is not a valid Parameter Group for [Engine: " + engine + "].");
                };
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>
        /// ValidateRdsOptionGroup: Confirm option group name is valid
        /// </summary>
        /// <param name="optionGroupName"></param>
        /// <returns></returns>
        internal void ValidateRdsOptionGroup(string optionGroupName, string engine)
        {
            try
            {
                var result = rdsClient.DescribeOptionGroups();
                if (!result.OptionGroupsList.Any(p => p.OptionGroupName == optionGroupName))
                {
                    ValidRequest.AddError(optionGroupName + " is not a valid Option Group Name.");        // TODO:  Determine if we should return valid values
                };

                // TODO: This might be overkill ... but think it could happen
                if (!result.OptionGroupsList.Any(p => p.OptionGroupName == optionGroupName && p.EngineName != engine))
                {
                    ValidRequest.AddError(optionGroupName + " is not a valid Parameter Group for [Engine: " + engine + "].");
                };

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }            
        }

        #endregion

        #region Reference              


        #endregion
    }
}