namespace Tutorial3Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using Tutorial3.Models;
using Xunit;

public class AdvancedEmpDeptTests
{
    [Fact]
    public void ShouldReturnMaxSalary()
    {
        var emps = Database.GetEmps();
        decimal? maxSalary = emps.Max(e => e.Sal);
        Assert.Equal(5000, maxSalary);
    }

    [Fact]
    public void ShouldReturnMinSalaryInDept30()
    {
        var emps = Database.GetEmps();
        decimal? minSalary = emps.Where(e => e.DeptNo == 30).Min(e => e.Sal);
        Assert.Equal(1250, minSalary);
    }

    [Fact]
    public void ShouldReturnFirstTwoHiredEmployees()
    {
        var emps = Database.GetEmps();
        var firstTwo = emps.OrderBy(e => e.HireDate).Take(2).ToList();
        Assert.Equal(2, firstTwo.Count);
        Assert.True(firstTwo[0].HireDate <= firstTwo[1].HireDate);
    }

    [Fact]
    public void ShouldReturnDistinctJobTitles()
    {
        var emps = Database.GetEmps();
        var jobs = emps.Select(e => e.Job).Distinct().ToList();
        Assert.Contains("PRESIDENT", jobs);
        Assert.Contains("SALESMAN", jobs);
    }

    [Fact]
    public void ShouldReturnEmployeesWithManagers()
    {
        var emps = Database.GetEmps();
        var withMgr = emps.Where(e => e.Mgr.HasValue).ToList();
        Assert.All(withMgr, e => Assert.NotNull(e.Mgr));
    }

    [Fact]
    public void AllEmployeesShouldEarnMoreThan500()
    {
        var emps = Database.GetEmps();
        var result = emps.All(e => e.Sal > 500);
        Assert.True(result);
    }

    [Fact]
    public void ShouldFindAnyWithCommissionOver400()
    {
        var emps = Database.GetEmps();
        var result = emps.Any(e => e.Comm.HasValue && e.Comm > 400);
        Assert.True(result);
    }

    [Fact]
    public void ShouldReturnEmployeeManagerPairs()
    {
        var emps = Database.GetEmps();
        var result = emps.Join(emps, e => e.Mgr, m => m.EmpNo,
            (e, m) => new { Employee = e.EName, Manager = m.EName }).ToList();
        Assert.Contains(result, r => r.Employee == "SMITH" && r.Manager == "FORD");
    }

    [Fact]
    public void ShouldReturnTotalIncomeIncludingCommission()
    {
        var emps = Database.GetEmps();
        var result = emps.Select(e => new {
            e.EName,
            Total = e.Sal + (e.Comm ?? 0)
        }).ToList();
        Assert.Contains(result, r => r.EName == "ALLEN" && r.Total == 1900);
    }

    [Fact]
    public void ShouldJoinEmpWithDeptAndSalgrade()
    {
        var emps = Database.GetEmps();
        var depts = Database.GetDepts();
        var grades = Database.GetSalgrades();

        var result = (from e in emps
                      join d in depts on e.DeptNo equals d.DeptNo
                      from s in grades
                      where e.Sal >= s.Losal && e.Sal <= s.Hisal
                      select new { e.EName, d.DName, s.Grade }).ToList();

        Assert.Contains(result, r => r.EName == "ALLEN" && r.DName == "SALES" && r.Grade == 3);
    }
}
