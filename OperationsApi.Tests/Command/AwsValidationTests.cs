using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Amazon.RDS.Model;
using OperationsApi.BusinessLogic.Validation;

namespace OperationsApi.BusinessLogic.Tests
{
    [TestClass]
    public class AwsRdsValidationTests
    {
        private AwsRdsValidator validator;

        private CreateDBInstanceRequest _createDBInstanceRequest;
        private CreateDBInstanceRequest createDBInstanceRequest
        {
            get
            {
                if(null == _createDBInstanceRequest)
                {
                    // this starts from a HAPPY PATH (all things work) scenario ...
                    _createDBInstanceRequest = new CreateDBInstanceRequest
                    {
                        DBInstanceIdentifier = "test-db-instance",
                        DBInstanceClass = "db.t1.micro",
                        Engine = "sqlserver-ex",
                        EngineVersion = "13.00.2164.0.v1"
                    };
                }
                
                return _createDBInstanceRequest;
            }
        }
        
        public AwsRdsValidationTests()
        {
            validator = new AwsRdsValidator();
        }
        
        [TestMethod]
        public void Create_DB_Instance_Good_Happy_Path()
        {
            var result = validator.ValidateRdsCreate(createDBInstanceRequest);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Create_DB_Instance_Good_No_Instance_Class()
        {
            createDBInstanceRequest.DBInstanceClass = string.Empty;
            var result = validator.ValidateRdsCreate(createDBInstanceRequest);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Create_DB_Instance_Good_No_Engine_Version()
        {
            createDBInstanceRequest.EngineVersion = string.Empty;
            var result = validator.ValidateRdsCreate(createDBInstanceRequest);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Create_DB_Instance_Bad_Engine()
        {
            createDBInstanceRequest.Engine = "mongo-db";
            var result = validator.ValidateRdsCreate(createDBInstanceRequest);
            Assert.IsFalse(result.IsValid, String.Join(String.Empty, result.ErrorList));
        }

        [TestMethod]
        public void Create_DB_Instance_Bad_Version()
        {
            createDBInstanceRequest.EngineVersion = "the.next.one";
            var result = validator.ValidateRdsCreate(createDBInstanceRequest);
            Assert.IsFalse(result.IsValid, String.Join(String.Empty, result.ErrorList));
        }

        [TestMethod]
        public void Create_DB_Instance_Bad_Instance_Class()
        {
            createDBInstanceRequest.DBInstanceClass = "db.c4.8xlarge";
            var result = validator.ValidateRdsCreate(createDBInstanceRequest);
            Assert.IsFalse(result.IsValid, String.Join(String.Empty, result.ErrorList));
        }

        [TestMethod]
        public void Create_DB_Instance_Bad_License_Model()
        {
            createDBInstanceRequest.LicenseModel = "pay-as-you-go";
            var result = validator.ValidateRdsCreate(createDBInstanceRequest);
            Assert.IsFalse(result.IsValid, String.Join(String.Empty, result.ErrorList));
        }

        [TestMethod]
        public void Create_DB_Instance_Bad_Port()
        {
            createDBInstanceRequest.Port = 1149;        // port needs to be greater than 1150 and less than 65535
            var result = validator.ValidateRdsCreate(createDBInstanceRequest);
            Assert.IsFalse(result.IsValid, String.Join(String.Empty, result.ErrorList));
        } 
    }
}
