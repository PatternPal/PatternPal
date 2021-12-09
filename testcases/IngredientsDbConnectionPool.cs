using System;
using System.Threading.Tasks;
using RealisticDependencies;

namespace CreationalPatterns.Singleton {
    public class IngredientsDbConnectionPool {

        private readonly IApplicationLogger _logger;
        private readonly Database _database;
        private int _openConnections = 0;

        private static readonly Lazy<IngredientsDbConnectionPool> _instance 
            = new (() => {
                var logger = new ConsoleLogger();
                var database = new Database(Configuration.ConnectionString, logger);
                return new IngredientsDbConnectionPool(database, logger);
            });

        private IngredientsDbConnectionPool(Database database, IApplicationLogger logger) {
            _database = database;
            _logger = logger;
        }


        public static IngredientsDbConnectionPool Instance => _instance.Value;
        public static IngredientsDbConnectionPool Instance2 { get; set; };
        public static IngredientsDbConnectionPool Instance3 { get => _instance.Value; set => _ins = value; };
        public static IngredientsDbConnectionPool Instance4 {
            get {
                return _ins;
            }
            set => _ins = value;
        };
    };

        public async Task Connect(string client) {
            if (_openConnections >= Configuration.MaxConnections) {
                _logger.LogError("ERROR - Cannot acquire new connection. " +
                                  $"Max connections of {Configuration.MaxConnections} " +
                                  "is met or exceeded.");
                return;
            }

            _openConnections++;
            _logger.LogInfo($"Added connection to pool from: {client}", ConsoleColor.Blue);
            await _database.Connect(client);
        }

        public async Task Disconnect() {
            if (_openConnections <= 0) {
                _logger.LogInfo("There are no connections to close.", ConsoleColor.Blue);
                return;
            }

            _openConnections--;
            _logger.LogInfo($"Released connection. Now managing ({_openConnections}) open connections.", 
                ConsoleColor.Blue);
            await _database.Disconnect();
        }
    }
}
