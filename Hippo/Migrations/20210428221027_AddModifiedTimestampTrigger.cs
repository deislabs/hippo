using Microsoft.EntityFrameworkCore.Migrations;

namespace Hippo.Migrations
{
    public partial class AddModifiedTimestampTrigger : Migration
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

CREATE TRIGGER trigger_channel_modified
  BEFORE UPDATE ON ""Channels""
  FOR EACH ROW
  EXECUTE PROCEDURE update_modified_column();

CREATE TRIGGER trigger_config_modified
  BEFORE UPDATE ON ""Configuration""
  FOR EACH ROW
  EXECUTE PROCEDURE update_modified_column();

CREATE TRIGGER trigger_domain_modified
  BEFORE UPDATE ON ""Domains""
  FOR EACH ROW
  EXECUTE PROCEDURE update_modified_column();

CREATE TRIGGER trigger_environmentvariable_modified
  BEFORE UPDATE ON ""EnvironmentVariables""
  FOR EACH ROW
  EXECUTE PROCEDURE update_modified_column();

CREATE TRIGGER trigger_key_modified
  BEFORE UPDATE ON ""Keys""
  FOR EACH ROW
  EXECUTE PROCEDURE update_modified_column();

CREATE TRIGGER trigger_release_modified
  BEFORE UPDATE ON ""Releases""
  FOR EACH ROW
  EXECUTE PROCEDURE update_modified_column();
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
@"DROP TRIGGER trigger_app_modified;
DROP TRIGGER trigger_channel_modified;
DROP TRIGGER trigger_config_modified;
DROP TRIGGER trigger_domain_modified;
DROP TRIGGER trigger_environmentvariable_modified;
DROP TRIGGER trigger_key_modified;
DROP TRIGGER trigger_release_modified;
DROP FUNCTION update_modified_column;");
        }
    }
}
