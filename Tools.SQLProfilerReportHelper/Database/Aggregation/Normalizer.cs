using System;
using System.Threading.Tasks;
using Tools.SQLProfilerReportHelper.Database.Common;

namespace Tools.SQLProfilerReportHelper.Database.Aggregation
{
    public class Normalizer
    {
        public const string NormalizedTextDataColumn = "TextKey";
        public const string NormalizationFunctionName = "NormalizeTextData0";

        private readonly DbObjectsManager _dbManager;
        private readonly Sql _sql;

        public Normalizer(DbObjectsManager dbManager, Sql sql)
        {
            _dbManager = dbManager;
            _sql = sql;
        }

        public Task<bool> CheckPrepared(string tableName)
        {
            return _dbManager.IsColumnExistInTable(tableName, NormalizedTextDataColumn);
        }

        public async Task Prepare(string tableName)
        {
			if (!await _dbManager.IsTableExist(tableName))
				throw new ArgumentException($"Table [{tableName}] not exists.", nameof(tableName));

            if (!await _dbManager.IsColumnExistInTable(tableName, NormalizedTextDataColumn))
            {
                await CreateTextKey(tableName);
            }
            if (!await _dbManager.IsFunctionExists(NormalizationFunctionName))
            {
                await CreateTextDataNormalizationFunction();
            }
        }

