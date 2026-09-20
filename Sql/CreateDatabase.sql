--Creates database if there isn't already created for SQL SERVER
-- Jonas Balante
-- 9/20/2026

USE master;
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SimpleWebsiteDB')
BEGIN
    CREATE DATABASE SimpleWebsiteDB;
END