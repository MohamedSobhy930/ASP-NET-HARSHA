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

namespace Services
{
    public class PersonService : IPersonService
    {
        private readonly AppDbContext _db;
        private readonly ICountriesService _countriesService;
        
        public PersonService(ICountriesService countriesService, AppDbContext db) 
        {
            _db = db;
            _countriesService = countriesService;
            
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? request)
        {
            if(request == null)
                throw new ArgumentNullException();

            ValidationHelper.ModelValidation(request);

            Person person = request.ToPerson();
            person.PersonId = Guid.NewGuid();
            await _db.AddAsync(person);
            await _db.SaveChangesAsync();
            // using sp
            //_db.sp_InsertPerson(person);
            return person.ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid? id)
        {
            if(id == null) 
                throw new ArgumentNullException(nameof(id));
            var person = _db.Persons.FirstOrDefault(p => p.PersonId == id);
            if (person == null)
                return false;
            _db.Persons.Remove(person);
            await _db.SaveChangesAsync();
            return true;   
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            var persons = await _db.Persons.Include(p => p.Country).ToListAsync();
            
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
            List<PersonResponse> allPersons = await GetAllPersons();
            List<PersonResponse> matchingPersons ;
            if(string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchPhrase))
                return allPersons;
            
            switch(searchBy.Trim().ToLower())
            {
                case "name":
                    matchingPersons = allPersons
                    .Where(p => p.Name != null && p.Name.Contains(searchPhrase, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                    break;
                case "address":
                    matchingPersons = allPersons
                    .Where(p => p.Address != null && p.Address.Contains(searchPhrase, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                    break;
                case "country":
                    matchingPersons = allPersons
                    .Where(p => p.Country != null && p.Country.Contains(searchPhrase, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                    break;
                case "dateofbirth":
                    if(DateTime.TryParse(searchPhrase , out DateTime searchDate))
                    {
                        matchingPersons = allPersons
                            .Where(p => p.DateOfBirth.HasValue && p.DateOfBirth.Value.Date ==  searchDate.Date)
                            .ToList();
                    }
                    else
                    {
                        matchingPersons = new List<PersonResponse>();
                    }
                    break;
                default:
                    matchingPersons = allPersons;
                    break;

            }
            return matchingPersons;
        }

        public async Task<PersonResponse?> GetPersonById(Guid? id)
        {
            if (id == null)
                return null;
            var person =await _db.Persons.Include(p => p.Country).FirstOrDefaultAsync(p => p.PersonId == id);
            if (person == null)
                return null;
            return person.ToPersonResponse();

        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> persons, string sortBy, SortDirectionOptions sortDirection)
        {
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
            var personFromDb =await _db.Persons.FirstOrDefaultAsync(p => p.PersonId == request.Id);
            if(personFromDb == null)
                throw new ArgumentException("person Id not existed");
            personFromDb.Name = request.Name;
            personFromDb.Email = request.Email;
            personFromDb.CountryId = request.CountryId;
            await _db.SaveChangesAsync();

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

                List<PersonResponse> persons =await _db.Persons
                    .Include(p => p.Country)
                    .Select(temp => temp.ToPersonResponse())
                    .ToListAsync();

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