        private Task CreateTextKey(string tableName)
        {
            return _sql.ExecuteNonQueryAsync(600, string.Format(@"
BEGIN TRANSACTION
ALTER TABLE [dbo].[{0}] ADD
	[TextKey] varchar(1000) NULL
ALTER TABLE [dbo].[{0}] SET (LOCK_ESCALATION = TABLE)
COMMIT
", tableName));
        }

        private Task CreateTextDataNormalizationFunction()
        {
            return _sql.ExecuteNonQueryAsync(600, @"
CREATE FUNCTION [dbo].[" + NormalizationFunctionName + @"]
(
	@textData varchar(2000)
)
RETURNS varchar(400)
AS
BEGIN
	
	if(@textData is NULL)
		return 'NULL'

	-- Declare the return variable here
	DECLARE @textKey NVARCHAR(2000)

	DECLARE @replaceTextIndex int

	-- Замена перевода строки, табуляции на пробел
	SET @textKey = 
	    Replace(Replace(Replace(
	    @textData,
	    char(9), ' '), char(10), ' '), char(13), ' ')

	SET @textKey = 
	    Replace(Replace(
	    @textKey
	   , 'varchar(max)', 'varchar(9)')
	   , 'varbinary(max)', 'varbinary(9)')

	-- Подготовка к обработке бинарных констант
	SET @textKey = 
		Replace(Replace(
		@textKey,
		'=0x', '={HEX0xFF*}'),'= 0x', '= {HEX0xFF*}')

	-- Подготовка к обработке числовых констант с ведущим символом присваивания
	SET @textKey = 
	    Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(
	    @textKey,
	    '=0', N'=№'), '=1', '=№'), '=2', '=№'), '=3', '=№'), '=4', N'=№'), '=5', N'=№'), '=6', N'№'), '=7', N'=№'), '=8', N'=№'), '=9', N'=№')

	-- Подготовка к обработке числовых констант
	SET @textKey = 
	    Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(
	    @textKey,
	    ' 0', N' №'), ' 1', N' №'), ' 2', N' №'), ' 3', N' №'), ' 4', N' №'), ' 5', N' №'), ' 6', N' №'), ' 7', N' №'), ' 8', N' №'), ' 9', N' №')

	-- Подготовка к обработке чисел в скобках
	SET @textKey = 
	    Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(
	    @textKey,
	    '(0', N'(№'), '(1', N'(№'), '(2', N'(№'), '(3', N'(№'), '(4', N'(№'), '(5', N'(№'), '(6', N'(№'), '(7', N'(№'), '(8', N'(№'), '(9', N'(№')

	-- Обработка числовых констант
	SET @replaceTextIndex = 1
	WHILE @replaceTextIndex > 0
	BEGIN
		SET @textKey=
			Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(
			@textKey,
			N'№0', N'№'),	N'№1', N'№'),	N'№2', N'№'),	N'№3', N'№'),	N'№4', N'№'),	N'№5', N'№'),	N'№6', N'№'),	N'№7', N'№'),	N'№8', N'№'),	N'№9', N'№'),	N'№№', N'№')
		
		SET @replaceTextIndex = 
		    CHARINDEX(N'№0', @textKey)+
			CHARINDEX(N'№1', @textKey)+
			CHARINDEX(N'№2', @textKey)+
			CHARINDEX(N'№3', @textKey)+
			CHARINDEX(N'№4', @textKey)+
			CHARINDEX(N'№5', @textKey)+
			CHARINDEX(N'№6', @textKey)+
			CHARINDEX(N'№7', @textKey)+
			CHARINDEX(N'№8', @textKey)+
			CHARINDEX(N'№9', @textKey)+
			CHARINDEX(N'№№', @textKey)
	END

	-- Подготовка к обработке дробной части числовых констант
	SET @textKey = 
	    Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(
	    @textKey,
	    N'№.0', N'№.№'), N'№.1', N'№.№'), N'№.2', N'№.№'), N'№.3', N'№.№'), N'№.4', N'№.№'), N'№.5', N'№.№'), N'№.6', N'№.№'), N'№.7', N'№.№'), N'№.8', N'№.№'), N'№.9', N'№.№')

	-- Обработка дробной части числовых констант
	SET @replaceTextIndex = 1
	WHILE @replaceTextIndex > 0
	BEGIN

		SET @textKey=
			Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(
			@textKey,
			N'№0', N'№'),	N'№1', N'№'),	N'№2', N'№'),	N'№3', N'№'),	N'№4', N'№'),	N'№5', N'№'),	N'№6', N'№'),	N'№7', N'№'),	N'№8', N'№'),	N'№9', N'№'),	N'№№', N'№')
		
		SET @replaceTextIndex = 
		    CHARINDEX(N'№0', @textKey)+
			CHARINDEX(N'№1', @textKey)+
			CHARINDEX(N'№2', @textKey)+
			CHARINDEX(N'№3', @textKey)+
			CHARINDEX(N'№4', @textKey)+
			CHARINDEX(N'№5', @textKey)+
			CHARINDEX(N'№6', @textKey)+
			CHARINDEX(N'№7', @textKey)+
			CHARINDEX(N'№8', @textKey)+
			CHARINDEX(N'№9', @textKey)+
			CHARINDEX(N'№№', @textKey)
	END

	SET @textKey=Replace(@textKey, N'№', '9')

	-- Обработка бинарных констант
	SET @replaceTextIndex = 1
	WHILE @replaceTextIndex > 0
	BEGIN

		SET @textKey=
			Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(
			@textKey,
			'{HEX0xFF*}0', '{HEX0xFF*}'),
			'{HEX0xFF*}1', '{HEX0xFF*}'),
			'{HEX0xFF*}2', '{HEX0xFF*}'),
			'{HEX0xFF*}3', '{HEX0xFF*}'),
			'{HEX0xFF*}4', '{HEX0xFF*}'),
			'{HEX0xFF*}5', '{HEX0xFF*}'),
			'{HEX0xFF*}6', '{HEX0xFF*}'),
			'{HEX0xFF*}7', '{HEX0xFF*}'),
			'{HEX0xFF*}8', '{HEX0xFF*}'),
			'{HEX0xFF*}9', '{HEX0xFF*}'),
			'{HEX0xFF*}A', '{HEX0xFF*}'),
			'{HEX0xFF*}B', '{HEX0xFF*}'),
			'{HEX0xFF*}C', '{HEX0xFF*}'),
			'{HEX0xFF*}D', '{HEX0xFF*}'),
			'{HEX0xFF*}E', '{HEX0xFF*}'),
			'{HEX0xFF*}F', '{HEX0xFF*}')
		
		SET @replaceTextIndex = 
			CHARINDEX('{HEX0xFF*}0', @textKey)+
			CHARINDEX('{HEX0xFF*}1', @textKey)+
			CHARINDEX('{HEX0xFF*}2', @textKey)+
		    CHARINDEX('{HEX0xFF*}3', @textKey)+
			CHARINDEX('{HEX0xFF*}4', @textKey)+
			CHARINDEX('{HEX0xFF*}5', @textKey)+
			CHARINDEX('{HEX0xFF*}6', @textKey)+
			CHARINDEX('{HEX0xFF*}7', @textKey)+
			CHARINDEX('{HEX0xFF*}8', @textKey)+
			CHARINDEX('{HEX0xFF*}9', @textKey)+
			CHARINDEX('{HEX0xFF*}A', @textKey)+
			CHARINDEX('{HEX0xFF*}B', @textKey)+
			CHARINDEX('{HEX0xFF*}C', @textKey)+
			CHARINDEX('{HEX0xFF*}D', @textKey)+
			CHARINDEX('{HEX0xFF*}E', @textKey)+
			CHARINDEX('{HEX0xFF*}F', @textKey)
	END
	set @textKey = Replace(@textKey, '{HEX0xFF*}', '0xFF')

	DECLARE @startPosition int
	DECLARE @startPositionTemp int
	DECLARE @endPosition int
	DECLARE @searchPatternStart varchar(10)
	DECLARE @searchPatternEnd varchar(10)
	DECLARE @isFound bit
	DECLARE @replaceStr varchar(10)

	set @replaceStr = '*'

	set @textKey = Replace(@textKey, '= ''', '=''')
	set @textKey = Replace(@textKey, '= N''', '=N''')

	-- Обработка строковых констант с ведущим символом присваивания
	set @endPosition = -1
	set @isFound = 1
	set @searchPatternStart = '='''
	set @searchPatternEnd = ''''
	WHILE @isFound = 1
	BEGIN
		set @isFound = 0
		set @startPositionTemp = CHARINDEX(@searchPatternStart, @textKey, @endPosition+1)
		IF @startPositionTemp > 0
		BEGIN
			set @startPosition = @startPositionTemp
			set @endPosition = CHARINDEX(@searchPatternEnd, @textKey, @startPosition + len(@searchPatternStart))
			IF @endPosition = 0
			BEGIN
				set @endPosition = LEN(@textKey)+1
			END

			set @isFound = 1
			set @textKey = LEFT(@textKey, @startPosition+len(@searchPatternStart)-1) + @replaceStr + RIGHT(@textKey, len(@textKey)-@endPosition+1)
			set @endPosition = @startPosition + len(@searchPatternStart) + len(@replaceStr) + len(@searchPatternEnd)

		END
	END

	-- Обработка строковых unicode-констант с ведущим символом присваивания
	set @endPosition = -1
	set @isFound = 1
	set @searchPatternStart = '=N'''
	set @searchPatternEnd = ''''
	WHILE @isFound = 1
	BEGIN
		set @isFound = 0
		set @startPositionTemp = CHARINDEX(@searchPatternStart, @textKey, @endPosition+1)
		IF @startPositionTemp > 0
		BEGIN
			set @startPosition = @startPositionTemp
			set @endPosition = CHARINDEX(@searchPatternEnd, @textKey, @startPosition + len(@searchPatternStart))
			IF @endPosition = 0
			BEGIN
				set @endPosition = LEN(@textKey)+1
			END

			set @isFound = 1
			set @textKey = LEFT(@textKey, @startPosition+len(@searchPatternStart)-1) + @replaceStr + RIGHT(@textKey, len(@textKey)-@endPosition+1)
			set @endPosition = @startPosition + len(@searchPatternStart) + len(@replaceStr) + len(@searchPatternEnd)
		END
	END

	-- Обработка строковых констант с ведущим пробелом, которые вложены в строковых константы
	set @endPosition = -1
	set @isFound = 1
	set @searchPatternStart = ' '''''
	set @searchPatternEnd = ''''''
	WHILE @isFound = 1
	BEGIN
		set @isFound = 0
		set @startPositionTemp = CHARINDEX(@searchPatternStart, @textKey, @endPosition+1)
		IF @startPositionTemp > 0
		BEGIN
			set @startPosition = @startPositionTemp
			set @endPosition = CHARINDEX(@searchPatternEnd, @textKey, @startPosition + len(@searchPatternStart))
			IF @endPosition = 0
			BEGIN
				set @endPosition = LEN(@textKey)+1
			END

			set @isFound = 1
			set @textKey = LEFT(@textKey, @startPosition+len(@searchPatternStart)-1) + @replaceStr + RIGHT(@textKey, len(@textKey)-@endPosition+1)
			set @endPosition = @startPosition + len(@searchPatternStart) + len(@replaceStr) + len(@searchPatternEnd)
		END
	END

	SET @textKey = 
	    Replace(
	    @textKey,
	    ' ', '')

	SET @textKey = LEFT(@textKey, 400)
	if RIGHT ( @textKey , 1 ) = '*'
	BEGIN
		SET @textKey = @textKey + ''''
	END

	RETURN @textKey
END");
        }
    }
}
