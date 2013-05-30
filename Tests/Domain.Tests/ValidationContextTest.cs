﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppReadyGo.Domain.CommandHandlers;
using AppReadyGo.Common;
using NHibernate;
using AppReadyGo.Domain.Model.Users;

namespace Domain.Tests
{
    [TestClass]
    public class ValidationContextTest
    {
        private Database database;

        [TestInitialize()]
        public void TestInitialize()
        {
            this.database = new Database(@"QA", "arg_unittests");
        }

        [TestCleanup()]
        public void TestCleanUp()
        {
            this.database.Clear();
        }

        [TestMethod]
        public void IsEmailExistsForNewUser()
        {
            int userId = UserTests.CreateMember(database).Id;
            using (ISession session = database.OpenSession())
            {
                var validation = new ValidationContext(session);
                Assert.IsTrue(validation.IsEmailExists("test@test.com"));
                Assert.IsFalse(validation.IsEmailExists("test1@test.com"));
            }
        }

        [TestMethod]
        public void IsEmailExistsForExistsUser()
        {
            int userId = UserTests.CreateMember(database, "test1@test.com").Id;
            userId = UserTests.CreateMember(database).Id;
            using (ISession session = database.OpenSession())
            {
                var validation = new ValidationContext(session);
                Assert.IsFalse(validation.IsEmailExists("test@test.com", userId));
                Assert.IsTrue(validation.IsEmailExists("test1@test.com", userId));
            }
        }
    }
}