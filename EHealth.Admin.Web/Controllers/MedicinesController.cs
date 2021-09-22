using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EHealth.Shared;
using EHealth.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using EHealth.Shared.Repositories;
using EHealth.Admin.Web.ViewModels;

namespace EHealth.Admin.Web.Controllers
{
    [Authorize]
    public class MedicinesController : Controller
    {
        private IRepository<Medicine> _medicineRepository;
        private IRepository<Category> _categoryRepository;

        public MedicinesController(
            IRepository<Medicine> medicineRepository,
            IRepository<Category> categoryRepository)
        {
            _medicineRepository = medicineRepository ?? throw new ArgumentNullException(nameof(medicineRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        // GET: Medicines
        public IActionResult Index()
        {
            var medicinesIndexViewModel = new MedicinesListViewModel(
                _medicineRepository.GetAll().ToList(),
                _categoryRepository.GetAll().ToList());

            return View(medicinesIndexViewModel);
        }

        public IActionResult Create()
        {
            return View(new NewMedicineViewModel(_categoryRepository.GetAll().ToList()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(NewMedicineViewModel newMedicineViewModel)
        {
            _medicineRepository.Insert(newMedicineViewModel.ToMedicine());
            return RedirectToAction("Index");
        }

        // GET: Categories/Edit/5
        public IActionResult Edit(long id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicine = _medicineRepository.Get(id);
            if (medicine == null)
            {
                return NotFound();
            }
            return View(medicine);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,CategoryId,Name,Description,Price,Image,Seller")] Medicine medicine)
        {
            if (id != medicine.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _medicineRepository.Update(medicine);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicineExists(medicine.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(medicine);
        }

        public IActionResult Delete(long id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicine = _medicineRepository.Get(id);
            //.FirstOrDefaultAsync(m => m.Id == id);
            if (medicine == null)
            {
                return NotFound();
            }

            return View(medicine);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var medicine = _medicineRepository.Get(id);
            _medicineRepository.Delete(medicine);
            return RedirectToAction("Index");
        }

        private bool MedicineExists(int id)
        {
            return _medicineRepository.GetAll().Any(medicine => medicine.Id == id);
        }
    }
}