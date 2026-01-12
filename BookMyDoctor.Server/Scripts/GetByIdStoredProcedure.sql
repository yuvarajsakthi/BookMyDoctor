CREATE PROCEDURE sp_GetById
    @TableName NVARCHAR(128),
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SQL NVARCHAR(MAX);
    DECLARE @PrimaryKeyColumn NVARCHAR(128);
    
    -- Get the primary key column name for the table
    SELECT @PrimaryKeyColumn = COLUMN_NAME
    FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
    WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1
    AND TABLE_NAME = @TableName;
    
    -- Build and execute dynamic SQL
    SET @SQL = 'SELECT * FROM ' + QUOTENAME(@TableName) + ' WHERE ' + QUOTENAME(@PrimaryKeyColumn) + ' = @Id';
    
    EXEC sp_executesql @SQL, N'@Id INT', @Id;
END