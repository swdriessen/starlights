global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Reqnroll;

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel, Workers = 8)]