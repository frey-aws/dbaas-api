using System;
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

        public AwsRdsValidator()
        {
            // TODO:  Determine if additional overloads will be required for this implementation
        }        

        // moved to typed class to take advantage of LINQ
        private ValidCreateRdsInstance validRdsTemplate;

        public IValidRequest ValidateRdsCreate(CreateDBInstanceRequest request)
        {
            //TODO: Determine if TRY/CATCH is warranted here if configuration is missing ... probably ...
            validRdsTemplate = SerializeHelper.GetObject<ValidCreateRdsInstance>
                                                    (File.ReadAllText(AppSetting.VALIDATION_JSON_DIRECTORY + AppSetting.VALIDATION_AWS_CREATE_RDS));

            // first validate the engine, version, instanceclass - most likely to have issues
            ValidateEngineVersionInstance(request.Engine, request.EngineVersion, request.DBInstanceClass);

            // if the engine, version, or instance class are not correct, bail out of continued validation until corrected
            if(!ValidRequest.IsValid)
            {
                // next if they chose DbSubnetGroupName, validate it
                if (string.IsNullOrEmpty(request.DBSubnetGroupName))
                {
                    ValidateDbSubnetGroup(request.DBSubnetGroupName);
                }

                // next if they chose AvailabilityZone, validate it
                if (string.IsNullOrEmpty(request.AvailabilityZone))
                {
                    ValidateAvailabilityZone(request.AvailabilityZone);
                }

                // next if they chose Port, validate it
                if (request.Port > 0)
                {
                    ValidateIngressPort(request.Port);
                }

                // next if they chose DBParameterGroupName, validate it
                if (string.IsNullOrEmpty(request.DBParameterGroupName))
                {
                    ValidateRdsParameterGroup(request.DBParameterGroupName, request.Engine);
                }

                // next if they chose OptionGroupName, validate it
                if (string.IsNullOrEmpty(request.OptionGroupName))
                {
                    ValidateRdsOptionGroup(request.OptionGroupName, request.Engine);
                }
            }
            
            return ValidRequest;
        }

        #region Validations

        /// <summary>
        /// ValidateEngineVersionInstance:  Multiple combinations are valid here.  See:  OperationsApi.Configs/valid-create-rds-instance.json for details
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="version"></param>
        /// <param name="instanceType"></param>
        private void ValidateEngineVersionInstance(string engine, string version, string instanceType)
        {
            var validEngine = validRdsTemplate.EngineList.Where(p => p.Engine == engine.ToLowerInvariant()).SingleOrDefault();

            if(!string.IsNullOrEmpty(engine))
            {
                if (null == validEngine)
                {
                    ValidRequest.AddError(engine + " is not a valid engine.");        // TODO: Potentially move to a configuration as well
                }
                else
                {
                    // if the version isn't set, take the default from the template             
                    if (string.IsNullOrEmpty(version))
                    {
                        version = validEngine.Default.Version;
                    }

                    var validVersion = validEngine.VersionList.Where(p => p.Version == version).SingleOrDefault();

                    if (null == validVersion)
                    {
                        ValidRequest.AddError(version + " is not a valid version for [Engine: " + engine + "].");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(instanceType))
                        {
                            instanceType = validVersion.InstanceClassDefault;
                        }

                        if (!validVersion.InstanceClassList.Any(p => p == instanceType))
                        {
                            ValidRequest.AddError(version + " is not a valid instance for [Engine: " + engine + ", Version: " + version + "].");
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
                    validRequest.ErrorList.Add(dBSubnetGroupName + " is not a valid DB Subnet Group selection");        // TODO:  Determine how we avoid DEFAULT VPC if possible
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
                    validRequest.ErrorList.Add(zoneName + " is not a valid Availability Zone");        // TODO:  Determine how we avoid DEFAULT VPC if possible
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
                    ValidRequest.ErrorList.Add(groupId + " is not a valid Security Group");        // TODO:  Determine how we avoid DEFAULT VPC if possible
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
                if (port < 1150 && port > 65535)
                {
                    ValidRequest.ErrorList.Add(port + " is not a valid port (allowed between 1150 and 65535)");        // TODO:  Determine how we avoid DEFAULT VPC if possible
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
                    ValidRequest.ErrorList.Add(groupId + " is not allowing Ingress on " + port);        // TODO:  Determine how we avoid DEFAULT VPC if possible
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
                    ValidRequest.ErrorList.Add(parameterGroupName + " is not a valid Parameter Group Name.");        // TODO:  Determine if we should return valid values
                };

                // TODO: This might be overkill ... but think it could happen
                if (!result.DBParameterGroups.Any(p => p.DBParameterGroupName == parameterGroupName && p.DBParameterGroupFamily.IndexOf(engine) < 0))
                {
                    ValidRequest.ErrorList.Add(parameterGroupName + " is not a valid Parameter Group for [Engine: " + engine + "].");
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
                    ValidRequest.ErrorList.Add(optionGroupName + " is not a valid Option Group Name.");        // TODO:  Determine if we should return valid values
                };

                // TODO: This might be overkill ... but think it could happen
                if (!result.OptionGroupsList.Any(p => p.OptionGroupName == optionGroupName && p.EngineName != engine))
                {
                    ValidRequest.ErrorList.Add(optionGroupName + " is not a valid Parameter Group for [Engine: " + engine + "].");
                };

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }            
        }

        #endregion
    }
}