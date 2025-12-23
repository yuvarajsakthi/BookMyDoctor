-- Generic Get All with Foreign Key Data
CREATE OR ALTER PROCEDURE sp_Generic_GetAll
    @TableName NVARCHAR(128)
AS
BEGIN
    DECLARE @SQL NVARCHAR(MAX);
    DECLARE @Joins NVARCHAR(MAX) = '';
    DECLARE @Columns NVARCHAR(MAX) = '';

    -- Build column list and joins dynamically
    SELECT @Columns = STRING_AGG(QUOTENAME(c.TABLE_NAME) + '.' + QUOTENAME(c.COLUMN_NAME), ', ')
    FROM INFORMATION_SCHEMA.COLUMNS c
    WHERE c.TABLE_NAME = @TableName;

    -- Build joins for foreign keys
    SELECT @Joins = @Joins + 
        ' LEFT JOIN ' + QUOTENAME(fk.ReferencedTable) + 
        ' ON ' + QUOTENAME(@TableName) + '.' + QUOTENAME(fk.FKColumn) + 
        ' = ' + QUOTENAME(fk.ReferencedTable) + '.' + QUOTENAME(fk.ReferencedColumn)
    FROM (
        SELECT 
            fk.name AS FKName,
            OBJECT_NAME(fk.parent_object_id) AS TableName,
            COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS FKColumn,
            OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable,
            COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumn
        FROM sys.foreign_keys fk
        INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
        WHERE OBJECT_NAME(fk.parent_object_id) = @TableName
    ) fk;

    SET @SQL = 'SELECT * FROM ' + QUOTENAME(@TableName) + @Joins;
    
    EXEC sp_executesql @SQL;
END
GO

-- Generic Get By Id with Foreign Key Data
CREATE OR ALTER PROCEDURE sp_Generic_GetById
    @TableName NVARCHAR(128),
    @IdColumn NVARCHAR(128),
    @Id INT
AS
BEGIN
    DECLARE @SQL NVARCHAR(MAX);
    DECLARE @Joins NVARCHAR(MAX) = '';

    -- Build joins for foreign keys
    SELECT @Joins = @Joins + 
        ' LEFT JOIN ' + QUOTENAME(fk.ReferencedTable) + 
        ' ON ' + QUOTENAME(@TableName) + '.' + QUOTENAME(fk.FKColumn) + 
        ' = ' + QUOTENAME(fk.ReferencedTable) + '.' + QUOTENAME(fk.ReferencedColumn)
    FROM (
        SELECT 
            OBJECT_NAME(fk.parent_object_id) AS TableName,
            COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS FKColumn,
            OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable,
            COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumn
        FROM sys.foreign_keys fk
        INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
        WHERE OBJECT_NAME(fk.parent_object_id) = @TableName
    ) fk;

    SET @SQL = 'SELECT * FROM ' + QUOTENAME(@TableName) + @Joins + 
               ' WHERE ' + QUOTENAME(@TableName) + '.' + QUOTENAME(@IdColumn) + ' = @Id';
    
    EXEC sp_executesql @SQL, N'@Id INT', @Id;
END
GO

-- Generic Insert
CREATE OR ALTER PROCEDURE sp_Generic_Insert
    @TableName NVARCHAR(128),
    @Columns NVARCHAR(MAX),
    @Values NVARCHAR(MAX)
AS
BEGIN
    DECLARE @SQL NVARCHAR(MAX);
    
    SET @SQL = 'INSERT INTO ' + QUOTENAME(@TableName) + ' (' + @Columns + ') VALUES (' + @Values + '); SELECT SCOPE_IDENTITY() AS Id;';
    
    EXEC sp_executesql @SQL;
END
GO

-- Generic Update
CREATE OR ALTER PROCEDURE sp_Generic_Update
    @TableName NVARCHAR(128),
    @SetClause NVARCHAR(MAX),
    @IdColumn NVARCHAR(128),
    @Id INT
AS
BEGIN
    DECLARE @SQL NVARCHAR(MAX);
    
    SET @SQL = 'UPDATE ' + QUOTENAME(@TableName) + ' SET ' + @SetClause + 
               ' WHERE ' + QUOTENAME(@IdColumn) + ' = @Id';
    
    EXEC sp_executesql @SQL, N'@Id INT', @Id;
END
GO

-- Generic Delete
CREATE OR ALTER PROCEDURE sp_Generic_Delete
    @TableName NVARCHAR(128),
    @IdColumn NVARCHAR(128),
    @Id INT
