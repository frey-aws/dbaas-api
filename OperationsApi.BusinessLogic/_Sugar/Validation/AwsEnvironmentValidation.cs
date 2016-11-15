using System;
using System.Linq;

using Amazon.EC2;

namespace OperationsApi.BusinessLogic.Validation
{
    public class AwsEnvironmentValidation
    {
        // TODO:  Determine the region aspect and may convert to a factory/List<T> of clients for each region
        private AmazonEC2Client _ec2Client;
        private AmazonEC2Client ec2Client
        {
            get
            {
                if(null == _ec2Client)
                {
                    _ec2Client = new AmazonEC2Client();            // use default configuration, may need to add in the ability to regionalize
                }

                return _ec2Client;
            }
        }

        public AwsEnvironmentValidation()
        {
            // TODO:  Determine if additional overloads will be required for this implementation
        }

        #region Standard AWS Validations

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

        // TODO:  Create VPC?  Unlikely

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

        #endregion
    }
}
