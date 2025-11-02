using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace MarketHustle.Gameplay
{
    public enum EmployeeRole
    {
        Cashier,
        StockClerk,
        Manager,
        MarketingSpecialist
    }

    [System.Serializable]
    public class Employee
    {
        public string name;
        public EmployeeRole role;
        public int skillLevel = 1; // 1-10
        public float salary = 1000f; // Monthly
        public float loyalty = 50f; // 0-100, affects retention
        public float morale = 50f; // 0-100, affects performance
        public float performance = 1f; // Multiplier for work effectiveness

        // Personal demands
        public float desiredSalary = 1000f;
        public bool wantsPromotion = false;
        public bool wantsPartnership = false;

        public UnityEvent OnPromotion;
        public UnityEvent OnQuit;
    }

    /// <summary>
    /// Manages employees with skills, morale, loyalty, and partnership opportunities.
    /// </summary>
    public class EmployeeSystem : MonoBehaviour
    {
        [Header("Employee Management")]
        public List<Employee> employees;
        public int maxEmployees = 5;

        [Header("Hiring")]
        public float baseHiringCost = 500f;
        public List<string> firstNames = new List<string> { "Alex", "Jordan", "Taylor", "Morgan", "Casey" };
        public List<string> lastNames = new List<string> { "Smith", "Johnson", "Williams", "Brown", "Davis" };

        public UnityEvent<Employee> OnEmployeeHired;
        public UnityEvent<Employee> OnEmployeeQuit;
        public UnityEvent<Employee> OnPartnershipFormed;

        void Start()
        {
            if (employees == null)
            {
                employees = new List<Employee>();
            }
        }

        void Update()
        {
            // Simulate employee morale changes
            foreach (var employee in employees)
            {
                // Morale decays slowly
                employee.morale = Mathf.Max(0, employee.morale - 0.01f * Time.deltaTime);

                // Performance based on morale and skill
                employee.performance = (employee.morale / 100f) * (employee.skillLevel / 10f);

                // Check for quitting
                if (employee.loyalty < 20f && Random.value < 0.001f) // 0.1% chance per frame
                {
                    FireEmployee(employee, false); // They quit
                }
            }
        }

        public bool HireEmployee(EmployeeRole role)
        {
            if (employees.Count >= maxEmployees) return false;

            var economy = MarketHustle.Economy.EconomyManager.Instance;
            if (economy == null || !economy.TrySpend((long)baseHiringCost)) return false;

            var newEmployee = new Employee
            {
                name = GenerateName(),
                role = role,
                skillLevel = Random.Range(1, 6), // 1-5 starting skill
                salary = GetBaseSalary(role),
                desiredSalary = GetBaseSalary(role) * Random.Range(1f, 1.3f),
                loyalty = Random.Range(40f, 70f),
                morale = Random.Range(50f, 80f)
            };

            employees.Add(newEmployee);
            OnEmployeeHired?.Invoke(newEmployee);

            return true;
        }

        public void FireEmployee(Employee employee, bool firedByPlayer = true)
        {
            if (firedByPlayer)
            {
                employee.loyalty = 0f;
                // Reputation hit for firing employees
                var customerSystem = FindObjectOfType<CustomerSystem>();
                if (customerSystem != null)
                {
                    // Small reputation penalty
                }
            }

            employees.Remove(employee);
            OnEmployeeQuit?.Invoke(employee);
        }

        public void PromoteEmployee(Employee employee)
        {
            if (employee.skillLevel < 10)
            {
                employee.skillLevel++;
                employee.salary *= 1.2f;
                employee.morale += 20f;
                employee.loyalty += 10f;

                employee.OnPromotion?.Invoke();
            }
        }

        public void OfferRaise(Employee employee, float raiseAmount)
        {
            var economy = MarketHustle.Economy.EconomyManager.Instance;
            if (economy != null && economy.TrySpend((long)raiseAmount))
            {
                employee.salary += raiseAmount;
                employee.loyalty += 15f;
                employee.morale += 10f;
            }
        }

        public void OfferPartnership(Employee employee)
        {
            if (employee.skillLevel >= 7 && employee.loyalty >= 80f)
            {
                // Create partnership - employee manages a new store location
                // Give them a percentage of profits
                employee.wantsPartnership = false;
                OnPartnershipFormed?.Invoke(employee);
            }
        }

        public void ProcessPayroll()
        {
            var economy = MarketHustle.Economy.EconomyManager.Instance;
            if (economy == null) return;

            long totalPayroll = 0;
            foreach (var employee in employees)
            {
                totalPayroll += (long)employee.salary;
            }

            economy.TrySpend(totalPayroll);
        }

        public float GetTotalEmployeeEfficiency()
        {
            float totalEfficiency = 0f;
            foreach (var employee in employees)
            {
                totalEfficiency += employee.performance;
            }
            return totalEfficiency;
        }

        public List<Employee> GetEmployeesByRole(EmployeeRole role)
        {
            return employees.FindAll(e => e.role == role);
        }

        private string GenerateName()
        {
            string first = firstNames[Random.Range(0, firstNames.Count)];
            string last = lastNames[Random.Range(0, lastNames.Count)];
            return $"{first} {last}";
        }

        private float GetBaseSalary(EmployeeRole role)
        {
            switch (role)
            {
                case EmployeeRole.Cashier: return 800f;
                case EmployeeRole.StockClerk: return 900f;
                case EmployeeRole.Manager: return 1500f;
                case EmployeeRole.MarketingSpecialist: return 1200f;
                default: return 1000f;
            }
        }

        public void CheckEmployeeDemands()
        {
            foreach (var employee in employees)
            {
                // Check if they want a raise
                if (employee.salary < employee.desiredSalary && Random.value < 0.1f)
                {
                    // Employee requests raise
                    // This would trigger UI notification
                }

                // Check if they want promotion
                if (employee.skillLevel < 10 && Random.value < 0.05f)
                {
                    employee.wantsPromotion = true;
                }

                // Check if they want partnership
                if (employee.skillLevel >= 7 && employee.loyalty >= 80f && Random.value < 0.02f)
                {
                    employee.wantsPartnership = true;
                }
            }
        }
    }
}