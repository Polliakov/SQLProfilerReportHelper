USE [master]; 
CREATE LOGIN [monitor] 
    WITH PASSWORD=N'monitor', 
    DEFAULT_DATABASE=[master], 
    CHECK_EXPIRATION=OFF, 
    CHECK_POLICY=OFF;

GRANT ALTER TRACE TO [monitor]; 

GO


USE [PerfomanceTests];

CREATE USER [monitor] FOR LOGIN [monitor] 
    WITH DEFAULT_SCHEMA=[dbo];

ALTER ROLE [db_owner] ADD MEMBER [monitor];

GO


