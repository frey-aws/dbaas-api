using System;
using System.Linq;

using Amazon.EC2;
using Amazon.RDS;

namespace OperationsApi.BusinessLogic.Validation
{
    public class AwsValidation
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

        public AwsValidation()
        {
            // TODO:  Determine if additional overloads will be required for this implementation
        }

        #region Configurable AWS Validation

        #endregion

        #region Standard AWS Validation

        /// <summary>
        /// ValidateVpc:  Make sure the VPC is valid
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool ValidateVpc(string vpcId)
        {
            bool valid = false;

            try
            {                
                var result = ec2Client.DescribeVpcs();
                valid = result.Vpcs.Any(p => p.VpcId == vpcId && !p.IsDefault);         // is valid VPC? - cannot be default VPC
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return valid;
        }

        /// <summary>
        /// ValidateAvailabilityZone:  Make sure the availability zone is valid
        /// </summary>
        /// <param name="zoneName"></param>
        /// <returns></returns>
        public bool ValidateAvailabilityZone(string zoneName)
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
        public bool ValidateSecurityGroup(string groupId)
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
        /// <param name="groupId"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool ValidateIngressPort(int port)
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
        public bool ValidateSecurityGroupPort(string groupId, int port)
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
        public bool ValidateRdsParameterGroup(string parameterGroupName)
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
        public bool ValidateRdsOptionGroup(string optionGroupName)
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