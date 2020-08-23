using AYClassLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AYPatients.Models
{
    [ModelMetadataTypeAttribute(typeof(PatientMetaData))]
    public partial class Patient : IValidatableObject
    {
        PatientsContext _context = new PatientsContext();
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //trim all strings
            //DateOfDeath, DateOfBirth are not trimmed because they are not string


            //first name process
            if (string.IsNullOrEmpty(FirstName))
            {
                yield return new ValidationResult("First Name is required", new[] { "FirstName" });
            }
            else
            {
                FirstName = FirstName.Trim();
                FirstName = AYValidation.AYCapitalize(FirstName);
            }

            if (string.IsNullOrEmpty(LastName))
            {
                yield return new ValidationResult("last Name is required", new[] { "LastName" });
            }
            else
            {
                LastName = LastName.Trim();
                LastName = AYValidation.AYCapitalize(LastName);
            }

            Regex genderPattern = new Regex(@"^[MFXmfx]$");
            
            if (string.IsNullOrEmpty(Gender))
            {
                yield return new ValidationResult("Gender is required", new[] { "Gender" });
            }
            else if (genderPattern.IsMatch(Gender))
            {
                Gender = AYValidation.AYCapitalize(Gender);                
            }
            else
            {
                yield return new ValidationResult("Gender is among: M, F, X", new[] { "Gender" });
            }


            Address = AYValidation.AYCapitalize(Address).Trim();
            City = AYValidation.AYCapitalize(City).Trim();


            Province correspondingProvince = null;
            if (!string.IsNullOrEmpty(ProvinceCode))
            {
                ProvinceCode = ProvinceCode.Trim().ToUpper();
                string err = string.Empty;
                try
                {
                    correspondingProvince = _context.Province.Where(a => a.ProvinceCode == ProvinceCode).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    err = ex.GetBaseException().Message;
                }

                if (correspondingProvince == null)
                {
                    yield return new ValidationResult("Province Code not on the list", new[] { "ProvinceCode" });
                }
                if (!string.IsNullOrEmpty(err))
                {
                    yield return new ValidationResult(err, new[] { "ProvinceCode" });
                }
            }

            if (!string.IsNullOrEmpty(PostalCode))
            {
                if (string.IsNullOrEmpty(ProvinceCode))
                {
                    yield return new ValidationResult("Province Code is needed first", new[] { "PostalCode" });
                }
                if (correspondingProvince != null) //countrycode could be US or CA
                {
                    if (correspondingProvince.CountryCode == "CA")
                    {
                        Regex firstLetterPattern = new Regex("^["+correspondingProvince.FirstPostalLetter+"]$");
                        if (!firstLetterPattern.IsMatch(PostalCode.Substring(0, 1)))
                        {
                            yield return new ValidationResult("Postal Code is not correct for that Province", new[] { "ProvinceCode", "PostalCode" });
                        }
                        else
                        {
                            if (AYValidation.AYPostalCodeValidation(PostalCode))
                                PostalCode = AYValidation.AYPostalCodeFormat(PostalCode);
                            else
                                yield return new ValidationResult("Postal Code is not correct.", new[] { "PostalCode" });
                        }
                    }
                }

            }

            if (!string.IsNullOrEmpty(Ohip))
            {
                Ohip = Ohip.Trim().ToUpper();
                Regex ohipPattern = new Regex(@"^\d\d\d\d[-]\d\d\d[-]\d\d\d[-][A-Z][A-Z]$", RegexOptions.IgnoreCase);
                if (!ohipPattern.IsMatch(Ohip))
                {
                    yield return new ValidationResult("Ohip should match pattern: 1234-123-123-XX", new[] { "Ohip" });
                }
            }

            if (!string.IsNullOrEmpty(HomePhone))
            {
                HomePhone = AYValidation.AYExtractDigits(HomePhone);
                if (HomePhone.Length != 10)
                {
                    yield return new ValidationResult("Home phone should be 10 digits", new[] { "HomePhone" });
                }
                else
                {
                    HomePhone = HomePhone.Insert(3, "-").Insert(7, "-");
                }

            }

            if (DateOfBirth != null) //dateofbirth is not string
            {
                if (DateOfBirth > DateTime.Now)
                {
                    yield return new ValidationResult("Date of birth is not correct", new[] { "DateOfBirth" });
                }               

            }

            if (Deceased)
            {
                if (DateOfDeath == null)
                {
                    yield return new ValidationResult("Please enter date of death", new[] { "DateOfBirth" });
                }
                if (DateOfDeath > DateTime.Now || DateOfDeath > DateTime.Now)
                {
                    yield return new ValidationResult("Please enter correct date of death", new[] { "DateOfBirth" });
                }

            }
            else
            {
                if (DateOfDeath != null)
                    yield return new ValidationResult("Don' enter date of death if not deceased", new[] { "DateOfDeath" });
            }

            yield return ValidationResult.Success;

        }
    }

    public class PatientMetaData
    {
        public int PatientId { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }        
        [Display(Name = "Street Address")]
        public string Address { get; set; }
        public string City { get; set; }
        [Display(Name = "Province Code")]
        public string ProvinceCode { get; set; }
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        public string Ohip { get; set; }
        [DisplayFormat(DataFormatString = "{0: d MMMM, yyyy}")]
        [Display(Name ="Date of Birth")]
        public DateTime? DateOfBirth { get; set; }
        public bool Deceased { get; set; }
        [Display(Name = "Date of Death")]
        [DisplayFormat(DataFormatString = "{0: d MMMM, yyyy}")]
        public DateTime? DateOfDeath { get; set; }
        [Display(Name = "Home Phone")]
        public string HomePhone { get; set; }
        [Required]
        public string Gender { get; set; }
    }
}
