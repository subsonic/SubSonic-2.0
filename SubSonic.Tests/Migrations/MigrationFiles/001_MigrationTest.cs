using System.Data;

namespace SubSonic
{
    public class Migration001 : Migration
    {
        public override void Up()
        {
            TableSchema.Table t = CreateTable("Test1");
            t.AddColumn("Name", DbType.String, 50);
            t.AddColumn("Description", DbType.String, 100);
            t.AddColumn("DateEntered", DbType.DateTime, 0, false, "getdate()");
        }

        public override void Down()
        {
            DropTable("Test1");
        }
    }
}