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

        #region Configurable AWS Validation

        // moved to typed class to take advantage of LINQ
        private ValidCreateRdsInstance validRdsTemplate = SerializeHelper.GetObject<ValidCreateRdsInstance>(
                                                            File.ReadAllText(AppSetting.VALIDATION_JSON_DIRECTORY + AppSetting.VALIDATION_AWS_CREATE_RDS));

        public IValidRequest ValidateRdsCreate(CreateDBInstanceRequest request)
        {
            // ValidateVpc(request.DBSubnetGroupName);
            // ValidateAvailabilityZone(request.AvailabilityZone);

            ValidateEngineVersionInstance(request.Engine, request.EngineVersion, request.DBInstanceClass);

            return ValidRequest;
        }

        private void ValidateEngineVersionInstance(string engine, string version, string instanceType)
        {
            var validEngine = validRdsTemplate.EngineList.Where(p => p.Engine == engine.ToLowerInvariant()).SingleOrDefault();

            if(null == validEngine)
            {
                ValidRequest.AddError(engine + " is not a valid engine.");        // TODO: Potentially move to a configuration as well
            }
            else
            {   
                // if the version isn't set, take the default from the template             
                if(string.IsNullOrEmpty(version))
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
                    if(string.IsNullOrEmpty(instanceType))
                    {
                        instanceType = validVersion.InstanceClassDefault;
                    }

                    if(!validVersion.InstanceClassList.Any(p => p == instanceType))
                    {
                        ValidRequest.AddError(version + " is not a valid instance for [Engine: " + engine + ", Version: " + version + "].");
                    }
                }                
            }
        }

        #endregion

        #region Standard AWS Validation

        /// <summary>
        /// ValidateVpc:  Make sure the VPC is valid
        /// </summary>
        /// <param name="vpcId"></param>
        /// <returns></returns>
        internal void ValidateVpc(string vpcId)
        {
            try
            {                
                var result = ec2Client.DescribeVpcs();
                if (!result.Vpcs.Any(p => p.VpcId == vpcId && !p.IsDefault))
                {
                    validRequest.ErrorList.Add(vpcId + " is not a valid VPC selection");
                };         // is valid VPC? - cannot be default VPC
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
                valid = result.AvailabilityZones.Any(p => p.ZoneName == zoneName);         // is valid Availability Zone?
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
        internal bool ValidateSecurityGroup(string groupId)
        {
            bool valid = false;

            try
            {
                var result = ec2Client.DescribeSecurityGroups();
                valid = result.SecurityGroups.Any(p => p.GroupId == groupId);         // is valid Security Group?
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return valid;
        }

        /// <summary>
        /// ValidateIngressPort:  Validate the ingress port is between 1150 and 65535
        /// </summary>        
        /// <param name="port"></param>
        /// <returns></returns>
        internal bool ValidateIngressPort(int port)
        {
            bool valid = false;

            try
            {
                return (port >= 1150 && port <= 65535);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return valid;
        }

        /// <summary>
        /// ValidateSecurityGroupPort:  Make sure that the port where the user selected has proper ingress for security group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        internal bool ValidateSecurityGroupPort(string groupId, int port)
        {
            bool valid = false;

            try
            {
                var result = ec2Client.DescribeSecurityGroups();
                var group = result.SecurityGroups.Where(p => p.GroupId == groupId).SingleOrDefault();

                return group.IpPermissions.Any(x => x.ToPort == port);          // is security group port open?                
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return valid;
        }

        /// <summary>
        /// ValidateRdsGroupName: Confirm parameter group name is valid
        /// </summary>
        /// <param name="parameterGroupName"></param>
        /// <returns></returns>
        internal bool ValidateRdsParameterGroup(string parameterGroupName)
        {
            bool valid = false;

            try
            {
                var result = rdsClient.DescribeDBSecurityGroups();
                return result.DBSecurityGroups.Any(p => p.DBSecurityGroupName == parameterGroupName);                               
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return valid;
        }

        /// <summary>
        /// ValidateRdsOptionGroup: Confirm option group name is valid
        /// </summary>
        /// <param name="optionGroupName"></param>
        /// <returns></returns>
        internal bool ValidateRdsOptionGroup(string optionGroupName)
        {
            bool valid = false;

            try
            {
                var result = rdsClient.DescribeOptionGroups();
                return result.OptionGroupsList.Any(p => p.OptionGroupName == optionGroupName);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return valid;
        }

        #endregion
    }
}