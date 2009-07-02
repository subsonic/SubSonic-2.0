using System.Data;

namespace SubSonic
{
    public class Migration002 : Migration
    {
        public override void Up()
        {
            //alter the description field - change to 100
            AlterColumn("Test1", "Description", DbType.String, 50);

            //add a new column to Products
            AddColumn("Test1", "MyNewColumn", DbType.DateTime);

            //add another one and set the default, as well as non-nullable
            AddColumn("Test1", "MaxInventory", DbType.Int32, 0, false, "100");
        }

        public override void Down()
        {
            //the Down() method reverses what Up() did
            //reset Description to 50
            AlterColumn("Test1", "Description", DbType.String, 50);

            //remove MyNewColumn and MaxInventory
            RemoveColumn("Test1", "MyNewColumn");
            RemoveColumn("Test1", "MaxInventory");
        }
    }
}