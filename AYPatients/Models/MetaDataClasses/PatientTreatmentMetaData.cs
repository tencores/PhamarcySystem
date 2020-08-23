using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AYPatients.Models
{
    [ModelMetadataTypeAttribute(typeof(PatientTreatmentMetaData))]
    public partial class PatientTreatment
    {

    }

    public class PatientTreatmentMetaData
    {        
        public int PatientTreatmentId { get; set; }
        public int TreatmentId { get; set; }

        [DisplayFormat(DataFormatString = "{0: dd MMMM yyy HH:mm}", ApplyFormatInEditMode = true)] //this 
        public DateTime DatePrescribed { get; set; }
        public string Comments { get; set; }
        public int PatientDiagnosisId { get; set; }
    }

    
}