AS
BEGIN
    DECLARE @SQL NVARCHAR(MAX);
    
    SET @SQL = 'DELETE FROM ' + QUOTENAME(@TableName) + ' WHERE ' + QUOTENAME(@IdColumn) + ' = @Id';
    
    EXEC sp_executesql @SQL, N'@Id INT', @Id;
END
GO

-- Generic Find with Condition
CREATE OR ALTER PROCEDURE sp_Generic_Find
    @TableName NVARCHAR(128),
    @WhereClause NVARCHAR(MAX)
AS
BEGIN
    DECLARE @SQL NVARCHAR(MAX);
    DECLARE @Joins NVARCHAR(MAX) = '';

    -- Build joins for foreign keys
    SELECT @Joins = @Joins + 
        ' LEFT JOIN ' + QUOTENAME(fk.ReferencedTable) + 
        ' ON ' + QUOTENAME(@TableName) + '.' + QUOTENAME(fk.FKColumn) + 
        ' = ' + QUOTENAME(fk.ReferencedTable) + '.' + QUOTENAME(fk.ReferencedColumn)
    FROM (
        SELECT 
            OBJECT_NAME(fk.parent_object_id) AS TableName,
            COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS FKColumn,
            OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable,
            COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumn
        FROM sys.foreign_keys fk
        INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
        WHERE OBJECT_NAME(fk.parent_object_id) = @TableName
    ) fk;

    SET @SQL = 'SELECT * FROM ' + QUOTENAME(@TableName) + @Joins + ' WHERE ' + @WhereClause;
    
    EXEC sp_executesql @SQL;
END
GO

-- Generic Count
CREATE OR ALTER PROCEDURE sp_Generic_Count
    @TableName NVARCHAR(128),
    @WhereClause NVARCHAR(MAX) = NULL
AS
BEGIN
    DECLARE @SQL NVARCHAR(MAX);
    
    IF @WhereClause IS NULL OR @WhereClause = ''
        SET @SQL = 'SELECT COUNT(*) AS Count FROM ' + QUOTENAME(@TableName);
    ELSE
        SET @SQL = 'SELECT COUNT(*) AS Count FROM ' + QUOTENAME(@TableName) + ' WHERE ' + @WhereClause;
    
    EXEC sp_executesql @SQL;
END
GO

-- Generic Exists
CREATE OR ALTER PROCEDURE sp_Generic_Exists
    @TableName NVARCHAR(128),
    @WhereClause NVARCHAR(MAX)
AS
BEGIN
    DECLARE @SQL NVARCHAR(MAX);
    
    SET @SQL = 'SELECT CASE WHEN EXISTS(SELECT 1 FROM ' + QUOTENAME(@TableName) + 
               ' WHERE ' + @WhereClause + ') THEN 1 ELSE 0 END AS Exists';
    
    EXEC sp_executesql @SQL;
END
GO

-- Generic Get All with Include Properties
CREATE OR ALTER PROCEDURE sp_Generic_GetAllWithIncludes
    @TableName NVARCHAR(128),
    @IncludeProperties NVARCHAR(MAX)
AS
BEGIN
    DECLARE @SQL NVARCHAR(MAX);
    DECLARE @Joins NVARCHAR(MAX) = '';
    DECLARE @Property NVARCHAR(128);
    DECLARE @Pos INT;
    
    -- Parse include properties and build joins
    WHILE LEN(@IncludeProperties) > 0
    BEGIN
        SET @Pos = CHARINDEX(',', @IncludeProperties);
        
        IF @Pos = 0
        BEGIN
            SET @Property = LTRIM(RTRIM(@IncludeProperties));
            SET @IncludeProperties = '';
        END
        ELSE
        BEGIN
            SET @Property = LTRIM(RTRIM(SUBSTRING(@IncludeProperties, 1, @Pos - 1)));
            SET @IncludeProperties = SUBSTRING(@IncludeProperties, @Pos + 1, LEN(@IncludeProperties));
        END
        
        -- Build join for this property (assumes property name matches table name)
        IF LEN(@Property) > 0
        BEGIN
            -- Find foreign key relationship for this property
            SELECT @Joins = @Joins + 
                ' LEFT JOIN ' + QUOTENAME(@Property + 's') + 
                ' ON ' + QUOTENAME(@TableName) + '.' + QUOTENAME(@Property + 'Id') + 
                ' = ' + QUOTENAME(@Property + 's') + '.' + QUOTENAME(@Property + 'Id')
            WHERE EXISTS (
                SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = @TableName AND COLUMN_NAME = @Property + 'Id'
            );
        END
    END
    
    SET @SQL = 'SELECT * FROM ' + QUOTENAME(@TableName) + @Joins;
    
    EXEC sp_executesql @SQL;
END
GO
