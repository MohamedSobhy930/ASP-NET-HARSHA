using Azure.Identity;
using CsvHelper;
using Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using ServiceContacts;
using ServiceContacts.DTOs.CountryDto;
using ServiceContacts.DTOs.PersonDto;
using ServiceContacts.Enums;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepoContracts;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class PersonService(IPersonsRepo personsRepo, ILogger<PersonService> logger) : IPersonService
    {
        private readonly IPersonsRepo _personRepo = personsRepo;
        private readonly ILogger<PersonService> _logger = logger;

        public async Task<PersonResponse> AddPerson(PersonAddRequest? request)
        {
            if(request == null)
                throw new ArgumentNullException();

            ValidationHelper.ModelValidation(request);

            Person person = request.ToPerson();
            person.PersonId = Guid.NewGuid();
            await _personRepo.AddPerson(person);
            // using sp
            //_db.sp_InsertPerson(person);
            return person.ToPersonResponse();
        }
        public async Task<bool> DeletePerson(Guid? id)
        {
            if(id == null) 
                throw new ArgumentNullException(nameof(id));
            Person? person =await _personRepo.GetPersonById(id.Value);
            if (person == null)
                return false;
            return await _personRepo.DeletePerson(person.PersonId);
        }
        public async Task<List<PersonResponse>> GetAllPersons()
        {
            _logger.LogInformation("Get All Persons in the PersonService");
            var persons = await _personRepo.GetAllPersons();
            
            return persons
                .Select(person => person.ToPersonResponse())
                .ToList();
            
            /*
             * STORED PROCEDURE
             * 1 - add migration then create the sp then update database 
             * 2 - use it inside the service instead of traditional retrieving 
             */
            //return _db.Database
            //    .SqlQuery<PersonResponse>($"EXEC sp_GetAllPersons")
            //    .ToList();
        }
        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchPhrase)
        {
            _logger.LogInformation("Get Filtered Persons in the PersonService");

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchPhrase))
            {
                var allPersons = await _personRepo.GetAllPersons();
                return allPersons.Select(temp => temp.ToPersonResponse()).ToList();
            }
            List<Person> persons = searchBy.Trim().ToLower() switch
            {
                "name" =>
                    await _personRepo
                    .GetFilteredPersons
                    (p => p.Name.Contains(searchPhrase)),

                "address" =>
                    await _personRepo
                    .GetFilteredPersons
                    (p => p.Address.Contains(searchPhrase)),

                "country" =>
                    await _personRepo
                    .GetFilteredPersons
                    (p => p.Country.Name.Contains(searchPhrase)),

                "dateofbirth" =>
                    DateTime.TryParse(searchPhrase, out DateTime searchDate)
                        ? await _personRepo
                            .GetFilteredPersons
                            (p => p.DateOfBirth.HasValue && p.DateOfBirth.Value.Date == searchDate.Date)
                        : new List<Person>(),

                _ => await _personRepo.GetAllPersons()
            };
            return persons.Select(temp => temp.ToPersonResponse()).ToList();
        }
        public async Task<PersonResponse?> GetPersonById(Guid? id)
        {
            if (id == null)
                return null;
            var person =await _personRepo.GetPersonById(id.Value);
            if (person == null)
                return null;
            return person.ToPersonResponse();

        }
        public List<PersonResponse> GetSortedPersons(List<PersonResponse> persons, string sortBy, SortDirectionOptions sortDirection)
        {
            _logger.LogInformation("Get Sorted Persons in the PersonService");

            if (string.IsNullOrEmpty(sortBy))
                return persons;
            var sortedPersons = persons;
            switch (sortBy.Trim().ToLower())
            {
                case "name":
                    sortedPersons = (sortDirection == SortDirectionOptions.ASC)
                        ? persons.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase).ToList()
                        : persons.OrderByDescending(p => p.Name, StringComparer.OrdinalIgnoreCase).ToList();
                    break;

                case "age":
                    sortedPersons = (sortDirection == SortDirectionOptions.ASC)
                        ? persons.OrderBy(p => p.Age).ToList()
                        : persons.OrderByDescending(p => p.Age).ToList();
                    break;

                case "country":
                    sortedPersons = (sortDirection == SortDirectionOptions.ASC)
                        ? persons.OrderBy(p => p.Country).ToList()
                        : persons.OrderByDescending(p => p.Country).ToList();
                    break;

                case "dateofbirth":
                    sortedPersons = (sortDirection == SortDirectionOptions.ASC)
                        ? persons.OrderBy(p => p.DateOfBirth).ToList()
                        : persons.OrderByDescending(p => p.DateOfBirth).ToList();
                    break;

                default:
                    break;
            }
            return sortedPersons;
        }
        public async Task<PersonResponse?> UpdatePerson(PersonUpdateRequest? request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            ValidationHelper.ModelValidation(request);
            Person? personFromDb = await _personRepo.GetPersonById(request.Id);
            personFromDb.Name = request.Name;
            personFromDb.Email = request.Email;
            personFromDb.CountryId = request.CountryId;
            await _personRepo.UpdatePerson(personFromDb);

            return personFromDb.ToPersonResponse();
        }
        public async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream = new MemoryStream();

            await using (StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, -1, leaveOpen: true))
            await using (CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteField("Id");
                csvWriter.WriteField("Name");
                csvWriter.WriteField("Email");
                csvWriter.WriteField("DateOfBirth");
                //csvWriter.WriteField("Gender");
                csvWriter.WriteField("Country");
                csvWriter.WriteField("Address");
                csvWriter.WriteField("ReceiveNewsletter");
                await csvWriter.NextRecordAsync();

                List<PersonResponse> persons = (await _personRepo.GetAllPersons())
                    .Select(temp => temp.ToPersonResponse())
                    .ToList();

                foreach(PersonResponse person in persons)
                {
                    csvWriter.WriteField(person.Id);
                    csvWriter.WriteField(person.Name);
                    csvWriter.WriteField(person.Email);
                    csvWriter.WriteField(person.DateOfBirth?.ToString("yyyy-MM-dd"));
                    //csvWriter.WriteField(person.Gender);
                    csvWriter.WriteField(person.Country);
                    csvWriter.WriteField(person.Address);
                    csvWriter.WriteField(person.ReceiveNewsletter == true ? "Yes" : "No");

                    await csvWriter.NextRecordAsync();
                }
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
        public async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();

            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                // 1- add work sheet 
                ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add("Persons");

                //2- add headers 
                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Email";
                worksheet.Cells[1, 3].Value = "Date of Birth";
                worksheet.Cells[1, 4].Value = "Age";
                worksheet.Cells[1, 5].Value = "Gender";
                worksheet.Cells[1, 6].Value = "Country";
                worksheet.Cells[1, 7].Value = "Address";
                worksheet.Cells[1, 8].Value = "Receives Newsletter";

                // 3.(additional) Style the Header Row
                using (var headerRange = worksheet.Cells["A1:I1"])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }
                List<PersonResponse> personResponses = await GetAllPersons();

                // this should be 2 because 1 for headers 
                int row = 2;
                foreach (var person in personResponses)
                {
                    worksheet.Cells[row, 1].Value = person.Name;
                    worksheet.Cells[row, 2].Value = person.Email;
                    worksheet.Cells[row, 3].Value = person.DateOfBirth;
                    worksheet.Cells[row, 3].Style.Numberformat.Format = "yyyy-mm-dd"; 
                    worksheet.Cells[row, 4].Value = person.Age;
                    worksheet.Cells[row, 5].Value = person.Gender;
                    worksheet.Cells[row, 6].Value = person.Country;
                    worksheet.Cells[row, 7].Value = person.Address;
                    worksheet.Cells[row, 8].Value = person.ReceiveNewsletter == true ? "Yes" : "No";
                    row++;
                }
                // 6. Auto-fit columns 
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                await excel.SaveAsync();
            }
            ;
            memoryStream.Position = 0;
            return memoryStream; 

        }
    }
}
