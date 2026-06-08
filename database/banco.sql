USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SoftlineDB')
    CREATE DATABASE SoftlineDB;
GO

USE SoftlineDB;
GO

-- Tabela de Usuários
CREATE TABLE Usuarios (
    Id       INT IDENTITY(1,1) PRIMARY KEY,
    Usuario  NVARCHAR(50)  NOT NULL,
    Senha    NVARCHAR(100) NOT NULL
);
GO

-- Tabela de Produtos
CREATE TABLE Produtos (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    Codigo       INT            NOT NULL,
    Descricao    NVARCHAR(60)   NOT NULL,
    CodigoBarras NVARCHAR(14)   NULL,
    ValorVenda   DECIMAL(18,2)  NOT NULL,
    PesoBruto    DECIMAL(18,3)  NOT NULL,
    PesoLiquido  DECIMAL(18,3)  NOT NULL
);
GO

-- Tabela de Clientes
CREATE TABLE Clientes (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    Codigo    INT            NOT NULL,
    Nome      NVARCHAR(60)   NOT NULL,
    Fantasia  NVARCHAR(100)  NULL,
    Documento NVARCHAR(18)   NOT NULL,
    Endereco  NVARCHAR(255)  NULL
);
GO

-- Usuário padrão (senha: 123456)
INSERT INTO Usuarios (Usuario, Senha) VALUES ('admin', '123456');
GO
