CREATE TABLE [dbo].[T_CacheUAFiglie] (
    [CodUA]       VARCHAR (20) NOT NULL,
    [CodUAFiglia] VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_T_CacheUAFiglie] PRIMARY KEY CLUSTERED ([CodUA] ASC, [CodUAFiglia] ASC)
);

