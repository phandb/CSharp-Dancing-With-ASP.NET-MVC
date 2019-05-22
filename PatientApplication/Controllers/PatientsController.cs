﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PatientApplication.Models;
using PatientApplication.ViewModels;

namespace PatientApplication.Controllers
{
    public class PatientsController : Controller
    {
        private ApplicationDbContext _context;

        public PatientsController()
        {
            _context = new ApplicationDbContext();
        }

        //Dispose the db context object
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();    
        }

        //GET Patients
        public ActionResult Index()
        {
            var patients = _context.Patients.ToList();

            return View(patients);
        }
        
        //GET Patients/Details/{id}
        public ActionResult Details(int patientId)
        {
            var patient = _context.Patients
                            .Include(p=>p.Medications)
                            .Include(p=>p.Pharmacies)
                            .Include(p=>p.Physcians)
                            .SingleOrDefault(p => p.Id == patientId);

            if (patient == null)
                return HttpNotFound();

            return View(patient);
                
        }

        public ActionResult Edit(int patientId)
        {
            var patient = _context.Patients.SingleOrDefault(p => p.Id == patientId);

            //check if patient exist
            if (patient == null)
                return HttpNotFound();

            var viewModel = new PatientViewModel
            {
                Patient = patient

            };

            return View("PatientForm", viewModel);
        }

        public ActionResult NewPatient()
        {
            var viewModel = new PatientViewModel { };

            return View("PatientForm", viewModel);
        }

        //Read and Save data from new patient form
        [HttpPost]
        public ActionResult Save(Patient patient)
        {
            //Check if it is new patient
            if( patient.Id == 0){
                //add new patient to database
                _context.Patients.Add(patient);
            }

            //otherwise update patient
            else
            {
                var patientInDb = _context.Patients.Single(p => p.Id == patient.Id);

                patientInDb.Name = patient.Name;
                patientInDb.Address = patient.Address;
                patientInDb.Birthdate = patient.Birthdate;
                patientInDb.Gender = patient.Gender;
               

            }

            //persist the change to database
            _context.SaveChanges();

            //redirect to index in the PatientsController
            return RedirectToAction("Index", "Patients");
        }

        //GET Patients/Delete/{id}
        
        public ActionResult Delete(int patientId)
        {
            Patient patient = _context.Patients.Find(patientId);

            //check if patient exist
            if (patient == null)
                return HttpNotFound();

            

            return View(patient);
        }

        //POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]        
        public ActionResult DeleteConfirmed(int patientId)
        {
            Patient patient = _context.Patients.Find(patientId);
            _context.Patients.Remove(patient);
            _context.SaveChanges();

            return RedirectToAction("Index", "Patients");

        }

        public ActionResult GetPhysicianList(int patientId)
        {
            List<Physician> physicians = new List<Physician>();
            physicians = _context.Physicians.ToList();

            Patient thePatient = _context.Patients.Find(patientId);

            if (thePatient == null)
            {
                return HttpNotFound();
            }
            
            var viewModel = new AssignedPhysicianData
            {
                Patient = thePatient,
                Physicians = physicians
                
            };
            
            return View("PhysicianList", viewModel );
        }


        

    }
}
