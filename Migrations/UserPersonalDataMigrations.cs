using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace MainBit.Users.Migrations
{
    public class UserPersonalDataMigrations : DataMigrationImpl
    {
        public int Create() {
            SchemaBuilder.CreateTable("UserPersonalDataPartRecord",
                table => table
                    .ContentPartRecord()
                        .Column<string>("FirstName", c => c.WithLength(128))
                        .Column<string>("MiddleName", c => c.WithLength(128))
                        .Column<string>("LastName", c => c.WithLength(128)));

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable("UserPersonalDataPartRecord", table => table.AddColumn<string>("Phone", c => c.WithLength(128)));

            return 2;
        }
    }
}