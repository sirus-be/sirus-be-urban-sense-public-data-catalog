create database datacatalog;
create user datacatalog with encrypted password '$(password)'; 
grant all privileges on database datacatalog to datacatalog;
