using Microsoft.EntityFrameworkCore;
using System;
using PatternPal.LoggingAPI;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Data.Sqlite;
using PatternPal.LoggingAPI.Controllers;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;

namespace Tests
{
    public class SqliteInMemoryLoggingControllerTest : LoggingControllerTests, IDisposable
    {
        private readonly DbConnection _connection;

        public SqliteInMemoryLoggingControllerTest()
            : base(
                new DbContextOptionsBuilder<TestContext>()
                    .UseSqlite(CreateInMemoryDatabase())
                    .Options)
        {
            _connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        public void Dispose() => _connection.Dispose();

        [Test]
        public void Can_add_session()
        {
            var context = Seed();
            var controller = new LoggingController(context);
            var expected = new Session()
            {
                ExtensionVersion = 2,
                StartSession = new System.DateTime(2020, 12, 21),
                TimeZone = 1
            };
            var actualFromController = controller.PostSession(expected).Value;
            var actualFromContext = context.Sessions.Find(expected.ID);
            Assert.AreEqual(expected, actualFromController);
            Assert.AreEqual(expected, actualFromContext);
        }

        [Test]
        public void Can_put_sessionEndTime()
        {
            var context = Seed();
            var controller = new LoggingController(context);
            var session = new Session()
            {
                ExtensionVersion = 30,
                StartSession = new System.DateTime(2021, 12, 21),
                TimeZone = 6
            };
            var expected = controller.PostSession(session).Value;
            expected.EndSession = new DateTime(2022, 3, 4);
            controller.PutEndSession(expected.ID, expected.EndSession);
            var actual = context.Sessions.Find(expected.ID);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Cannot_put_sessionEndTime_before_startTime()
        {
            var context = Seed();
            var controller = new LoggingController(context);
            var expected = new Session()
            {
                ExtensionVersion = 30,
                StartSession = new System.DateTime(2021, 12, 21),
                TimeZone = 6
            };
            var wrong = controller.PostSession(expected).Value;
            wrong.EndSession = new DateTime(2019, 3, 4);
            controller.PutEndSession(wrong.ID, wrong.EndSession);
            var actual = context.Sessions.Find(expected.ID);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Cannot_post_with_sessionEndTime_before_startTime()
        {
            var context = Seed();
            var controller = new LoggingController(context);
            var wrongDate = new Session()
            {
                ExtensionVersion = 30,
                StartSession = new System.DateTime(2021, 12, 21),
                EndSession = new DateTime(2019, 3, 2),
                TimeZone = 6
            };
            var result = controller.PostSession(wrongDate).Result;
            Assert.IsInstanceOf(typeof(BadRequestResult), result);
        }

        [Test]
        public void Can_add_actionType()
        {
            var context = Seed();
            var controller = new LoggingController(context);
            var expected = new ActionType()
            {
                ID = "testing"
            };
            var actualFromController = controller.PostActionType(expected).Value;
            var actualFromContext = context.ActionTypes.Find(expected.ID);
            Assert.AreEqual(expected, actualFromController);
            Assert.AreEqual(expected, actualFromContext);
        }

        [Test]
        public void Can_add_mode()
        {
            var context = Seed();
            var controller = new LoggingController(context);
            var expected = new Mode()
            {
                ID = "testing"
            };
            var actualFromController = controller.PostMode(expected).Value;
            var actualFromContext = context.Modes.Find(expected.ID);
            Assert.AreEqual(expected, actualFromController);
            Assert.AreEqual(expected, actualFromContext);
        }

        [Test]
        public void Can_add_action()
        {
            var context = Seed();
            var controller = new LoggingController(context);
            var session = new Session()
            {
                ExtensionVersion = 2,
                StartSession = new System.DateTime(2020, 12, 21),
                TimeZone = 1
            };
            var actionType = new ActionType() { ID = "test" };
            var mode = new Mode() { ID = "test" };
            controller.PostSession(session);
            controller.PostActionType(actionType);
            controller.PostMode(mode);
            var expected = new PatternPal.LoggingAPI.Action()
            {
                Time = new DateTime(2020, 12, 22),
                SessionID = session.ID,
                ActionTypeID = "test",
                ModeID = "test"
            };
            var actualFromController = controller.PostAction(expected).Value;
            var actualFromContext = context.Actions.Find(expected.ID);
            Assert.AreEqual(expected, actualFromController);
            Assert.AreEqual(expected, actualFromContext);
        }

        [Test]
        public void Cannot_add_action_time_before_or_after_session()
        {
            var context = Seed();
            var controller = new LoggingController(context);
            var session = new Session()
            {
                ExtensionVersion = 2,
                StartSession = new System.DateTime(2020, 12, 21),
                EndSession = new System.DateTime(2020, 12, 23),
                TimeZone = 1
            };
            var actionType = new ActionType() { ID = "1234" };
            var mode = new Mode() { ID = "1234" };
            controller.PostSession(session);
            controller.PostActionType(actionType);
            controller.PostMode(mode);
            var before = new PatternPal.LoggingAPI.Action()
            {
                Time = new DateTime(2020, 12, 20),
                SessionID = session.ID,
                ActionTypeID = "1234",
                ModeID = "1234"
            };
            var after = new PatternPal.LoggingAPI.Action()
            {
                Time = new DateTime(2021, 12, 20),
                SessionID = session.ID,
                ActionTypeID = "1234",
                ModeID = "1234"
            };
            var beforeResultFromController = controller.PostAction(before).Result;
            var afterResultFromController = controller.PostAction(after).Result;
            Assert.IsInstanceOf(typeof(BadRequestResult), beforeResultFromController);
            Assert.IsInstanceOf(typeof(BadRequestResult), afterResultFromController);
        }
        [Test]
        public void Can_add_extensionError()
        {
            var context = Seed();
            var controller = new LoggingController(context);
            var session = new Session()
            {
                ExtensionVersion = 2,
                StartSession = new System.DateTime(2020, 12, 21),
                TimeZone = 1
            };
            controller.PostSession(session);
            controller.PostActionType(new ActionType() { ID = "actiontypeId" });
            controller.PostMode(new Mode() { ID = "modeId" });
            var action = new PatternPal.LoggingAPI.Action()
            {
                Time = new DateTime(2020, 12, 22),
                SessionID = session.ID,
                ActionTypeID = "actiontypeId",
                ModeID = "modeId"
            };
            controller.PostAction(action);
            var expected = new ExtensionError()
            {
                Message = "message",
                ActionID = action.ID
            };
            var actualFromController = controller.PostExtensionError(expected).Value;
            var actualFromContext = context.ExtensionErrors.Find(expected.ID);
            Assert.AreEqual(expected, actualFromController);
            Assert.AreEqual(expected, actualFromContext);
        }
        [Test]
        public void Can_put_extensionError()
        {
            var context = Seed();
            var controller = new LoggingController(context);
            var session = new Session()
            {
                ExtensionVersion = 2,
                StartSession = new System.DateTime(2020, 12, 21),
                TimeZone = 1
            };
            controller.PostSession(session);
            controller.PostActionType(new ActionType() { ID = "actiontypeId2" });
            controller.PostMode(new Mode() { ID = "modeId2" });
            var action = new PatternPal.LoggingAPI.Action()
            {
                Time = new DateTime(2019, 11, 22),
                SessionID = session.ID,
                ActionTypeID = "actiontypeId2",
                ModeID = "modeId2"
            };
            controller.PostAction(action);
            var extensionError = new ExtensionError()
            {
                Message = "message!",
                ActionID = action.ID
            };
            var expected = controller.PostExtensionError(extensionError).Value;
            expected.Message = "the correct message!";
            controller.PutExtensionError(expected.ID, expected);
            var actual = context.ExtensionErrors.Find(expected.ID);
            Assert.AreEqual(expected, actual);
        }
    }
}
