using Microsoft.EntityFrameworkCore.Migrations;

namespace hippo.Migrations
{
    public partial class ModifiedTimestampTrigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
@"CREATE FUNCTION update_modified_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.""Modified"" = now();
    RETURN NEW;
END;
$$ language 'plpgsql';

CREATE TRIGGER trigger_app_modified
  BEFORE UPDATE ON ""Applications""
  FOR EACH ROW
  EXECUTE PROCEDURE update_modified_column();
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
@"DROP TRIGGER trigger_app_modified;
DROP FUNCTION update_modified_column;");
        }
    }
}
