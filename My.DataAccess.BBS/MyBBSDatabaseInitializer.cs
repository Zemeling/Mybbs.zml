using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.DataAccess.BBS
{
    public abstract class MyBBsDatabaseInitializer<T> where T : DbContext
    {
        private static readonly string _connectionString = System.Configuration.ConfigurationManager.AppSettings["MyDataContext"];
        
        public void RollBackByMigration(string targetMigration)
        {
            if (!string.IsNullOrWhiteSpace(targetMigration))
            {
                this.RollbackMigrationBy(targetMigration);
            }
        }

        private void Initialize()
        {
            DbMigrationsConfiguration<T> dbMigrationsConfiguration = new DbMigrationsConfiguration<T>();
            dbMigrationsConfiguration.TargetDatabase = new DbConnectionInfo(_connectionString, "System.Data.SqlClient");
            DbMigrationsConfiguration<T> configuration = dbMigrationsConfiguration;
            DbMigrator dataCtxMigrator = new DbMigrator(configuration);
            if (Enumerable.Any<string>(dataCtxMigrator.GetPendingMigrations()))
            {
                dataCtxMigrator.Update();
            }
        }

        private void RollBack()
        {
            DbMigrationsConfiguration<T> dbMigrationsConfiguration = new DbMigrationsConfiguration<T>();
            dbMigrationsConfiguration.TargetDatabase = new DbConnectionInfo(_connectionString, "System.Data.SqlClient");
            DbMigrationsConfiguration<T> configuration = dbMigrationsConfiguration;
            DbMigrator myDataCtxMigrator = new DbMigrator(configuration);
            string targetMigration = this.GetTargetMigration();
            myDataCtxMigrator.Update(targetMigration);
        }

        private void RollbackMigrationBy(string targetMigration)
        {
            DbMigrationsConfiguration<T> dbMigrationsConfiguration = new DbMigrationsConfiguration<T>();
            dbMigrationsConfiguration.TargetDatabase = new DbConnectionInfo(_connectionString, "System.Data.SqlClient");
            DbMigrationsConfiguration<T> configuration = dbMigrationsConfiguration;
            DbMigrator myDataCtxMigrator = new DbMigrator(configuration);
            myDataCtxMigrator.Update(targetMigration);
        }

        protected abstract string GetTargetMigration();
    }
}
